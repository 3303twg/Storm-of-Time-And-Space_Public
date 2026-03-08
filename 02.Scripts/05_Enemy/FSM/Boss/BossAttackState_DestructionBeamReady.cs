using UnityEngine;

public class BossAttackState_DestructionBeamReady : EnemyState
{
    public BossAttackState_DestructionBeamReady(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    float speedRatio = 3f;

    Vector3 rushDir;
    float rushTime = 1.3f;
    float endRushTime;

    public override void Enter()
    {
        rushDir = (enemyController.target.transform.position - enemyController.transform.position).normalized;
        enemyController.transform.up = rushDir;
        endRushTime = Time.time + rushTime;
        /*
        //플레이어 기준 랜덤위치 텔포
        float angle = Random.Range(0f, Mathf.PI * 2f); // 0 ~ 360도
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0); // XY 평면 기준
        Vector3 point = enemyController.target.transform.position + direction * radius;
        enemyController.transform.position = point;
        */
    }
    public override void Update()
    {
        if (endRushTime < Time.time)
        {
            endRushTime = Time.time + rushTime;

            stateMachine.ChangeState(new BossAttackState_DestructionBeam(enemyController, stateMachine));
        }
        enemyController.transform.position += rushDir * enemyController.enemyStat.speed * speedRatio * Time.deltaTime;
    }

    public override void Exit() { }


}
