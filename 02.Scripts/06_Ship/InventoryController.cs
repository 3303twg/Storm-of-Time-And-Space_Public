using Photon.Pun;
using UnityEngine;

public class InventoryController : BaseInteractionController
{
    private Inventory inventory;

    private int currentIndex = 0; // 현재 선택된 슬롯 인덱스

    protected override void Awake()
    {
        base.Awake();

        inventory = GetComponentInParent<Ship>().Inventory;
    }

    protected override void InteractUseControl()
    {
        // <- , ->키 입력으로 아이템 정보 확인
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSlot(1);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSlot(-1);
        }


        // space bar로 아이템 사용
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if (currentIndex >= inventory.Items.Count)
            {
                Logger.Log("아이템이 없습니다.");
                return;
            }

            // 아이템 사용을 RPC로 동기화.
            photonView.RPC("RPC_UseItem", RpcTarget.All, currentIndex);
            MoveSlot(currentIndex);
            Logger.Log("아이템 사용");
        }
    }

    private void MoveSlot(int direction)
    {
        currentIndex = (currentIndex + direction + inventory.MaxItemCount) % inventory.MaxItemCount;
        EventBus.Publish(EventBusType.SlotActive, currentIndex);
    }


    protected override void ActivateInteraction()
    {
        base.ActivateInteraction();
        currentIndex = 0;
        MoveSlot(0);
        EventBus.Publish(EventBusType.ShipHealthBarToggle, true);
    }

    protected override void DeactivateInteraction() 
    { 
        base.DeactivateInteraction();

        EventBus.Publish(EventBusType.SlotActive, -1);
        EventBus.Publish(EventBusType.ShipHealthBarToggle, false);
    }

    [PunRPC]
    public void RPC_UseItem(int currentIndex)
    {
        inventory.AfterItemUse(currentIndex);
    }
}
