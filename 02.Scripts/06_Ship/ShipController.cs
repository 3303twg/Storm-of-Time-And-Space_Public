using Photon.Pun;
using System.Collections;
using UnityEngine;
    
public class ShipController : BaseInteractionController
{
    [Header("Move")]
    [SerializeField] private Ship ship;
    
    [SerializeField] private Transform nozzle;
    [SerializeField] private float rotationSpeed;
    
    public float moveSpeed;
    public float moveSpeedBuff = 0;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 8f;

    private float currentSpeed = 0f;
    private bool isHealthBarOpened = false;

    protected override void Update()
    {
        if (IsInteractable&&ship.boostParticle.isPlaying)
        {
            photonView.RPC("RPC_SetBoost", RpcTarget.All, false);
        }
        SetShipHealthBar();
        base.Update();
    }

    protected override void InteractUseControl()
    {
        
        float axis = Input.GetAxisRaw("Horizontal");
        if (axis != 0)
        {
            Vector3 currentRotation = nozzle.eulerAngles;
            currentRotation.z -= axis * rotationSpeed * Time.deltaTime;
            nozzle.eulerAngles = currentRotation;
        }

        
        if (Input.GetKey(KeyCode.Space))
        {
            if (!ship.boostParticle.isPlaying)
            {
                photonView.RPC("RPC_SetBoost", RpcTarget.All, true);
            }

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, moveSpeed + moveSpeedBuff);
        }
        else
        {
            if (ship.boostParticle.isPlaying)
            {
                photonView.RPC("RPC_SetBoost", RpcTarget.All, false);
            }

            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }

        float distance = currentSpeed * Time.deltaTime;
        
        // if (CheckObstacleInCircle()||CheckObstacleOnForward())
        // {
        //     ship.transform.position += nozzle.right * distance;   
        // }
        
        if (CheckObstacleInCircle())
        {
            ship.transform.position += nozzle.right * distance;   
        }
    }

    protected override void RequestOwnershipAndSync()
    {
        if (!ship.Pv.IsMine)
        {
            ship.Pv.RequestOwnership();
        }
    }

    private void SetShipHealthBar()
    {
        if (isInteractable == isHealthBarOpened)
        {
            isHealthBarOpened = !isHealthBarOpened;
            if (_playerController != null)
            {
                EventBus.Publish(EventBusType.ShipHealthBarToggle, true);    
            }
            else
            {
                EventBus.Publish(EventBusType.ShipHealthBarToggle, false);
            }
        }
    }
    
    private bool CheckObstacleInCircle()
    {
        return Physics2D.OverlapCircle(ship.transform.position+(nozzle.right*1.2f), ship.ShipRadius,ship.ObstacleLayerMask) == null;
    }
    
    
    private void OnDrawGizmosSelected()
    {
        if (ship == null || nozzle == null)
            return;

        // 🔵 1. CircleCast 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ship.transform.position+(nozzle.right*1.2f), ship.ShipRadius);
    }

    [PunRPC]
    public void RPC_SpeedUP(float value)
    {
        moveSpeedBuff += value;
    }

    private IEnumerator SpeedBuff_Cor(float value, float duration)
    {
        photonView.RPC("RPC_SpeedUP", RpcTarget.All, value);

        yield return new WaitForSeconds(duration);

        photonView.RPC("RPC_SpeedUP", RpcTarget.All, -value);
    }

    public void SpeedBuff(float value, float duration)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        StartCoroutine(SpeedBuff_Cor(value, duration));
    }
    
    [PunRPC]
    public void RPC_SetBoost(bool isPlay)
    {
        if (isPlay)
        {
            ship.boostParticle.Play();
        }
        else
        {
            ship.boostParticle.Stop();
        }
    }
}
