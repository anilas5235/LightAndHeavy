using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(WallSlideParameters))]
    public class WallSlideParametersPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _canWallSlideProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _canWallSlideProperty = property.FindPropertyRelative("canWallSlide");
           EditorGUILayout.BeginVertical("box");
                
           EditorGUILayout.PropertyField(_canWallSlideProperty);
           if (_canWallSlideProperty.boolValue)
           {
               EditorGUILayout.PropertyField(property.FindPropertyRelative("maxWallSlidingSpeed"));
           }

           EditorGUILayout.EndVertical();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }
}