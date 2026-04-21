using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerSide
{
    Natural,
    Red,
    Blue
}
public enum HitType
{
    Straight,
    Down,
    Set
}

public class PlayerController : MonoBehaviour
{
    public PlayerIdentity Identity;
    public PlayerSide side;
    public bool facingRight = true;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDoubleJumpUsed;
    private bool isMoveable = true;
    [SerializeField]private Animator anim;

    [Header("Gravity Multipliers")]
    [SerializeField] float fallMultiplier = 2.5f;      
    [SerializeField] float lowJumpMultiplier = 2f; 
    [SerializeField] float defaultGravityScale = 1f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 moveInput;
    private HitType pendingHitType;

    public event Action<PlayerController,HitType> OnPlayerHit;

    public void Initialize(PlayerIdentity identity)
    {
        Identity = identity;
        this.side = Identity.side; 
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isMoveable)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isMoving", false);
            return; 
        }

        Vector2 movement = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;


        bool isActuallyMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        anim.SetBool("isMoving", isActuallyMoving);


        anim.SetBool("isGrounded", isGrounded);  
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.x > 0 && !facingRight)
        {
            flip();
        }
        else if (moveInput.x < 0 && facingRight)
        {
            flip();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isGrounded)
            {
                Debug.Log("Jumping");
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isGrounded = false;
                anim.SetTrigger("Jump");
            }
            else if (!isDoubleJumpUsed)
            {
                Debug.Log("Double Jump!");
                rb.linearVelocityY = 0f; 
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isDoubleJumpUsed = true;
            }
        }
    }

    public void HitAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            if (!isGrounded && Mathf.Abs(moveInput.x) > 0.1f)
            {
                anim.SetTrigger("DownwardHit");
                pendingHitType = HitType.Down;
                Debug.Log("Aerial Spike! (Down Hit)");
            }
            
            else
            {
                pendingHitType = HitType.Straight;
                anim.SetTrigger("Hit");
                Debug.Log("Straight Hit!");
            }
        }
    }

    public void ExecuteHit()
    {
        OnPlayerHit?.Invoke(this, pendingHitType);
    }

    public void SetAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            anim.SetTrigger("Set");
            pendingHitType = HitType.Set;
        }
    }


    public void flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void SetMoveable(bool state)
    {
        isMoveable = state;
        
        if (!isMoveable)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0; 
        }
        else
        {
            rb.gravityScale = 1; 
        }
    }
    void FixedUpdate()
    {

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);


        if (isGrounded && !wasGrounded)
        {
            isDoubleJumpUsed = false;
        }


        if (!isMoveable) return; 

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }
}
