using Photon.Pun;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class EnemyController : MonoBehaviourPun, IPunObservable, IDamageable
{

    [SerializeField]
    public EnemyStateMachine stateMachine;
    [Header("디버그 용도")]
    [SerializeField] private string currentStateName;

    [SerializeField]
    public EnemyStat enemyStat;
    public Transform target;

    public Vector2 timeRange;
    public float endStateTime;

    public float distance;
    public float rotateSpeed = 15f;

    public Transform targetPos;
    public AIDestinationSetter setter;
    public AIPath aiPath;
    public Transform shotPos;
    public float time;

    public bool attackFlag = false;

    int layerMask;
    int shipLayer;
    float tickDelay = 1f;
    float delayTime;
    public float crashRatio = 5f;
    public BodyContainer body;

    public EnemyStat ConvertSoToStat(EnemyStatSo so)
    {
        EnemyStat stat = new EnemyStat();
        stat.enemyName = so.enemyName;
        stat.isBoss = so.isBoss;
        stat.maxHp = so.hp;
        stat.curHp = so.hp;
        stat.speed = so.speed;
        stat.safeDistance = so.safeDistance;
        stat.enemyPrefab = so.enemyPrefab;
        stat.projectile = new List<GameObject>(so.projectile);
        stat.attackPower = so.attackPower;
        stat.isLongRange = so.isLongRange;
        stat.attackRange = so.attackRange;
        stat.attackSpeed = so.attackSpeed;
        stat.dropTable = new List<DropEntry>(so.dropTable);
        stat.NoDropWeight = so.NoDropWeight;

        return stat;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(enemyStat.curHp);
        }
        else
        {
            enemyStat.curHp = (float)stream.ReceiveNext();
        }
    }
    Transform temp;

    private void Awake()
    {
        if (enemyStat.isBoss)
            target = GameManager.Instance.Ship.transform;
        //머지때는 변경하기

        else
        {
            GameObject go = new GameObject("TargetPos");
            targetPos = go.transform;
            target = GameManager.Instance.Ship.transform;
            setter = GetComponent<AIDestinationSetter>();
        }

        stateMachine = new EnemyStateMachine();
        temp = target;//GameObject.Find("Cicle").transform;
        layerMask = LayerMask.GetMask("Ship", "Obstacle");
        shipLayer = LayerMask.GetMask("Ship");
    }

    
    void Start()
    {
        if (enemyStat.isBoss == true)
        {
            //보스BossAttackState
            attackFlag = true;
            var idleState = new BossAttackState(this, stateMachine);
            stateMachine.Initialize(idleState);
            body = GetComponent<BodyContainer>();
        }
        else
        {
            aiPath = GetComponent<AIPath>();
            if (setter != null)
            {
                setter.target = temp;
                aiPath.canMove = false;
                aiPath.maxSpeed = enemyStat.speed;
            }


            //잡몹

            var idleState = new EnemyIdleState(this, stateMachine);
            stateMachine.Initialize(idleState);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            //마스크 나중에 꼭 지정하기
            if (enemyStat.isBoss == false)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, target.position - transform.position, enemyStat.attackRange, layerMask);

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                if (hits.Length > 0 && ((1 << hits[0].collider.gameObject.layer) & shipLayer) != 0)
                    {
                        attackFlag = true;
                    }
                    else
                    {
                        attackFlag = false;
                    }

            }
            //transform.position += transform.right * 5f * Time.deltaTime;
            //거리 미리 계산
            if (target != null)
            {
                distance = Vector2.Distance(target.position, transform.position);
            }
            stateMachine.Update();

            currentStateName = stateMachine.CurrentStateName;
        }
        
    }

    [PunRPC]
    public void RefreshHpUI()
    {
        EventBus.Publish(EventBusType.BossHealthChange, enemyStat.curHp / enemyStat.maxHp);
    }
    public void OnDamage(float value)
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            enemyStat.curHp -= value;
            if (enemyStat.isBoss)
            {
                photonView.RPC("RefreshHpUI", RpcTarget.All);
                
            }

            if (enemyStat.curHp < 0)
            {
                //photonView.RPC("SpawnItem", RpcTarget.All);
                if (!enemyStat.isBoss)
                {
                    SpawnItem();
                }

                if (enemyStat.isBoss)
                {
                    EventBus.Publish(EventBusType.CameraZoomChange, false);
                    EventBus.Publish(EventBusType.GameClearMotion, null);
                    DestroyAllBody();
                    //PhotonNetwork.Destroy(gameObject);
                }
                else
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            

        }
        //photonView.RPC("RPC_SyncStat", RpcTarget.All, data);
    }

    [PunRPC]
    public void SpawnItem()
    {
        ItemInfo data = GetRandomDrop();
        if (data != null)
        {
            PhotonNetwork.Instantiate(data.itemPrefab.name, transform.position, Quaternion.identity);
            //Instantiate(data.itemPrefab);
        }
    }



    public ItemInfo GetRandomDrop()
    {
        float rand = Random.Range(0f, 100f);
        float accum = 0f;

        List<ItemInfo> itemInfo = new List<ItemInfo>();
        if (enemyStat.dropTable.Count != 0)
        {
            foreach (var entry in enemyStat.dropTable)
            {
                accum += entry.dropChance;
                if (rand <= accum)
                {
                    itemInfo.Add(entry.itemInfo);

                }
            }

            int tempValue = Random.Range(0, enemyStat.NoDropWeight);

            for (int i = 0; i < tempValue; i++)
            {
                itemInfo.Add(null);
            }
            return itemInfo[Random.Range(0, tempValue)];
        }

        else
        {
            return null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ShipCrashCheck>(out var a))
        {
            if (delayTime < Time.time)
            {
                IDamageable temp = collision.GetComponent<IDamageable>();
                if (temp != null)
                {
                    temp?.OnDamage(enemyStat.attackPower * crashRatio);
                    delayTime = Time.time + tickDelay;
                }
            }
        }
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Logger.Log("충돌함1");
        if (collision.gameObject.TryGetComponent<ShipCrashCheck>(out var a))
        {
            Logger.Log("충돌함2");
            if (delayTime < Time.time)
            {
                IDamageable temp = collision.gameObject.GetComponent<IDamageable>();
                if (temp != null)
                {
                    temp?.OnDamage(enemyStat.attackPower * crashRatio);
                    delayTime = Time.time + tickDelay;
                }
            }
        }
    }
    */


    public void LookTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public void LookTargetNow()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle- 90);
    }

    public void shot()
    {
        GameObject go = PhotonNetwork.Instantiate(enemyStat.projectile[Random.Range(0, enemyStat.projectile.Count - 1)].name, shotPos.transform.position, shotPos.transform.rotation);
        Projectile projectile = go.GetComponent<Projectile>();

        if (projectile)
        {
            projectile.Initialize((Vector2)go.transform.right, 10, enemyStat.attackPower, 7f);
        }
        SoundManager.Instance.PlaySFX("MonsterDeath", transform.position);

    }


    //Boss
    
    public void DestroyAllBody()
    {
        photonView.RPC("RPC_DieEvnet",RpcTarget.All);
        
        /*
        foreach(var body in body.BodyController)
        {
            Destroy(body.gameObject);
        }
        */
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void RPC_DieEvnet()
    {
        EventBus.Publish("DieBoss", null);
    }

    public void RPC_DestroyBody_Call(int index)
    {
        photonView.RPC("RPC_DestroyBody", RpcTarget.All, index);

    }
    [PunRPC]
    public void RPC_DestroyBody(int index)
    {
        body.BodyController.RemoveAt(index);
        int cnt = 0;
        foreach (var body in body.BodyController)
        {
            if (cnt <= 0)
            {
                body.wormSegment.target = transform;
            }
            else
            {
                if (body.wormSegment.index >= index)
                {
                    body.wormSegment.target = this.body.BodyController[body.wormSegment.index - 2].transform;
                }
            }
            body.wormSegment.index = cnt;
            body.RefreshHp();
            cnt++;
        }
    }

    [PunRPC]
    public void RPC_LaserOn()
    {
        GetComponent<Laser>().laser.SetActive(true);
    }
    [PunRPC]
    public void RPC_LaserOff()
    {
        GetComponent<Laser>().laser.SetActive(false);
    }
}
