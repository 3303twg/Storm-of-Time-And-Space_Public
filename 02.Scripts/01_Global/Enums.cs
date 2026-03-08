// 모든 enum

public enum EventBusType
{
    // Inventory
    InventoryUpdate,
    SlotActive,
    CoinValueUpdate,
    
    // HealthBar
    ShipHealthChange,
    ShipHealthBarToggle,
    BossHealthChange,
    BossHealthBarToggle,
    ShieldDurabilityChange,
    ShieldDurabilityBarToggle,
    
    // Camera
    CameraZoomChange,
    CameraSpotlight,

    //Ending
    GameOverMotion,
    GameClearMotion,
    GameOver,
    GameClear,

    // Shop
    UIShopActive,
    ShopButton,
    ShopSlotUpdate,

    //Deirection
    Spotlight,
    
    // Sound
    SoundPanelToggle,
}

public enum ItemType // 아이템 타입
{
    Consume,
    Currency
}

public enum ItemStat // 아이템 능력
{
    Atk,
    Speed,
    Hp,
}

public enum StatType
{
    MaxHealth = 0, // 최대 체력
    MoveSpeed, // 기체 속도
    Damage, // 캐논 데미지
    FireRate, // 캐논 발사 속도
    ProjectileSpeed, // 탄속
    ShieldMaxHealth // 방어포탑 최대 체력
}