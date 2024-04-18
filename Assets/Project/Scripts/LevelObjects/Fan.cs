using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class Fan : BoolInteractable
    {
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private AreaEffector2D areaEffector2D;
        [SerializeField] private Animator animator;
    
        private static readonly int On = Animator.StringToHash("On");

        protected override void OnEnable()
        {
            base.OnEnable();
            StateChanged(State);
        }

        protected override void StateChanged(bool newState)
        {
            base.StateChanged(newState);
            boxCollider2D.enabled = newState;
            areaEffector2D.enabled = newState;
            animator.SetBool(On, newState);
        }
    }
}
