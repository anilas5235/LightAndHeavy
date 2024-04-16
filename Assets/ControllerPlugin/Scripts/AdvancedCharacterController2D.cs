﻿using Project.Scripts.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static ControllerPlugin.Scripts.Character2DFacingDirection;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleRayCaster2D)),DisallowMultipleComponent]
    public class AdvancedCharacterController2D : MonoBehaviour
    {
        #region GeneralVars
        [SerializeField, Range(0.01f, 10f)] private float gravityScale = 1f;
        [SerializeField, Range(0f, 90f)] private float maxSlopeAngle = 45f;
        #endregion

        #region SpeedVars
        [SerializeField, Range(0.01f, 50f)] protected float maxSpeed = 4f;
        [SerializeField, Range(0.01f, 50f)] private float maxFallSpeed = 20f;
        [SerializeField, Range(0.01f, 50f)] private float maxAcceleration = 15f;
        [SerializeField, Range(0.01f, 50f)] private float maxAirAcceleration = 8f;
        #endregion

        #region JumpVars
        [SerializeField] private bool canJump = true;
        [SerializeField] private FloatMinMax jumpHeight = new() { max = 3f, min = .25f };
        [SerializeField, Range(0f, 3f)] private float timeTillApex = .6f;

        [SerializeField] private bool canAirJump;
        [SerializeField] private bool infiniteAirJumps;
        [SerializeField, Range(0, 15)] private int maxAirJumps = 1;
        #endregion

        #region WallSliding

        [SerializeField] private bool canWallSlide = true;
        [SerializeField, Range(0.01f, 50f)] private float maxWallSlidingSpeed = 1.5f;

        #endregion

        #region InfoVars
        [SerializeField, ReadOnly] private Vector2 currentVelocity;
        [SerializeField, ReadOnly] private Vector2Int input;
        [SerializeField, ReadOnly] private CharacterActionState currentActionState;
        [SerializeField, ReadOnly] private Character2DFacingDirection current2DFacingDirection;
        [SerializeField, ReadOnly] protected float gravity;
        [SerializeField, ReadOnly] private float maxJumpVelocity;
        [SerializeField, ReadOnly] private float minJumpVelocity;
        #endregion

        #region protetedVars
        protected Rigidbody2D rigidBody2D;
        protected CapsuleCollider2D capsuleCollider2D;
        protected PhysicsMaterial2D physicsMaterial2D;
        protected CapsuleRayCaster2D capsuleRayCaster2D;

        protected bool jumpCanceledInput;
        protected bool jumpInput;
        protected int airJumps;
        protected float castDistance = .05f;
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
                OnActionStateChanged(old,currentActionState);
                onStateChanged?.Invoke(old,currentActionState);
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
                CalculateJumpValues();
            }
        }

        public float MaxSlopeAngle
        {
            get => maxSlopeAngle;
            set => maxSlopeAngle = Mathf.Clamp(value,0,90);
        }

        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        public float MaxFallSpeed
        {
            get => maxFallSpeed;
            set => maxFallSpeed = value;
        }

        public float MaxAcceleration
        {
            get => maxAcceleration;
            set => maxAcceleration = value;
        }

        public float MaxAirAcceleration
        {
            get => maxAirAcceleration;
            set => maxAirAcceleration = value;
        }

        public bool CanJump
        {
            get => canJump;
            set => canJump = value;
        }

        public FloatMinMax JumpHeight
        {
            get => jumpHeight;
            set
            {
                jumpHeight = value;
                CalculateJumpValues();
            }
        }

        public float TimeTillApex
        {
            get => timeTillApex;
            set
            {
                timeTillApex = value;
                CalculateJumpValues();
            }
        }

        public bool CanAirJump
        {
            get => canAirJump;
            set => canAirJump = value;
        }

        public bool InfiniteAirJumps
        {
            get => infiniteAirJumps;
            set => infiniteAirJumps = value;
        }

        public int MaxAirJumps
        {
            get => maxAirJumps;
            set => maxAirJumps = value;
        }

        public Vector2 CurrentVelocity => currentVelocity;

        public Vector2Int Input
        {
            get => input;
            protected set => input = value;
        }

        public bool OnGround => capsuleRayCaster2D.OnGround;

        public bool OnSlope => capsuleRayCaster2D.OnSlope;

        public float SlopeAngle => capsuleRayCaster2D.SlopeAngle;
        public float Gravity
        {
            get => gravity;
            protected set => gravity = value;
        }

        public float MaxJumpVelocity
        {
            get => maxJumpVelocity;
            protected set => maxJumpVelocity = value;
        }

        public float MinJumpVelocity
        {
            get => minJumpVelocity;
            protected set => minJumpVelocity = value;
        }
        public int AirJumps => airJumps;
        
        #endregion

        #region Events

        public UnityEvent<CharacterActionState,CharacterActionState> onStateChanged;
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
            CalculateJumpValues();
        }

        private void OnValidate() => CalculateJumpValues();

        private void OnEnable()
        {
            CurrentCharacter2DFacingDirection = None;
            CurrentCharacterActionState = CharacterActionState.Idle;
        }
        
        #endregion

        protected void FixedUpdate()
        {
            currentVelocity = rigidBody2D.velocity;
            
            capsuleRayCaster2D.UpdateAll();
            
            UpdateStateValues(currentVelocity);

            ApplyMovement(ref currentVelocity, input);

            if (canJump) HandleJumpInput(ref currentVelocity);

            ApplyGravity(ref currentVelocity);

            VelocityConstrains(ref currentVelocity);

            UpdateStateValues(currentVelocity);

            rigidBody2D.velocity = currentVelocity;
        }

        #region Gravity

        protected virtual void CalculateJumpValues()
        {
            gravity = 2 * jumpHeight.max / Mathf.Pow(timeTillApex, 2) * gravityScale;
            maxJumpVelocity = Mathf.Abs(gravity) * timeTillApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight.min);
        }

        protected virtual void ApplyGravity(ref Vector2 currVelocity)
        {
           currVelocity.y -= gravity * Time.fixedDeltaTime * (1-physicsMaterial2D.friction);
        }

        #endregion

        #region Movement

        protected virtual void ApplyMovement(ref Vector2 currVelocity, Vector2Int inputVector)
        {
            var xDirectVal = Mathf.Clamp(inputVector.x, -1, 1);
            var acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            var desiredVelocity = Vector2.zero;
            if (xDirectVal != 0)
            {
                if (OnSlope)
                {
                   desiredVelocity = capsuleRayCaster2D.GroundNormalPerpendicular * (maxSpeed * -xDirectVal); 
                }
                else
                {
                    desiredVelocity = Vector2.right * (maxSpeed * xDirectVal);
                }
            }
            currVelocity.x = Mathf.MoveTowards(currVelocity.x, desiredVelocity.x, acceleration * Time.fixedDeltaTime);
            if (OnGround && OnSlope) currVelocity.y = desiredVelocity.y;
        }

        #endregion

        #region JumpFunctions
        protected virtual void HandleJumpInput(ref Vector2 currVelocity)
        {
            if (jumpInput)
            {
                jumpInput = false;
                jumpCanceledInput = false;
                switch (CurrentCharacterActionState)
                {
                    case CharacterActionState.Jumping:
                    case CharacterActionState.Falling:
                        if (!canAirJump || airJumps >= maxAirJumps) break;
                        if(!infiniteAirJumps)++airJumps;
                        CurrentCharacterActionState = CharacterActionState.Jumping;
                        HandleAirJump(ref currVelocity);
                        break;
                    case CharacterActionState.WallSliding:
                        break;
                    case CharacterActionState.Dashing:
                        break;
                    default:
                        if (!OnGround) break;
                        CurrentCharacterActionState = CharacterActionState.Jumping;
                        airJumps = 0;
                        HandleNewJump(ref currVelocity);
                        break;
                }
            }
            else if (jumpCanceledInput && CurrentCharacterActionState == CharacterActionState.Jumping)
            {
                jumpCanceledInput = false;
                HandleJumpCanceled(ref currVelocity);
            }
        }

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

        #endregion

        #region StatesAndConstraints
        protected virtual void UpdateStateValues(Vector2 currVelocity)
        {
            current2DFacingDirection = currVelocity.x switch
            {
                < 0 => Left,
                > 0 => Right,
                _ => None
            };

            switch (CurrentCharacterActionState)
            {
                case CharacterActionState.Idle:
                    if (current2DFacingDirection != None)CurrentCharacterActionState = CharacterActionState.Walking;
                    break;
                case CharacterActionState.Walking:
                    if (current2DFacingDirection == None)CurrentCharacterActionState = CharacterActionState.Idle;
                    break;
                case CharacterActionState.Jumping:
                    WallSlidingTransitionCheck();
                    FallingTransitionCheck(currentVelocity);
                    break;
                case CharacterActionState.Falling:
                    GroundTransitionCheck();
                    WallSlidingTransitionCheck();
                    break;
                case CharacterActionState.WallSliding:
                    GroundTransitionCheck();
                    FallingTransitionCheck(currentVelocity);
                    break;
            }
        }

        protected virtual void OnActionStateChanged(CharacterActionState oldState,CharacterActionState newState)
        {
            switch (oldState)
            {
                case CharacterActionState.Idle:
                    physicsMaterial2D.friction = 0;
                    break;
                case CharacterActionState.WallSliding:
                    physicsMaterial2D.friction = 0;
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
                    if (currVelocity.y < -maxFallSpeed) currVelocity.y = -maxFallSpeed;
                    break;
                case CharacterActionState.WallSliding:
                    if(currVelocity.y < -maxWallSlidingSpeed) currVelocity.y = -maxWallSlidingSpeed;
                    break;
            }
            
            if(!OnSlope && OnGround && currVelocity.y < 0)currVelocity.y = 0;
        }

        private void GroundTransitionCheck()
        {
            if (!OnGround || capsuleRayCaster2D.WallDetection != None) return;
                
            CurrentCharacterActionState = CurrentCharacter2DFacingDirection == None
                ? CharacterActionState.Idle
                : CharacterActionState.Walking;
        }

        private void FallingTransitionCheck(Vector2 currVelocity)
        {
            if (CurrentCharacterActionState == CharacterActionState.WallSliding)
            {
                if(capsuleRayCaster2D.WallDetection == None)CurrentCharacterActionState = CharacterActionState.Falling;
            }
            else if (!OnGround && currentVelocity.y < 0) CurrentCharacterActionState = CharacterActionState.Falling;
        }

        private void WallSlidingTransitionCheck()
        {
            if(!canWallSlide) return;
            if (OnGround || capsuleRayCaster2D.WallDetection == None) return;
            CurrentCharacterActionState = CharacterActionState.WallSliding;
        }
        
        #endregion
        
        #region Input

        public virtual void JumpInput(InputAction.CallbackContext context)
        {
            if (!jumpInput && context.started) jumpInput = true;
            if (!jumpCanceledInput && context.canceled) jumpCanceledInput = true;
        }

        public virtual void HorizontalInput(InputAction.CallbackContext context)
        {
            input.x = Mathf.RoundToInt(context.ReadValue<float>());
        }

        #endregion
    }
}