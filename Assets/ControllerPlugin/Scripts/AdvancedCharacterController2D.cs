using System;
using System.Collections;
using ControllerPlugin.ReadOnly;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleRayCaster2D)), DisallowMultipleComponent]
    public class AdvancedCharacterController2D : MonoBehaviour
    {

        [SerializeField, Range(0.01f, 10f)] private float gravityScale = 1f;
        [SerializeField, Range(0f, 90f)] private float maxSlopeAngle = 45f;
        [SerializeField] protected bool slipWay;

        [SerializeField] protected SpeedParameters speedSettings;

        [SerializeField] protected JumpHandler jumpSettings;

        [SerializeField] protected WallSlideParameters wallSlideSettings;

        [SerializeField] protected DashHandler dashSettings;

        #region InfoVars

        [SerializeField, ReadOnly] protected Vector2 currentVelocity;
        [SerializeField, ReadOnly] protected Vector2Int input;
        [SerializeField, ReadOnly] private CharacterActionState currentActionState;
        [SerializeField, ReadOnly] private Character2DFacingDirection current2DFacingDirection;
        [SerializeField, ReadOnly] protected float gravity;

        #endregion

        #region protetedVars

        protected Rigidbody2D rigidBody2D;
        protected CapsuleCollider2D capsuleCollider2D;
        protected PhysicsMaterial2D physicsMaterial2D;
        protected CapsuleRayCaster2D capsuleRayCaster2D;

        protected CharacterInputState jumpInput;
        protected CharacterInputState dashInput;

        #endregion

        #region Properties

        public CharacterActionState CurrentCharacterActionState
        {
            get => currentActionState;
            protected set
            {
                if (currentActionState == value) return;
                var old = currentActionState;
                currentActionState = value;
                OnActionStateChanged(old, currentActionState);
                onStateChanged?.Invoke(old, currentActionState);
            }
        }

        public Character2DFacingDirection CurrentCharacter2DFacingDirection
        {
            get => current2DFacingDirection;
            protected set
            {
                if (current2DFacingDirection == value) return;
                current2DFacingDirection = value;
                onFacingDirectionChanged?.Invoke(current2DFacingDirection);
            }
        }

        public float GravityScale
        {
            get => gravityScale;
            set
            {
                gravityScale = value;
                gravity = jumpSettings.CalculateGravityWithJumpValues(gravityScale);
            }
        }

        public float MaxSlopeAngle
        {
            get => maxSlopeAngle;
            set => maxSlopeAngle = Mathf.Clamp(value, 0, 90);
        }

        public float MaxSpeed
        {
            get => speedSettings.maxSpeed;
            set => speedSettings.maxSpeed = value;
        }

        public float MaxFallSpeed
        {
            get => speedSettings.maxFallSpeed;
            set => speedSettings.maxFallSpeed = value;
        }

        public float MaxAcceleration
        {
            get => speedSettings.maxAcceleration;
            set => speedSettings.maxAcceleration = value;
        }

        public float MaxAirAcceleration
        {
            get => speedSettings.maxAirAcceleration;
            set => speedSettings.maxAirAcceleration = value;
        }

        public bool CanJump
        {
            get => jumpSettings.canJump;
            set => jumpSettings.canJump = value;
        }

        public Vector2 JumpHeight
        {
            get => jumpSettings.jumpHeightMinMax;
            set
            {
                jumpSettings.jumpHeightMinMax = value;
                UpdateGravity();
            }
        }

        public float TimeTillApex
        {
            get => jumpSettings.timeTillApex;
            set
            {
                jumpSettings.timeTillApex = value;
                UpdateGravity();
            }
        }

        public bool CanAirJump
        {
            get => jumpSettings.canAirJump;
            set => jumpSettings.canAirJump = value;
        }

        public bool InfiniteAirJumps
        {
            get => jumpSettings.infiniteAirJumps;
            set => jumpSettings.infiniteAirJumps = value;
        }

        public int MaxAirJumps
        {
            get => jumpSettings.maxAirJumps;
            set => jumpSettings.maxAirJumps = value;
        }

        public Vector2 CurrentVelocity => currentVelocity;

        public bool OnGround => capsuleRayCaster2D.OnGround;
        public bool InAir => capsuleRayCaster2D.InAir;
        public bool OnSlope => capsuleRayCaster2D.OnSlope;
        public float SlopeAngle => capsuleRayCaster2D.SlopeAngle;
        public Vector2 GroundNormal => capsuleRayCaster2D.GroundNormal;

        public float Gravity
        {
            get => gravity;
            protected set => gravity = value;
        }

        #endregion

        #region Events

        public UnityEvent<CharacterActionState, CharacterActionState> onStateChanged;
        public UnityEvent<Character2DFacingDirection> onFacingDirectionChanged;

        #endregion

        #region AwakeValidateEnable

        protected virtual void Awake()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            capsuleRayCaster2D = GetComponent<CapsuleRayCaster2D>();
            rigidBody2D.sharedMaterial = physicsMaterial2D = new PhysicsMaterial2D()
            {
                bounciness = 0,
                friction = 0,
                name = "Character2DPhysicsMaterial"
            };
            rigidBody2D.gravityScale = 0;
            rigidBody2D.angularDrag = 0;
            rigidBody2D.drag = 0;
            rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            UpdateGravity();
        }

        protected virtual void OnValidate()
        {
            UpdateGravity();
        }

        private void UpdateGravity()
        {
            gravity = jumpSettings.CalculateGravityWithJumpValues(gravityScale);
        }

        protected virtual void OnEnable()
        {
            CurrentCharacter2DFacingDirection = Character2DFacingDirection.None;
            CurrentCharacterActionState = CharacterActionState.Idle;
        }

        #endregion

        protected void FixedUpdate()
        {
            currentVelocity = rigidBody2D.velocity;

            capsuleRayCaster2D.UpdateAll();

            UpdateStateValues(currentVelocity);

            if (dashSettings.canDash) HandleDashInput();

            HandleMovementInput(ref currentVelocity, input);

            if (jumpSettings.canJump) HandleJumpInput(ref currentVelocity);

            ApplyGravity(ref currentVelocity);

            VelocityConstrains(ref currentVelocity);

            UpdateStateValues(currentVelocity);

            rigidBody2D.velocity = currentVelocity;
        }

        #region Gravity

        protected virtual void ApplyGravity(ref Vector2 currVelocity)
        {
            if (CurrentCharacterActionState != CharacterActionState.Dashing && InAir)
            {
                currVelocity.y -= gravity * Time.fixedDeltaTime * (1 - physicsMaterial2D.friction);
            }
        }

        #endregion

        #region HandleInput

        protected virtual void HandleDashInput()
        {
            if (dashInput != CharacterInputState.None && dashSettings.CanDashNow)
            {
                StartCoroutine(Dash(dashSettings));
                CurrentCharacterActionState = CharacterActionState.Dashing;
                dashInput = CharacterInputState.None;
            }
            else if (CurrentCharacterActionState == CharacterActionState.Dashing &&
                     dashSettings.currentDashState == DashState.Ready)
            {
                CurrentCharacterActionState = CharacterActionState.Idle;
            }
        }

        protected virtual void HandleMovementInput(ref Vector2 currVelocity, Vector2Int inputVector)
        {
            var xDirectVal = Mathf.Clamp(inputVector.x, -1, 1);

            Vector2 desiredVelocity;
            var overrideVelocity = false; 

            if (OnSlope)
            {
                desiredVelocity = capsuleRayCaster2D.GroundNormalPerpendicular * (xDirectVal * speedSettings.maxSpeed);
                if (OnSlope && SlopeAngle > maxSlopeAngle && Math.Sign(GroundNormal.x) != Math.Sign(inputVector.x))
                {
                    desiredVelocity = slipWay ? capsuleRayCaster2D.GroundNormalPerpendicular * (speedSettings.maxSpeed * .5f * GroundNormal.x)
                        : Vector2.zero;
                    overrideVelocity = true;
                }
                else if (CurrentCharacterActionState == CharacterActionState.Dashing)
                {

                    desiredVelocity = capsuleRayCaster2D.GroundNormalPerpendicular *
                                   (dashSettings.DashSpeed * Mathf.Sign(dashSettings.DashVector.x));
                    overrideVelocity = true;
                }
            }
            else
            {
                desiredVelocity = Vector2.right * (xDirectVal * speedSettings.maxSpeed);
                
                if (CurrentCharacterActionState == CharacterActionState.Dashing)
                {
                    desiredVelocity = dashSettings.DashVector;
                    overrideVelocity = true;
                }
            }

            if (overrideVelocity)
            {
                currentVelocity = desiredVelocity;
            }
            else
            {
                var acceleration = speedSettings.GetAcceleration(OnGround);
                currVelocity.x = Mathf.MoveTowards(currVelocity.x, desiredVelocity.x,
                    acceleration * Time.fixedDeltaTime);
                if (OnGround && OnSlope) currVelocity.y = desiredVelocity.y;
            }
        }

        protected void HandleJumpInput(ref Vector2 currVelocity)
        {
            if (jumpInput == CharacterInputState.Triggered)
            {
                jumpSettings.Jump(CurrentCharacterActionState, OnGround, ref currentVelocity);
                jumpInput = CharacterInputState.None;
            }
            else if (jumpInput == CharacterInputState.Canceled)
            {
                jumpSettings.CancelJump(CurrentCharacterActionState, ref currVelocity);
                jumpInput = CharacterInputState.None;
            }
        }

        #endregion

        #region StatesAndConstraints

        protected virtual void UpdateStateValues(Vector2 currVelocity)
        {
            CurrentCharacter2DFacingDirection = currVelocity.x switch
            {
                < 0 => Character2DFacingDirection.Left,
                > 0 => Character2DFacingDirection.Right,
                _ => Character2DFacingDirection.None
            };

            switch (CurrentCharacterActionState)
            {
                case CharacterActionState.Idle:
                    if (CurrentCharacter2DFacingDirection != Character2DFacingDirection.None)
                        CurrentCharacterActionState = CharacterActionState.Walking;
                    JumpTransitionCheck(currentVelocity);
                    FallingTransitionCheck(currentVelocity);
                    break;
                case CharacterActionState.Walking:
                    if (CurrentCharacter2DFacingDirection == Character2DFacingDirection.None)
                        CurrentCharacterActionState = CharacterActionState.Idle;
                    JumpTransitionCheck(currentVelocity);
                    FallingTransitionCheck(currentVelocity);
                    break;
                case CharacterActionState.Jumping:
                    WallSlidingTransitionCheck();
                    FallingTransitionCheck(currentVelocity);
                    GroundTransitionCheck();
                    break;
                case CharacterActionState.Falling:
                    GroundTransitionCheck();
                    WallSlidingTransitionCheck();
                    break;
                case CharacterActionState.WallSliding:
                    GroundTransitionCheck();
                    FallingTransitionCheck(currentVelocity);
                    break;
                case CharacterActionState.Dashing:
                    if (dashSettings.currentDashState != DashState.Dashing)
                        CurrentCharacterActionState = CharacterActionState.Idle;
                    break;
            }
        }

        protected virtual void OnActionStateChanged(CharacterActionState oldState, CharacterActionState newState)
        {
            physicsMaterial2D.friction = 0;
            switch (oldState)
            {
                case CharacterActionState.Idle:
                    break;
                case CharacterActionState.Walking:
                    break;
                case CharacterActionState.Jumping:
                    break;
                case CharacterActionState.Falling:
                    break;
                case CharacterActionState.WallSliding:
                    break;
                case CharacterActionState.Dashing:
                    break;
            }

            switch (newState)
            {
                case CharacterActionState.Idle:
                    physicsMaterial2D.friction = 1;
                    break;
                case CharacterActionState.WallSliding:
                    physicsMaterial2D.friction = .4f;
                    break;
            }
        }

        protected virtual void VelocityConstrains(ref Vector2 currVelocity)
        {
            switch (CurrentCharacterActionState)
            {
                case CharacterActionState.Falling:
                    if (currVelocity.y < -speedSettings.maxFallSpeed) currVelocity.y = -speedSettings.maxFallSpeed;
                    break;
                case CharacterActionState.WallSliding:
                    if (currVelocity.y < -wallSlideSettings.maxWallSlidingSpeed) currVelocity.y = -wallSlideSettings.maxWallSlidingSpeed;
                    break;
            }

            if (!OnSlope && OnGround && currVelocity.y < 0) currVelocity.y = 0;
        }

        private void GroundTransitionCheck()
        {
            if (OnGround)
            {
                CurrentCharacterActionState = CurrentCharacter2DFacingDirection == Character2DFacingDirection.None
                    ? CharacterActionState.Idle
                    : CharacterActionState.Walking;
            }
        }

        #endregion
        
        #region TransitionChecks
        private void FallingTransitionCheck(Vector2 currVelocity)
        {
            if (CurrentCharacterActionState == CharacterActionState.WallSliding)
            {
                if (capsuleRayCaster2D.WallDetection == Character2DFacingDirection.None)
                    CurrentCharacterActionState = CharacterActionState.Falling;
            }
            else if (!OnGround && currVelocity.y < 0)
            {
                CurrentCharacterActionState = CharacterActionState.Falling;
            }
            else if (SlopeAngle > maxSlopeAngle)
            {
                if (wallSlideSettings.canWallSlide) CurrentCharacterActionState = CharacterActionState.WallSliding;
                else CurrentCharacterActionState = CharacterActionState.Falling;
            }
        }

        private void WallSlidingTransitionCheck()
        {
            if (!wallSlideSettings.canWallSlide) return;
            if (OnGround || capsuleRayCaster2D.WallDetection == Character2DFacingDirection.None) return;
            CurrentCharacterActionState = CharacterActionState.WallSliding;
        }

        private void JumpTransitionCheck(Vector2 currVelocity)
        {
            if (InAir && currVelocity.y > 0) CurrentCharacterActionState = CharacterActionState.Jumping;
        }

        #endregion

        #region Input

        public virtual void JumpInput(InputAction.CallbackContext context)
        {
            if (jumpInput == CharacterInputState.None)
            {
                if (context.started) jumpInput = CharacterInputState.Triggered;
                else if (context.canceled) jumpInput = CharacterInputState.Canceled;
            }
        }

        public virtual void HorizontalInput(InputAction.CallbackContext context)
        {
            input.x = Mathf.RoundToInt(context.ReadValue<float>());
        }

        public void DashInput(Vector2 direction)
        {
            if (dashInput == CharacterInputState.None)
            {
                dashInput = CharacterInputState.Triggered;
                dashSettings.useOverrideParams = false;
                dashSettings.dashParams.direction = direction;
            }
        }

        public void DashInput(DashParams dashParams)
        {
            if (dashInput == CharacterInputState.None)
            {
                dashInput = CharacterInputState.Triggered;
                dashSettings.useOverrideParams = true;
                dashSettings.overrideParams = dashParams;
            }
        }

        #endregion

        #region Coroutines

        private static IEnumerator Dash(DashHandler dashHandler)
        {
            dashHandler.currentDashState = DashState.Dashing;
            yield return new WaitForSeconds(dashHandler.DashDuration);
            dashHandler.currentDashState = DashState.CoolDown;
            if (dashHandler.UseDashCoolDown) yield return new WaitForSeconds(dashHandler.DashCoolDown);
            dashHandler.currentDashState = DashState.Ready;
        }

        #endregion
    }
}