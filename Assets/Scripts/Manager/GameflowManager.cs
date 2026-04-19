using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;
    public List<PlayerIdentity> connectedPlayers = new List<PlayerIdentity>();

    [Header("Match Rules")]
    public int winsRequiredToMatch = 2; // ชนะ 2 ใน 3
    private bool isRoundActive = false;

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

    public void ApplyDamage(PlayerSide side, float damageAmount)
    {
        var player = connectedPlayers.Find(p => p.side == side);

        if (player != null)
        {
            player.currentHealth -= damageAmount;
            player.currentHealth = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);

            HealthManager.Instance.UpdateHealthUI(player.side, player.currentHealth, player.maxHealth);

            if (player.currentHealth <= 0)
            {
                OnPlayerDefeated(side);
            }
        }
    }

    public void OnPlayerDefeated(PlayerSide loserSide)
    {
        if (!isRoundActive) return;
        isRoundActive = false;

        PlayerIdentity winner = connectedPlayers.Find(p => p.side != loserSide);
        PlayerIdentity loser = connectedPlayers.Find(p => p.side == loserSide);

        if (winner != null)
        {
            winner.roundWins++;
            Debug.Log($"Round Ended! Winner: {winner.side} (Wins: {winner.roundWins})");


            if (winner.roundWins >= winsRequiredToMatch)
            {
                FinishMatch(winner.side);
            }
            else
            {
                StartCoroutine(PrepareNextRound());
            }
        }
    }
    private IEnumerator PrepareNextRound()
    {
        yield return new WaitForSeconds(3f);

        foreach (var player in connectedPlayers)
        {
            player.currentHealth = player.maxHealth;
        }

        // 2. สั่ง HealthManager ให้รีเซ็ตหลอดเลือดบนจอ
        HealthManager.Instance.ResetAllHealthBars();

        // 3. ย้ายตำแหน่งผู้เล่นกลับไปจุด Spawn (ถ้ามี Reference ของ Controller)
        // MovePlayersToSpawnPoints();

        // 4. กลับไปเริ่มสถานะนับถอยหลังใหม่
        // StartCountdown();
        
        isRoundActive = true;
        Debug.Log("Next Round Starts!");
    }
    
    private void FinishMatch(PlayerSide matchWinner)
    {
        Debug.Log($"MATCH OVER! {matchWinner} IS THE CHAMPION!");
    }
}