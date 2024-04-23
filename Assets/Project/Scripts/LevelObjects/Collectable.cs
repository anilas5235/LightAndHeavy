using System;
using System.Collections;
using Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.LevelObjects
{
    public class Collectable : MonoBehaviour,IHaveElementType
    {
        [SerializeField] private ElementType type;
        [SerializeField] private ParticleSystem effectParticleSystem;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private float magnitude = .05f;

        public UnityEvent<ElementType> onCollect;

        private float startY;

        private void Awake()
        {
            startY = transform.position.y;
        }

        private void FixedUpdate()
        {
            var pos = transform.position;
            pos.y = startY + Mathf.Sin(Time.time) * magnitude;
            transform.position = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(type != ElementType.None && other.gameObject.GetComponent<IHaveElementType>().GetElementType() != type) return;
                onCollect?.Invoke(type);
                if (effectParticleSystem)
                {
                    effectParticleSystem.Play();
                    if(spriteRenderer) spriteRenderer.enabled = false;
                }
                else Destroy(gameObject);
            }
        }

        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }

        public ElementType GetElementType()
        {
            return type;
        }
    }
}
