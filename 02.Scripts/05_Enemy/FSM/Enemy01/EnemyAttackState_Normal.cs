using UnityEngine;

public class EnemyAttackState_Normal : EnemyState
{
    EnemyStat stat;

    bool isGuidedAttack;

    public EnemyAttackState_Normal(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        isGuidedAttack = Random.value < 0.5f;
        if (isGuidedAttack)
        {
            enemyController.time = Time.time + 1.5f;
        }
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


        enemyController.LookTargetNow();
        SafeDistance();

        
        if (enemyController.distance < stat.attackRange)
        {
            if (isGuidedAttack == false)
            {
                if (Time.time > enemyController.time)
                {
                    enemyController.time = Time.time + stat.attackSpeed;
                    //원거리공격
                    enemyController.shot();
                    //LongRangeAttack();
                }
            }

            else
            {
                
                if (Time.time > enemyController.time)
                {
                    enemyController.time = Time.time + stat.attackSpeed;
                    //원거리공격
                    enemyController.shot();
                    //GuidedAttack();
                }
            }
        }
        else if (enemyController.distance > stat.attackRange)
        {
            stateMachine.ChangeState(new EnemyIdleState(enemyController, stateMachine));
        }
    }
    public override void Exit() { }

/*
    public void GuidedAttack()
    {
        //일단은 투사체 고정
        GameObject go = PhotonNetwork.Instantiate("Prefabs/Projectile/GuidedBullet", enemyController.transform.position, enemyController.transform.rotation);
        go.GetComponent<Bullet>().damage = enemyController.enemyStat.attackPower;
    }
*/

    public void SafeDistance()
    {
        if (enemyController.distance < stat.safeDistance)
        {
            stateMachine.ChangeState(new EnemyAttackState_Kiting(enemyController, stateMachine));
        }
    }


}
