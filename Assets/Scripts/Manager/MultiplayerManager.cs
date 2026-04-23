using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;
    
    [Header("Player Prefabs")]
    [SerializeField] private GameObject redPrefab;
    [SerializeField] private GameObject bluePrefab;

    [Header("Spawn Points")]
    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;
    public Transform redBallSpawnPoint;
    public Transform blueBallSpawnPoint;

    [Header("Ball Prefab for Ready State")]
    public GameObject ballPrefab;

    private PlayerInputManager manager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            manager = GetComponent<PlayerInputManager>();
            manager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 1. จัดการการ Join ผ่าน Keyboard (ใช้ Enter สำหรับทั้ง P1 และ P2)
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HandleKeyboardJoin();
        }

        // 2. จัดการการ Join ผ่าน Gamepad (ใช้ Start)
        HandleGamepadJoin();
    }

    private void HandleKeyboardJoin()
    {
        int playerCount = PlayerInput.all.Count;

        if (playerCount == 0)
        {
            // P1 เข้าคนแรก
            JoinPlayer(0, Keyboard.current, "P1_Keyboard");
        }
        else if (playerCount == 1)
        {
            // P2 เข้าคนที่สอง (แชร์ Keyboard เดียวกัน)
            // เช็คก่อนว่า P1 ไม่ได้ใช้ Keyboard ใน Scheme อื่นอยู่ (เพื่อความชัวร์)
            JoinPlayer(1, Keyboard.current, "P2_Keyboard");
        }
    }

    private void HandleGamepadJoin()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                int playerCount = PlayerInput.all.Count;

                if (playerCount == 0)
                {
                    JoinPlayer(0, gamepad, "Gamepad");
                    return;
                }
                else if (playerCount == 1)
                {
                    // เช็คว่าจอยนี้ไม่ใช่จอยที่ P1 ถืออยู่
                    bool isDeviceBusy = PlayerInput.all.Any(p => p.devices.Contains(gamepad));
                    if (!isDeviceBusy)
                    {
                        JoinPlayer(1, gamepad, "Gamepad");
                        return;
                    }
                }
            }
        }
    }

    private void JoinPlayer(int index, InputDevice device, string scheme)
    {
        // ป้องกันการ Join ซ้ำ index เดิมในเฟรมเดียวกัน
        if (PlayerInput.all.Any(p => p.playerIndex == index)) return;

        Debug.Log($"Attempting to Join: P{index + 1} | Device: {device.name} | Scheme: {scheme}");
        
        manager.playerPrefab = (index == 0) ? redPrefab : bluePrefab;

        // บังคับ Pair Device เข้ากับ Player Index นั้นๆ
        var player = manager.JoinPlayer(index, -1, scheme, device);

        if (player != null)
        {
            Debug.Log($"P{index + 1} JOINED SUCCESS! (Scheme: {scheme})");
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.RegisterNewPlayer(playerInput);
        }

        // ย้ายตำแหน่ง Spawn
        if (playerInput.playerIndex == 0)
        {
            Ball ball = Instantiate(ballPrefab, redBallSpawnPoint.position, Quaternion.identity).GetComponent<Ball>();
            ball.ResetBallToSideInMenuSceneOnly(redBallSpawnPoint.position);

            playerInput.transform.position = redSpawnPoint.position;
        }
        else if (playerInput.playerIndex == 1)
        {
            Ball ball = Instantiate(ballPrefab, blueBallSpawnPoint.position, Quaternion.identity).GetComponent<Ball>();
            ball.ResetBallToSideInMenuSceneOnly(blueBallSpawnPoint.position);

            playerInput.transform.position = blueSpawnPoint.position;
        }
    }

    public void ResetManagerForLobby()
    {
        var allPlayers = PlayerInput.all.ToList();
        foreach (var p in allPlayers)
        {
            Destroy(p.gameObject);
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.connectedPlayers.Clear();
        }

        Debug.Log("[MultiplayerManager] All players cleared. Ready for new joins.");
    }
}