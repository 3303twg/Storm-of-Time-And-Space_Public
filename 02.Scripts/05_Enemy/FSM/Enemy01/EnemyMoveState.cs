using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        enemyController.aiPath.canMove = true;
    }

    public override void Update()
    {
        enemyController.LookTargetNow();
        //MoveToTarget();
        if (enemyController.distance < enemyController.enemyStat.attackRange)
        {
            if (enemyController.attackFlag == true)
            {
                stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
            }
        }
    }

    public override void Exit()
    {
        enemyController.aiPath.canMove = false;
    }


    public void MoveToTarget()
    {
        if(enemyController.distance > enemyController.enemyStat.attackRange)
        {
            enemyController.transform.position += enemyController.transform.up * enemyController.enemyStat.speed * Time.deltaTime;
        }
    }

}
