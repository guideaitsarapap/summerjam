using UnityEngine;

[CreateAssetMenu(menuName = "ToolKitByArm/Musics ScriptableObject", fileName = "Music SO")]
public class MusicSO : ScriptableObject
{
    [Header("Information")]
    [Space(15)]
    [TextArea(1,10)]
    public string NoEdit = "You need to edit in MusicValues file that what name of music is? Example in there. No need to click plus or minus here";
    [Space(15)]

    public MusicList[] musics;
}
