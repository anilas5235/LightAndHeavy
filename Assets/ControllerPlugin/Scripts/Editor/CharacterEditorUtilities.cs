using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    public static class CharacterEditorUtilities
    {
        private static readonly GUIStyle HeaderStyle= new GUIStyle()
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState()
            {
                textColor = Color.white,
            }
        };
        public static void Space()
        {
            GUILayout.Space(5);
        }

        public static void DrawSetting(SerializedProperty serializedProperty)
        {
            MakeHeader(serializedProperty.displayName);
            EditorGUILayout.PropertyField(serializedProperty);
        }

        public static void MakeHeader(string header)
        {
            Space();
            GUILayout.Label(header, HeaderStyle);
            Space();
        }
        
        public static void DrawScriptField<T>(MonoBehaviour target) where T : MonoBehaviour
        {
            EditorGUI.BeginDisabledGroup(true); 
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((T)target), typeof(T), false);
            EditorGUI.EndDisabledGroup();
        }
    }
}