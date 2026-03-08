using Photon.Pun;
using UnityEngine;

public class BossAttackState_DestructionBeam : EnemyState
{
    Vector2 leftPos;
    Vector2 rightPos;

    float rotationDuration = 3f;
    float elapsedTime = 0f;

    float startAngle;
    float targetAngle;

    LaserController laser;

    public BossAttackState_DestructionBeam(EnemyController enemyController, EnemyStateMachine stateMachine)
        : base(enemyController, stateMachine) { }

    public override void Enter()
    {
        laser = enemyController.GetComponent<LaserController>();
        Vector2 dir = (Vector2)(enemyController.transform.position - enemyController.target.transform.position);
        Vector2 perpendicularLeft = new Vector2(-dir.y, dir.x);
        Vector2 perpendicularRight = new Vector2(dir.y, -dir.x);
        leftPos = (Vector2)enemyController.target.transform.position + perpendicularLeft.normalized * 20f;
        rightPos = (Vector2)enemyController.target.transform.position + perpendicularRight.normalized * 20f;


        // 시작 위치 방향 (leftPos)
        Vector2 startDir = leftPos - (Vector2)enemyController.transform.position;
        enemyController.transform.up = startDir.normalized;

        // 시작 각도 (현재 transform.up 기준)
        startAngle = enemyController.transform.eulerAngles.z;

        // 목표 각도 (rightPos 방향)
        Vector2 targetDir = rightPos - (Vector2)enemyController.transform.position;
        targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;

        elapsedTime = 0f;

        laser.laser.SetActive(true);

        enemyController.photonView.RPC("RPC_LaserOn", RpcTarget.All);
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / rotationDuration);

        float angle = Mathf.LerpAngle(startAngle, targetAngle, t);

        enemyController.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (t >= 1f)
        {
            laser.laser.SetActive(false);
            stateMachine.ChangeState(new BossAttackState(enemyController, stateMachine));
        }
    }

    public override void Exit()
    { 
        enemyController.photonView.RPC("RPC_LaserOff", RpcTarget.All);
    }
}
