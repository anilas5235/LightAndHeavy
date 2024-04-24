using System;
using ControllerPlugin.Scripts;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DestructibleBlock : DestructibleSprite
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            Check(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            Check(other);
        }

        private void Check(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<AdvancedCharacterController2D>().CurrentCharacterActionState == CharacterActionState.Dashing)
            {
                TriggerDestruction();
            }
        }
    }
}