using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;
    private CinemachineConfiner2D confiner;
    
    private WaitForSeconds ws = new WaitForSeconds(0.1f);
    
    #region Zoom

    [Header("Zoom")]
    [SerializeField] private float zoomInSize = 5f;
    [SerializeField] private float zoomOutSize = 8f;
    [SerializeField] private float zoomDuration = 0.3f;
    
    private Coroutine zoomRoutine;
    #endregion Zoom
    
    #region Shake
    [Space(10)]
    [Header("Shake")]
    private float shakeTimeRemaining;
    private bool isInit = false;

    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeFrequency;
    
    private Coroutine shakeRoutine;
    #endregion Shake

    
    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        confiner = GetComponent<CinemachineConfiner2D>();
    }
    
    void OnEnable()
    {
        EventBus.Subscribe(EventBusType.CameraZoomChange, CameraZoomChange);
        EventBus.Subscribe(EventBusType.CameraSpotlight, Spotlight);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.CameraZoomChange, CameraZoomChange);
        EventBus.Subscribe(EventBusType.CameraSpotlight, Spotlight);
    }

    void Start()
    {
        if (isInit == false) Init();
    }
    
    private void Init()
    {
        virtualCamera.m_Lens.OrthographicSize = zoomInSize;
        
        confiner.m_BoundingShape2D = GameManager.Instance.borderCollider;
        confiner.InvalidateCache();
        confiner.m_MaxWindowSize = 0f;
        
        perlin.m_AmplitudeGain = 0f;
        perlin.m_FrequencyGain = 0f;
        
        isInit = true;
    }
    

    private IEnumerator SetShipFollow()
    {
        if (GameManager.Instance.Ship == null)
        {
            yield return ws;
        }

        virtualCamera.Follow = GameManager.Instance.Ship.transform;
    }

    private void CameraZoomChange(object isRide)
    {
        float targetSize = (bool)isRide ? zoomOutSize : zoomInSize;

        if (zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);   
        }
        
        zoomRoutine = StartCoroutine(ChangeCameraZoomRoutine(targetSize));
    }
    
    private IEnumerator ChangeCameraZoomRoutine(float targetSize)
    {
        float currentZoomTime = 0f; // 줌 시간
        float startSize = virtualCamera.m_Lens.OrthographicSize;

        while (currentZoomTime < zoomDuration)
        {
            currentZoomTime += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, currentZoomTime / zoomDuration);
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    public void Spotlight(object obj)
    {
        
        virtualCamera.Follow = (Transform)obj;
        StartCoroutine(SpotlightRoutine());
    }

    private IEnumerator SpotlightRoutine()
    {
        float currentZoomTime = 0f; // 줌 시간
        float startSize = virtualCamera.m_Lens.OrthographicSize;

        while (currentZoomTime < 5f)
        {
            currentZoomTime += Time.deltaTime;
            //virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, currentZoomTime / duration);
            yield return null;
        }

        virtualCamera.Follow = GameManager.Instance.Ship.transform;
    }

    public void ShakeCamera(float _duration)
    {
        if (isInit == false) Init();
        
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(ShakeCameraRoutine(_duration));
    }

    private IEnumerator ShakeCameraRoutine(float _duration)
    {
        shakeTimeRemaining = _duration;
        perlin.m_AmplitudeGain = shakeAmplitude;
        perlin.m_FrequencyGain = shakeFrequency;

        while (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        StopShake();
    }

    private void StopShake()
    {
        shakeTimeRemaining = 0f;
        perlin.m_FrequencyGain = 0f;
        perlin.m_AmplitudeGain = 0f;
    }

}
