public interface IHittable
{
    // กำหนดว่าถ้าโดนตี จะให้เกิดอะไรขึ้น
    void OnGetHit(PlayerController hitter, HitType hitType);
}