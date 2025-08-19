// Slightly modified version of this scriptable object provided by REPOLib-SDK
// https://github.com/ZehsTeam/REPOLib/blob/main/REPOLib/Objects/Sdk/Mod.cs
using System.Collections.Generic;
using UnityEngine;

namespace MoreEyes.SDK
{
    /// <summary>
    /// A MoreEyes content mod.
    /// </summary>
    [CreateAssetMenu(menuName = "MoreEyes/Mod", order = 0, fileName = "New MoreEyes Mod")]
    public class MoreEyesMod : ScriptableObject
    {
        [SerializeField]
        private string _name = null!;

        [SerializeField]
        private string _author = null!;

        [SerializeField]
        private string _version = "1.0.0";

        /// <summary>
        /// The name of this mod.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The author of this mod.
        /// </summary>
        public string Author => _author;

        /// <summary>
        /// The version of this mod.
        /// </summary>
        public string Version => _version;

        /// <summary>
        /// Unqique identifier of this mod.<br/>
        /// Format is $"{<see cref="Author"/>}-{<see cref="Name"/>}-{<see cref="Version"/>}".
        /// </summary>
        public string Identifier => $"{Author}-{Name}-{Version}";

        [SerializeField]
        private List<GameObject> _prefabs = new();

        public List<GameObject> Prefabs => _prefabs;

        public void SetName(string value) => _name = value;
        public void SetAuthor(string value) => _author = value;
        public void SetVersion(string value) => _version = value;
        public void SetPrefabs(List<GameObject> value) => _prefabs = value;
    }
}
