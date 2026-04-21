using UnityEngine;

public class PlayMusicAtMenu : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayMusic(MusicType.Menu);
    }
}
