using UnityEditor;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomEditor(typeof(AdvancedCharacterController2D))]
    public class AdvancedCharacterController2DCustomEditor : BasicCharacterController2DCustomEditor
    {
        protected override void AdditionalEvents(SerializedObject serializedObj)
        {
            
        }

        protected override void AdditionalInfos(SerializedObject serializedObj)
        {
            EditorGUILayout.PropertyField(serializedObj.FindProperty("detectRightWall"));
            EditorGUILayout.PropertyField(serializedObj.FindProperty("detectLeftWall"));
        }

        protected override void AdditionalSettings(SerializedObject serializedObj)
        {
            
        }
    }
}