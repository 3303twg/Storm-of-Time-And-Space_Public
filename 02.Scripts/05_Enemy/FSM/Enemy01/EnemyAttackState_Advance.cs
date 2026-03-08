using UnityEngine;

public class EnemyAttackState_Advance : EnemyState
{
    EnemyStat stat;

    public EnemyAttackState_Advance(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        enemyController.aiPath.maxSpeed = enemyController.enemyStat.speed * 0.9f;
        enemyController.aiPath.canMove = true;
        enemyController.targetPos.position = enemyController.transform.position + enemyController.transform.right * Random.Range(1f, 3f);
        enemyController.setter.target = enemyController.targetPos;

        stat = enemyController.enemyStat;
        //얼마나 이상태를 수행할건지
        enemyController.endStateTime = Random.Range(0.5f, 1.5f);
    }
    public override void Update()
    {
        enemyController.endStateTime -= Time.deltaTime;
        if (enemyController.endStateTime < 0)
        {
            if (enemyController.attackFlag == true)
            {
                stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
            }
            else
            {
                stateMachine.ChangeState(new EnemyMoveState(enemyController, stateMachine));
            }
        }
        if (enemyController.aiPath.reachedEndOfPath)
        {
            stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
        }


        enemyController.LookTarget();
        SafeDistance();

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
    public override void Exit()
    {
        enemyController.aiPath.maxSpeed = enemyController.enemyStat.speed;
        enemyController.aiPath.canMove = true;
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

