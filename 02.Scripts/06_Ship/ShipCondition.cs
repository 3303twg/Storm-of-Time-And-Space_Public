using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShipCondition : MonoBehaviourPun
{
    private bool isHit = false;
    public bool IsDead { get; private set; } = false;
    
    public float maxHealth;

    private CameraController cameraController;
    
    [SerializeField] private float currentHealth;
    [SerializeField] private float hitRecoveryTime;
    [SerializeField] private float currentHitRecoveryTime;

    private void Awake()
    {
        cameraController = GetComponentInChildren<CameraController>();
    }

    public void Init()
    {
        IsDead = false;
        currentHealth = maxHealth;
        EventBus.Publish(EventBusType.ShipHealthChange, currentHealth / maxHealth);
    }

    
    public void OnDamage(float damage)
    {
        if (isHit || IsDead) return;
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    
    
    private IEnumerator Hit()
    {
        isHit = true;
        
        currentHitRecoveryTime = hitRecoveryTime;
        while (currentHitRecoveryTime > 0)
        {
            currentHitRecoveryTime -= Time.deltaTime;
            yield return null;
        }
        isHit = false;
    }
    
    public void Heal(float healAmount)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC("RPC_Heal", RpcTarget.All, healAmount);
    }
    
    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (cameraController != null)
        {
            cameraController.ShakeCamera(0.4f);
        }

        EventBus.Publish(EventBusType.ShipHealthChange,  currentHealth/maxHealth);
        
        if (currentHealth <= 0 && !IsDead)
        {
            EventBus.Publish(EventBusType.CameraZoomChange, false);
            EventBus.Publish(EventBusType.GameOverMotion, null);
            IsDead = true;
            Logger.Log("Ship Destroyed");
            // TODO: GameOver!
            return;
        }

        StartCoroutine(Hit());
    }
    
    [PunRPC]
    public void RPC_Heal(float healAmount)
    {
        currentHealth += healAmount;
        Logger.Log("currentHP "+ currentHealth);
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        EventBus.Publish(EventBusType.ShipHealthChange, currentHealth/maxHealth);
    }
}
