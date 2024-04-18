using System;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DestructibleSprite : MonoBehaviour
    {
        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private float destroyParticleAfter;
        private SpriteRenderer spriteRenderer;
        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void TriggerDestruction()
        {
            var eff = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            eff.GetComponent<ParticleSystem>().Play();
            Destroy(eff,destroyParticleAfter);
            Destroy(gameObject);
        }
    }
}
