using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>, IPunPrefabPool
{
    [System.Serializable]
    public class Pool
    {
        public string tag; // 풀의 이름
        public GameObject prefab; // 풀에 저장될 오브젝트의 프리팹
        public int size; // 풀에 미리 생성해둘 오브젝트의 개수
    }

    public List<Pool> pools; // 관리할 모든 풀의 리스트

    private Dictionary<string, Queue<GameObject>> pooledObjects;
    private Dictionary<string, GameObject> prefabLookup;

    public override void Awake()
    {
        base.Awake();
        
        pooledObjects = new Dictionary<string, Queue<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            string prefabName = pool.prefab.name;

            if (pooledObjects.ContainsKey(prefabName))
            {
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, this.transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            pooledObjects.Add(prefabName, objectPool);
            prefabLookup.Add(prefabName, pool.prefab);
        }
    }
    
    void OnEnable()
    {
        if (PhotonNetwork.IsConnectedAndReady) 
        {
            PhotonNetwork.PrefabPool = this;
        }
    }
    // PhotonNetwork.Instantiate("프리팹이름", position, rotation) 호출 시 이 메서드가 사용됩니다.
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        // prefabId는 Resources 폴더에 있는 프리팹 이름입니다 (예: "Base_Cannon_Projectile").
        if (!pooledObjects.ContainsKey(prefabId))
        {
            GameObject newObj = GameObject.Instantiate(Resources.Load<GameObject>(prefabId));
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;   
            return newObj;
        }

        Queue<GameObject> objectPool = pooledObjects[prefabId];

        if (objectPool.Count == 0)
        {
            GameObject newObj = GameObject.Instantiate(prefabLookup[prefabId]);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            return newObj;
        }

        GameObject obj = objectPool.Dequeue();
        
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }
    
    // PhotonNetwork.Destroy(gameObject) 호출 시 이 메서드가 사용됩니다.
    public void Destroy(GameObject gameObject)
    {
        // gameObject의 원래 프리팹 이름을 알아야 합니다.
        // PhotonView를 통해 프리팹 이름을 얻을 수 있습니다.
        PhotonView pv = gameObject.GetComponent<PhotonView>();
        if (pv == null)
        {
            GameObject.Destroy(gameObject); 
            return;
        }

        string prefabName = pv.name.Replace("(Clone)", ""); 
        // pv.viewPrefabName은 Instantiate 시 사용된 Resources 경로 내의 프리팹 이름입니다.

        if (!pooledObjects.ContainsKey(prefabName))
        {
            GameObject.Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
        gameObject.transform.position = Vector3.one * 9999f; // 화면 밖으로 이동
        
        pooledObjects[prefabName].Enqueue(gameObject);
    }

    public GameObject GetObjectFromPool(string tag)
    {
        if (!pooledObjects.ContainsKey(tag))
        {
            return null;
        }
    
        Queue<GameObject> objectPool = pooledObjects[tag];
    
        if (objectPool.Count == 0)
        {
            return null;
        }
    
        GameObject obj = objectPool.Dequeue();
    
        obj.SetActive(true);
    
        return obj;
    }
    
    public void ReturnObjectToPool(string tag, GameObject obj)
    {
        if (!pooledObjects.ContainsKey(tag))
        {
            Destroy(obj);
            return;
        }
    
        obj.SetActive(false);
    
        pooledObjects[tag].Enqueue(obj); // 풀에 다시 넣기
    }
}