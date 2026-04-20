using UnityEngine;
using UnityEngine.UI;

// --------------------------
//
// Example implementation of a UIComponent
//
// --------------------------
public class PauseUI : UIComponent
{
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button gameOverBtn;
    [SerializeField] private Button toMenuBtn;

    private void OnEnable()
    {
        resumeBtn.onClick.AddListener(OnDisabled);
        toMenuBtn.onClick.AddListener(ToMenuHandler);
        settingsBtn.onClick.AddListener(SettingsHandler);
        gameOverBtn.onClick.AddListener(GameOverHandler);
    }
    
    private void OnDisable()
    {
        resumeBtn.onClick.RemoveListener(OnDisabled);
        toMenuBtn.onClick.RemoveListener(ToMenuHandler);
        settingsBtn.onClick.RemoveListener(SettingsHandler);
        gameOverBtn.onClick.RemoveListener(GameOverHandler);
    }

    // remove or modify as needed
    protected override void OnEnabled()
    {
        base.OnEnabled();
        Time.timeScale = 0; 
    }

    protected override void OnDisabled()
    {
        base.OnEnabled();
        Time.timeScale = 1; 
    }

    private void SettingsHandler()
    {
        settingsPanel.SetActive(true);
    }
    
    private void GameOverHandler()
    {}
    
    private void ToMenuHandler()
    {}
}