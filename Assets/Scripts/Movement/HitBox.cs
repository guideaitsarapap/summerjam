using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private PlayerController myPlayer;
    bool isHitBoxCanHit = true;
    [SerializeField]private List<IHittable> HittableObjects = new List<IHittable>();

    [Header("Force Set Up Power")]
    [SerializeField] Vector2 forceSetUp = new Vector2(0, 8f);

    private void Awake()
    {
        myPlayer = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        if (myPlayer != null) 
        {
            myPlayer.OnPlayerHit += HandleHit;
            myPlayer.OnPlayerFinishHit += ResetHitBox;
        }
    }

    private void OnDisable()
    {
        if (myPlayer != null) 
        {
            myPlayer.OnPlayerHit -= HandleHit;
            myPlayer.OnPlayerFinishHit -= ResetHitBox;
        }
    }

    private void HandleHit(PlayerController player, HitType type)
    {
        if(!isHitBoxCanHit)
            return;

        
        for (int i = HittableObjects.Count - 1; i >= 0; i--)
        {
            if (HittableObjects[i] != null)
            {
                HittableObjects[i].OnGetHit(player, type);
            }
        }

        isHitBoxCanHit = false;
    }

    private void ResetHitBox(PlayerController player, HitType type)
    {
        isHitBoxCanHit = true;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ลองเช็คดูว่าวัตถุที่เข้ามา "ตีได้" หรือไม่ (มี Interface IHittable ไหม)
        var hittableObject = other.GetComponent<IHittable>();

        if (hittableObject != null)
        {
            // ถ้าตีได้ และยังไม่มีชื่อในสมุด ก็Addเพิ่มเข้าไป
            if (!HittableObjects.Contains(hittableObject))
            {
                HittableObjects.Add(hittableObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var hittableObject = other.GetComponent<IHittable>();

        if (hittableObject != null)
        {
            
            if (HittableObjects.Contains(hittableObject))
            {
                HittableObjects.Remove(hittableObject);
            }
        }
    }

    private void OnDrawGizmos() // for debugging, แสดง HitBox ใน Scene view และ Game view
    {
        // ตั้งค่าสีของ Gizmos (สีแดงโปร่งแสง)
        Gizmos.color = new Color(1, 0, 0, 0.4f);


        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
    }
}