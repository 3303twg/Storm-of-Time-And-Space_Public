using UnityEngine;

public class RotateSpawner : MonoBehaviour
{
    public float rotationSpeed;
    
    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += rotationSpeed * Time.deltaTime;
        transform.eulerAngles = currentRotation;
    }
}
