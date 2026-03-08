public class ConsumeItemData : ItemData
{
    public ConsumeItemData(ItemInfo info) : base(info)
    {
    }

    public override void OnUse(Ship ship)
    {
        // 효과 적용
        ApplyStat(ship);

    }

    void ApplyStat(Ship ship)
    {
        foreach (var modifier in Info.statModifiers)
        {
            switch (modifier.stat)
            {
                case ItemStat.Hp:
                    ship.ShipCondition.Heal(modifier.value);
                    break;
                case ItemStat.Speed:
                    ship.ShipController.SpeedBuff(modifier.value, modifier.duration);
                    break;
                case ItemStat.Atk:
                    foreach (var controller in ship.CannonControllers)
                    {
                        controller.DamageBuff(modifier.value, modifier.duration);
                    }
                        break;

            }
        }
    }
}
