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

    private static TextField modName;
    private static TextField modVer;
    private static TextField modAuthor;


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
        var currentPath = GetActiveWindowPath();
        string modPath = Path.Combine(currentPath, modAssetName);

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
            mods.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(MoreEyesMod)) as MoreEyesMod);
        }
        ;

        if (mods.Count == 0)
        {
            Debug.LogWarning("No existing Mod assets found!");
        }

        string getPath = EditorUtility.SaveFolderPanel($"EyesBundles Location", "", "");
        List<AssetBundleBuild> bundlebuilds = new();
        string bundles = "Assets/bundleFiles";
        if (!AssetDatabase.IsValidFolder(bundles))
            AssetDatabase.CreateFolder("Assets", "bundleFiles");


        foreach (var mod in mods)
        {
            List<string> assets = new()
            {
                AssetDatabase.GetAssetPath(mod)
            };
            foreach (var prefab in mod.Prefabs)
                assets.Add(AssetDatabase.GetAssetPath(prefab));


            bundlebuilds.Add(new AssetBundleBuild()
            {
                assetBundleName = mod.name,
                assetNames = assets.ToArray(),
            });
        }

        var buildParams = new BuildAssetBundlesParameters()
        {
            bundleDefinitions = bundlebuilds.ToArray(),
            outputPath = bundles,
            options = BuildAssetBundleOptions.ChunkBasedCompression,
            targetPlatform = BuildTarget.StandaloneWindows64
        };

        var file = BuildPipeline.BuildAssetBundles(buildParams);

        foreach (var fileName in file.GetAllAssetBundles())
        {
            string finalBundlePath = Path.Combine(getPath, fileName + ".eyesbundle");
            if (File.Exists(finalBundlePath))
                File.Delete(finalBundlePath);

            File.Copy(Path.Combine(bundles, fileName), finalBundlePath);
            Debug.Log($"Eyes Bundle saved to {finalBundlePath}");
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