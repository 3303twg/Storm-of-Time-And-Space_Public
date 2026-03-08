using UnityEngine;

public class EnemyAttackState_Strafing : EnemyState
{
    EnemyStat stat;

    float speed;

    public EnemyAttackState_Strafing(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        //속도조절
        enemyController.aiPath.maxSpeed = enemyController.enemyStat.speed * 1.4f;

        Vector2 randomPosition = new Vector2(
            Random.Range(enemyController.transform.position.x - 5f, enemyController.transform.position.x + 5f),
            Random.Range(enemyController.transform.position.y - 5f, enemyController.transform.position.y + 5f)
        );

        enemyController.aiPath.canMove = true;
        enemyController.targetPos.position = randomPosition;

        enemyController.setter.target = enemyController.targetPos;

        stat = enemyController.enemyStat;
        //2초~5초동안 무빙샷
        enemyController.endStateTime = Random.Range(enemyController.timeRange.x, enemyController.timeRange.y);
        //direction = Random.Range(-)

        speed = stat.speed / (Random.Range(2f, 3.5f));
    }

    public override void Update()
    {
        enemyController.endStateTime -= Time.deltaTime;
        if (enemyController.endStateTime < 0)
        {
            stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
        }
        if (enemyController.aiPath.reachedEndOfPath)
        {
            stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
        }

        enemyController.LookTargetNow();
        SafeDistance();//안전거리유지

        if (enemyController.distance < stat.attackRange)
        {
            if (Time.time > enemyController.time)
            {
                enemyController.time = Time.time + stat.attackSpeed;
                //원거리공격
                enemyController.shot();
                //LongRangeAttack();
            }
        }
        else if (enemyController.distance > stat.attackRange)
        {
            stateMachine.ChangeState(new EnemyIdleState(enemyController, stateMachine));
        }
    }
    public override void Exit() {
        enemyController.aiPath.maxSpeed = enemyController.enemyStat.speed;
        enemyController.aiPath.canMove = false;
        enemyController.setter.target = enemyController.target;
    }

    public void SafeDistance()
    {
        if (enemyController.distance < stat.safeDistance)
        {
            stateMachine.ChangeState(new EnemyAttackState_Kiting(enemyController, stateMachine));
        }
    }

}
