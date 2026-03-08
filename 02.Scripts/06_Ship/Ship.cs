using Photon.Pun;
using UnityEngine;

public class Ship : MonoBehaviourPun
{
    public PhotonView Pv { get; private set; }
    public Inventory Inventory { get; private set; }
    public CoinWallet CoinWallet { get; private set; }

    public ShipController ShipController { get; private set; }
    public ShipCondition ShipCondition { get; private set; }
    public InventoryController InventoryController { get; private set; }
    public ShopController ShopController { get; private set; }
    public CannonController[] CannonControllers { get; private set; }
    public ShieldController ShieldController { get; private set; }
    public ShieldCondition  ShieldCondition { get; private set; }
    

    public LayerMask ObstacleLayerMask { get=>obstacleLayerMask; }

    public float ShipForwardRayLength { get => shipForwardRayLength; }
    public float ShipRadius { get => shipRadius; }
    public ParticleSystem boostParticle;
    
    
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private int startCoin = 500;
    [SerializeField] private float shipRadius;
    [SerializeField] private float shipForwardRayLength;

    public void Awake()
    {
        Pv = GetComponent<PhotonView>();
        GameManager.Instance.Ship = this;

        Inventory = new Inventory(this);
        CoinWallet = new CoinWallet(startCoin);

        ShipCondition = GetComponent<ShipCondition>();
        ShipController = GetComponentInChildren<ShipController>();
        InventoryController = GetComponentInChildren<InventoryController>();
        ShopController = GetComponentInChildren<ShopController>();
        CannonControllers = GetComponentsInChildren<CannonController>();
        ShieldController = GetComponentInChildren<ShieldController>();
        ShieldCondition = GetComponentInChildren<ShieldCondition>();
    }

    private void Start()
    {
        ShopController.Init();
        ShipCondition.Init();
        ShieldCondition.Init();
        CoinWallet.Init();
        GameManager.Instance.stageManager.CreateFirstSpawner();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemHandler>(out ItemHandler handler))
        {    
            if (handler.ItemInfo == null)
                return;

            switch (handler.ItemInfo.type)
            {
                case ItemType.Consume:
                    Inventory.AddItem(handler.ItemInfo);
                    break;
                case ItemType.Currency:
                    CoinWallet.AddCoin(handler.ItemInfo.coinValue);
                    break;
            }

            Destroy(handler.gameObject);
        }
    }
}
