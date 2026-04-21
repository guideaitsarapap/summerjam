using UnityEngine;
using VInspector;
using System.Collections;

public enum BallSide
{
    Neutral,
    Red,
    Blue
}

public class Ball : MonoBehaviour, IHittable
{
    private Rigidbody2D ballRigidbody;
    private SpriteRenderer ballSpriteRenderer;

    [Header("Movement Settings")]
    public float baseSpeed = 8f;
    public float speedIncreaseMultiplier = 1.2f;
    public float maximumSpeed = 25f;
    private float currentSpeed;

    [Header("Physics & Collision")]
    private Vector2 velocityBeforePhysicsUpdate;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask boundLayer;

    [Header("Ball Status")]
    public BallSide currentSide = BallSide.Neutral;
    private bool isFollowingPlayer = false;
    private Transform targetToFollow;
    private PlayerSide loserSide;
    private PlayerSide lastSetterSide;

    [Header("Force Settings")]
    [SerializeField] private Vector2 forceSetUp = new Vector2(0, 8f);

    private void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody2D>();
        ballSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetBallToCenter();
    }

    private void Update()
    {
        // อยู่ในโหมดลอยตามตัว (หลังมีคนตาย)
        if (isFollowingPlayer && targetToFollow != null)
        {
            transform.position = targetToFollow.position + new Vector3(0, 1.5f, 0);
        }
    }

    private void FixedUpdate()
    {
        // บันทึกความเร็วก่อนชนกำแพง
        if (!isFollowingPlayer)
        {
            velocityBeforePhysicsUpdate = ballRigidbody.linearVelocity;
        }
    }

    // --- IHitable functions ถ้าโดนตี ---
    public void OnGetHit(PlayerController hitter, HitType hitType)
    {
        // เงื่อนไข: ถ้าบอลลอยตามผู้แพ้ จะต้องเป็นผู้แพ้เท่านั้นที่ตีได้
        if (isFollowingPlayer && hitter.side != loserSide) return;

        // ถ้าตีแล้ว ให้เลิกติดตามตัวละคร
        StopFollowingPlayer();

        float directionX = hitter.facingRight ? 1f : -1f;
        Vector2 finalDirection = Vector2.zero;

        switch (hitType)
        {
            case HitType.Straight:
                TimeManager.Instance.DoHitStop(false);
                finalDirection = new Vector2(directionX, 0f).normalized;
                ApplyHit(finalDirection, hitter.side);
                break;

            case HitType.Down:
                TimeManager.Instance.DoHitStop(false);
                finalDirection = new Vector2(directionX, -1f).normalized;
                ApplyHit(finalDirection, hitter.side);
                break;

            case HitType.Set:
                ApplySet(forceSetUp, hitter.side);
                break;
        }
    }

    private void ApplyHit(Vector2 direction, PlayerSide hitterSide)
    {
        ballRigidbody.simulated = true;
        ballRigidbody.gravityScale = 0f;

        // เพิ่มความเร็ว
        currentSpeed *= speedIncreaseMultiplier;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maximumSpeed);

        ballRigidbody.linearVelocity = direction * currentSpeed;
        SwitchBallSide(hitterSide);
    }

    private void ApplySet(Vector2 force, PlayerSide setterSide)
    {
        ballRigidbody.simulated = true;
        ballRigidbody.gravityScale = 1f;
        ballRigidbody.linearVelocity = force;

        lastSetterSide = setterSide;
        currentSide = BallSide.Neutral;
        ballSpriteRenderer.color = Color.white;
    }

    private void SwitchBallSide(PlayerSide hitterSide)
    {
        if (hitterSide == PlayerSide.Red)
        {
            currentSide = BallSide.Red;
            ballSpriteRenderer.color = Color.red;
        }
        else
        {
            currentSide = BallSide.Blue;
            ballSpriteRenderer.color = Color.blue;
        }
    }

    // --- การจัดการการชน ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ชนกำแพง (Reflect)
        if (((1 << collision.gameObject.layer) & boundLayer) != 0)
        {
            ReflectFromWall(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameFlowManager.Instance.IsBattleActive) {
            Debug.Log("Damage failed: GameState is not Playing");
            return;
        }

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            HurtBoxPlayer hurtBox = other.gameObject.GetComponent<HurtBoxPlayer>();
            if (hurtBox != null)
            {
                Debug.Log($"Ball Status: Side={currentSide}, Speed={currentSpeed}");
                Debug.Log($"Player Status: Side={hurtBox.playerSide}");
                
                bool isOpponent = IsOpponent(hurtBox.playerSide, currentSide);
                bool isNotNeutral = currentSide != BallSide.Neutral;

                Debug.Log($"Logic Check: isOpponent={isOpponent}, isNotNeutral={isNotNeutral}");

                if (IsOpponent(hurtBox.playerSide, currentSide) && currentSide != BallSide.Neutral)
                {
                    hurtBox.DamageHit(currentSpeed);
                }
            }
        }
    }

    private void ReflectFromWall(Collision2D collision)
    {
        ballRigidbody.gravityScale = 0f;
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflectDirection = Vector2.Reflect(velocityBeforePhysicsUpdate.normalized, normal);
        
        ballRigidbody.linearVelocity = reflectDirection * currentSpeed;
        transform.position += (Vector3)(normal * 0.05f);

        if (currentSide == BallSide.Neutral && !isFollowingPlayer)
        {
            SwitchBallSide(lastSetterSide);
        }
    }

    // --- ระบบ Reset และติดตามผู้แพ้ ---

    public void ResetBallToCenter()
    {
        StopAllCoroutines();
        isFollowingPlayer = false;
        ballRigidbody.simulated = true;
        ballRigidbody.bodyType = RigidbodyType2D.Dynamic;
        ballRigidbody.linearVelocity = Vector2.zero;
        ballRigidbody.gravityScale = 0f;

        transform.position = GameFlowManager.Instance.GameplayballSpawnPoint.position;
        currentSpeed = baseSpeed;
        currentSide = BallSide.Neutral;
        ballSpriteRenderer.color = Color.white;
    }

    public void SetupFollowLoser(PlayerController loser)
    {
        ResetBallToCenter(); // ล้างค่า Speed เดิม
        
        targetToFollow = loser.transform;
        loserSide = loser.side;
        isFollowingPlayer = true;

        // เริ่มนับถอยหลัง ถ้าไม่ตีภายใน 7 วินาที ให้ดรอปลงพื้น
        StartCoroutine(MercyTimerRoutine());
    }

    private void StopFollowingPlayer()
    {
        if (!isFollowingPlayer) return;
        
        StopAllCoroutines();
        isFollowingPlayer = false;
        targetToFollow = null;
        ballRigidbody.simulated = true;
    }

    private IEnumerator MercyTimerRoutine()
    {
        yield return new WaitForSeconds(7f); // ระยะเวลา 5-10 วินาที
        if (isFollowingPlayer)
        {
            StopFollowingPlayer();
            ballRigidbody.gravityScale = 1f; // ดรอปลงพื้น
            currentSide = BallSide.Neutral; // ใครมาเก็บไปตีต่อก็ได้
            ballSpriteRenderer.color = Color.white;
        }
    }

    private bool IsOpponent(PlayerSide playerSide, BallSide ballSide)
    {
        if (ballSide == BallSide.Red && playerSide == PlayerSide.Blue) return true;
        if (ballSide == BallSide.Blue && playerSide == PlayerSide.Red) return true;
        return false;
    }
}