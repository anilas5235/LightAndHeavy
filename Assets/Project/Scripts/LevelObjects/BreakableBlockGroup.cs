using System;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class BreakableBlockGroup : MonoBehaviour
    {
        private DestructibleBlock[] blocks;
        private void OnEnable()
        {
            blocks = GetComponentsInChildren<DestructibleBlock>();
            foreach (var destructibleBlock in blocks)
            {
                destructibleBlock.onDestruction.AddListener(Trigger);
            }
        }

        private void Trigger()
        {
            foreach (var block in blocks)
            {
                if(block) block.TriggerDestruction();
            }

            blocks = Array.Empty<DestructibleBlock>();
        }
    }
}
