using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShieldCondition :MonoBehaviourPun, IDamageable
{
    public PhotonView pv;
    
    public float maxDurability;
    public float currentDurability;
    
    private bool isHit;
    private bool isActive=true;
    
    private WaitForSeconds ws = new WaitForSeconds(0.1f);
    
    [SerializeField] private GameObject shieldParticle;
    [SerializeField] private BoxCollider2D shieldCollider;
    
    [SerializeField] private float hitRecoveryTime;
    [SerializeField] private float currentHitRecoveryTime;
    
    public void Init()
    {
        isActive = true;
        currentDurability = maxDurability;
        EventBus.Publish(EventBusType.ShieldDurabilityChange, currentDurability / maxDurability);
    }
    
    public void OnDamage(float damage)
    {
        if (isHit) return;
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    public void Heal(float healAmount)
    {
        if (currentDurability >= maxDurability-0.01f) return;
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC("RPC_Heal", RpcTarget.All, healAmount);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        currentDurability -= damage;
        currentDurability = Mathf.Max(currentDurability, 0);
        
        EventBus.Publish(EventBusType.ShieldDurabilityChange, currentDurability/maxDurability);
        
        if (currentDurability <= 0 && isActive)
        {
            isActive = false;
            SetShield();
            return;
        }

        StartCoroutine(Hit());
    }
    
    private IEnumerator Hit()
    {
        isHit = true;
        
        currentHitRecoveryTime = hitRecoveryTime;
        while (currentHitRecoveryTime > 0)
        {
            currentHitRecoveryTime -= Time.deltaTime;
            yield return ws;
        }
        isHit = false;
    }
    
    [PunRPC]
    public void RPC_Heal(float healAmount)
    {
        if (isActive != true)
        {
            isActive = true;
            SetShield();
        }
        
        currentDurability += healAmount;
        
        currentDurability = Mathf.Min(currentDurability, maxDurability);
        
        EventBus.Publish(EventBusType.ShieldDurabilityChange, currentDurability/maxDurability);
    }

    private void SetShield()
    {
        shieldParticle.SetActive(isActive);
        shieldCollider.enabled = isActive;
    }
}
