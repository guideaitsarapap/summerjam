//Author: Small Hedge Games
//Updated: 13/06/2024

using UnityEngine;


[CreateAssetMenu(menuName = "ToolKitByArm/Sounds ScriptableObject", fileName = "Sounds SO")]
public class SoundsSO : ScriptableObject
{
    [Header("Information")]
    [Space(15)]
    [TextArea(1,10)]
    public string NoEdit = "You need to edit in SoundValues file that what name of sound is? Example in there. No need to click plus or minus here";
    [Space(15)]

    public SoundList[] sounds;
}
