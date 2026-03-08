using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }



    public override void Enter()
    {
        Debug.Log("Idle 상태 진입");
        // 애니메이션 시작 등
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
    }

    public override void Exit()
    {
        Debug.Log("Idle 상태 종료");
    }



}
