using UnityEngine;
using UnityEngine.UI;

public class UIMenuPanel : MonoBehaviour
{
    public GameObject soundSettingsButtonGameObject;
    
    public GameObject soundPanelGameObject;

    private Button soundButtonComponent; 

    void Awake()
    {
        if (soundSettingsButtonGameObject != null)
        {
            soundButtonComponent = soundSettingsButtonGameObject.GetComponent<Button>();
            if (soundButtonComponent == null)
            {
                return;
            }
        }
    }

    void Start()
    {
        gameObject.SetActive(false);
    }
    
    void OnEnable()
    {
        if (soundSettingsButtonGameObject != null)
        {
            soundSettingsButtonGameObject.SetActive(true);
        }
        
        if (soundButtonComponent != null)
        {
            soundButtonComponent.onClick.AddListener(OnSoundSettingsButtonClicked);
        }
        
        EventBus.Subscribe(EventBusType.SoundPanelToggle, OnSoundPanelToggled);
    }

    void OnDisable()
    {
        if (soundSettingsButtonGameObject != null)
        {
            soundSettingsButtonGameObject.SetActive(false);
        }
        
        if (soundButtonComponent != null)
        {
            soundButtonComponent.onClick.RemoveListener(OnSoundSettingsButtonClicked);
        }
        
        EventBus.Unsubscribe(EventBusType.SoundPanelToggle, OnSoundPanelToggled);
    }
    
    public void OnSoundSettingsButtonClicked()
    {
        if (soundPanelGameObject != null)
        {
            soundPanelGameObject.SetActive(true); 
        }
        
        if (soundSettingsButtonGameObject != null)
        {
            soundSettingsButtonGameObject.SetActive(false);
        }
    }
    
    private void OnSoundPanelToggled(object isActive)
    {
        if (!this.gameObject.activeInHierarchy) return;

        bool _isActive = (bool)isActive;
        
        if (!_isActive && soundSettingsButtonGameObject != null)
        {
            soundSettingsButtonGameObject.SetActive(true);
        }
    }
}
