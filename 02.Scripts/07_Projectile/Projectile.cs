using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPunCallbacks
{
    private Vector2 direction;  // 총알 방향
    private float speed;    // 속도
    private float damage;   // 데미지
    private float lifetime; // 남아있는 시간
    private float spawnTime;   // 생성된 시간
    public ParticleData projectileImpact;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask wallLayer;

    public string poolTag = "PlayerBullet";

    public void Initialize(Vector2 dir, float spd, float dmg, float lft)    // 총알 생성
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        lifetime = lft;
        spawnTime = Time.time;
    }

    [PunRPC]
    public void RPC_InitializeProjectile(Vector2 dir, float spd, float dmg, float lft)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        lifetime = lft;
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
        // 발사 방향 적용
        if (Time.time - spawnTime >= lifetime)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_ReturnToPool", RpcTarget.All);
            }
        }
        // 현재시간 - 생성된시간 >= 남아있는 시간 일때 리턴풀
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine && !PhotonNetwork.IsMasterClient) return;

        int collidedLayer = other.gameObject.layer; // 충돌한 오브젝트의 레이어 인덱스

        if (((1 << collidedLayer) & enemyLayer) != 0) // Enemy 레이어와 충돌했는지 검사
        {
            other.GetComponent<IDamageable>()?.OnDamage(damage);
            // RPC에 레이어 인덱스를 직접 전달
            //photonView.RPC("RPC_ApplyDamageAndReturn", RpcTarget.All, damage, collidedLayer);
            photonView.RPC("RPC_ApplyDamageAndReturn", RpcTarget.All);
        }
        else if (((1 << collidedLayer) & wallLayer) != 0) // Wall 레이어와 충돌했는지 검사
        {
            // RPC에 레이어 인덱스를 직접 전달
            //photonView.RPC("RPC_ApplyDamageAndReturn", RpcTarget.All, -1, 0, collidedLayer);
            photonView.RPC("RPC_ApplyDamageAndReturn", RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_ReturnToPool()
    {
        if (projectileImpact.prefab != null && projectileImpact.tag != null)
        {
            ParticlePoolManager.Instance.GetObject(projectileImpact.prefab,projectileImpact.tag , transform.position, transform.rotation);
        }
        ReturnToPool();
    }

    [PunRPC]
    void RPC_ApplyDamageAndReturn()
    {
        if (projectileImpact.prefab != null && projectileImpact.tag != null)
        {
            ParticlePoolManager.Instance.GetObject(projectileImpact.prefab,projectileImpact.tag , transform.position, transform.rotation);
        }
        ReturnToPool();
    }
    /*
    [PunRPC]    
    void RPC_ApplyDamageAndReturn(int targetViewID, float dmg, int collidedLayerIndex)
    {
        
        if (PhotonNetwork.IsMasterClient) 
        {
            if (((1 << collidedLayerIndex) & enemyLayer) != 0) 
            {
                PhotonView targetPV = PhotonView.Find(targetViewID);
                if (targetPV)
                {
                    
                }
            }
        }
        if (((1 << collidedLayerIndex) & wallLayer) != 0) 
        {
        }

        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.Destroy(gameObject);
        }
        
        ReturnToPool(); 
    }
    */
    private void ReturnToPool() // 총알 반환 매서드
    {
        if (ObjectPoolManager.Instance)
        {
            ObjectPoolManager.Instance.Destroy(gameObject);
        }
    }
}