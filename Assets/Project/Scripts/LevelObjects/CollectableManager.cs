using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class CollectableManager : MonoBehaviour
    {
        private Dictionary<ElementType, int> collectInfo;
        private void Awake()
        {
            var allCollectables = FindObjectsOfType<Collectable>();
            foreach (var collectable in allCollectables)
            {
                collectable.onCollect.AddListener(CollectDetected);
                collectInfo[collectable.GetElementType()] += 1;
            }
        }

        private void CollectDetected(ElementType type)
        {
            collectInfo[type] -= 1;
        }

        public bool CollectedAll(ElementType type)
        {
            return collectInfo[type] < 1;
        }
    }
}
