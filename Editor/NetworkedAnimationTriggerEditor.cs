using UnityEditor;
using UnityEngine;
using MoreEyes.Addons;
using static MoreEyes.Utility.Enums;

[CustomEditor(typeof(NetworkedAnimationTrigger))]
public class NetworkedAnimationTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NetworkedAnimationTrigger nat = (NetworkedAnimationTrigger)target;

        nat.triggerKey = (NumpadKey)EditorGUILayout.EnumPopup("Trigger Key", nat.triggerKey);
        nat.paramName = EditorGUILayout.TextField("Parameter Name", nat.paramName);
        nat.paramType = (NetworkedAnimationTrigger.ParamType)EditorGUILayout.EnumPopup("Parameter Type", nat.paramType);

        // Conditionally show float or int field
        switch (nat.paramType)
        {
            case NetworkedAnimationTrigger.ParamType.Float:
                nat.floatValue = EditorGUILayout.FloatField("Float Value", nat.floatValue);
                break;

            case NetworkedAnimationTrigger.ParamType.Int:
                nat.intValue = EditorGUILayout.IntField("Int Value", nat.intValue);
                break;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(nat);
    }
}