using UnityEngine;

public class BossAttackState_BodyNormalAttack : EnemyState
{

    public BossAttackState_BodyNormalAttack(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }


    public override void Enter()
    {
        foreach (var body in enemyController.body.BodyController)
        {
            if (Random.value < 0.25f) {
                //body.LeftAttack();
            }
            else if (Random.value > 0.75f)
            {
                //body.RightAttack();
            }
        }

        stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
        //enemyController.body.BodyController[Random()]
    }
    public override void Update()
    {

    }

    public override void Exit()
    {

    }

}