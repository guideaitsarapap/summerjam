using UnityEngine;

public class WaterItem : MonoBehaviour, IPickable
{
    [SerializeField] private float healAmount = 20f;

    public void OnPickedUp(PlayerController player)
    {
        player.Heal(healAmount); 
        Destroy(gameObject);
    }
}