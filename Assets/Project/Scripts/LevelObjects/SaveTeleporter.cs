using System;
using ControllerPlugin.Scripts;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class SaveTeleporter : MonoBehaviour
    {
        [SerializeField] private bool relativeTeleport;
        [SerializeField] private Vector3 position;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<AdvancedCharacterController2D>().OnGround)
            {
                if (relativeTeleport)
                {
                    other.gameObject.transform.position += position;
                }
                else
                {
                    other.gameObject.transform.position = position;
                }
            }
        }
    }
}
