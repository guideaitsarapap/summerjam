using UnityEngine;

public class HitBox : MonoBehaviour
{
    private Ball ballInRange;
    private PlayerController myPlayer;

    private void Awake()
    {
        myPlayer = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        if (myPlayer != null) myPlayer.OnPlayerHit += HandleHit;
    }

    private void OnDisable()
    {
        if (myPlayer != null) myPlayer.OnPlayerHit -= HandleHit;
    }

    private void HandleHit(PlayerController player, HitType type)
    {
        if (ballInRange == null) 
        {
            Debug.Log("No ball in range to hit!");
            return;
        }

        float dirX = player.facingRight ? 1f : -1f;
        Vector2 finalDir = Vector2.zero;

        switch (type)
        {
            case HitType.Straight:
                finalDir = new Vector2(dirX, 0f).normalized;
                ballInRange.Hit(finalDir, player.side);
                break;

            case HitType.Down:
                finalDir = new Vector2(dirX, -1f).normalized;
                ballInRange.Hit(finalDir, player.side);
                break;

            case HitType.Set:
                ballInRange.SetBall(new Vector2(0, 8f)); 
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = other.GetComponent<Ball>();
        }
    }
}
