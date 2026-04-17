using UnityEngine;

public class PlayerMovement2DTopDownBasic : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        input.Normalize();
    }

    void FixedUpdate()
    {
        // Not Good to used for Prototype show only.
        rb.linearVelocity = input * speed;
    }
    

}
