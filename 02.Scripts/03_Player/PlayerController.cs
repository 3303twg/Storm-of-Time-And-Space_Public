using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PhotonView photonView;

    public float moveSpeed = 1f;

    public float moveX;
    public float moveY;

    public Vector2 moveDir;
    public bool isInputShift;

    public bool isJump;
    public bool isRideLadder;
    public bool isGameEnd;

    [SerializeField] private bool canMove = true;

    public bool tryInteractionUiOn = false;

    private Rigidbody2D _rigidbody;

    public float jumpPower = 2f;
    public float climbSpeed = 0.6f;

    [SerializeField] private LayerMask groundLayer;

    private float originalGravity = 2.0f;

    private IInteractable _interactable;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (!photonView.IsMine)
        {
            _rigidbody.gravityScale = 0f;
        }
        else
        {
            spriteRenderer.sortingOrder += 10;
        }
        
        photonView.RPC("RPC_SetParentToShip", RpcTarget.AllBuffered);
    }
    

    void Update()
    {
        if (photonView.IsMine)
        {
            if (canMove && !isGameEnd)
            {
                Movement();
                Jump();
            }

            if (_rigidbody.velocity.y > 0.1f || !isGrounded())
            {

            }
            else
            {
                isJump = false; // 착지 상태
            }

            if (_interactable != null)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    canMove = _interactable.TryInteract(this);
                    tryInteractionUiOn = false;
                    moveX = 0.0f;
                    moveY = 0.0f;
                }
            }
        }
    }

    private void Movement()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        if (isRideLadder)
        {
           moveY = Input.GetAxisRaw("Vertical");
           if (_rigidbody.gravityScale != 0)
            {
                originalGravity = _rigidbody.gravityScale;
            }

            _rigidbody.gravityScale = 0f; // 중력 제거
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, moveY * climbSpeed);
        }
        else
        {
            moveY = 0;
            _rigidbody.gravityScale = originalGravity; // 중력 복원
        }

        if (Input.GetButton("isShift")) //Input Manager의 left shift부분
        {
            isInputShift = true;
        }
        else
        {
            isInputShift = false;

            moveDir = new Vector2(moveX, moveY).normalized;

            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14)
        {
            isRideLadder = true;
            isJump = false;
        }
        
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            _interactable = interactable;
            tryInteractionUiOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14)
        {
            isRideLadder = false;
            isJump = true;
        }

        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            _interactable = null;
            tryInteractionUiOn = false;
        }
    }

    private void Jump()
    {
        if(!isRideLadder && isGrounded() && Input.GetButtonDown("Jump"))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpPower);
            isJump = true;
        }
    }

    private bool isGrounded()
    {
        float lineLength = 1.5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, lineLength, groundLayer);

        Debug.DrawRay(transform.position, Vector2.down *  lineLength, Color.yellow);

        return hit.collider != null;
    }
    
    [PunRPC]
    public void RPC_SetParentToShip()
    {
        transform.SetParent(GameManager.Instance.Ship.transform);
    }


}
