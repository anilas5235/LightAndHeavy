using System;
using ControllerPlugin.Scripts;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class DashTrailHandler : MonoBehaviour
    {
        [SerializeField] private AdvancedCharacterController2D characterController;
        [SerializeField] private TrailRenderer trailRenderer;
        private void OnEnable()
        {
            characterController.onStateChanged.AddListener(ControllerStateChanged);
            trailRenderer.emitting = false;
        }

        private void OnDisable()
        {
            characterController.onStateChanged.RemoveListener(ControllerStateChanged);
        }

        private void ControllerStateChanged(CharacterActionState old, CharacterActionState newState)
        {
            if (newState == CharacterActionState.Dashing) trailRenderer.emitting = true;
            if (old == CharacterActionState.Dashing) trailRenderer.emitting = false;
        }
    }
}
