using UnityEngine;
using System.Collections;

public class HurtBoxPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    [Header("Invincibility Settings")]
    [SerializeField] private float iFrameDuration = 0.5f; 
    private bool isInvincible = false;

    [Header("Visual Feedback")]
    [SerializeField] private ChangeMaterials playerSprite;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 0.2f;

    public PlayerSide playerSide
    {
        get { return playerController.side; }
    }

    public void DamageHit(float damageAmount)
    {
        // 1. ถ้ายังอยู่ในช่วงอมตะ จะไม่โดนดาเมจ
        if (isInvincible) return;
        
        StartCoroutine(StunRoutine());

        // 2.Pause Game ให้มี Impact
        TimeManager.Instance.DoHitStop(true);

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
        bool isDefautlMaterial = true;

        while (timer < iFrameDuration)
        {
            if (playerSprite != null)
            {
                // สลับการแสดงผล Sprite (เปิด/ปิด) เพื่อให้ดูเหมือนกะพริบ
                if (isDefautlMaterial)
                {
                    playerSprite.ChangeToGetHitMaterial();
                }
                else
                {
                    playerSprite.ChangeToDefaultMaterial();
                }
            }
            
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // คืนค่าสถานะปกติ
        if (playerSprite != null) playerSprite.ChangeToDefaultMaterial();
        isInvincible = false;
        Debug.Log($"[HurtBox] {playerSide} Invincibility ended.");
    }

    private IEnumerator StunRoutine()
    {
        playerController.SetMoveable(false);
        
        // ใช้ Realtime เพื่อให้ไม่โดน HitStop รบกวนเวลา
        yield return new WaitForSecondsRealtime(stunDuration);
        
        playerController.SetMoveable(true);
    }
}