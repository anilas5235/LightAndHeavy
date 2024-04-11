using System;
using UnityEngine;
using UnityEngine.Serialization;

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
            foreach (var trigger in triggers)
            {
                trigger.onStateChange.AddListener(CheckTriggers);
            }
        }

        private void CheckTriggers(bool arg0)
        {
            var newState = false;
            foreach (var trigger in triggers)
            {
                if(!trigger.State)continue;
                newState = true;
                break;
            }
            State = newState;
        }
    }
}