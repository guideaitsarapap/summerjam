using UnityEngine;
using System.Collections;

public class HurtBoxPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    [Header("Invincibility Settings")]
    [SerializeField] private float iFrameDuration = 0.5f; 
    private bool isInvincible = false;
    [Header("OnHit Pause time Settings")]
    [SerializeField] private float hitStopDuration = 1f;
    [SerializeField] [Range(0, 1)] private float timeScaleIntensity = 0f; // 0 คือหยุดสนิท, 0.1 คือสโลว์สุดๆ

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

        // 2.Pause Game ให้มี Impact
        StartCoroutine(HitStopRoutine());

        // 3. ส่งดาเมจไปที่ GameFlowManager
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ApplyDamage(playerSide, damageAmount);
        }

        // 4. เริ่มสถานะอมตะชั่วขณะ
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

    private IEnumerator HitStopRoutine()
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = timeScaleIntensity;

        yield return new WaitForSecondsRealtime(hitStopDuration);

        // คืนค่าเดิม
        Time.timeScale = originalTimeScale;
    }
}