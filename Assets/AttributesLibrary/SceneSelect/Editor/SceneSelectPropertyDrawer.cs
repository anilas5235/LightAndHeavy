using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AttributesLibrary.SceneSelect.Editor
{
    [CustomPropertyDrawer(typeof(SceneSelect))]
    public class SceneSelectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);


            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
            var names = new string[scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
            {
                names[i] = scenes[i].path.Split('/').Last().Split('.').First();
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUILayout.Popup(property.intValue, names);
                    break;
                case SerializedPropertyType.String:
                    var oldIndex = 0;
                    for (var i = 0; i < names.Length; i++)
                    {
                        if (property.stringValue != names[i]) continue;
                        oldIndex = i;
                        break;
                    }
                    var newIndex = EditorGUILayout.Popup(oldIndex, names);
                    property.stringValue = names[newIndex];
                    break;
                
                default:
                    EditorGUILayout.HelpBox("SceneSelect only works with int and string", MessageType.Warning);
                    break;
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}