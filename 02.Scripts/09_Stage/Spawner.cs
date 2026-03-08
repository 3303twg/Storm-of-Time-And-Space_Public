using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviourPun, IDamageable, IPunObservable

{
    /// <summary>
    /// Spawner : EnemySpawner의 역할
    /// </summary>
    [SerializeField] private SpawnerData _spawnerData; // 스포너의 데이터

    [SerializeField] private float spawnerHp; // 스포너의 체력

    [SerializeField] private GameObject ship; // 기체

    [SerializeField] private EnemyStatSo[]  _enemyStatSo; //몬스터의 데이터 배열

    [SerializeField] private bool isBossSpawn = false;


    [SerializeField] private float standardDistance = 30.0f; // 몬스터를 스폰하기 위한 기체와 스포너 사이 거리

    [SerializeField] private Vector2 spawnAreaX; // 몬스터가 스폰될 수 있는 지역 x좌표
    [SerializeField] private Vector2 spawnAreaY; // 몬스터가 스폰될 수 있는 지역 y좌표

    [SerializeField] private int spawnAreaLength = 7; // spawnArea 의 한 변의 길이 / 2

    [SerializeField] private float checkRadius = 5.0f; // 무언가가 있는지 확인하기 위한 원의 길이 (조사 레이어 : Obstacle, ship, monster)
    [SerializeField] private LayerMask checkLayer; // 확인하는 레이어 종류

    [SerializeField] private int maxSpawnMonster = 5; //스폰 가능한 최대 몬스터 수
    [SerializeField] private int maxAttempts = 10; // 생성 시도 최대 횟수

    [SerializeField] private int spawnCooldown = 10; // 생성 주기


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(spawnerHp);
        }
        else
        {
            spawnerHp = (float)stream.ReceiveNext();
        }
    }


    private void Start() 
    {
        spawnerHp = _spawnerData.spawnerHp;
        ship = GameManager.Instance.Ship.gameObject;
    }

    private void ComputeDistance()
    {
        float currentDistance = Vector3.Distance(transform.position, ship.transform.position); //현재 거리

        if (currentDistance < standardDistance && !isBossSpawn)
        {
            StartCoroutine(SpawnDelay()); 
        }
    }

    private void Update()
    {
        if (ship != null)
        {
            ComputeDistance();
            DestroySpawner();
        }
    }

    IEnumerator SpawnDelay()
    {
        isBossSpawn = true;
        SpawnEnemy();
        yield return new WaitForSeconds(spawnCooldown);
        isBossSpawn = false;
    }

    private void SpawnEnemy()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        for (int i = 0; i < maxSpawnMonster; i++)
        {
            spawnAreaX = new Vector2(transform.position.x - spawnAreaLength, transform.position.x + spawnAreaLength);
            spawnAreaY = new Vector2(transform.position.y - spawnAreaLength, transform.position.y + spawnAreaLength);

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                float randomX = Random.Range(spawnAreaX.x, spawnAreaX.y);
                float randomY = Random.Range(spawnAreaY.x, spawnAreaY.y);

                Vector3 randomPosition = new Vector3(randomX, randomY, 0);

                bool isOverlapping = Physics2D.OverlapCircle(randomPosition, checkRadius, checkLayer);

                if (!isOverlapping)
                {
                    //Instantiate(_enemyStatSo[0].enemyPrefab, randomPosition, Quaternion.identity);
                    int randIndex = Random.Range(0, _enemyStatSo.Length - 1);
                    GameObject go = PhotonNetwork.Instantiate(_enemyStatSo[randIndex].enemyPrefab.name, randomPosition, Quaternion.identity); //몬스터 생성
                    go.GetComponent<EnemyController>().enemyStat = go.GetComponent<EnemyController>().ConvertSoToStat(_enemyStatSo[randIndex]);
                    break;
                }
            }
        }
    }

    private void DestroySpawner()
    {
        if (spawnerHp <= 0)
        {
            FindObjectOfType<StageManager>().OnSpawnerDestroyed();
            FindObjectOfType<StageManager>().CreateFromSecondSpawner();

            PhotonNetwork.Destroy(gameObject);
        }
    }
    
    public void OnDamage(float value)
    {
        spawnerHp -= value;
    }

}
