using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    public int playerIndex;
    public PlayerSide side;
    public PlayerController controllerReference;
    
    public float currentHealth;
    public float maxHealth = 100f;
    public int roundWins = 0;
    public bool isReadyInLobby = false;

    public PlayerIdentity(int index, PlayerSide side, PlayerController controller)
    {
        this.playerIndex = index;
        this.side = side;
        this.controllerReference = controller;
        this.currentHealth = maxHealth;
    }
}