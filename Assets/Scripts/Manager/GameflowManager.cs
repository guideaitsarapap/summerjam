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
    public Transform GameplayballSpawnPoint;
    public Transform[] LobbyBallSpawnPoints;
    public List<PlayerIdentity> connectedPlayers = new List<PlayerIdentity>();
    
    [Header("Ball Management")]
    public GameObject ballPrefab;
    private List<GameObject> activeBalls = new List<GameObject>();

    [Header("Match Settings")]
    public GameState currentGameState = GameState.Lobby;
    public int winsRequiredToMatch = 2;

    [Header("Movement Configuration")]
    public bool allowMoveInLobby = false;
    public bool allowMoveInCountdown = false;
    public bool allowMoveInPlaying = false;
    public bool allowMoveInRoundEnd = false;
    public bool allowMoveInMatchOver = false;
    
    [Header("Round Logic")]
    private bool isFirstRound = true;
    private PlayerSide lastRoundLoser;
    
    // เอาไว้เช็คว่าตอนนี้ลดเลือดได้ไหม
    public bool IsBattleActive => currentGameState == GameState.Playing;

    [Header("Object References for Menu")]
    [SerializeField] private List<GameObject> menuObjectsToClear = new List<GameObject>();

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

    void Update()
    {
        CheckAndStartGameTransition();
    }

 #region     --- Lobby ---

    public void RegisterNewPlayer(PlayerInput playerInput)
    {
        if (currentGameState != GameState.Lobby) return;

        PlayerController controller = playerInput.GetComponent<PlayerController>();
        
        if (controller != null)
        {
            PlayerIdentity newIdentity = new PlayerIdentity(
                playerInput.playerIndex, 
                controller.side, 
                controller
            );

            controller.Initialize(newIdentity);

            if (!connectedPlayers.Any(p => p.playerIndex == playerInput.playerIndex))
            {
                connectedPlayers.Add(newIdentity);
                Debug.Log($"[Flow] Registered P{newIdentity.playerIndex + 1} with Identity");
            }
        }
    }

    public void MarkPlayerAsReady(PlayerController characterController)
    {
        var playerFound = connectedPlayers.Find(p => p.controllerReference == characterController);

        Debug.Log(playerFound);


        if (playerFound != null && !playerFound.isReadyInLobby)
        {
            playerFound.isReadyInLobby = true;
            // เปลี่ยนสีตัวละครเพื่อบอกว่า Ready แล้ว
            //characterController.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            
            CheckAndStartGameTransition();
        }
    }

    public void ResetReadyStatus()
    {
        foreach (var player in connectedPlayers)
        {
            player.isReadyInLobby = false;
            Debug.Log($"[Flow] Reset Ready status for P{player.playerIndex + 1}");
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
        ResetReadyStatus();

        // 1. โหลด Scene (สมมติว่าชื่อ GameScene)
        // SceneManager.LoadScene("GameScene");
        // yield return new WaitForSeconds(0.5f); // รอโหลดแป๊บนึง

        // 2. เมื่อเข้าฉากใหม่ ให้เริ่มนับถอยหลัง
        StartNewRound();
        yield return null;
    }
#endregion

#region      --- Gameplay ---

    public void StartNewRound()
    {
        ClearObjectsAndUInMenuForGamePlay();

        ResetPosition();
        currentGameState = GameState.Countdown;

        if (activeBalls.Count > 0)
        {
            GameObject currentBallObj = activeBalls[activeBalls.Count - 1];
            Ball ballScript = currentBallObj.GetComponent<Ball>();

            if (ballScript != null)
            {
                if (isFirstRound)
                {
                    Debug.Log("[Flow] First Round: Ball at Center");
                    ballScript.ResetBallToCenter();
                    isFirstRound = false;
                }
                else
                {
                    var loserIdentity = connectedPlayers.Find(p => p.side == lastRoundLoser);
                    if (loserIdentity != null && loserIdentity.controllerReference != null)
                    {
                        ballScript.SetupFollowLoser(loserIdentity.controllerReference);
                    }
                }
            }
        }
        // รีเซ็ตเลือดในข้อมูล
        foreach (var player in connectedPlayers)
        {
            player.controllerReference.isDead = false;
            player.currentHealth = player.maxHealth;
        }
        
        HealthManager.Instance.ResetAllHealthBars();
        TimeManager.Instance.StartPreRoundCountdown(3 , () => {currentGameState = GameState.Playing;});
    }

    private void ClearObjectsAndUInMenuForGamePlay()
    {
        foreach (GameObject obj in menuObjectsToClear)
        {
            obj.SetActive(false);
        }
    }

    public void ApplyDamage(PlayerSide side, float damageAmount)
    {
        if (currentGameState != GameState.Playing) return;

        var player = connectedPlayers.Find(p => p.side == side);

        if (player != null)
        {
            player.currentHealth -= damageAmount;
            player.controllerReference.anim.SetTrigger("GetHit");
            player.currentHealth = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);

            HealthManager.Instance.UpdateHealthUI(player.side, player.currentHealth, player.maxHealth);

            if (player.currentHealth <= 0)
            {
                player.controllerReference.isDead = true;
                OnPlayerDefeated(side);
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
        isFirstRound = true;
        //Show who won the match and return to lobby or something here
    }

    public void HandleTimeOut()
    {
        if (currentGameState != GameState.Playing) return;

        currentGameState = GameState.RoundEnd;
        TimeManager.Instance.showImageUI.SetEnable(true);
        TimeManager.Instance.showImageUI.ShowImageDisplay();
        Debug.Log("[Flow] Time Up!");

        // หาผู้ชนะตอนหมดเวลา (คนที่มีเลือดมากกว่า)
        PlayerIdentity p1 = connectedPlayers.Find(p => p.side == PlayerSide.Red);
        PlayerIdentity p2 = connectedPlayers.Find(p => p.side == PlayerSide.Blue);

        if (p1 != null && p2 != null)
        {
            if (p1.currentHealth > p2.currentHealth)
            {
                OnRoundWin(p1);
            }
            else if (p2.currentHealth > p1.currentHealth)
            {
                OnRoundWin(p2);
            }
            else
            {
                // กรณีเลือดเท่ากัน (Draw)
                Debug.Log("Round Draw!");
                StartCoroutine(PrepareNextRoundRoutine());
            }
        }
    }

    private void OnRoundWin(PlayerIdentity winner)
    {
        winner.roundWins++;
        WinSlotManager.Instance?.AddWin(winner.side);
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

    public void OnPlayerDefeated(PlayerSide loserSide)
    {
        if (currentGameState != GameState.Playing) return;
        
        currentGameState = GameState.RoundEnd;
        TimeManager.Instance.showImageUI.SetEnable(true);
        TimeManager.Instance.showImageUI.ShowImageDisplay();
        
        lastRoundLoser = loserSide;

        // --- ส่วนที่เพิ่ม: เมื่อมีคนตาย ต้องสั่งหยุดเวลา 60 วิ ทันที ---
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.StopTimer();
        }

        PlayerIdentity winner = connectedPlayers.Find(p => p.side != loserSide);
        if (winner != null)
        {
            OnRoundWin(winner);
        }
    }

    public void ResetPosition()
    {
        ClearAllBalls();
        SpawnBall();

        foreach (var playerIdentity in connectedPlayers)
        {
            if (playerIdentity.controllerReference != null)
            {
                PlayerController playerController = playerIdentity.controllerReference;
                Transform spawnTarget = (playerController.side == PlayerSide.Red) ? MultiplayerManager.Instance.redSpawnPoint : MultiplayerManager.Instance.blueSpawnPoint;

                if (spawnTarget != null) playerController.transform.position = spawnTarget.position;

                Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
                if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

                bool shouldFaceRight = (playerController.side == PlayerSide.Red);
                if (playerController.facingRight != shouldFaceRight) playerController.flip();
                
                playerController.SetMoveable(true); 
            }
        }
        //Time.timeScale = 1f;
    }

    public void SpawnBall()
    {
        if (ballPrefab == null || GameplayballSpawnPoint == null)
        {
            Debug.LogWarning("[Flow] Ball Prefab or Spawn Point is missing!");
            return;
        }

        GameObject newBall = Instantiate(ballPrefab, GameplayballSpawnPoint.position, Quaternion.identity);
        
        activeBalls.Add(newBall);

        Debug.Log("[Flow] Ball Spawned!");
    }
    public void ClearAllBalls()
    {
        foreach (GameObject ball in activeBalls)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
        }

        activeBalls.Clear();

        GameObject[] extraBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject eb in extraBalls)
        {
            Destroy(eb);
        }

        Debug.Log("[Flow] All balls cleared.");
    }

    public bool CanPlayersMove()
    {
        switch (currentGameState)
        {
            case GameState.Lobby:     return allowMoveInLobby;
            case GameState.Countdown: return allowMoveInCountdown;
            case GameState.Playing:   return allowMoveInPlaying;
            case GameState.RoundEnd:  return allowMoveInRoundEnd;
            case GameState.MatchOver: return allowMoveInMatchOver;
            default: return false;
        }
    }
#endregion

}