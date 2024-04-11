using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.LevelObjects
{
    public abstract class BoolInteractable : MonoBehaviour
    {
        [SerializeField] private bool state;
        public bool State
        {
            get => state;
            protected set
            {
                if(value == state)return;
                state = value;
                onStateChange?.Invoke(state);
                if(state)onActivate?.Invoke();
                else onDeactivate?.Invoke();
            }
        }
        
        [Space]public UnityEvent<bool> onStateChange;
        [Space]public UnityEvent onActivate;
        [Space]public UnityEvent onDeactivate;
    }
}