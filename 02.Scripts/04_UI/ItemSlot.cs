using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("UI Setting")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectSlot;

    private ItemData itemData;
    
    public void SetSlot(ItemData data) // 아이템 획득
    {
        itemData = data;

        if (itemData == null || itemData.Info == null)
        {
            Logger.Log("[ItemSlot] itemData 또는 itemData.Info가 null입니다.");
            return;
        }

        if (iconImage == null)
        {
            Logger.Log("[ItemSlot] iconImage가 인스펙터에 할당되지 않았습니다.");
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = itemData.Info.icon;

    }

    public void ClearSlot() // 슬롯 초기화
    {
        itemData = null;

        iconImage.sprite = null;
        iconImage.enabled = false;
        selectSlot.SetActive(false);
    }

    public void SlotActive(bool isActive)
    {
        selectSlot.SetActive(isActive);
    }
}
