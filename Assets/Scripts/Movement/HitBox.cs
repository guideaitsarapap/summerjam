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

    private void HandleHit(PlayerController player)
    {
        if (ballInRange != null)
        {
            float dirX = player.facingRight ? 1f : -1f;
            
            Vector2 hitDir = new Vector2(dirX, 0.5f).normalized;

            ballInRange.Hit(hitDir, player.side); 
        }
        if (ballInRange == null)
        {
            Debug.Log("No ball in range to hit.");
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
