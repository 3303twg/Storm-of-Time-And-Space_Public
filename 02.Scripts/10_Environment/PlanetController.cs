using UnityEngine;

public class PlanetController : MonoBehaviour
{
    Collider2D collider;
    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }
}
