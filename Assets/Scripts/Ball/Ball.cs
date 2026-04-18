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

    void Update()   
    {
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            Hit(new Vector2(1f, 0)); 
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Hit(new Vector2(1f, -1f)); 
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetBall(new Vector2(0.2f, 10f));
        }
    }

    public void Hit(Vector2 direction)
    {
        rb.gravityScale = 0f;

        currentSpeed *= speedIncrease;
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        rb.linearVelocity = direction.normalized * currentSpeed;
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
