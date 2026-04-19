using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [Header("UI References")]
    [SerializeField] private Image redHealthBar;
    [SerializeField] private Image blueHealthBar;

    private void Awake()
    {
        // ตั้งค่า Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UpdateHealthUI(PlayerSide side, float currentHealth, float maxHealth)
    {
        // คำนวณเปอร์เซ็นต์ (0 ถึง 1)
        float fillAmount = currentHealth / maxHealth;

        if (side == PlayerSide.Red)
        {
            if (redHealthBar != null)
            {
                redHealthBar.fillAmount = fillAmount;
            }
        }
        else if (side == PlayerSide.Blue)
        {
            if (blueHealthBar != null)
            {
                blueHealthBar.fillAmount = fillAmount;
            }
        }
    }

    public void ResetAllHealthBars()
    {
        if (redHealthBar != null) redHealthBar.fillAmount = 1f;
        if (blueHealthBar != null) blueHealthBar.fillAmount = 1f;
    }
}