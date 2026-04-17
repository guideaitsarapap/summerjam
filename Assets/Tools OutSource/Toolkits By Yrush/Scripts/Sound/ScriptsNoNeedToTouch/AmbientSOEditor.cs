#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;


[CustomEditor(typeof(AmbientSO))]
public class AmbientSOEditor : Editor
{
    private void OnEnable()
    {
        ref AmbientList[] ambientList = ref ((AmbientSO)target).ambients;

        if (ambientList == null)
            return;

        string[] names = Enum.GetNames(typeof(AmbientType));
        bool differentSize = names.Length != ambientList.Length;

        Dictionary<string, AmbientList> ambients = new();

        if (differentSize)
        {
            for (int i = 0; i < ambientList.Length; ++i)
            {
                ambients.Add(ambientList[i].name, ambientList[i]);
            }
        }

        Array.Resize(ref ambientList, names.Length);
        for (int i = 0; i < ambientList.Length; i++)
        {
            string currentName = names[i];
            ambientList[i].name = currentName;
            if (ambientList[i].volume == 0) ambientList[i].volume = 1;

            if (differentSize)
            {
                if (ambients.ContainsKey(currentName))
                {
                    AmbientList current = ambients[currentName];
                    UpdateElement(ref ambientList[i], current.volume, current.ambients, current.mixer);
                }
                else
                    UpdateElement(ref ambientList[i], 1, null, null);

                static void UpdateElement(ref AmbientList element, float volume, AudioClip sounds, AudioMixerGroup mixer)
                {
                    element.volume = volume;
                    element.ambients = sounds;
                    element.mixer = mixer;
                }
            }
        }
    }

}

#endif