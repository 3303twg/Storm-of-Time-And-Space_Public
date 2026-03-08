using Photon.Pun;
using UnityEngine;

public class EnemyAttackState_Kiting : EnemyState
{
    EnemyStat stat;
    public EnemyAttackState_Kiting(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        //속도조절
        enemyController.aiPath.maxSpeed = enemyController.enemyStat.speed * 1.2f;

        enemyController.aiPath.canMove = true;
        enemyController.targetPos.position = enemyController.transform.position + -enemyController.transform.up * Random.Range(1f, 3f);
        enemyController.setter.target = enemyController.targetPos;

        stat = enemyController.enemyStat;
        //얼마나 이상태를 수행할건지
        enemyController.endStateTime = Random.Range(enemyController.timeRange.x, enemyController.timeRange.y);
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
