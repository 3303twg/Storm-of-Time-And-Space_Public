using System.Collections.Generic;
using UnityEngine;

public class WormSegment : MonoBehaviour
{
    public Transform target;       // 따라갈 부모 Transform (앞 Segment)
    public float followSpeed = 10f;  // 따라가는 속도
    public float rotationSpeed = 10f; // 회전 보간 속도
    private float minDistance;

    public EnemyController head;

    public int index;
    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            //반지름
            minDistance = sr.bounds.size.x;
        }
    }
    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > minDistance)
        {
            // 최소 거리보다 멀면 target 쪽으로 이동
            Vector3 moveDir = direction.normalized;
            float moveAmount = (distance - minDistance);

            // 이동량에 따라 보간
            transform.position = Vector3.Lerp(transform.position, target.position - moveDir * minDistance, Time.deltaTime * followSpeed);
        }
        // 최소거리 위치유지

        // 항상 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public EnemyController GetHead(HashSet<Transform> visited = null)
    {
        if (visited == null)
            visited = new HashSet<Transform>();

        if (target == null || visited.Contains(target))
            return null;

        visited.Add(target);

        if (target.TryGetComponent<EnemyController>(out head))
        {
            return head;
        }

        WormSegment segment = target.GetComponent<WormSegment>();
        if (segment != null)
        {
            return segment.GetHead(visited);  // 재귀 + 방문 체크
        }

        return null;
    }

}