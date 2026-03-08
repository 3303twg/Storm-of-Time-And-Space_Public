using UnityEngine;

public class ShipCrashCheck : MonoBehaviour, IDamageable
{
    [SerializeField]
    private ShipCondition shipCondition;
    public void OnDamage(float value)
    {
        shipCondition.OnDamage(value);
    }


}
