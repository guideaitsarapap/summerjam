using UnityEngine;

public class ReadyBall : MonoBehaviour, IHittable
{
    public PlayerSide forPlayerSide = PlayerSide.Natural;
    public Color readyColor = Color.green;

    // นี่คือฟังก์ชันที่ Interface บังคับให้มี (ประตูทางเข้า)
    public void OnGetHit(PlayerController hitter, HitType hitType)
    {
        if (forPlayerSide == PlayerSide.Natural || forPlayerSide == hitter.side)
        {
            OnActivated();
            
            if (GameFlowManager.Instance != null)
            {
                GameFlowManager.Instance.MarkPlayerAsReady(hitter);
            }
        }
    }

    public void OnActivated()
    {
        //do something fun here, like play a sound or particle effect
    }
}