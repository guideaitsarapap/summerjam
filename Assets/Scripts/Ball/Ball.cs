using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    public float baseSpeed = 8f;
    public float speedIncrease = 1.2f;
    public float maxSpeed = 25f;
    private Vector2 lastVelocity;

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

    public void Hit(Vector2 direction, PlayerSide hitterSide)
    {
        rb.gravityScale = 0f;
        currentSpeed *= speedIncrease;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        rb.linearVelocity = direction * currentSpeed;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (hitterSide == PlayerSide.Red)
            sr.color = Color.red;
        else
            sr.color = Color.blue;
    }

    public void SetBall(Vector2 force)
    {
        rb.gravityScale = 1f;
        currentSpeed = baseSpeed;
        rb.linearVelocity = force;
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

    private void ReflectBall(Collision2D collision)
    {
        
        Vector2 normal = collision.contacts[0].normal;

        Vector2 reflectDir = Vector2.Reflect(lastVelocity.normalized, normal);
        rb.linearVelocity = reflectDir * currentSpeed;

        transform.position += (Vector3)(normal * 0.05f);
    }
}
