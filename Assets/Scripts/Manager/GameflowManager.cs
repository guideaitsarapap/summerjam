using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;
    public List<PlayerIdentity> connectedPlayers = new List<PlayerIdentity>();

    void Awake()
    {
        Instance = this;
    }

    // ฟังก์ชันสำหรับลงทะเบียน (ที่เรียกจาก MultiplayerManager)
    public void RegisterNewPlayer(PlayerInput playerInput)
    {
        PlayerController controller = playerInput.GetComponent<PlayerController>();
        if (controller != null && !connectedPlayers.Any(p => p.playerIndex == playerInput.playerIndex))
        {
            PlayerIdentity newIdentity = new PlayerIdentity(playerInput.playerIndex, controller.side, controller);
            connectedPlayers.Add(newIdentity);
            Debug.Log($"Registered P{playerInput.playerIndex + 1}");
        }
    }

    public void MarkPlayerAsReady(PlayerController characterController)
    {
        var playerFound = connectedPlayers.Find(p => p.controllerReference == characterController);

        if (playerFound != null && !playerFound.isReadyInLobby)
        {
            playerFound.isReadyInLobby = true;
            characterController.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            
            Debug.Log($"[Flow] Player {playerFound.playerIndex + 1} Ready!");
            CheckAndStartGameTransition();
        }
    }

    private void CheckAndStartGameTransition()
    {
        if (connectedPlayers.Count >= 2 && connectedPlayers.All(p => p.isReadyInLobby))
        {
            Debug.Log("GO TO GAMEPLAY!");
            // SceneManager.LoadScene("GameScene");
        }
    }
}


[Serializable]
public class PlayerIdentity
{
    public int playerIndex;           // ลำดับผู้เล่น (0 หรือ 1)
    public PlayerSide playerSide;      // ฝั่งของผู้เล่น (Red หรือ Blue)
    public bool isReadyInLobby;       // สถานะความพร้อม
    public int currentHealth;

    [NonSerialized]
    public PlayerController controllerReference;

    public PlayerIdentity(int index, PlayerSide side, PlayerController controller)
    {
        this.playerIndex = index;
        this.playerSide = side;
        this.controllerReference = controller;
        this.isReadyInLobby = false;
    }
}