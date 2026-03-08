using UnityEngine;

public class SpawnBody : MonoBehaviour
{
    public GameObject bodyPrefab;

    public int bodyCount = 5;
    public float scaleRatio = 0.8f;

    EnemyController enemyController;
    private void Awake()
    {
       enemyController = GetComponent<EnemyController>();
    }
    void Start()
    {
        for (int i = 0; i < bodyCount; i++)
        {
            GameObject go = Instantiate(bodyPrefab);
            enemyController.body.BodyController.Add(go.GetComponent<BodyController>());
            WormSegment wormSegment = go.GetComponent<WormSegment>();
            if (enemyController.body.BodyController.Count <= 1)
            {
                wormSegment.target = transform;
            }
            else
            {
                wormSegment.target = enemyController.body.BodyController[enemyController.body.BodyController.Count - 2].transform;
            }
            wormSegment.index = enemyController.body.BodyController.Count - 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
