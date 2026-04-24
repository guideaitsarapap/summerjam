using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private List<IPickable> itemsInRange = new List<IPickable>();

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnPlayerPickup  += HandlePickUp;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnPlayerPickup -= HandlePickUp;
        }
    }

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
        SoundManager.instance.PlaySound(SoundType.Got_Hit);
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
        playerController.SetMoveable(false,false);
        
        // ใช้ Realtime เพื่อให้ไม่โดน HitStop รบกวนเวลา
        yield return new WaitForSecondsRealtime(stunDuration);
        
        playerController.SetMoveable(true,true);
    }

    private void HandlePickUp(PlayerController pc)
    {
        if (itemsInRange.Count > 0)
        {
            IPickable itemToPick = itemsInRange[0];
            
            if (itemToPick != null)
            {
                itemToPick.OnPickedUp(pc);
                itemsInRange.Remove(itemToPick);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IPickable item = other.GetComponent<IPickable>();
        if (item != null)
        {
            if (!itemsInRange.Contains(item))
            {
                itemsInRange.Add(item);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IPickable item = other.GetComponent<IPickable>();
        if (item != null)
        {
            itemsInRange.Remove(item);
        }
    }
}