using UnityEngine;

[System.Serializable]

public class PlayerIdentity
{
    public int playerIndex;
    public PlayerSide side;
    public PlayerController controllerReference;
    
    public float currentHealth;
    public float maxHealth = 100f;
    public int roundWins = 0;
    public bool isReadyInLobby = false;
    [Header("Item Visuals")]
    [SerializeField] public Color overdriveColor = new Color(1f, 0.3f, 0.3f);
    public bool hasRedWaterBuff = false;

    public PlayerIdentity(int index, PlayerSide side, PlayerController controller)
    {
        this.playerIndex = index;
        this.side = side;
        this.controllerReference = controller;
        this.currentHealth = maxHealth;
    }
}