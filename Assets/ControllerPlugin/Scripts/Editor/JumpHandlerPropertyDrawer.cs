using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(JumpHandler))]
    public class JumpHandlerPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _canJumpProperty;
        private SerializedProperty _canAirJumpProperty;
        private SerializedProperty _infiniteAirJumpsProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _canJumpProperty = property.FindPropertyRelative("canJump");
            _canAirJumpProperty = property.FindPropertyRelative("canAirJump");
            _infiniteAirJumpsProperty = property.FindPropertyRelative("infiniteAirJumps");
           
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canJumpProperty);
            if (_canJumpProperty.boolValue)
            {
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("jumpHeight"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("timeTillApex"));
                    EditorGUILayout.PropertyField(_canAirJumpProperty);
                    if (_canAirJumpProperty.boolValue)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.PropertyField(_infiniteAirJumpsProperty);
                        if (!_infiniteAirJumpsProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(property.FindPropertyRelative("maxAirJumps"));
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}