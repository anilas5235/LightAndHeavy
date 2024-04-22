using System;
using Project.Scripts.Interfaces;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class ResetZone : MonoBehaviour
    {
        [SerializeField] private ElementType type;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(type != ElementType.None && other.gameObject.GetComponent<IHaveElementType>().GetElementType() != type) return;
                FindObjectOfType<LevelState>().TriggerGameOver();
            }
        }
    }
}
