using UnityEngine;

public class HurtBoxPlayer : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public PlayerSide playerSide{
        get { return playerController.side; }
        private set { playerController.side = value; }
    }

    public void DamageHit(float damageAmount)
    {
        GameFlowManager.Instance.ApplyDamage(playerSide, (int)damageAmount);
    }
}

    


