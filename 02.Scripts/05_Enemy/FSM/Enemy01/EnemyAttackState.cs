using UnityEngine;

public class EnemyAttackState : EnemyState
{
    int index = 0;
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }
    public override void Enter()
    {

        //흠 나중에 자동화 해야할듯?? 근데 기준을 뭐로잡지?
        index = Random.Range(0, 7);

        if (enemyController.attackFlag == true)
        {
            switch (index)
            {
                case 0:
                    stateMachine.ChangeState(new EnemyAttackState_Normal(enemyController, stateMachine));
                    break;
                case 1:
                    stateMachine.ChangeState(new EnemyAttackState_Strafing(enemyController, stateMachine));
                    break;
                case 2:
                    stateMachine.ChangeState(new EnemyAttackState_Kiting(enemyController, stateMachine));
                    break;
                case 3:
                    stateMachine.ChangeState(new EnemyAttackState_Advance(enemyController, stateMachine));
                    break;
                case 4:
                    stateMachine.ChangeState(new EnemyAttackState_Strafing(enemyController, stateMachine));
                    break;
                case 5:
                    stateMachine.ChangeState(new EnemyAttackState_Strafing(enemyController, stateMachine));
                    break;
                case 6:
                    stateMachine.ChangeState(new EnemyAttackState_Strafing(enemyController, stateMachine));
                    break;
            }
        }
        else
        {
            stateMachine.ChangeState(new EnemyMoveState(enemyController, stateMachine));
        }
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        
    }

    

    
    
}
