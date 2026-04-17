//Author: Small Hedge Games
//Updated: 13/06/2024

using System;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [Header("INFORMATION!!")]
    [TextArea(1, 10)]
    public string NoEditAlert = "You need to create SoundsSO, MusicSO, AmbientSO first. Mixer for those too.";
    [Space(10)]

    [Tooltip("Right Click at files -> Create -> ToolKitByArm -> Sounds ScriptableObject. And put in here")]
    [SerializeField] private SoundsSO SoundsScriptableObject;
    [Tooltip("Right Click at files -> Create -> ToolKitByArm -> Music ScriptableObject. And put in here")]
    [SerializeField] private MusicSO MusicScriptableObject;
    [Tooltip("Right Click at files -> Create -> ToolKitByArm -> Ambient ScriptableObject. And put in here")]
    [SerializeField] private AmbientSO AudioScriptableObject;

    public static SoundManager instance;
    [HideInInspector] public AudioSource audioSFXSource;
    [HideInInspector] public AudioSource audioMusicSource;
    [HideInInspector] public AudioSource audioAmbientSource;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
        
    }

    public void PlaySound(SoundType sound, AudioSource source = null, float volume = 1) // If have own AudioSource position to play Give it name then
    {
        SoundList soundList = instance.SoundsScriptableObject.sounds[(int)sound];
        AudioClip[] clips = soundList.sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (source)
        {
            if (soundList.IsRandomPitch) source.pitch = UnityEngine.Random.Range(1f, 1.5f);
            else source.pitch = 1f;

            source.outputAudioMixerGroup = soundList.mixer;
            source.clip = randomClip;
            source.volume = volume * soundList.volume;
            source.Play();
        }
        else
        {
            if (soundList.IsRandomPitch) instance.audioSFXSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
            else instance.audioSFXSource.pitch = 1f;

            instance.audioSFXSource.outputAudioMixerGroup = soundList.mixer;
            instance.audioSFXSource.PlayOneShot(randomClip, volume * soundList.volume);
        }
    }

    public void PlayMusic(MusicType music, AudioSource source = null, float volume = 1)
    {
        MusicList musicList = instance.MusicScriptableObject.musics[(int)music];
        AudioClip clips = musicList.musics;
        //AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (source)
        {

            source.outputAudioMixerGroup = musicList.mixer;
            //source.clip = randomClip;
            source.volume = volume * musicList.volume;
            source.Play();
        }
        else
        {
            instance.audioMusicSource.outputAudioMixerGroup = musicList.mixer;
            instance.audioMusicSource.volume = volume * musicList.volume;
            instance.audioMusicSource.resource = clips;
            instance.audioMusicSource.Play();
            //instance.audioMusicSource.PlayOneShot(clips, volume * musicList.volume);
        }
        

    }

    public void StopPlayMusic(AudioSource source = null)
    {

        if (source)
        {
            source.Stop();
        }
        else instance.audioMusicSource.Stop();

    }

    public void PlayAmbient(AmbientType ambient, AudioSource source = null, float volume = 1)
    {
        AmbientList ambientList = instance.AudioScriptableObject.ambients[(int)ambient];
        AudioClip clips = ambientList.ambients;
        //AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (source)
        {

            source.outputAudioMixerGroup = ambientList.mixer;
            //source.clip = randomClip;
            source.volume = volume * ambientList.volume;
            source.Play();
        }
        else
        {
            instance.audioAmbientSource.outputAudioMixerGroup = ambientList.mixer;
            instance.audioAmbientSource.volume = volume * ambientList.volume;
            instance.audioAmbientSource.resource = clips;
            instance.audioAmbientSource.Play();
            //instance.audioAmbientSource.PlayOneShot(clips, volume * ambientList.volume);
        }


    }

    public void StopPlayAmbient(AudioSource source = null)
    {

        if (source)
        {
            source.Stop();
        }
        else instance.audioAmbientSource.Stop();

    }


}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public bool IsRandomPitch;
    public AudioClip[] sounds;  
    // You need to set SoundValue enum What sound you have before do anything
}

[Serializable]
public struct MusicList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip musics;
}

[Serializable]
public struct AmbientList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip ambients;
}
