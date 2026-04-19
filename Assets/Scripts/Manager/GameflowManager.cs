using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public enum GameState
{
    Lobby,          
    Countdown,   
    Playing,        
    RoundEnd,       
    MatchOver       
}

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;
    public List<PlayerIdentity> connectedPlayers = new List<PlayerIdentity>();

    [Header("Match Settings")]
    public GameState currentGameState = GameState.Lobby;
    public int winsRequiredToMatch = 2;
    
    // เอาไว้เช็คว่าตอนนี้ลดเลือดได้ไหม
    public bool IsBattleActive => currentGameState == GameState.Playing;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Lobby ---

    public void RegisterNewPlayer(PlayerInput playerInput)
    {
        if (currentGameState != GameState.Lobby) return;

        PlayerController controller = playerInput.GetComponent<PlayerController>();
        if (controller != null && !connectedPlayers.Any(p => p.playerIndex == playerInput.playerIndex))
        {
            PlayerIdentity newIdentity = new PlayerIdentity(playerInput.playerIndex, controller.side, controller);
            connectedPlayers.Add(newIdentity);
            Debug.Log($"[Flow] Registered P{playerInput.playerIndex + 1}");
        }
    }

    public void MarkPlayerAsReady(PlayerController characterController)
    {
        var playerFound = connectedPlayers.Find(p => p.controllerReference == characterController);

        if (playerFound != null && !playerFound.isReadyInLobby)
        {
            playerFound.isReadyInLobby = true;
            // เปลี่ยนสีตัวละครเพื่อบอกว่า Ready แล้ว
            //characterController.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            
            CheckAndStartGameTransition();
        }
    }

    private void CheckAndStartGameTransition()
    {
        if (connectedPlayers.Count >= 2 && connectedPlayers.All(p => p.isReadyInLobby))
        {
            Debug.Log("[Flow] ALL READY! Loading Gameplay...");
            // ตรงนี้เปลี่ยน State เป็น Countdown (หรือจะเปลี่ยนหลังจากโหลด Scene เสร็จก็ได้)
            StartCoroutine(TransitionToGameplayRoutine());
        }
    }

    private IEnumerator TransitionToGameplayRoutine()
    {
        // 1. โหลด Scene (สมมติว่าชื่อ GameScene)
        // SceneManager.LoadScene("GameScene");
        // yield return new WaitForSeconds(0.5f); // รอโหลดแป๊บนึง

        // 2. เมื่อเข้าฉากใหม่ ให้เริ่มนับถอยหลัง
        StartNewRound();
        yield return null;
    }

    // --- Gameplay ---

    public void StartNewRound()
    {
        currentGameState = GameState.Countdown;
        
        // รีเซ็ตเลือดในข้อมูล
        foreach (var player in connectedPlayers)
        {
            player.currentHealth = player.maxHealth;
        }
        
        HealthManager.Instance.ResetAllHealthBars();
        
        StartCoroutine(RoundCountdownRoutine());
    }

    private IEnumerator RoundCountdownRoutine()
    {
        Debug.Log("3...");
        yield return new WaitForSeconds(1f);
        Debug.Log("2...");
        yield return new WaitForSeconds(1f);
        Debug.Log("1...");
        yield return new WaitForSeconds(1f);
        Debug.Log("FIGHT!");

        currentGameState = GameState.Playing;
    }

    public void ApplyDamage(PlayerSide side, float damageAmount)
    {
        if (currentGameState != GameState.Playing) return;

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
        if (currentGameState != GameState.Playing) return;
        
        currentGameState = GameState.RoundEnd;

        PlayerIdentity winner = connectedPlayers.Find(p => p.side != loserSide);
        
        if (winner != null)
        {
            winner.roundWins++;
            Debug.Log($"Round Ended! Winner: {winner.side} (Total Wins: {winner.roundWins})");

            if (winner.roundWins >= winsRequiredToMatch)
            {
                FinishMatch(winner.side);
            }
            else
            {
                StartCoroutine(PrepareNextRoundRoutine());
            }
        }
    }

    private IEnumerator PrepareNextRoundRoutine()
    {
        //Show round end screen or something here
        yield return new WaitForSeconds(3f);
        StartNewRound(); 
    }
    
    private void FinishMatch(PlayerSide matchWinner)
    {
        currentGameState = GameState.MatchOver;
        Debug.Log($"MATCH OVER! {matchWinner} IS THE CHAMPION!");
        //Show who won the match and return to lobby or something here
    }
}