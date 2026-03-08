using UnityEngine;

public class BossAttackState_Rush : EnemyState
{

    public BossAttackState_Rush(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }


    private Vector3 rushDir;

    int rushCnt = 2;
    float radius = 50f; // 반지름

    float rushTime = 4f;
    float endRushTime;

    float speedRatio = 3f;

    float coolDown = 10f;
    float coolTime;
    public override void Enter()
    {
        rushDir = (enemyController.target.transform.position - enemyController.transform.position).normalized;
        endRushTime = Time.time + rushTime;

        if(Time.time < coolTime)
        {
            enemyController.stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
            return;
        }
    }

    public override void Update()
    {
        enemyController.endStateTime -= Time.deltaTime;
        /*
        if (enemyController.endStateTime < 0)
        {
            enemyController.stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
        }
        */
        if (endRushTime < Time.time)
        {
            endRushTime = Time.time + rushTime;
            //플레이어 기준 랜덤 위치 텔포 후 돌진
            float angle = Random.Range(0f, Mathf.PI * 2f); // 0 ~ 360도
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0); // XY 평면 기준
            Vector3 point = enemyController.target.transform.position + direction * radius;
            enemyController.transform.position = point;

            rushDir = (enemyController.target.transform.position - enemyController.transform.position).normalized;
            enemyController.transform.up = rushDir;
            //모든 돌진 후 텔포 까지 시킨후 체인지
            rushCnt--;
            if (rushCnt < 0)
            {
                enemyController.stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
                return;
            }
        }
        
        enemyController.transform.position += rushDir * enemyController.enemyStat.speed * speedRatio * Time.deltaTime;
        

    }

    public override void Exit() 
    {
        coolTime = Time.time + coolDown;
    }
}
