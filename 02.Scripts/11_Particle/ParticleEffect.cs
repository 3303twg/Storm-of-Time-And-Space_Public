using System;
using UnityEngine;

[Serializable]
public class ParticleData
{
    public GameObject prefab;
    public string tag;
}

public class ParticleEffect : MonoBehaviour, IPoolable
{
    private Action<GameObject> OnReturnAction;

    private void OnDisable()
    {
        OnDespawn();
    }

    public void Initialize(Action<GameObject> _returnAction)
    {
        OnReturnAction += _returnAction;
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        OnReturnAction?.Invoke(gameObject);
    }
}
