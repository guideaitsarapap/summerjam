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

    void Start()
    {
        SetUpHealthAtStart();
    }

    public void TakeDamage(PlayerSide playerSide, int damageAmount)
    {
        if (playerSide == PlayerSide.Red)
        {
            player1CurrentHealth -= damageAmount;
            Debug.Log("Player 1 Health: " + player1CurrentHealth);
            if (player1CurrentHealth <= 0)
            {
                player1CurrentHealth = 0;
                Debug.Log("Player 1 is Dead!");
            }
        }
        else if (playerSide == PlayerSide.Blue)
        {
            player2CurrentHealth -= damageAmount;
            Debug.Log("Player 2 Health: " + player2CurrentHealth);
            if (player2CurrentHealth <= 0)
            {
                player2CurrentHealth = 0;
                Debug.Log("Player 2 is Dead!");
            }
        }
        UpdateHealthBar();
    }

    public void SetUpHealthAtStart()
    {
        player1CurrentHealth = player1MaxHealth;
        player2CurrentHealth = player2MaxHealth;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        player1HealthBar.fillAmount = player1CurrentHealth / player1MaxHealth;
        player2HealthBar.fillAmount = player2CurrentHealth / player2MaxHealth;
    }

    public void ResetHealth(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1CurrentHealth = player1MaxHealth;
            Debug.Log("Player 1 Health Reset: " + player1CurrentHealth);
        }
        else if (playerNumber == 2)
        {
            player2CurrentHealth = player2MaxHealth;
            Debug.Log("Player 2 Health Reset: " + player2CurrentHealth);
        }
        UpdateHealthBar();
    }

}
