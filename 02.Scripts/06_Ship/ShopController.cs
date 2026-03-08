using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : BaseInteractionController
{
    Ship ship;

    [Header("강화 관련")]
    [SerializeField] private List<UpgradeTableSO> upgradeDataList;

    private Dictionary<StatType, int> upgradeLevels = new();


    protected override void Awake()
    {
        base.Awake();

        EventBus.Subscribe(EventBusType.ShopButton, UpgradeStat);
        ship = GetComponentInParent<Ship>();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        EventBus.Unsubscribe(EventBusType.ShopButton, UpgradeStat);
    }

    public void Init()
    {
        foreach (var data in upgradeDataList)
        {
            // 초기화 없을 경우 0으로 설정
            if (!upgradeLevels.ContainsKey(data.StatType))
            {
                upgradeLevels[data.StatType] = 0;
            }

            ApplyStat(data.StatType);

            int level = upgradeLevels[data.StatType];
            if (level < data.Levels.Length)
            {
                UpgradeData upgradeData;

                upgradeData.Type = data.StatType;
                upgradeData.upgradeLevelData = data.Levels[level];

                EventBus.Publish(EventBusType.ShopSlotUpdate, upgradeData);
            }
            else
            {
                Debug.LogWarning($"{data.StatType} 레벨 인덱스가 Levels 범위를 벗어났습니다.");
            }
        }
    }

    public void UpgradeStat(object type)
    {
        photonView.RPC(nameof(RPC_UpgradeStat), RpcTarget.All, (int)type);
    }

    [PunRPC]
    private void RPC_UpgradeStat(int statIndex)
    {
        var type = (StatType)statIndex;
        var data = upgradeDataList.Find(s => s.StatType == type);
        if (data == null) return;

        int level = upgradeLevels[type];
        if (level + 1 >= data.Levels.Length)
        {
            Logger.Log($"{type}는 최대 레벨입니다.");
            return;
        }

        if (ship.CoinWallet.TrySpendCoin(data.Levels[level].cost))
        {
            upgradeLevels[type]++;
            ApplyStat(type);

            UpgradeData upgradeData;

            upgradeData.Type = type;
            upgradeData.upgradeLevelData = data.Levels[upgradeLevels[type]];

            EventBus.Publish(EventBusType.ShopSlotUpdate, upgradeData);
            Logger.Log($"{type} 강화 완료! 현재 레벨: {upgradeLevels[type]}");
        }
    }

    private void ApplyStat(StatType type)
    {
        var so = upgradeDataList.Find(s => s.StatType == type);
        if (so == null) return;

        var level = upgradeLevels[type];
        var data = so.GetDataForLevel(level);

        switch (type)
        {
            case StatType.MaxHealth:
                float healHp = data.value - ship.ShipCondition.maxHealth;
                ship.ShipCondition.maxHealth = data.value;
                ship.ShipCondition.Heal(healHp);
                break;
            case StatType.MoveSpeed:
                ship.ShipController.moveSpeed = data.value;
                break;
            case StatType.Damage:
                foreach (var controller in ship.CannonControllers)
                    controller.projectileDamage = data.value;
                break;
            case StatType.FireRate:
                foreach (var controller in ship.CannonControllers)
                    controller.fireRate = data.value;
                break;
            case StatType.ProjectileSpeed:
                foreach (var controller in ship.CannonControllers)
                    controller.projectileSpeed = data.value;
                break;
            case StatType.ShieldMaxHealth:
                ship.ShieldCondition.maxDurability = data.value;
                break;
        }
    }

    protected override void InteractUseControl()
    {
        
    }

    protected override void ActivateInteraction()
    {
        base.ActivateInteraction();

        EventBus.Publish(EventBusType.UIShopActive, true);
        EventBus.Publish(EventBusType.ShipHealthBarToggle, true);
    }

    protected override void DeactivateInteraction()
    {
        base.DeactivateInteraction();

        EventBus.Publish(EventBusType.UIShopActive, false);
        EventBus.Publish(EventBusType.ShipHealthBarToggle, false);
    }
}

public struct UpgradeData
{
    public StatType Type;

    public UpgradeLevelData upgradeLevelData;
}
