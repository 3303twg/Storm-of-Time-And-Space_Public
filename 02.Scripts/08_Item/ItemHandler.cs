using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;

    public ItemInfo ItemInfo => itemInfo;
}