using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Publish(EventBusType.BossHealthBarToggle, true);
        StartCoroutine(LerpValue(5f, LerpValueEvent));
    }

    private void OnDisable()
    {
        EventBus.Publish(EventBusType.BossHealthBarToggle, false);
    }

    public void LerpValueEvent(float value)
    {
        // value: 0 ~ 1
        EventBus.Publish(EventBusType.BossHealthChange, value);
    }

    private IEnumerator LerpValue(float duration, System.Action<float> onUpdate)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            onUpdate?.Invoke(t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        onUpdate?.Invoke(1f);
    }
}
