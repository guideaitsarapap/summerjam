using UnityEngine;

public class RedWaterItem : MonoBehaviour, IPickable
{
    [SerializeField] private float overdriveSpeed = 40f;

    public void OnPickedUp(PlayerController player)
    {
        foreach (GameObject obj in GameFlowManager.Instance.activeBalls)
        {
            Ball ball = obj.GetComponent<Ball>();
            if (ball != null)
            {
                ball.ActivateRedWaterStatus();
            }
        }
        Destroy(gameObject);
    }
}
