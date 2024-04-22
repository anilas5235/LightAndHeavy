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
            var script = fieldInfo.GetValue(property.serializedObject.targetObject) as DashHandler;
            _canDashProperty = property.FindPropertyRelative(nameof(script.canDash));
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canDashProperty);
            if (_canDashProperty.boolValue)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.dashParams)));
                EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.currentDashState)));
            }

            EditorGUILayout.EndVertical();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}