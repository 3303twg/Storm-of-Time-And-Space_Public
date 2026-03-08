using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed;
    public Transform[] backgrounds;

    private float leftPosX;
    private float rightPosX;
    private float xScreenHalfSize;
    private float backgroundWidth;

    void Start()
    {
        float yScreenHalfSize = Camera.main.orthographicSize;
        xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;

        // 배경 하나의 너비 구하기 (SpriteRenderer 기준)
        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            backgroundWidth = sr.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer가 없습니다. backgroundWidth를 수동으로 설정해야 합니다.");
            backgroundWidth = xScreenHalfSize * 2f; // 임시 fallback
        }

        // 화면 기준 왼쪽/오른쪽 기준 계산 (local 기준)
        leftPosX = -backgroundWidth * (backgrounds.Length / 2f + 0.5f);
        rightPosX = backgroundWidth * (backgrounds.Length - 1);
    }

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // LocalPosition 기준으로 이동
            Vector3 localPos = backgrounds[i].localPosition;
            localPos += Vector3.left * speed * Time.deltaTime;
            backgrounds[i].localPosition = localPos;

            // 왼쪽으로 나가면 오른쪽으로 워프
            if (localPos.x < leftPosX)
            {
                float maxX = GetRightmostLocalX();
                backgrounds[i].localPosition = new Vector3(maxX + backgroundWidth, localPos.y, localPos.z);
            }
        }
    }

    float GetRightmostLocalX()
    {
        float maxX = float.MinValue;
        foreach (Transform bg in backgrounds)
        {
            float x = bg.localPosition.x;
            if (x > maxX)
                maxX = x;
        }
        return maxX;
    }
}
