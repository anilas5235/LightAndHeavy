using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(DashParams))]
    public class DashParamsPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _useDashCoolDownProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginVertical("box");
            _useDashCoolDownProperty = property.FindPropertyRelative("useDashCoolDown");
            EditorGUILayout.PropertyField(property.FindPropertyRelative("dashSpeed"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("dashDuration"));
            EditorGUILayout.PropertyField(_useDashCoolDownProperty);
            if (_useDashCoolDownProperty.boolValue)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("dashCoolDown"));
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}