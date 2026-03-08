using UnityEngine;
public class SpawnDirection : MonoBehaviour
{

    void Start()
    {
        EventBus.Publish(EventBusType.CameraSpotlight, transform);
        EventBus.Publish(EventBusType.Spotlight, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
