using System;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    [Serializable]
    public class JumpHandler
    {
        public bool canJump = true;
        public FloatMinMax jumpHeight = new() { max = 1.5f, min = .25f };
        [Range(0f, 3f)] public float timeTillApex = .6f;
        public bool canAirJump;
        public bool infiniteAirJumps;
        [Range(0, 15)] public int maxAirJumps = 1; 
        [HideInInspector] public float maxJumpVelocity;
        [HideInInspector] public float minJumpVelocity;
        protected int airJumps;
        
        public float CalculateGravityWithJumpValues(float gravityScale)
        {
            var gravity = 2 * jumpHeight.max / Mathf.Pow(timeTillApex, 2) * gravityScale;
            maxJumpVelocity = Mathf.Abs(gravity) * timeTillApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight.min);
            
            return gravity;
        }

        public bool Jump(CharacterActionState currentCharacterActionState, bool onGround, ref Vector2 currVelocity)
        {
            switch (currentCharacterActionState)
            {
                case CharacterActionState.Jumping:
                case CharacterActionState.Falling:
                    if (!CanAirJumpNow()) break;
                    HandleAirJump(ref currVelocity);
                    return true;
                default:
                    if (!onGround) break;
                    ResetAirJumps();
                    HandleNewJump(ref currVelocity);
                    return true;
            }
            return false;
        }

        public void CancelJump(CharacterActionState currentCharacterActionState,ref Vector2 currVelocity)
        {
            if(currentCharacterActionState == CharacterActionState.Jumping) HandleJumpCanceled(ref currVelocity);
        }

        protected bool CanAirJumpNow()
        {
            if (!canJump) return false;
            if (infiniteAirJumps) return true;
            if (airJumps >= maxAirJumps) return false;
            ++airJumps;
            return true;
        }

        protected void ResetAirJumps() => airJumps = 0;
        
        protected virtual void HandleNewJump(ref Vector2 currVelocity)
        {
            currVelocity.y = maxJumpVelocity;
        }

        protected virtual void HandleAirJump(ref Vector2 currVelocity)
        {
            currVelocity.y = maxJumpVelocity;
        }

        protected virtual void HandleJumpCanceled(ref Vector2 currVelocity)
        {
            if (currVelocity.y > minJumpVelocity) currVelocity.y = minJumpVelocity;
        }
    }
}