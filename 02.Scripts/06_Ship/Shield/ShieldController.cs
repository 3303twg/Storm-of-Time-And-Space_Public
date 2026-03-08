using UnityEngine;

public class ShieldController : BaseInteractionController
{
    [SerializeField] private Transform shield;
    [SerializeField] private ShieldCondition shieldCondition;
    
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float healAmount = 5;
    
    private bool isDurabilityBarOpened = false;
    
    protected override void Update()
    {
        SetShieldDurabilityBar();
        base.Update();
    }
    
    protected override void InteractUseControl()
    {
        float axis = Input.GetAxisRaw("Horizontal");
        if (axis != 0)
        {
            Vector3 currentRotation = shield.eulerAngles;
            currentRotation.z -= axis * rotationSpeed * Time.deltaTime;
            shield.eulerAngles = currentRotation;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            shieldCondition.Heal(healAmount);
        }
    }

    protected override void RequestOwnershipAndSync()
    {
        if (!shieldCondition.pv.IsMine)
        {
            shieldCondition.pv.RequestOwnership();
        }
    }
    
    private void SetShieldDurabilityBar()
    {
        if (isInteractable == isDurabilityBarOpened)
        {
            isDurabilityBarOpened = !isDurabilityBarOpened;
            if (_playerController != null)
            {
                EventBus.Publish(EventBusType.ShieldDurabilityBarToggle, true);    
            }
            else
            {
                EventBus.Publish(EventBusType.ShieldDurabilityBarToggle, false);
            }
        }
    }
}
