using System.Collections.Generic;
using System;

public class BossAttackState : EnemyState
{
    private List<Func<EnemyState>> attackStateFactories;
    private List<int> weights;  // 가중치 리스트
    private System.Random rng = new System.Random();

    public BossAttackState(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine)
    {
        attackStateFactories = new List<Func<EnemyState>>()
        {
            () => new BossAttackState_Chop(enemyController, stateMachine),
            () => new BossAttackState_Rush(enemyController, stateMachine),
            () => new BossAttackState_DestructionBeamReady(enemyController, stateMachine),
            () => new BossAttackState_BodyNormalAttack(enemyController, stateMachine),
            () => new BossAttackState_TurnMove(enemyController, stateMachine),
        };

        weights = new List<int>() { 6, 4, 9, 30, 120 };
        //weights = new List<int>() { 0, 0, 1, 0, 0 };
    }

    public override void Enter()
    {
        enemyController.attackFlag = enemyController.distance < enemyController.enemyStat.attackRange;

        if (!enemyController.attackFlag)
        {
            stateMachine.ChangeState(new BossMoveState(enemyController, stateMachine));
            return;
        }

        int totalWeight = 0;
        foreach (var w in weights)
        {
            totalWeight += w;
        }

        int randomValue = rng.Next(totalWeight);
        int cumulative = 0;
        int selectedIndex = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
            {
                selectedIndex = i;
                break;
            }
        }

        EnemyState nextState = attackStateFactories[selectedIndex]();
        stateMachine.ChangeState(nextState);
    }

    public override void Update() { }
    public override void Exit() { }
}
