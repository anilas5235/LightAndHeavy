using System;
using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomEditor(typeof(AdvancedCharacterController2D))]
    public class AdvancedCharacterController2DCustomEditor : UnityEditor.Editor
    {
        private static GUIStyle _headerStyle;
        
        private bool _infoFoldOut;
        private bool _eventFoldout;

        private SerializedProperty _canJumpProperty;
        private SerializedProperty _canAirJumpProperty;
        private SerializedProperty _infiniteAirJumpsProperty;
        private SerializedProperty _canDashProperty;
        private SerializedProperty _useDashCoolDownProperty;

        protected virtual void OnEnable()
        {
            _canJumpProperty = serializedObject.FindProperty("canJump");
            _canAirJumpProperty = serializedObject.FindProperty("canAirJump");
            _infiniteAirJumpsProperty = serializedObject.FindProperty("infiniteAirJumps");
            _canDashProperty = serializedObject.FindProperty("canDash");
            _useDashCoolDownProperty = serializedObject.FindProperty("useDashCoolDown");
            _headerStyle ??= new GUIStyle()
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = Color.white,
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                DrawGeneralBlock();

                DrawSpeedBlock();

                DrawJumpBlock();

                DrawDashBlock();
                
                AdditionalSettings(serializedObject);
                
                DrawEventBlock();

                DrawInfoBlock();
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
        private void DrawSpeedBlock()
        {
            MakeHeader("Speed Settings");
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFallSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAcceleration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAirAcceleration"));
            }
            EditorGUILayout.EndVertical();
        }
        private void DrawJumpBlock()
        {
            MakeHeader("Jump Settings");
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canJumpProperty);
            if (_canJumpProperty.boolValue)
            {
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("timeTillApex"));
                    EditorGUILayout.PropertyField(_canAirJumpProperty);
                    if (_canAirJumpProperty.boolValue)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.PropertyField(_infiniteAirJumpsProperty);
                        if (!_infiniteAirJumpsProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAirJumps"));
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
        private void DrawDashBlock()
        {
            MakeHeader("Dash Settings");
            EditorGUILayout.BeginVertical("box");
                
            EditorGUILayout.PropertyField(_canDashProperty);
            if (_canDashProperty.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dashSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dashDuration"));
                EditorGUILayout.PropertyField(_useDashCoolDownProperty);
                if (_useDashCoolDownProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashCoolDown"));
                }
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
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("canDashNow"));
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

        #region Helpers

        protected static void Space()
        {
            GUILayout.Space(5);
        }

        protected static void MakeHeader(string header)
        {
            Space();
            GUILayout.Label(header, _headerStyle);
            Space();
        }
        
        #endregion
    }
}