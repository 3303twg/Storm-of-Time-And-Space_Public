using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private Image[] images;

    [SerializeField] private EventBusType healthChangeType;
    [SerializeField] private EventBusType healthChangeToggle;
    
    private void Awake()
    {
        slider = GetComponent<Slider>();
        images = GetComponentsInChildren<Image>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe(healthChangeType,OnChangeShipHealth);
        EventBus.Subscribe(healthChangeToggle, ShipHealthBarToggle);
        ShipHealthBarToggle(false);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(healthChangeType,OnChangeShipHealth);
        EventBus.Unsubscribe(healthChangeToggle, ShipHealthBarToggle);
    }

    private void OnChangeShipHealth(object obj)
    {
        slider.value = (float)obj;
    }

    private void ShipHealthBarToggle(object obj)
    {
        foreach(var image in images)
        {
            image.enabled = (bool)obj;
        }
    }
}