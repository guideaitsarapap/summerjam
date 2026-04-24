using UnityEngine;
using VInspector;
using System.Collections;
using Unity.VisualScripting;

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
    private bool isOverdriveMode = false;
    

    [Header("Force Settings")]
    [SerializeField] private Vector2 forceSetUp = new Vector2(0, 8f);
    [SerializeField] private float overdriveSpeed = 40f;

    [Header("Mercy Rule Settings")]
    [SerializeField] private Vector3 redFollowOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private Vector3 blueFollowOffset = new Vector3(0, 1.5f, 0);

    private Vector3 currentFollowOffset;
    [SerializeField]private TrailRenderer ballTrail;


    private void Awake()
    {
        ballRigidbody = GetComponent<Rigidbody2D>();
        ballSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //ballTrail = GetComponent<TrailRenderer>();
        currentSpeed = baseSpeed;

        //if (ballTrail != null) ballTrail.enabled = false;
    }

    private void LateUpdate()
    {
        // อยู่ในโหมดลอยตามตัว (หลังมีคนตาย)
        if (isFollowingPlayer && targetToFollow != null)
        {
            transform.position = targetToFollow.position + currentFollowOffset;
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
        if (isFollowingPlayer)
        {
            if (hitter.side != loserSide) 
            {
                Debug.Log("[Ball] Not your turn to serve!");
                return;
            }
            
            // ถ้าใช่คนแพ้ที่ตี (เสิร์ฟสำเร็จ) ให้ปลดล็อกสถานะทันที
            StopFollowingPlayer();
        }


        float directionX = hitter.facingRight ? 1f : -1f;
        Vector2 finalDirection = Vector2.zero;

        switch (hitType)
        {
            case HitType.Straight:
                if(currentSpeed > 25f)
                {
                    if(currentSpeed > 40f)
                    {
                        SoundManager.instance.PlaySound(SoundType.OnFire);
                        TimeManager.Instance.DoHitStop(false, 0.3f);
                    } 
                    else
                    {
                        TimeManager.Instance.DoHitStop(false, 0.25f);
                    } 
                    
                }
                
                finalDirection = new Vector2(directionX, 0f).normalized;
                ApplyHit(finalDirection, hitter.side);
                break;

            case HitType.Down:
                if(currentSpeed > 25f)
                {
                    if(currentSpeed > 40f)
                    {
                        SoundManager.instance.PlaySound(SoundType.OnFire);
                        TimeManager.Instance.DoHitStop(false, 0.35f);
                    } 
                    else
                    {
                        TimeManager.Instance.DoHitStop(false, 0.25f);
                    } 
                    
                }
                
                finalDirection = new Vector2(directionX, -1f).normalized;
                ApplyHit(finalDirection, hitter.side, 0.4f);
                break;

            case HitType.Set:
                
                ApplySet(forceSetUp, hitter.side);
                SoundManager.instance.PlaySound(SoundType.Set);
                break;
        }
    }

    private void ApplyHit(Vector2 direction, PlayerSide hitterSide, float speedPlus = 0f)
    {
        ballRigidbody.simulated = true;
        ballRigidbody.gravityScale = 0f;

        if (isOverdriveMode)
        {
            currentSpeed = overdriveSpeed;
        }
        else
        {
            currentSpeed *= speedIncreaseMultiplier + speedPlus;
            currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maximumSpeed);
        }

        ballRigidbody.linearVelocity = direction * currentSpeed;
        SwitchBallSide(hitterSide);
        isOverdriveMode = false;
    }

    private void ApplySet(Vector2 force, PlayerSide setterSide)
    {
        ballRigidbody.simulated = true;
        ballRigidbody.gravityScale = 1f;
        ballRigidbody.linearVelocity = force;

        lastSetterSide = setterSide;
        currentSide = BallSide.Neutral;
        if(ballTrail!= null)
        {
            ballTrail.startColor = Color.white;
            ballTrail.endColor = new Color(1, 1, 1, 0);
        }
    }

    private void SwitchBallSide(PlayerSide hitterSide)
    {
        if (hitterSide == PlayerSide.Red)
        {
            currentSide = BallSide.Red;
            if(ballTrail!= null)
            {
                ballTrail.startColor = Color.red;
                ballTrail.endColor = new Color(1, 0, 0, 0);
            }
        }
        else
        {
            currentSide = BallSide.Blue;
            if(ballTrail!= null)
            {
                ballTrail.startColor = Color.blue;
                ballTrail.endColor = new Color(0, 0, 1, 0);
            }
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
            return;
        }

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            HurtBoxPlayer hurtBox = other.gameObject.GetComponent<HurtBoxPlayer>();
            if (hurtBox != null)
            {
                
                bool isOpponent = IsOpponent(hurtBox.playerSide, currentSide);
                bool isNotNeutral = currentSide != BallSide.Neutral;


                if (IsOpponent(hurtBox.playerSide, currentSide) && currentSide != BallSide.Neutral)
                {
                    hurtBox.DamageHit(currentSpeed);
                }
            }
        }
    }

    private void ReflectFromWall(Collision2D collision)
    {
        SoundManager.instance.PlaySound(SoundType.Ball_Bounce);
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
        if(ballTrail != null)
        {
            ballTrail.enabled = true;
            ballTrail.Clear();
        }
        StopAllCoroutines();
        isFollowingPlayer = false;
        ballRigidbody.simulated = true;
        ballRigidbody.bodyType = RigidbodyType2D.Dynamic;
        ballRigidbody.linearVelocity = Vector2.zero;
        ballRigidbody.gravityScale = 0f;

        transform.position = GameFlowManager.Instance.GameplayballSpawnPoint.position;
        currentSpeed = baseSpeed;
        currentSide = BallSide.Neutral;
    }

    // ใช้สำหรับรีเซ็ตบอลในหน้าMenu เพื่อให้บอลกลับมาที่ฝั่งตัวเอง
    public void ResetBallToSideInMenuSceneOnly(Vector3 positionBall)
    {
        StopAllCoroutines();
        isFollowingPlayer = false;
        ballRigidbody.simulated = true;
        ballRigidbody.bodyType = RigidbodyType2D.Dynamic;
        ballRigidbody.linearVelocity = Vector2.zero;
        ballRigidbody.gravityScale = 0f;

        transform.position = positionBall;
        currentSpeed = baseSpeed;
        currentSide = BallSide.Neutral;
    }

    public void SetupFollowLoser(PlayerController loser)
    {
        targetToFollow = loser.transform;
        loserSide = loser.side;
        isFollowingPlayer = true;

        if (loser.side == PlayerSide.Red)
        {
            currentFollowOffset = redFollowOffset;
        }
        else
        {
            currentFollowOffset = blueFollowOffset;
        }

        ballRigidbody.gravityScale = 0f;
        ballRigidbody.linearVelocity = Vector2.zero;
        // เริ่มนับถอยหลัง ถ้าไม่ตีภายใน 7 วินาที ให้ดรอปลงพื้น
        StartCoroutine(MercyTimerRoutine());
    }

    private void StopFollowingPlayer()
    {
        if (!isFollowingPlayer) return;
        
        isFollowingPlayer = false;
        targetToFollow = null;
        StopAllCoroutines();
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

    public void ActivateRedWaterStatus()
    {
        isOverdriveMode = true;
        currentSpeed = overdriveSpeed;
    }
}