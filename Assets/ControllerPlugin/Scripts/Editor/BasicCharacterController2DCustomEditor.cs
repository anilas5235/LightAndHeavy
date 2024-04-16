using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomEditor(typeof(BasicCharacterController2D))]
    public class BasicCharacterController2DCustomEditor : UnityEditor.Editor
    {
        private static GUIStyle headerStyle;
        private bool _infoFoldOut;
        private bool _eventFoldout;

        private SerializedProperty
            _canJumpProperty,
            _canAirJumpProperty,
        _infiniteAirJumpsProperty;

        private void OnEnable()
        {
            _canJumpProperty = serializedObject.FindProperty("canJump");
            _canAirJumpProperty = serializedObject.FindProperty("canAirJump");
            _infiniteAirJumpsProperty = serializedObject.FindProperty("infiniteAirJumps");
            headerStyle ??= new GUIStyle()
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
                MakeHeader("GeneralSettings");
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("groundLayers"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("gravityScale"));
                }
                EditorGUILayout.EndVertical();

                MakeHeader("Speed Settings");
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFallSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAcceleration"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAirAcceleration"));
                }
                EditorGUILayout.EndVertical();

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

                AdditionalSettings(serializedObject);
                
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

                _infoFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_infoFoldOut, "Info");
                {
                    if (_infoFoldOut)
                    {
                        EditorGUILayout.BeginVertical("box");
                        {
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentVelocity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("input"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentActionState"));
                            EditorGUILayout.PropertyField(
                                serializedObject.FindProperty("current2DFacingDirection"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("onGround"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("gravity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxJumpVelocity"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("minJumpVelocity"));
                            AdditionalInfos(serializedObject);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void AdditionalSettings(SerializedObject serializedObj) { }
        protected virtual void AdditionalInfos(SerializedObject serializedObj) { }
        protected virtual void AdditionalEvents(SerializedObject serializedObj) { }

        protected static void Space()
        {
            GUILayout.Space(5);
        }

        protected static void MakeHeader(string header)
        {
            Space();
            GUILayout.Label(header, headerStyle);
            Space();
        }
    }
}