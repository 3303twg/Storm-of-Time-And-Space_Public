public abstract class ItemData
{
    public ItemInfo Info { get; private set; }

    public ItemData(ItemInfo info)
    {
        Info = info;
    }

    // 공통 사용 메서드
    public void UseItem(Ship ship)
    {
        OnUse(ship);
    }

    // 타입별 사용 구현
    public abstract void OnUse(Ship ship);
}
