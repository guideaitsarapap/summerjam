using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject redPrefab;
    public GameObject bluePrefab;

    [Header("Spawn Points")]
    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;

    [Header("Join Settings")]
    public InputActionReference joinAction;

    private PlayerInputManager manager;

    void Awake()
    {
        manager = GetComponent<PlayerInputManager>();
        manager.playerPrefab = redPrefab;
    }

    void OnEnable() 
    {
        joinAction.action.Enable();
        joinAction.action.performed += OnJoinActionTriggered;
    }

    void OnDisable() 
    {
        joinAction.action.performed -= OnJoinActionTriggered;
        joinAction.action.Disable();
    }
    // ฟังก์ชันนี้จะทำงานเฉพาะเมื่อมีการกดปุ่ม Join ที่เราตั้งไว้ (เช่น Enter)
    private void OnJoinActionTriggered(InputAction.CallbackContext context)
    {
        // ถ้าผู้เล่นยังไม่ครบ 2 คน และคนกดคือ Keyboard
        if (PlayerInput.all.Count < 2 && context.control.device is Keyboard)
        {
            JoinKeyboardPlayer2();
        }
    }

    public void JoinKeyboardPlayer2()
    {
        //Player2 need to join with Join action referenced on keyboard to avoid conflict with gamepad input
        manager.JoinPlayer(1, -1, "P2_Keyboard", Keyboard.current);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (playerInput.playerIndex == 0)
        {
            playerInput.transform.position = redSpawnPoint.position;
            manager.playerPrefab = bluePrefab;
            Debug.Log($"P1 Joined with: {playerInput.currentControlScheme}");
        }
        else if (playerInput.playerIndex == 1)
        {
            playerInput.transform.position = blueSpawnPoint.position;
            Debug.Log($"P2 Joined with: {playerInput.currentControlScheme}");
        }
    }
}