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
    public bool isDead = false;
    private bool CanControl => isMoveable && !isDead && (GameFlowManager.Instance == null || GameFlowManager.Instance.CanPlayersMove());
    [SerializeField]public Animator anim;

    [Header("Gravity Multipliers")]
    [SerializeField] float fallMultiplier = 2.5f;      
    [SerializeField] float lowJumpMultiplier = 2f; 
    [SerializeField] float defaultGravityScale = 1f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Rare Idle Settings")]
    [SerializeField] private float minIdleTime = 5f;
    [SerializeField] private float maxIdleTime = 10f;
    [SerializeField] [Range(0, 100)] private float rareIdleChance = 20f;

    private float idleTimer;
    private float nextCheckTime;

    private Vector2 moveInput;
    private HitType pendingHitType;

    public event Action<PlayerController,HitType> OnPlayerHit;
    public event Action<PlayerController,HitType> OnPlayerFinishHit;

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
        anim.SetBool("isDead", isDead);
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);


        if (isGrounded && !wasGrounded)
        {
            isDoubleJumpUsed = false;
        }

        HandleRareIdle();

        if (!CanControl)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("isMoving", false);
            return; 
        }

        float currentHorizontalSpeed = moveInput.x * moveSpeed;

        if (anim.GetBool("isCrouch")) 
        {
            currentHorizontalSpeed = 0;
        }

        Vector2 movement = new Vector2(currentHorizontalSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
        CheckMoveDirection();


        bool isActuallyMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        anim.SetBool("isMoving", isActuallyMoving);


        anim.SetBool("isGrounded", isGrounded); 
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (!CanControl) return;
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
            if (!CanControl) return;
            if (isGrounded)
            {
                rb.linearVelocityY = 0f; 
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isGrounded = false;
                anim.SetTrigger("Jump");
                SoundManager.instance.PlaySound(SoundType.Jump);
            }
            else if (!isDoubleJumpUsed)
            {
                rb.linearVelocityY = 0f; 
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isDoubleJumpUsed = true;
                SoundManager.instance.PlaySound(SoundType.Jump);
            }
            
        }
    }

    public void HitAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!CanControl) return;
            if (!isGrounded && Mathf.Abs(moveInput.x) > 0.1f)
            {
                anim.SetTrigger("DownwardHit");
                pendingHitType = HitType.Down;
                SoundManager.instance.PlaySound(SoundType.Hit_Hard);
            }
            
            else
            {
                pendingHitType = HitType.Straight;
                anim.SetTrigger("Hit");
                SoundManager.instance.PlaySound(SoundType.Hit);
            }
        }
    }
    public void CrouchAction(InputAction.CallbackContext context)
    {
        if (!CanControl || !isGrounded) 
        {
            anim.SetBool("isCrouch", false);
            return;
        }

        if (context.started)
        {
            SoundManager.instance.PlaySound(SoundType.Crouch);
            // Do get Item Pickup logic here

        }
        if (context.performed)
        {
            anim.SetBool("isCrouch", true);
            // ใส่โค้ดลดขนาด Collider ตรงนี้
        }
        else if (context.canceled) // เมื่อปล่อยปุ่ม
        {
            anim.SetBool("isCrouch", false);
            // ใส่โค้ดคืนขนาด Collider ตรงนี้
        }
    }

    public void ExecuteHit()
    {
        OnPlayerHit?.Invoke(this, pendingHitType);
    }

    public void ClearHitBoxHit()
    {
        OnPlayerFinishHit?.Invoke(this, pendingHitType);
    }

    public void SetAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            anim.SetTrigger("Set");
            pendingHitType = HitType.Set;
            SoundManager.instance.PlaySound(SoundType.Object_Bounce);
        }
    }


    public void flip()
    {
        if (Time.timeScale == 0) return;
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void SetMoveable(bool state, bool useGravity = true)
    {
        isMoveable = state;
        
        if (!isMoveable)
        {
            rb.linearVelocity = Vector2.zero;
            
            rb.gravityScale = useGravity ? defaultGravityScale : 0;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    public void CheckMoveDirection()
    {
        if (Time.timeScale == 0) return;
        if(rb.linearVelocity.x > 0 && !facingRight)
        {
            flip();
        }
        if(rb.linearVelocity.x < 0 && facingRight)
        {
            flip();
        }
    }

    private void HandleRareIdle()
    {
        bool isStationary = isGrounded && Mathf.Abs(rb.linearVelocity.x) < 0.01f && Mathf.Abs(moveInput.x) < 0.01f;

        if (isDead || !isStationary || !isMoveable)
        {
            ResetIdleTimer();
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= nextCheckTime)
        {
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll <= rareIdleChance)
            {
                anim.SetTrigger("RareIdle");
            }
            
            ResetIdleTimer();
        }
    }

    private void ResetIdleTimer()
    {
        idleTimer = 0f;
        nextCheckTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }
    void FixedUpdate()
    {

        if (!CanControl) return; 

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
