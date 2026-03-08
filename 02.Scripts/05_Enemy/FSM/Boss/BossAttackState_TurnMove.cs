using UnityEngine;
public class BossAttackState_TurnMove : EnemyState
{
    public BossAttackState_TurnMove(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    float rotateSpeed = 30f;
    int rotateDirection;

    public override void Enter()
    {
        rotateSpeed = Random.Range(10, 110);
        enemyController.endStateTime = Random.Range(enemyController.timeRange.x, enemyController.timeRange.y);
        enemyController.endStateTime = Random.Range(0.3f, 1.0f);

        //방향 결정
        rotateDirection = (Random.value > 0.5f) ? 1 : -1;
    }

    public override void Update()
    {
        enemyController.endStateTime -= Time.deltaTime;
        if (enemyController.endStateTime < 0)
        {
            stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
            return;
        }

        // 회전
        float angle = rotateSpeed * Time.deltaTime * rotateDirection;
        enemyController.transform.Rotate(0, 0, angle);

        if(enemyController.distance > enemyController.enemyStat.attackRange / 2)
        {
            enemyController.LookTarget();
        }
        // 현재 바라보는 방향으로 이동
        enemyController.transform.position += enemyController.transform.up * enemyController.enemyStat.speed * Time.deltaTime;
    }

    public override void Exit() { }
}
