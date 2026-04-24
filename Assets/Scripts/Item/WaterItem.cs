using UnityEngine;

public class WaterItem : MonoBehaviour, IPickable,IHittable
{
    [SerializeField] private float healAmount = 20f;
    private Rigidbody2D rb;
    private bool isHit = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void OnGetHit(PlayerController hitter, HitType hitType)
    {
        if (isHit) return; 
        isHit = true;

        // 1. Setup Rigidbody และ Collider
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        // ปิดการชนกับพื้น/กำแพงทันที เพื่อให้ตกแมพได้ (ทะลุพื้น)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 

        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        // ตั้งค่า Gravity ให้สูงหน่อยเพื่อให้ร่วงลงมาเร็วแบบ Mario
        rb.gravityScale = 3f; 

        // 2. คำนวณแรงดีดขึ้น (Mario Jump Style)
        float directionX = hitter.facingRight ? 1f : -1f;
        Vector2 marioLaunchForce = new Vector2(directionX * 5f, 15f); // เน้นแรงส่งขึ้น Y สูงๆ

        // ล้างความเร็วเก่าก่อนส่งแรงใหม่
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(marioLaunchForce, ForceMode2D.Impulse);

        // 3. ใส่แรงหมุนติ้วๆ
        rb.AddTorque(20f * -directionX, ForceMode2D.Impulse);

        // 4. ทำลายทิ้งเมื่อร่วงพ้นจอ (ประมาณ 2 วินาที)
        Destroy(gameObject, 2f);
    }
    public void OnPickedUp(PlayerController player)
    {
        SoundManager.instance.PlaySound(SoundType.Refresh);
        player.Heal(healAmount); 
        Destroy(gameObject);
    }
}