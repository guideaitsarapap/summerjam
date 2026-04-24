using UnityEngine;

public class UIInteractiveWorldSpace : UIComponent, IHittable
{
    [SerializeField] protected ButtonWoldSpaceType buttonType;

    public void Start()
    {
        UIManager.Instance.AddUIComponent(this);
        if(UIType == UIType.Setting) SetEnable(false);
    }

    public virtual void OnGetHit(PlayerController hitter, HitType hitType)
    {
        SoundManager.instance.PlaySound(SoundType.Object_Bounce);
        // switch (buttonType)
        // {
        //     case ButtonWoldSpaceType.Quit:
        //         GoQuitGame();
        //         break;
        //     case ButtonWoldSpaceType.Setting:
        //         GoToSettingMenu();
        //         break;
        //     case ButtonWoldSpaceType.Back:
        //         GoToMainMenu();
        //         break;
        // }
        
    }

    public void GoToSettingMenu()
    {
        GameFlowManager.Instance.GoToSettingFromLobby();
    }

    public void GoToMainMenu()
    {
        GameFlowManager.Instance.ReturnToLobbyFromSetting();
    }
    
    public void GoQuitGame()
    {
        Application.Quit();
    }

}

public enum ButtonWoldSpaceType
{
    Quit,
    Setting,
    Back,
    MusicSetting,
    SoundEffectSetting,
}
