using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class CollectableManager : MonoBehaviour
    {
        private Dictionary<ElementType, int> collectInfo = new Dictionary<ElementType, int>();
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

        public bool CollectedAll()
        {

            if (!CollectedAll(ElementType.Light)) return false;
            if (!CollectedAll(ElementType.Heavy)) return false;
            if (!CollectedAll(ElementType.None)) return false;
            return true;
        }
    }
}
