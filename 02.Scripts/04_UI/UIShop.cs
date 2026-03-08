using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinValue;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject shopSlot;
    [SerializeField] private List<ShopSlot> uiSlots = new List<ShopSlot>();

    [Header("스크롤 뷰")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollMargin = 20f;

    private StatType[] statTypes = { StatType.MaxHealth, StatType.MoveSpeed, StatType.Damage, StatType.FireRate, StatType.ProjectileSpeed, StatType.ShieldMaxHealth };
    private GameObject defaultSelectable;    // 기본 포커싱할 버튼/슬롯
    private GameObject lastSelected;

    public void Init()
    {
        this.coinValue.text = "0";
        shopPanel.SetActive(false);

        EventBus.Subscribe(EventBusType.CoinValueUpdate, RefreshCoinValue);
        EventBus.Subscribe(EventBusType.UIShopActive, UIShopActive);
        EventBus.Subscribe(EventBusType.ShopSlotUpdate, ShopSlotUpdate);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.CoinValueUpdate, RefreshCoinValue);
        EventBus.Unsubscribe(EventBusType.UIShopActive, UIShopActive);
        EventBus.Unsubscribe(EventBusType.ShopSlotUpdate, ShopSlotUpdate);
    }

    private void Start()
    {
        CreateShopSlots();
        defaultSelectable = uiSlots[0].gameObject;
    }

    private void CreateShopSlots() // 슬롯 초기 생성
    {
        uiSlots = new List<ShopSlot>();

        for (int i = 0; i < statTypes.Length; i++)
        {
            int capturedIndex = i; // 클로저 문제 방지용 캡처 변수

            // 인스턴트 생성
            ShopSlot slot = Instantiate(shopSlot, content).GetComponent<ShopSlot>();

            // 버튼 이벤트 추가
            slot.button.onClick.AddListener(() => EventBus.Publish(EventBusType.ShopButton, statTypes[capturedIndex]));

            slot.ClearSlot();
            slot.nameText.text = statTypes[i].ToString();

            // 리스트에 추가
            uiSlots.Add(slot);
        }
    }

    private void RefreshCoinValue(object coinValue) // 코인 값 업데이트
    {
        this.coinValue.text = coinValue.ToString();
    }

    private void ShopSlotUpdate(object upgradeData)
    {
        UpgradeData _upgradeData = (UpgradeData)upgradeData;
        int index = (int)_upgradeData.Type;

        if (index < 0 || index >= uiSlots.Count)
        {
            Debug.LogWarning($"ShopSlotUpdate 오류: StatType.{_upgradeData.Type}에 대한 슬롯이 없습니다.");
            return;
        }

        uiSlots[index].SetSlot(_upgradeData.upgradeLevelData);
    }

    void OnEnable()
    {
        StartCoroutine(FocusFirstButton());
    }

    IEnumerator FocusFirstButton() // 활성화 시 처음 포커스
    {
        yield return null;

        foreach (Transform child in scrollRect.content)
        {
            var selectable = child.GetComponent<Selectable>();
            if (selectable != null && selectable.interactable)
            {
                EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                break;
            }
        }
    }

    private void UIShopActive(object isActive) // Shop UI 활성화 / 비활성화
    {
        shopPanel.SetActive((bool)isActive);
    }

    void Update()
    {
        // UI가 켜져 있는 동안에만 동작
        if (shopPanel.activeInHierarchy)
        {

            GameObject current = EventSystem.current.currentSelectedGameObject;

            // UI 내에서 선택된 오브젝트가 바뀌었고, ScrollRect 안의 항목일 때
            if (current != null && current != lastSelected && current.transform.IsChildOf(scrollRect.content))
            {
                RectTransform target = current.GetComponent<RectTransform>();
                if (target != null)
                {
                    ScrollToVisible(target);
                    lastSelected = current;
                }
            }

            // 포커싱이 없거나, 비활성화된 오브젝트에 포커싱된 경우
            if (current == null || !current.activeInHierarchy)
            {
                if (defaultSelectable != null && defaultSelectable.activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(defaultSelectable);
                }
            }
        }
    }

    private void ScrollToVisible(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector3[] itemCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        target.GetWorldCorners(itemCorners);
        scrollRect.viewport.GetWorldCorners(viewportCorners);

        float itemTop = itemCorners[1].y;
        float itemBottom = itemCorners[0].y;
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        float offset = 0f;

        if (itemTop > viewportTop)
        {
            offset = itemTop - viewportTop + scrollMargin;
        }
        else if (itemBottom < viewportBottom)
        {
            offset = itemBottom - viewportBottom - scrollMargin;
        }

        if (offset != 0f)
        {
            Vector2 newPos = scrollRect.content.anchoredPosition;
            newPos.y -= offset;
            scrollRect.content.anchoredPosition = newPos;
        }
    }
}