using UnityEngine;

public class BossAttackState_Chop : EnemyState
{
    Vector3 startPos;
    Vector3 targetPos;
    float timer;

    float rotateDuration = 0.4f;
    float chopDuration = 0.5f;
    float delayValue = 0.2f; // 딜레이 시간 1초

    Quaternion startRotation;
    Quaternion targetRotation;

    enum ChopPhase { Rotating, Moving, Waiting }
    ChopPhase phase = ChopPhase.Rotating;

    float delayStartTime;

    int cnt;
    public BossAttackState_Chop(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        cnt = Random.Range(3, 6);
        InitChop();
    }

    void InitChop()
    {
        startPos = enemyController.transform.position;
        targetPos = enemyController.target.transform.position;

        Vector3 dir = targetPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        startRotation = enemyController.transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, angle);

        timer = 0f;
        phase = ChopPhase.Rotating;
    }

    public override void Update()
    {
        if(enemyController.distance > enemyController.enemyStat.attackRange / 3)
        {
            enemyController.LookTarget();
            enemyController.transform.position += enemyController.transform.up * enemyController.enemyStat.speed * Time.deltaTime;
        }
        timer += Time.deltaTime;

        if(cnt <= 0)
        {
            stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
            return;
        }
        
        switch (phase)
        {
            case ChopPhase.Rotating:
                {
                    float t = Mathf.Clamp01(timer / rotateDuration);
                    enemyController.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                    if (t >= 1f)
                    {
                        timer = 0f;
                        phase = ChopPhase.Moving;
                    }
                }
                break;

            case ChopPhase.Moving:
                {
                    float t = Mathf.Clamp01(timer / chopDuration);
                    enemyController.transform.position = Vector3.Lerp(startPos, targetPos, t);
                    if (t >= 1f)
                    {
                        timer = 0f;
                        phase = ChopPhase.Waiting;
                        delayStartTime = Time.time; // 대기 시작 시간 저장
                    }
                }
                break;

            case ChopPhase.Waiting:
                {
                    if (Time.time - delayStartTime >= delayValue)
                    {
                        cnt--;
                        InitChop(); // 다시 초기화해서 반복 시작
                    }
                }
                break;
        }
    }

    public override void Exit()
    {
        // 필요하면 초기화
    }
}
