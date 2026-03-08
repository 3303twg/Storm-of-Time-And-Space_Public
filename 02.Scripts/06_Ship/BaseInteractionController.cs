using Photon.Pun;
using UnityEngine;

public abstract class BaseInteractionController : MonoBehaviourPunCallbacks , IInteractable
{
    public PlayerController _playerController {  get; private set; }

    [SerializeField] protected GameObject OnSprite;
    [SerializeField] protected GameObject OffSprite;
    
    [SerializeField] protected bool isInteractable = true; // 상호작용 가능한 상태인지 여부
    [SerializeField] protected bool isOwnershipAndSync = true; // Ownership 변경 여부
    public bool IsInteractable => isInteractable;

    protected virtual void Awake()
    {
        SetSprite();
    }

    protected virtual void Update()
    {
        SetSprite();
        if(isInteractable == true) return;
        if (_playerController == null) return;
        if (PhotonNetwork.LocalPlayer != _playerController.photonView.Owner) return;

        // 실행
        InteractUseControl();
    }

    protected abstract void InteractUseControl(); // 플레이어가 사용하는 메서드

    public virtual bool TryInteract(PlayerController player) // 플레이어가 상호작용 키를 눌렀을 때 호출되는 함수
    {
        if (isInteractable == false && _playerController == null) // 누군가 상호작용 중이라면 true 반환
        {
            Logger.Log("누군가 탑승 중");
            return true; 
        }

        if ( _playerController == null)
        {
            _playerController = player;
            ActivateInteraction();

            Logger.Log("탑승");
            EventBus.Publish(EventBusType.CameraZoomChange, true);
            return isInteractable;
        }
        else
        {
            _playerController = null;
            DeactivateInteraction();

            Logger.Log("탈출");
            EventBus.Publish(EventBusType.CameraZoomChange, false);
            return isInteractable;
        }
    }

    protected virtual void RequestOwnershipAndSync() // 소유권 요청 및 RPC를 통한 상호작용 상태 동기화 로직
    {

    }

    protected virtual void ActivateInteraction() // 상호작용 활성화 매서드
    {
        isInteractable = false;
        photonView.RPC("RPC_SetInteractionAvailable", RpcTarget.All, false);
        if (isOwnershipAndSync == true) RequestOwnershipAndSync();
    }

    protected virtual void DeactivateInteraction() // 상호작용 비활성화 매서드
    {
        isInteractable = true;
        photonView.RPC("RPC_SetInteractionAvailable", RpcTarget.All, true);
    }

    [PunRPC]
    public void RPC_SetInteractionAvailable(bool available) //  RPC로 호출될 상호작용 가능 상태 설정 메서드
    {
        isInteractable = available;
    }

    protected void SetSprite()
    {
        if (OnSprite.activeSelf == !isInteractable) return;
        
        OnSprite.SetActive(!isInteractable);
        OffSprite.SetActive(isInteractable);
    }
}
