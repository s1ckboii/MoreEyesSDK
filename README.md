# MoreEyesSDK
Unity editor tools for creating MoreEyes mods for R.E.P.O.


## 🚧 Work in Progress

- Example pictures will be added to the repo soon for easier navigation.
- For now, you can create your own mod by following the steps below.

## 📦 Setup & Usage

1. Follow the setup guide for [R.E.P.O. Project Patcher by Kesomannen](https://github.com/Kesomannen/unity-repo-project-patcher).
2. Add this package to your Unity project: `https://github.com/darmuh/MoreEyesSDK.git`
3. In the Unity editor, press **`+` or `Right Click` inside a folder → MoreEyes → ˙Mod** in your preferred folder.
4. Fill out the generated **ScriptableObject (SO)** with your mod’s details.
5. Once ready, at the top of the editor you’ll find a **More Eyes** menu:
- Click **More Eyes** → **Package Mods**
- Select your preferred folder
- Build the package
6. After building, follow [Thunderstore’s instructions](https://thunderstore.io/package/create/docs) for publishing your mod.
- You can also preview your README using [Thunderstore’s Markdown Preview](https://thunderstore.io/tools/markdown-preview).
- Be sure your `manifest.json` lists [our mod](https://thunderstore.io/c/repo/p/s1ckboy/MoreEyes )'s dependency string.

## Naming convention for prefabs

- Prefab names should follow the pattern: `<name>_<pupil|iris>_<left|right|both>`
- Example setup for clarity:
  - `cat_pupil_right`
  - `cat_pupil_left`
  - `cat_iris_right`
  - `cat_iris_left`
  - `spiral1_pupil_both`
 
### ℹ️ More info

When uploading a mod, please include at minimum:
- A short list of what it contains
- A few screenshots of your pupils / irises, for example: `![](https://i.imgur.com/example.png)`
