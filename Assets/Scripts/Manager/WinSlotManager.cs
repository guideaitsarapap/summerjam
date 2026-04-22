using UnityEngine;
using UnityEngine.UI;

public class WinSlotManager : MonoBehaviour
{
    public static WinSlotManager Instance { get; private set; }

    [SerializeField] private Image[] redSlots;
    [SerializeField] private Image[] blueSlots;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private Sprite redWinSprite;
    [SerializeField] private Sprite blueWinSprite;
    [SerializeField] private float winSpriteScale = 1.5f;
    [SerializeField] private float emptySpriteScale = 1f;

    private void Awake()
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

    public void AddWin(PlayerSide side)
    {
        Image[] slots = side == PlayerSide.Red ? redSlots : blueSlots;
        Sprite winSprite = side == PlayerSide.Red ? redWinSprite : blueWinSprite;
        foreach(var slot in slots)
        {
            if (slot.sprite == emptySlotSprite)
            {
                slot.sprite = winSprite;
                slot.rectTransform.localScale = Vector3.one * winSpriteScale;
                return;
            }
        }

        Debug.LogWarning($"[WinSlotManager] No empty slots available for {side}!");
    }

    public void ResetWins()
    {
        foreach(var slot in redSlots)
        {
            slot.sprite = emptySlotSprite;
            slot.rectTransform.localScale = Vector3.one * emptySpriteScale;
        }
        foreach(var slot in blueSlots)
        {
            slot.sprite = emptySlotSprite;
            slot.rectTransform.localScale = Vector3.one * emptySpriteScale;
        }
    }
}
