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
            var script = fieldInfo.GetValue(property.serializedObject.targetObject) as DashParams;
            EditorGUILayout.BeginVertical("box");
            _useDashCoolDownProperty = property.FindPropertyRelative(nameof(script.useDashCoolDown));
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.dashSpeed)));
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.dashDuration)));
            EditorGUILayout.PropertyField(_useDashCoolDownProperty);
            if (_useDashCoolDownProperty.boolValue)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.dashCoolDown)));
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}