using UnityEngine;
using System.Collections;

public class HurtBoxPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    [Header("Invincibility Settings")]
    [SerializeField] private float iFrameDuration = 0.5f; 
    private bool isInvincible = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite;

    public PlayerSide playerSide
    {
        get { return playerController.side; }
    }

    public void DamageHit(float damageAmount)
    {
        // 1. ถ้ายังอยู่ในช่วงอมตะ จะไม่โดนดาเมจ
        if (isInvincible) return;

        // 2. ส่งดาเมจไปที่ GameFlowManager
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ApplyDamage(playerSide, damageAmount);
        }

        // 3. เริ่มสถานะอมตะชั่วขณะ
        StartCoroutine(BecomeInvincibleRoutine());
    }

    private IEnumerator BecomeInvincibleRoutine()
    {
        isInvincible = true;
        Debug.Log($"[HurtBox] {playerSide} is now Invincible!");

        // --- Visual Feedback (ตัวละครกะพริบ) ---
        float timer = 0f;
        float blinkInterval = 0.1f; // ความเร็วในการกะพริบ

        while (timer < iFrameDuration)
        {
            if (playerSprite != null)
            {
                // สลับการแสดงผล Sprite (เปิด/ปิด) เพื่อให้ดูเหมือนกะพริบ
                playerSprite.enabled = !playerSprite.enabled;
            }
            
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // คืนค่าสถานะปกติ
        if (playerSprite != null) playerSprite.enabled = true;
        isInvincible = false;
        Debug.Log($"[HurtBox] {playerSide} Invincibility ended.");
    }
}