using UnityEngine;
using Photon.Pun;

public class SpawnerManager : MonoBehaviourPun
{
    /// <summary>
    /// SpawnerManager : Spawner를 Spawn하는 역할
    /// </summary>

    [SerializeField] private Vector2 spawnAreaRangeX;
    [SerializeField] private Vector2 spawnAreaRangeY;

    [SerializeField] private float checkRadius = 7.5f;
    [SerializeField] private LayerMask checkLayer;

    [SerializeField] private int maxAttempts = 100;

    [SerializeField] private SpawnerData[] _spawnerDatas;


    public void SpawnAtRandomPosition(int trySpawn)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (trySpawn == 1)
        {
            spawnAreaRangeX = new Vector2(30.0f, 100.0f);
            spawnAreaRangeY = new Vector2(30.0f, 100.0f);
        }
        else
        {
            spawnAreaRangeX = new Vector2(0.0f, 100.0f);
            spawnAreaRangeY = new Vector2(0.0f, 100.0f);
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(spawnAreaRangeX.x, spawnAreaRangeX.y);
            float randomY = Random.Range(spawnAreaRangeY.x, spawnAreaRangeY.y);

            Vector3 randomPosition = new Vector3(randomX, randomY, 0);

            bool isOverlapping = Physics2D.OverlapCircle(randomPosition, checkRadius, checkLayer);

            if (!isOverlapping)
            {
                //Instantiate(_spawnerDatas[0].spawnerObject, randomPosition, Quaternion.identity);
                PhotonNetwork.Instantiate(_spawnerDatas[0].spawnerObject.name, randomPosition, Quaternion.identity); // 스포너 생성
                Debug.Log($"Spawned at {randomPosition} after {attempt + 1} attempt(s)");
                break;
            }
            else
            {
                if (attempt == maxAttempts - 1)
                {
                    Debug.Log("시도 가능 횟수 초과");
                }
            }
        }
    }
}       