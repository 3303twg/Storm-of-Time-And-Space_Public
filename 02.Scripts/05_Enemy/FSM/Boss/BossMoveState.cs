using UnityEngine;

public class BossMoveState : EnemyState
{
    public BossMoveState(EnemyController enemyController, EnemyStateMachine stateMachine) 
        : base(enemyController, stateMachine){}

    public override void Enter()
    {

    }
    public override void Update()
    {
        enemyController.LookTargetNow();
        enemyController.transform.position += enemyController.transform.up * enemyController.enemyStat.speed * Time.deltaTime;

        if(enemyController.distance < enemyController.enemyStat.attackRange)
        {
            stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
        }
    }


    public override void Exit()
    {

    }

}
