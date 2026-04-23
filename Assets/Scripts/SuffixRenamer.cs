using UnityEditor;
using UnityEngine;
using System.IO;

public class SuffixRenamer : EditorWindow
{
    private string suffix = "_Suffix";

    [MenuItem("Tools/Suffix Renamer")]
    public static void ShowWindow() => GetWindow<SuffixRenamer>("Suffix Renamer");

    void OnGUI()
    {
        GUILayout.Label("Batch Add Suffix", EditorStyles.boldLabel);
        suffix = EditorGUILayout.TextField("Suffix to Add", suffix);

        if (GUILayout.Button("Apply Suffix"))
        {
            RenameSelectedAssets();
        }
    }

    void RenameSelectedAssets()
    {
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) continue;

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string newName = fileName + suffix;

            // AssetDatabase.RenameAsset handles the actual file renaming
            AssetDatabase.RenameAsset(assetPath, newName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}