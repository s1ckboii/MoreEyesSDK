using MoreEyes.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public class MoreEyesEditorMenu : EditorWindow
{
    //Create MoreEyes Mod
    [MenuItem("More Eyes/Create Mod")]
    static void CreateModAsset()
    {
        var window = GetWindow<MoreEyesEditorMenu>();
        window.titleContent = new GUIContent("Create MoreEyes Mod");
    }

    private TextField modName;
    private TextField modVer;
    private TextField modAuthor;


    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label textlabel = new("Create a Mod Asset for your More Eyes Mod.\n");
        modName = new("Mod Name", 32, false, false, '*');
        root.Add(modName);
        modVer = new("Mod Version", 5, false, false, '*');
        root.Add(modVer);
        modAuthor = new("Mod Author", 64, false, false, '*');
        root.Add(modAuthor);

        // Create button
        Button submit = new(CreateMod)
        {
            name = "Submit",
            text = "Create Mod"
        };
        root.Add(submit);
    }

    private void CreateMod()
    {
        var eyesMod = ObjectFactory.CreateInstance<MoreEyesMod>();
        eyesMod.SetName(modName.text);
        eyesMod.SetAuthor(modAuthor.text);
        eyesMod.SetVersion(modVer.text);

        string modAssetName = $"{eyesMod.Name}.asset";
        string currentPath = GetActiveWindowPath();
        string modPath = Path.Combine(currentPath, modAssetName);
        if (File.Exists(modPath))
        {
            if (!EditorUtility.DisplayDialog("Overwrite Asset?", $"The asset {modAssetName} already exists. Overwrite?", "Yes", "No"))
                return;
        }

        AssetDatabase.CreateAsset(eyesMod, modPath);
        var window = GetWindow<MoreEyesEditorMenu>();
        window.Close();
    }

    [MenuItem("More Eyes/Package Mods")]
    private static void BuildAssetBundle()
    {
        var guids = AssetDatabase.FindAssets("t:MoreEyesMod");
        List<MoreEyesMod> mods = new();
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            mods.Add(AssetDatabase.LoadAssetAtPath<MoreEyesMod>(assetPath);
        }

        if(mods.Count == 0)
        {
            Debug.LogWarning("No existing Mod assets found!");
            return;
        }

        string getPath = EditorUtility.SaveFolderPanel($"EyesBundles Location", "", "");
        if (string.IsNullOrEmpty(getPath))
            return;

        foreach (var mod in mods)
        {
            string assetPath = AssetDatabase.GetAssetPath(mod);
            AssetImporter.GetAtPath(assetPath).assetBundleName = mod.name + ".eyesbundle";

            foreach (var prefab in mod.Prefabs)
            {
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                AssetImporter.GetAtPath(prefabPath).assetBundleName = mod.name + ".eyesbundle";
            }
        }

        BuildPipeline.BuildAssetBundles(outputFolder, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        Debug.Log($"Eyes Bundle saved to {outputFolder}");
    }
}

//https://discussions.unity.com/t/how-to-get-path-from-the-current-opened-folder-in-the-project-window-in-unity-editor/226209
private static string GetActiveWindowPath()
{
    Type projectWindowUtilType = typeof(ProjectWindowUtil);
    MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
    object obj = getActiveFolderPath.Invoke(null, new object[0]);
    return obj.ToString();
}
}