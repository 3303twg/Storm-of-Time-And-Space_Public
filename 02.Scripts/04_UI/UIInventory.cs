using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    [Header("Item Info")]
    [SerializeField] private TextMeshProUGUI itemInfoText;
    [SerializeField] private GameObject infoPanel;

    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private List<ItemData> items;

    public int MaxSlotCount { get; private set; } = 5;

    public void Init() // 인벤토리 초기 설정
    {
        itemInfoText.text = string.Empty;
        infoPanel.SetActive(false);
        itemSlots.Clear();

        for (int i = 0; i < MaxSlotCount; i++) // 초기 슬롯 생성
        {
            var _itemSlots = Instantiate(slotPrefab, slotParent).GetComponent<ItemSlot>();
            _itemSlots.ClearSlot();

            itemSlots.Add(_itemSlots);
        }

        EventBus.Subscribe(EventBusType.InventoryUpdate, RefreshInventory);
        EventBus.Subscribe(EventBusType.SlotActive, SlotActive);
    }

    public void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.InventoryUpdate, RefreshInventory);
        EventBus.Unsubscribe(EventBusType.SlotActive, SlotActive);
    }

    private void RefreshInventory(object items) // 아이템 슬롯 재설정
    {
        this.items = (List<ItemData>)items;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < this.items.Count)
            {
                itemSlots[i].SetSlot(this.items[i]);
            }
            else
            {
                itemSlots[i].ClearSlot();
            }
        }
    }

    public void SlotActive(object num) // 슬롯 활성화
    {
        int _num = (int)num;

        if(_num < 0)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].SlotActive(false);
            }
            InfoPanelActive(false);
            return;
        }

        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(_num == i) itemSlots[i].SlotActive(true);
            else itemSlots[i].SlotActive(false);
        }

        if (items != null){
            itemInfoText.text = items[_num].Info.description;
            InfoPanelActive(true);
        }
    }

    public void InfoPanelActive(bool isActive)
    {
        infoPanel.SetActive(isActive);
    }
}
