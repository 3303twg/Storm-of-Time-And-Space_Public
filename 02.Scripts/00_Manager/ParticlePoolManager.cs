using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Initialize(Action<GameObject> returnAction);

    void OnSpawn();
    void OnDespawn();
}

public class ParticlePoolManager : Singleton<ParticlePoolManager>
{
      private Dictionary<GameObject, Queue<GameObject>> pools = new();
      
        public GameObject GetObject(GameObject prefab, string _tag,Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(prefab))
            {
                pools[prefab] = new Queue<GameObject>();
            }

            GameObject obj;
            if (pools[prefab].Count > 0)
            {
                obj = pools[prefab].Dequeue();
            }
            else
            {
                obj = ObjectPoolManager.Instance.GetObjectFromPool(_tag);
                //obj = PhotonNetwork.Instantiate(prefab.name, position, rotation);
                // 생성이될 때 초기화
                obj.GetComponent<IPoolable>()?.Initialize(o=>ReturnObject(prefab,o)); 
            }
            
            obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        // 가져갈 때 초기화
        obj.GetComponent<IPoolable>()?.OnSpawn();
        return obj;
    }

    public void ReturnObject(GameObject prefab,GameObject obj)
    {
        if (!pools.ContainsKey(prefab))
        {
            Destroy(obj);
            return;
        }
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }

    public void DestroyObjects(GameObject prefab)
    {
        if (pools.ContainsKey(prefab))
        {
            while (pools[prefab].Count > 0)
            {
                Destroy(pools[prefab].Dequeue());   
            }
            pools.Remove(prefab);
        }

    }
}
