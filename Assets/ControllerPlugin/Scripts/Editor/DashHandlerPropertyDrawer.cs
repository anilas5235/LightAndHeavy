using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(DashHandler))]
    public class DashHandlerPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _canDashProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _canDashProperty = property.FindPropertyRelative("canDash");
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canDashProperty);
            if (_canDashProperty.boolValue)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("dashParams"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("currentDashState"));
            }

            EditorGUILayout.EndVertical();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}