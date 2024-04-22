using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.LevelObjects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DestructibleSprite : MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private float destroyParticleAfter;
        private SpriteRenderer spriteRenderer;
        private bool destruction;

        public UnityEvent onDestruction;
        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void TriggerDestruction()
        {
            if(destruction) return;
            destruction = true;
            var eff = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            eff.GetComponent<ParticleSystem>().Play();
            Destroy(eff,destroyParticleAfter);
            Destroy(gameObject);
            onDestruction?.Invoke();
        }
    }
}
