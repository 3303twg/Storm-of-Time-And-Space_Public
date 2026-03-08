using Photon.Pun;
using UnityEngine;

public class Singleton<T> : MonoBehaviourPun where T : Component // T는 반드시 Component 클래스만 가능
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                // FindObjectOfType<T>()은 지정된 타입의 첫번째로 로드된 오브젝트를 검색

                if (_instance == null) // 검색된 오브젝트가 없으면
                {
                    GameObject obj = new GameObject(); // 새로운 GameObject를 생성
                    obj.name = typeof(T).Name; // GameObject의 이름을 T타입으로
                    _instance = obj.AddComponent<T>(); // 컴포넌트 추가
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T; // 최초 Awake인 경우 자신을 인스턴스로 설정
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
