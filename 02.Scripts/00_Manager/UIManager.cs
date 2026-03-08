using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UIInventory UIInventory { get; private set; }
    public UIShop UIShop { get; private set; }

    public GameObject uiMainMenu;                
    
    public override void Awake()
    {
        base.Awake();

        UIInventory = GetComponentInChildren<UIInventory>();
        UIShop = GetComponentInChildren<UIShop>();

        UIInventory.Init();
        UIShop.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMainMenuPanel(); 
        }
    }

    private void ToggleMainMenuPanel()
    {
        bool isActive = !uiMainMenu.activeSelf;
        uiMainMenu.SetActive(isActive);
    }
}
