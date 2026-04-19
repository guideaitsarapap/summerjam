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

    [Header("Gravity Multipliers")]
    [SerializeField] float fallMultiplier = 2.5f;      
    [SerializeField] float lowJumpMultiplier = 2f; 
    [SerializeField] float defaultGravityScale = 1f;

    private Vector2 moveInput;

    public event Action<PlayerController,HitType> OnPlayerHit;

    public void Initialize(PlayerIdentity identity)
    {
        Identity = identity;
        this.side = Identity.side; 
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 movement = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
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
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isDoubleJumpUsed = false; // Reset double jump when landing
        }
    }

    public void HitAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            HitType type = HitType.Straight;

            if (!isGrounded && Mathf.Abs(moveInput.x) > 0.1f)
            {
                type = HitType.Down;
                Debug.Log("Aerial Spike! (Down Hit)");
            }

            OnPlayerHit?.Invoke(this, type);
        }
    }

    public void SetAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            HitType type = HitType.Set;
            OnPlayerHit?.Invoke(this, type);
        }
    }


    public void flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void FixedUpdate()
    {
        if (rb.linearVelocity.y <= 0)
        {
            // Falling — apply extra gravity
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 )
        {
            // Rising but jump released — cut arc short
            rb.gravityScale = lowJumpMultiplier;

        }
    }
}
