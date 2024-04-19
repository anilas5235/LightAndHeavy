using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(SpeedParameters))]
    public class SpeedParametersPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("maxSpeed"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("maxFallSpeed"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("maxAcceleration"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("maxAirAcceleration"));
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
    