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
    internal enum SelectedMenuItem
    {
        CreateMod,
        PackageMods
    }
    internal static SelectedMenuItem CurrentWindow;

    //Create MoreEyes Mod
    [MenuItem("More Eyes/Create Mod")]
    static void CreateModAsset()
    {
        CurrentWindow = SelectedMenuItem.CreateMod;
        if (HasOpenInstances<MoreEyesEditorMenu>())
            GetWindow<MoreEyesEditorMenu>().Close();

        GetWindow<MoreEyesEditorMenu>("Create MoreEyes Mod");
    }

    private static TextField modName;
    private static TextField modVer;
    private static TextField modAuthor;

    private static VisualElement HorizontalLine()
    {
        VisualElement horizontalLine = new();
        horizontalLine.style.height = 1;
        horizontalLine.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        horizontalLine.style.marginTop = 10;
        horizontalLine.style.marginBottom = 10;

        return horizontalLine;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        ScrollView scrollView = new()
        {
            horizontalScrollerVisibility = ScrollerVisibility.Hidden
        };
        rootVisualElement.Add(scrollView);

        if (CurrentWindow == SelectedMenuItem.CreateMod)
        {
            Label header = new($"Build your Mod's Scriptable Object");
            header.style.fontSize = 16;
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.paddingBottom = 3;
            header.style.paddingLeft = 1;
            header.style.paddingRight = 1;
            header.style.paddingTop = 14;
            root.Add(header);
            root.Add(HorizontalLine());
            modName = new("Mod Name", 32, false, false, '*');
            root.Add(modName);
            modVer = new("Mod Version", 5, false, false, '*');
            root.Add(modVer);
            modAuthor = new("Mod Author", 64, false, false, '*');
            root.Add(modAuthor);
            root.Add(HorizontalLine());
            // Create button
            Button submit = new(CreateMod)
            {
                name = "Submit",
                text = "Create Mod"
            };
            root.Add(submit);

            return;
        }

        if (CurrentWindow == SelectedMenuItem.PackageMods)
        {
            var guids = AssetDatabase.FindAssets("t:MoreEyesMod");
            List<MoreEyesMod> mods = new();
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                mods.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(MoreEyesMod)) as MoreEyesMod);
            }

            if (mods.Count == 0)
            {
                Debug.LogWarning("No existing Mod assets found!");
                return;
            }

            List<ModTrackValues> modTracker = new();
            foreach (var mod in mods)
            {
                Label header = new($"{mod.Name}");
                header.style.fontSize = 16;
                header.style.unityTextAlign = TextAnchor.MiddleCenter;
                header.style.unityFontStyleAndWeight = FontStyle.Bold;
                header.style.paddingBottom = 3;
                header.style.paddingLeft = 1;
                header.style.paddingRight = 1;
                header.style.paddingTop = 3;
                root.Add(header);

                Foldout foldout = new()
                {
                    value = false
                };
                root.Add(foldout);

                // Scrollable prefab list
                ScrollView prefabScroll = new ScrollView
                {
                    style =
        {
            maxHeight = 150, // Adjust to taste
            overflow = Overflow.Hidden
        }
                };

                foreach (var item in mod.Prefabs)
                {
                    prefabScroll.Add(new Label($"{item.name}"));
                }

                foldout.Add(prefabScroll);
                foldout.text = $"{mod.Name} Prefab List [{mod.Prefabs.Count}]";

                TextField path = new("Build path", 999, false, false, '*')
                {
                    tooltip = "Click to set path...",
                    value = ""
                };
                path.RegisterCallback<ClickEvent>(click =>
                {
                    string newPath = EditorUtility.SaveFolderPanel($"{mod.name} Build Location", "", "");
                    if (!string.IsNullOrEmpty(newPath))
                        path.value = newPath;

                    Debug.Log($"Selected path {newPath} to build {mod.name} bundle");
                });
                root.Add(path);

                Toggle modToggle = new()
                {
                    value = true,
                    text = "Package Contents",
                };
                root.Add(modToggle);

                modTracker.Add(new ModTrackValues(mod, path, modToggle));

                root.Add(HorizontalLine());
            }

            Button submit = new(() =>
            {
                List<ModTrackValues> buildList = new();
                foreach (var item in modTracker)
                {
                    if (item.BuildBundle.value && !string.IsNullOrEmpty(item.Path.text))
                        buildList.Add(item);
                }
                BundleMods(buildList);
            })
            {
                name = "Submit",
                text = "Create Bundles"
            };
            root.Add(submit);
        }
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

    private void BundleMods(List<ModTrackValues> buildList)
    {
        string bundles = "Assets/bundleFiles";
        if (!AssetDatabase.IsValidFolder(bundles))
            AssetDatabase.CreateFolder("Assets", "bundleFiles");

        foreach (var item in buildList)
        {
            List<AssetBundleBuild> bundlebuilds = new();
            List<string> assets = new()
            {
                AssetDatabase.GetAssetPath(item.Mod)
            };
            foreach (var prefab in item.Mod.Prefabs)
                assets.Add(AssetDatabase.GetAssetPath(prefab));

            foreach (string name in assets)
                Debug.Log($"bundling {name} in item: {item.Mod.Name}");

            bundlebuilds.Add(new AssetBundleBuild()
            {
                assetBundleName = item.Mod.name,
                assetNames = assets.ToArray(),
            });

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
                string finalBundlePath = Path.Combine(item.Path.text, fileName + ".eyes");
                if (File.Exists(finalBundlePath))
                    File.Delete(finalBundlePath);

                File.Copy(Path.Combine(bundles, fileName), finalBundlePath);
                Debug.Log($"Eyes Bundle saved to {Path.GetFullPath(finalBundlePath)}");
            }
        }
    }


    [MenuItem("More Eyes/Package Mods")]
    private static void BuildAssetBundle()
    {
        CurrentWindow = SelectedMenuItem.PackageMods;
        if (HasOpenInstances<MoreEyesEditorMenu>())
            GetWindow<MoreEyesEditorMenu>().Close();

        GetWindow<MoreEyesEditorMenu>("Package MoreEyes Mod");
    }

    //https://discussions.unity.com/t/how-to-get-path-from-the-current-opened-folder-in-the-project-window-in-unity-editor/226209
    private static string GetActiveWindowPath()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        return obj.ToString();
    }

    internal class ModTrackValues
    {
        internal MoreEyesMod Mod;
        internal TextField Path;
        internal Toggle BuildBundle;
        internal Toggle BuildManifest;

        internal ModTrackValues(MoreEyesMod _mod, TextField _path, Toggle _buildBundle)
        {
            Mod = _mod;
            Path = _path;
            BuildBundle = _buildBundle;
        }
    }
}