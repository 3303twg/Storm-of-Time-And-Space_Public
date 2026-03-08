using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        /*
        if (enemyController.IsPlayerInRange())
        {
            //stateMachine.ChangeState(new EnemyChaseState(enemyController, stateMachine));
        }
        */
        
        if (enemyController.distance < enemyController.enemyStat.attackRange)
        {
            stateMachine.ChangeState(new EnemyAttackState(enemyController, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new EnemyMoveState(enemyController, stateMachine));
        }
        
    }

    public override void Exit()
    {

    }
}