using UnityEngine;

public class HurtBoxPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public PlayerSide playerSide{
        get { return playerController.side; }
        private set { playerController.side = value; }
    }


    public void Dead()
    {
        playerController.Dead();
    }

    public void DamageHit(float damageAmount)
    {
        HealthManager.Instance.TakeDamage(playerSide, (int)damageAmount);
    
        
    }


}

    


