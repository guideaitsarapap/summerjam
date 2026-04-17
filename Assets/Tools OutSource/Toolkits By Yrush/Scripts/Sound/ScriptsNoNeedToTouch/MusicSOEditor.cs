#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;


[CustomEditor(typeof(MusicSO))]
public class MusicSOEditor : Editor
{
    private void OnEnable()
    {
        ref MusicList[] musicList = ref ((MusicSO)target).musics;

        if (musicList == null)
            return;

        string[] names = Enum.GetNames(typeof(MusicType));
        bool differentSize = names.Length != musicList.Length;

        Dictionary<string, MusicList> musics = new();

        if (differentSize)
        {
            for (int i = 0; i < musicList.Length; ++i)
            {
                musics.Add(musicList[i].name, musicList[i]);
            }
        }

        Array.Resize(ref musicList, names.Length);
        for (int i = 0; i < musicList.Length; i++)
        {
            string currentName = names[i];
            musicList[i].name = currentName;
            if (musicList[i].volume == 0) musicList[i].volume = 1;

            if (differentSize)
            {
                if (musics.ContainsKey(currentName))
                {
                    MusicList current = musics[currentName];
                    UpdateElement(ref musicList[i], current.volume, current.musics, current.mixer);
                }
                else
                    UpdateElement(ref musicList[i], 1, null, null);

                static void UpdateElement(ref MusicList element, float volume, AudioClip sounds, AudioMixerGroup mixer)
                {
                    element.volume = volume;
                    element.musics = sounds;
                    element.mixer = mixer;
                }
            }
        }
    }

}

#endif