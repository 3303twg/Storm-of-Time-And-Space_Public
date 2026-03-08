using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;

    public Button button;

    public void SetSlot(UpgradeLevelData upgradeLevelData)
    {
        levelText.text = upgradeLevelData.level.ToString();

        if (upgradeLevelData.cost > 0) costText.text = upgradeLevelData.cost.ToString();
        else costText.text = string.Empty;
    }

    public void ClearSlot()
    {
        nameText.text = string.Empty;
        levelText.text = string.Empty;
        costText.text = string.Empty;
    }
}
