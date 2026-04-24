using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIComponent : MonoBehaviour
{
    public CanvasGroup CanvasGroup { get; private set; }
    public UIType UIType => uiType;

    [SerializeField] private UIType uiType;
    [SerializeField] private bool OutsideMainCanvas = false;

    public void SetEnable(bool isEnable)
    {
        CanvasGroup.interactable = isEnable;
        CanvasGroup.blocksRaycasts = isEnable;

        if (isEnable) OnEnabled();
        else OnDisabled();
    
        gameObject.SetActive(isEnable);
    }

    protected void Start()
    {
        if(OutsideMainCanvas)
        {
            UIManager.Instance.AddUIComponent(this);
        }

    }

    protected virtual void OnEnabled() { }
    protected virtual void OnDisabled() { }

    private void OnValidate()
    {
        CanvasGroup ??= GetComponent<CanvasGroup>();
    }
}