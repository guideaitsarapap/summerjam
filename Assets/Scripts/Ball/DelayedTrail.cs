using UnityEngine;

public class DelayedTrail : MonoBehaviour 
{
    public Transform target; // ตัวละครหรือ Object หลัก
    public float smoothSpeed = 5f; // ยิ่งน้อยยิ่งช้า/หน่วง

    void LateUpdate() 
    {
        if (target == null) return;

        // ใช้ Lerp เพื่อให้ตำแหน่งของ Trail ค่อยๆ วิ่งตามไป ไม่ได้วาร์ปไปทันที
        Vector3 desiredPosition = target.position;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}