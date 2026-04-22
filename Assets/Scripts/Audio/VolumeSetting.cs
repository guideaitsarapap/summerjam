using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    // https://www.youtube.com/watch?v=G-JUp8AMEx0 Watch this, This is ref code.
    // Set Slider 0.0001 to 1.2
    [SerializeField] AudioMixer myMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider ambientSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetAmbientVolume();
        }
    }

    public void SetMusicVolume()
    {
        if (musicSlider == null)
            return;
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        if (sfxSlider == null)
            return;
        float volume = sfxSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetAmbientVolume()
    {
        if (ambientSlider == null)
            return;
        float volume = ambientSlider.value;
        myMixer.SetFloat("ambient", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("ambientVolume", volume);
    }



    private void LoadVolume()
    {
        if(musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            else Debug.LogWarning("Music slider reference is missing!");
        if(sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            else Debug.LogWarning("SFX slider reference is missing!");
        if(ambientSlider != null) ambientSlider.value = PlayerPrefs.GetFloat("ambientVolume");
            else Debug.LogWarning("Ambient slider reference is missing!");

        SetMusicVolume();
        SetSFXVolume();
        SetAmbientVolume();
    }

}
