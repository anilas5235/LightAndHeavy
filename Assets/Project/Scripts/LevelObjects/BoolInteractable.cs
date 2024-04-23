using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.LevelObjects
{
    public abstract class BoolInteractable : MonoBehaviour
    {
        [SerializeField] private bool state;
        [SerializeField] private BoolInteractable[] triggers;
        [SerializeField] private bool invert = false;
        public bool State
        {
            get => state;
            protected set
            {
                if(value == state)return;
                state = value;
                StateChanged(state);
                onStateChange?.Invoke(state);
                if(state)onActivate?.Invoke();
                else onDeactivate?.Invoke();
            }
        }
        
        [Space]public UnityEvent<bool> onStateChange;
        [Space]public UnityEvent onActivate;
        [Space]public UnityEvent onDeactivate;
        
        protected virtual void OnEnable()
        {
            foreach (var trigger in triggers)
            {
                if(!trigger)continue;
                trigger.onStateChange.AddListener(CheckTriggers);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var trigger in triggers)
            {
                if(!trigger)continue;
                trigger.onStateChange.RemoveListener(CheckTriggers);
            }
        }

        protected virtual void StateChanged(bool newState){}

        protected virtual void CheckTriggers(bool newState)
        {
            var temp = newState || triggers.Any(trigger => trigger && trigger.State);
            State = invert ? !temp: temp;
        }
    }
}