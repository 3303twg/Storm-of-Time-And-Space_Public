using Photon.Pun;
using UnityEngine;


public class BossStage : MonoBehaviourPun
{
    [SerializeField] private EnemyStatSo _enemyStatSo;

    public void SpawnBoss()
    {
        //Instantiate(bossMonster, bossMonster.transform.position, Quaternion.identity);
        //PhotonNetwork.Instantiate(bossMonster.name, bossMonster.transform.position, Quaternion.identity); // 보스 몬스터 생성
        if (!PhotonNetwork.IsMasterClient) return;
        
        GameObject go = PhotonNetwork.Instantiate(_enemyStatSo.enemyPrefab.name, transform.position, Quaternion.identity); //보스몬스터 생성
        go.GetComponent<EnemyController>().enemyStat = go.GetComponent<EnemyController>().ConvertSoToStat(_enemyStatSo);
    }

}
