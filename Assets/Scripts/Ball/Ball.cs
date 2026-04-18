using UnityEngine;
using VInspector;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    public float baseSpeed = 8f;
    public float speedIncrease = 1.2f;
    public float maxSpeed = 25f;
    private Vector2 lastVelocity;
    [SerializeField] private LayerMask playerLayer;
    private BallSide currentSide = BallSide.Neutral;

    private float currentSpeed;
    [SerializeField] LayerMask boundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void FixedUpdate()
    {
        lastVelocity = rb.linearVelocity;
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
