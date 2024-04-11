using System.Linq;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class MovablePlatform : BoolInteractable
    {
        [SerializeField] private Vector2 lowerOffsetPoint;
        [SerializeField] private Vector2 upperOffsetPoint;

        private Vector2 _startPosition;

        [SerializeField] private BoolInteractable[] triggers;

        private void Awake()
        {
           _startPosition = transform.position;
        }

        private void OnEnable()
        {
            foreach (var trigger in triggers)trigger.onStateChange.AddListener(CheckTriggers);
        }

        private void OnDisable()
        {
            foreach (var trigger in triggers)trigger.onStateChange.RemoveListener(CheckTriggers);
        }

        private void CheckTriggers(bool arg0)
        {
            State = triggers.Any(trigger => trigger.State);
        }
    }
}