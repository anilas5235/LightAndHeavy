using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(SpeedParameters))]
    public class SpeedParametersPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var script = fieldInfo.GetValue(property.serializedObject.targetObject) as SpeedParameters;
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.maxSpeed)));
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.maxFallSpeed)));
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.maxAcceleration)));
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.maxAirAcceleration)));
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}
    