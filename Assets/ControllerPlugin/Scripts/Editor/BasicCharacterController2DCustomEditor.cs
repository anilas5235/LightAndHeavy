using UnityEditor;
using UnityEngine;

namespace ControllerPlugin.Scripts.Editor
{
    [CustomEditor(typeof(BasicCharacterController2D))]
    public class BasicCharacterController2DCustomEditor : UnityEditor.Editor
    {
        private static GUIStyle headerStyle;
        private bool _infoFoldOut;

        private SerializedProperty
            _canJumpProperty,
            _canAirJumpProperty;

        private void OnEnable()
        {
            _canJumpProperty = serializedObject.FindProperty("canJump");
            _canAirJumpProperty = serializedObject.FindProperty("canAirJump");
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
                Space();
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
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAirJumps"));
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                AdditionalSettings();

                _infoFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(_infoFoldOut, "Info");
                {
                    if (_infoFoldOut)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("input"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentCharacterActionState"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentCharacter2DFacingDirection"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("onGround"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("gravity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxJumpVelocity"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("minJumpVelocity"));
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void AdditionalSettings() { }

        protected static void Space()
        {
            GUILayout.Space(5);
        }

        protected static void MakeHeader(string header)
        {
            GUILayout.Label(header, headerStyle);
            Space();
        }
    }
}