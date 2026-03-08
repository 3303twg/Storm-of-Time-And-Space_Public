using Photon.Pun;
using UnityEngine;

public class BodyController : MonoBehaviour, IDamageable
{
    public float attackDelay;
    float attackTime;

    public float bodyHp;

    public EnemyController head;

    public WormSegment wormSegment;


    private void OnEnable()
    {
        EventBus.Subscribe("DieBoss", DestroyBody);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe("DieBoss", DestroyBody);
    }

    public void DestroyBody(object obj)
    {
        Destroy(gameObject);
    }
    public void Start()
    {
        wormSegment = GetComponent<WormSegment>();
        head = wormSegment.GetHead();
        attackTime = Time.time + 5f;
        bodyHp = head.enemyStat.curHp / (head.body.BodyController.Count + 2);
    }
    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time > attackTime)
            {
                attackDelay = Random.Range(0.5f, 2.5f);
                attackTime = Time.time + attackDelay;
                if (Random.value < 0.5)
                {
                    //LeftAttack();
                }
                else
                {
                    //RightAttack();
                }
            }
        }
        

    }

    public void LeftAttack()
    {
        GameObject go = PhotonNetwork.Instantiate(head.enemyStat.projectile[Random.Range(0, head.enemyStat.projectile.Count -1)].name, transform.position, Quaternion.identity);
        
        // 부모 기준 왼쪽 방향은 transform.right * -1
        Vector3 leftDir = -transform.right;
        float angle = Mathf.Atan2(leftDir.y, leftDir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        Projectile projectile = go.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.Initialize((Vector2)go.transform.right, 10, head.enemyStat.attackPower, 7f);
        }

        SoundManager.Instance.PlaySFX("MonsterDeath", transform.position);
    }

    public void RightAttack()
    {
        GameObject go = PhotonNetwork.Instantiate("Base_Cannon_Projectile_Enemy", transform.position, Quaternion.identity);
        // 부모 기준 오른쪽 방향은 transform.right
        Vector3 rightDir = transform.right;
        float angle = Mathf.Atan2(rightDir.y, rightDir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        Projectile projectile = go.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.Initialize((Vector2)go.transform.right, 10, head.enemyStat.attackPower, 7f);
        }
        SoundManager.Instance.PlaySFX("MonsterDeath", transform.position);
    }

    public void OnDamage(float value)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (head != null)
            {
                bodyHp -= value;
                head.OnDamage(value);
                if (bodyHp < 0)
                {
                    //리스트에도 빼는 작업이 필요함
                    head.RPC_DestroyBody_Call(wormSegment.index);
                    Destroy(gameObject);
                }
            }
            //흠 머리를 어캐찾지
        }
    }

    public void RefreshHp()
    {
        bodyHp = head.enemyStat.curHp / (head.body.BodyController.Count + 2);
    }
}
