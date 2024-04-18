using System;
using ControllerPlugin.Scripts;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private AdvancedCharacterController2D advancedCharacterController2D;
        private Animator animator;
        private static readonly int Run = Animator.StringToHash("Run");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            advancedCharacterController2D.onStateChanged.AddListener(StateChanged);
        }

        private void OnDisable()
        {
            advancedCharacterController2D.onStateChanged.RemoveListener(StateChanged);
        }

        private void StateChanged(CharacterActionState old, CharacterActionState newState)
        {
            animator.SetBool(Run,newState == CharacterActionState.Walking);
        }
    }
}