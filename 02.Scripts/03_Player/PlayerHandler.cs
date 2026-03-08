using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerHandler : MonoBehaviourPun, IPunObservable
{
    private PlayerController _playerController;
    private SpriteRenderer _spriteRenderer;

    public Animator anim;

    [SerializeField] private float lastX;
    [SerializeField] private float lastY;

    public bool isFlipX;
    [SerializeField] private GameObject interactionUi;


    private void OnEnable()
    {
        EventBus.Subscribe(EventBusType.GameClearMotion, ClearGameMotion);
        EventBus.Subscribe(EventBusType.GameOverMotion, DiePlayer);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.GameClearMotion, ClearGameMotion);
        EventBus.Unsubscribe(EventBusType.GameOverMotion, DiePlayer);
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            ChangeMovementSprite();
            ChangeJumpSprite();
            ChangeInteractionUi();
        }

        _spriteRenderer.flipX = isFlipX;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFlipX);
        }
        else
        {
            isFlipX = (bool)stream.ReceiveNext();
        }
    }

    void ChangeMovementSprite()
    {
        GetInputValue();

        bool isMoving = _playerController.moveDir != Vector2.zero && !_playerController.isInputShift;
        anim.SetBool("IsMove", isMoving);
    }

    void ChangeJumpSprite()
    {
        GetInputValue();

        anim.SetBool("IsJump", _playerController.isJump);
    }

    void ChangeInteractionUi() // 상호작용 UI ( 사용하기 ) 출력
    {
        interactionUi.gameObject.SetActive(_playerController.tryInteractionUiOn);
    }

    void GetInputValue()
    {
        float currentX = _playerController.moveX;
        float currentY = _playerController.moveY;

        if (currentX != 0 || currentY != 0) //움직일 때
        {
            lastX = currentX;
            lastY = currentY;
        }

        if (!_playerController.isRideLadder)
        {
            anim.SetFloat("PosX", lastX);
        }
        else
        {
            lastX = 0;
            anim.SetFloat("PosX", lastX);
        }
            anim.SetFloat("PosY", lastY);

        if (lastX > 0)
        {
            isFlipX = true;
        }
        else if (lastX < 0)
        {
            isFlipX = false;
        }
    }

    public void ClearGameMotion(object obj)
    {

        anim.SetBool("IsClear", true);

        Invoke("PublishEndingEvent", 5f);
    }

    public void PublishEndingEvent()
    {
        EventBus.Publish(EventBusType.GameClear, 0f);
    }

    public void DiePlayer(object obj)
    {
        
        anim.SetBool("IsDie", true);

        StartCoroutine(DieAfterAnimation());

        //Invoke("PublishGameOverEvent", 5f);

    }
    public void PublishGameOverEvent()
    {
        //EventBus.Publish(EventBusType.GameOver, 0f);
    }

    IEnumerator DieAfterAnimation()
    {
        yield return new WaitForSeconds(2.5f);
        EventBus.Publish(EventBusType.GameOver, 0f);
        Destroy(gameObject);
    }
}
