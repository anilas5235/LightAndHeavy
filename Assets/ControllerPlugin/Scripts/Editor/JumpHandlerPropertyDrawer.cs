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
            var script = fieldInfo.GetValue(property.serializedObject.targetObject) as JumpHandler;
            _canJumpProperty = property.FindPropertyRelative(nameof(script.canJump));
            _canAirJumpProperty = property.FindPropertyRelative(nameof(script.canAirJump));
            _infiniteAirJumpsProperty = property.FindPropertyRelative(nameof(script.infiniteAirJumps));
           
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canJumpProperty);
            if (_canJumpProperty.boolValue)
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.jumpHeightMinMax)));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.timeTillApex)));
                    EditorGUILayout.PropertyField(_canAirJumpProperty);
                    if (_canAirJumpProperty.boolValue)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.PropertyField(_infiniteAirJumpsProperty);
                        if (!_infiniteAirJumpsProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(script.maxAirJumps)));
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}