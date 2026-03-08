using Photon.Pun;
using UnityEngine;

public class StageManager : MonoBehaviourPun
{
    private SpawnerManager _spawnerManager;

    private BossStage _bossStage;

    [SerializeField] private int remainingSpawners = 4;


    private void Start()
    {
        _spawnerManager = GetComponent<SpawnerManager>();
        _bossStage = GetComponent<BossStage>();
    }

    public void CreateFirstSpawner()
    {
        _spawnerManager.SpawnAtRandomPosition(1);   
    }

    public void CreateFromSecondSpawner()
    {
        if (remainingSpawners > 0)
        {
            _spawnerManager.SpawnAtRandomPosition(2);
        }
    }

    public void OnSpawnerDestroyed()
    {
        remainingSpawners--;
        
        if (remainingSpawners == 0)
        {
            SoundManager.Instance.PlayBossBGM();
            //보스 연출
            _bossStage.SpawnBoss();
        }
    }
}
