using UnityEngine;

[CreateAssetMenu(menuName = "ToolKitByArm/Ambients ScriptableObject", fileName = "Ambient SO")]
public class AmbientSO : ScriptableObject
{
    [Header("Information")]
    [Space(15)]
    [TextArea(1,10)]
    public string NoEdit = "You need to edit in AmbientValues file that what name of ambient is? Example in there. No need to click plus or minus here";
    [Space(30)]

    public AmbientList[] ambients;
}
