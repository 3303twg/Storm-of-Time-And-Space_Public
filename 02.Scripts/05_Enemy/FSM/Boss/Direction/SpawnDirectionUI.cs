using System.Collections;
using UnityEngine;

public class SpawnDirectionUI : MonoBehaviour
{
    public RectTransform top;
    public RectTransform bottom;

    public float distance = 200f;
    public Vector2 targetOffset;  // 이동할 거리
    public float duration = 1.0f;  // 걸리는 시간

    private void OnEnable()
    {
        EventBus.Subscribe(EventBusType.Spotlight, StartMoving);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.Spotlight, StartMoving);
    }
    private void Awake()
    {
        targetOffset = new Vector2(0, distance);
        //StartMoving(null);
    }

    public void StartMoving(object obj)
    {
        StartCoroutine(MoveRect());
        Invoke("ReturnMoveRoutine", 4f);
    }

    public void ReturnMoveRoutine()
    {
        StartCoroutine(ReturnMoveRect());
    }
    private IEnumerator MoveRect()
    {
        Vector2 topStartPos = top.anchoredPosition;
        Vector2 topEndPos = topStartPos - targetOffset;

        Vector2 bottomStartPos = bottom.anchoredPosition;
        Vector2 bottomEndPos = bottomStartPos - (targetOffset * -1);

        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            top.anchoredPosition = Vector2.Lerp(topStartPos, topEndPos, timeElapsed / duration);
            bottom.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomEndPos, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        top.anchoredPosition = topEndPos;
        bottom.anchoredPosition = bottomEndPos;
        
    }

    private IEnumerator ReturnMoveRect()
    {
        Vector2 topStartPos = top.anchoredPosition;
        Vector2 topEndPos = topStartPos + targetOffset;

        Vector2 bottomStartPos = bottom.anchoredPosition;
        Vector2 bottomEndPos = bottomStartPos + (targetOffset * -1);

        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            top.anchoredPosition = Vector2.Lerp(topStartPos, topEndPos, timeElapsed / duration);
            bottom.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomEndPos, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        top.anchoredPosition = topEndPos;
        bottom.anchoredPosition = bottomEndPos;
    }
}