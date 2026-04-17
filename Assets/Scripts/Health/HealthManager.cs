using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;
    public Image player1HealthBar;
    public Image player2HealthBar;

    public float player1MaxHealth = 100;
    public float player2MaxHealth = 100;
    public float player1CurrentHealth;
    public float player2CurrentHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void TakeDamage(int playerNumber, int damageAmount)
    {
        if (playerNumber == 1)
        {
            player1CurrentHealth -= damageAmount;
            Debug.Log("Player 1 Health: " + player1CurrentHealth);
        }
        else if (playerNumber == 2)
        {
            player2CurrentHealth -= damageAmount;
            Debug.Log("Player 2 Health: " + player2CurrentHealth);
        }
    }

    public void UpdateHealthBar()
    {
        player1HealthBar.fillAmount = player1CurrentHealth / player1MaxHealth;
        player2HealthBar.fillAmount = player2CurrentHealth / player2MaxHealth;
    }

    public void ResetHealth()
    {
        player1CurrentHealth = player1MaxHealth;
        player2CurrentHealth = player2MaxHealth;
        UpdateHealthBar();
    }
}
