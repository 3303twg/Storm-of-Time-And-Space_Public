using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<ItemData> items = new List<ItemData>(); // 보관하고 있는 아이템

    public int MaxItemCount { get; private set; } = 5; // 최대 아이템 숫자

    public IReadOnlyList<ItemData> Items => items;

    public Ship Ship { get; private set; }

    public Inventory(Ship ship)
    {
        Ship = ship;
    }

    public bool AddItem(ItemInfo info) // 아이템 획득
    {
        // 인벤토리 가득 찼을 경우 실패
        if (items.Count >= MaxItemCount)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
            return false;
        }

        // 아이템 생성 및 추가
        var newItem = CreateItem(info);
        items.Add(newItem);

        EventBus.Publish(EventBusType.InventoryUpdate, items);
        return true;
    }

    public void RemoveItem(int num) // 아이템 삭제
    {
        if (items.Remove(items[num]))
        {
            EventBus.Publish(EventBusType.InventoryUpdate, items);
        }
    }

    public void AfterItemUse(int num)
    {
        items[num].UseItem(Ship);
        RemoveItem(num);

        EventBus.Publish(EventBusType.InventoryUpdate, items);
    }

    private ItemData CreateItem(ItemInfo info) // 아이템 생성
    {
        return info.type switch
        {
            ItemType.Consume => new ConsumeItemData(info),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
