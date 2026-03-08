using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CannonController : BaseInteractionController
{
    public PhotonView pv;
    public CannonData cannonData; // 해당하는 스크립터블 오브젝트
    public Transform shotBulletPoint; // 총알 발사 위치
    public Transform cannonHeadTransform; // 캐논 머리 위치 조절하는 Anchor 오브젝트
    public ParticleData cannonMuzzle;

    public Vector2 baseDirection = Vector2.right; // 캐논이 바라보는 방향 ( 기본 오른쪽으로 설정)
    [Range(0, 90)] // 인스펙터에서 슬라이더로 머리 각도 범위 설정가능
    public float maxRotationAngle = 90f; // 기본 방향으로부터 최대 회전할 수 있는 각도 (+- 적용되서 x2 만큼의 범위 현재 180도)
    public float rotationSpeed = 60f;   // 머리 회전 속도
    private float currentRelativeAngle = 0f; // ⭐ 현재 기준 방향으로부터의 상대 각도

    private float nextFireTime;

    [Header("캐논 세팅")]
    public float fireRate;          // 발사 속도 (초당 발사 횟수)
    public float projectileSpeed;   // 총알 속도 (탄속)
    public float projectileDamage;  // 총알 데미지

    public float projectileDamageBuff = 0;

    protected override void InteractUseControl()
    {
        HandleInputRotation(); // 방향키 좌,우 키로 회전
        HandleFireInput(); // Space 키로 발사

        //cannonHeadTransform.rotation = Quaternion.Lerp(cannonHeadTransform.rotation,
        //remoteCannonHeadRotation, Time.deltaTime * 10f); // 10f는 보간 속도, 조절 가능
    }

    // 키보드 입력에 따른 회전 처리 함수 
    void HandleInputRotation()
    {
        float rotationInput = 0f;

        // 'A' 키를 누르면 위로 회전 (각도 증가)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationInput = 1f;
        }
        // 'D' 키를 누르면 아래로 회전 (각도 감소)
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationInput = -1f;
        }

        if (rotationInput != 0)
        {
            // 현재 상대 각도를 업데이트
            currentRelativeAngle += rotationInput * rotationSpeed * Time.deltaTime;

            // 각도 제한 적용
            currentRelativeAngle = Mathf.Clamp(currentRelativeAngle, -maxRotationAngle, maxRotationAngle);
        }

        // 최종 회전 각도 계산 및 적용
        Vector2 rotatedBaseDirection = transform.TransformDirection(baseDirection);
        float baseRotationAngle = Mathf.Atan2(rotatedBaseDirection.y, rotatedBaseDirection.x) * Mathf.Rad2Deg;
        float finalRotationZ = baseRotationAngle + currentRelativeAngle;

        cannonHeadTransform.rotation = Quaternion.Euler(0, 0, finalRotationZ);
    }

    // ⭐ 총알 발사 입력 처리 함수
    void HandleFireInput()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Fire()
    {
        photonView.RPC("RPC_RequestFire", RpcTarget.MasterClient,
            shotBulletPoint.position, shotBulletPoint.rotation,
            projectileSpeed, projectileDamage + projectileDamageBuff, cannonData.projectileLifetime);
        SoundManager.Instance.PlaySFX("BulletHitMonster", transform.position);
        
        photonView.RPC("RPC_SetFireMuzzle", RpcTarget.All);
    }

    [PunRPC]
    void RPC_RequestFire(Vector3 position, Quaternion rotation, float spd, float dmg, float lft)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        GameObject projectileGo = PhotonNetwork.Instantiate("Base_Cannon_Projectile", position, rotation);

        Projectile projectile = projectileGo.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.photonView.RPC("RPC_InitializeProjectile", RpcTarget.All,
                (Vector2)shotBulletPoint.right, spd, dmg, lft);
        }
    }

    [PunRPC]
    private void RPC_SetFireMuzzle()
    {
        if(cannonMuzzle.prefab == null) Logger.Log("Cannon Muzzle is null");
        ParticlePoolManager.Instance.GetObject(cannonMuzzle.prefab,cannonMuzzle.tag , shotBulletPoint.position, shotBulletPoint.rotation);
    }

    protected override void RequestOwnershipAndSync()
    {
        if (!pv.IsMine)
        {
            pv.RequestOwnership();
        }
    }

    [PunRPC]
    public void RPC_DamageUP(float value)
    {
        projectileDamageBuff += value;
    }

    private IEnumerator DamageBuff_Cor(float value, float duration)
    {
        photonView.RPC("RPC_DamageUP", RpcTarget.All, value);

        yield return new WaitForSeconds(duration);

        photonView.RPC("RPC_DamageUP", RpcTarget.All, -value);
    }

    public void DamageBuff(float value, float duration)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        StartCoroutine(DamageBuff_Cor(value, duration));
    }

}
