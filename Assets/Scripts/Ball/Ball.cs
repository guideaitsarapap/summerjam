using UnityEngine;
using VInspector;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Speed Settings")]
    public float baseSpeed = 8f;
    public float speedIncrease = 1.2f;
    public float maxSpeed = 25f;
    private float currentSpeed;

    [Header("Physics & Collision")]
    private Vector2 lastVelocity;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask boundLayer;

    [Header("Ball Status")]
    private BallSide currentSide = BallSide.Neutral;

    [Header("Force Set Up Power")]
    [SerializeField] Vector2 forceSetUp = new Vector2(0, 8f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }

    public void OnGetHit(PlayerController hitter, HitType hitType)
    {
        float dirX = hitter.facingRight ? 1f : -1f;
        Vector2 finalDir = Vector2.zero;

        switch (hitType)
        {
            case HitType.Straight:
                finalDir = new Vector2(dirX, 0f).normalized;
                Hit(finalDir, hitter.side);
                break;

            case HitType.Down:
                // ตบลงพื้น (Spike/Down Hit)
                finalDir = new Vector2(dirX, -1f).normalized;
                Hit(finalDir, hitter.side);
                break;

            case HitType.Set:
                // เดาะบอลขึ้น (Set)
                SetBall(forceSetUp, hitter.side);
                break;
        }
    }

    public void Hit(Vector2 direction, PlayerSide hitterSide, float speedIncreaseMultiplier = 1.2f)
    {
        rb.gravityScale = 0f;
        currentSpeed *= speedIncrease;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        rb.linearVelocity = direction * currentSpeed;
        SwitchSideBall(hitterSide);
        
    }

    private void SwitchSideBall(PlayerSide hitterSide)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (hitterSide == PlayerSide.Red)
        {
            if (currentSide == BallSide.Red) return;
            currentSide = BallSide.Red;
            sr.color = Color.red;
            Debug.Log("Ball switch to Red!");
        }
        else
        {
            if (currentSide == BallSide.Blue) return;
            currentSide = BallSide.Blue;
            sr.color = Color.blue;
            Debug.Log("Ball switch to Blue!");
        }

    }

    public void SetBall(Vector2 force, PlayerSide setterSide)
    {
        rb.gravityScale = 1f;
        //currentSpeed = baseSpeed;
        rb.linearVelocity = force;
        SwitchSideBall(setterSide);
    }

    public float GetDamage()
    {
        return currentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & boundLayer) != 0)
        {
            ReflectBall(collision);
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            HurtBoxPlayer hurtBoxPlayer = collision.gameObject.GetComponent<HurtBoxPlayer>();
            if(hurtBoxPlayer != null)
            {
                if (hurtBoxPlayer.playerSide != (PlayerSide)currentSide && currentSide != BallSide.Neutral)
                {
                    hurtBoxPlayer.DamageHit(GetDamage());
                }
            }
        }
    }

    private void ReflectBall(Collision2D collision)
    {
        rb.gravityScale = 0f;
        Vector2 normal = collision.contacts[0].normal;

        Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, normal);
        rb.linearVelocity = reflectDir * currentSpeed;

        transform.position += (Vector3)(normal * 0.05f);
    }

    [Button]
    void TestSwitchSideBall()
    {
        if(currentSide != BallSide.Red)
        {
            SwitchSideBall(PlayerSide.Red);
        }
        else
        {
            SwitchSideBall(PlayerSide.Blue);
        }
    }

}

public enum BallSide
{
    Neutral,
    Red,
    Blue
}
