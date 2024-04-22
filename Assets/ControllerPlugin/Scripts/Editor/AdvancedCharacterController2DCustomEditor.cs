using System;
using UnityEditor;
using UnityEngine;
using static ControllerPlugin.Scripts.Editor.CharacterEditorUtilities;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomEditor(typeof(AdvancedCharacterController2D))]
    public class AdvancedCharacterController2DCustomEditor : UnityEditor.Editor
    {
        private static bool _infoFoldOut;
        private static bool _eventFoldout;

        public override void OnInspectorGUI()
        {
            DrawScriptField<AdvancedCharacterController2D>(target as MonoBehaviour);

            var script = target as AdvancedCharacterController2D;

            EditorGUILayout.BeginVertical();
            {
                DrawGeneralBlock();

                DrawSetting(serializedObject.FindProperty("speedSettings"));
                
                DrawSetting(serializedObject.FindProperty("jumpSettings"));
                
                DrawSetting(serializedObject.FindProperty("wallSlideSettings"));
                
                DrawSetting(serializedObject.FindProperty("dashSettings"));
                
                AdditionalSettings(serializedObject);
                
                Space();
                
                DrawInfoBlock();
                
                DrawEventBlock();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw

        private void DrawGeneralBlock()
        {
            MakeHeader("GeneralSettings");
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSlopeAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gravityScale"));
            }
            EditorGUILayout.EndVertical();
        }
        private void DrawEventBlock()
        {
            _eventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_eventFoldout, "Events");
            {
                if (_eventFoldout)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("onStateChanged"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("onFacingDirectionChanged"));
                        AdditionalEvents(serializedObject);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void DrawInfoBlock()
        {
            _infoFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_infoFoldOut, "Info");
            {
                if (_infoFoldOut)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("input"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentVelocity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentActionState"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("current2DFacingDirection"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("gravity"));
                        AdditionalInfos(serializedObject);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        #endregion

        #region vitrual
        protected virtual void AdditionalSettings(SerializedObject serializedObj) { }
        protected virtual void AdditionalInfos(SerializedObject serializedObj) { }
        protected virtual void AdditionalEvents(SerializedObject serializedObj) { }
        
        #endregion
    }
}
