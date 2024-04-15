using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(FloatMinMax))]
    public class FloatMinMaxPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var minProp= property.FindPropertyRelative("min");
            var maxProp= property.FindPropertyRelative("max");

            var labelPosition = new Rect(position);

            position = EditorGUI.PrefixLabel(labelPosition, GUIUtility.GetControlID(FocusType.Passive),
                label);
            position.width -= 5;

            EditorGUI.indentLevel = 0;
            var labRec = new Rect(position) { width = 30 };
            var rec = new Rect(position)
            {
                width = (position.width-labRec.width*2) / 2,
                x = position.x+30,
            };
            var totalLength = labRec.width + rec.width + 4;
            
            EditorGUI.LabelField(labRec,minProp.displayName);
            EditorGUI.PropertyField(rec, minProp,GUIContent.none);
            rec.x += totalLength;
            labRec.x += totalLength;
            EditorGUI.LabelField(labRec,maxProp.displayName);
            EditorGUI.PropertyField(rec, maxProp,GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}