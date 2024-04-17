using System;
using ControllerPlugin.Scripts;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class SpriteFliper : MonoBehaviour
    {
        [SerializeField] private AdvancedCharacterController2D advancedCharacterController2D;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Character2DFacingDirection initialDirection = Character2DFacingDirection.Left;

        private Character2DFacingDirection old;

        private void Awake()
        {
            old = initialDirection;
        }

        private void OnEnable()
        {
            advancedCharacterController2D.onFacingDirectionChanged.AddListener(FaceChange);
        }

        private void OnDisable()
        {
            advancedCharacterController2D.onFacingDirectionChanged.RemoveListener(FaceChange);
        }

        private void FaceChange(Character2DFacingDirection state)
        {
            if(state == Character2DFacingDirection.None)return;

            if (state != old)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                old = state;
            }
        }
    }
}
