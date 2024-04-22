using System;
using Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.LevelObjects
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private ElementType type;

        public UnityAction<ElementType> onCollect;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(type != ElementType.None && other.gameObject.GetComponent<IHaveElementType>().GetElementType() != type) return;
                onCollect?.Invoke(type);
                Destroy(gameObject);
            }
        }
        
    }
}
