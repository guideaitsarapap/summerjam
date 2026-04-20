using System.Collections.Generic;
using UnityEngine;


public enum UIType
{
    Menu,
    Lobby,
    CountDown,
    Game,
    Pause,
    GameOver,
}

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private List<UIComponent> uiComponents;

    private void Awake()
    {
        uiComponents = new List<UIComponent>();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        GetUIComponents();
    }

    /// <summary>
    /// Enables ui components of the ui type
    /// </summary>
    /// <param name="type"> Menu, Game, Pause, or GameOver </param>
    /// <param name="isEnable"></param>
    public void SetEnableUIComponent(UIType type, bool isEnable)
    {
        foreach (var component in uiComponents)
        {
            if (component.UIType == type)
            {
                component.SetEnable(isEnable);
            }
        }
    }

    private void GetUIComponents()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out UIComponent uiComponent))
            {
                uiComponents.Add(uiComponent);
            }
        }
    }
}