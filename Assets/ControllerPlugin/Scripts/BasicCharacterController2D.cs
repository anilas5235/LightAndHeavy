using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D)),DisallowMultipleComponent]
    public class BasicCharacterController2D : MonoBehaviour
    {
        #region GeneralVars
        [SerializeField] private LayerMask groundLayers = new() { value = int.MaxValue };
        [SerializeField, Range(0.01f, 10f)] private float gravityScale = 1f;
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

        #region InfoVars
        [SerializeField, ReadOnly] private Vector2 currentVelocity;
        [SerializeField, ReadOnly] private Vector2Int input;
        [SerializeField, ReadOnly] private CharacterActionState currentActionState = CharacterActionState.Idle;

        [SerializeField, ReadOnly]
        private Character2DFacingDirection current2DFacingDirection = Character2DFacingDirection.None;

        [SerializeField, ReadOnly] private bool onGround;
        [SerializeField, ReadOnly] private float gravity;
        [SerializeField, ReadOnly] private float maxJumpVelocity;
        [SerializeField, ReadOnly] private float minJumpVelocity;
        #endregion

        #region protetedVars
        protected Rigidbody2D rigidBody2D;
        protected BoxCollider2D boxCollider2D;

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
                currentActionState = value;
                onStateChanged?.Invoke(currentActionState);
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

        public LayerMask GroundLayers
        {
            get => groundLayers;
            set => groundLayers = value;
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
        public bool OnGround
        {
            get => onGround;
            protected set => onGround = value;
        }

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

        public UnityEvent<CharacterActionState> onStateChanged;
                public UnityEvent<Character2DFacingDirection> onFacingDirectionChanged;

        #endregion

        #region AwakeValidateEnable
        protected virtual void Awake()
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            rigidBody2D.gravityScale = 0;
            CalculateJumpValues();
        }

        private void OnValidate() => CalculateJumpValues();

        private void OnEnable()
        {
            CurrentCharacter2DFacingDirection = Character2DFacingDirection.None;
            CurrentCharacterActionState = CharacterActionState.Idle;
        }
        
        #endregion

        protected void FixedUpdate()
        {
            currentVelocity = rigidBody2D.velocity;
            
            ExecuteEnvironmentChecks();
            
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
           currVelocity.y -= gravity * Time.fixedDeltaTime;
        }

        #endregion

        #region Movement

        protected virtual void ApplyMovement(ref Vector2 currVelocity, Vector2Int inputVector)
        {
            var desiredVelocity = Vector2.right * (Mathf.Clamp(inputVector.x, -1, 1) * maxSpeed);
            var acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            currVelocity.x =
                Mathf.MoveTowards(currVelocity.x, desiredVelocity.x, acceleration * Time.fixedDeltaTime);
        }

        #endregion

        #region JumpFunctions
        protected virtual void HandleJumpInput(ref Vector2 currVelocity)
        {
            if (jumpInput)
            {
                jumpInput = false;
                jumpCanceledInput = false;
                switch (currentActionState)
                {
                    case CharacterActionState.Jumping:
                    case CharacterActionState.Falling:
                        if (!canAirJump || airJumps >= maxAirJumps) break;
                        if(!infiniteAirJumps)++airJumps;
                        currentActionState = CharacterActionState.Jumping;
                        HandleAirJump(ref currVelocity);
                        break;
                    case CharacterActionState.WallSliding:
                        break;
                    case CharacterActionState.Dashing:
                        break;
                    default:
                        if (!onGround) break;
                        currentActionState = CharacterActionState.Jumping;
                        airJumps = 0;
                        HandleNewJump(ref currVelocity);
                        break;
                }
            }
            else if (jumpCanceledInput && currentActionState == CharacterActionState.Jumping)
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
                < 0 => Character2DFacingDirection.Left,
                > 0 => Character2DFacingDirection.Right,
                _ => Character2DFacingDirection.None
            };

            switch (currentActionState)
            {
                case CharacterActionState.Idle:
                    if (current2DFacingDirection != Character2DFacingDirection.None)
                    {
                        CurrentCharacterActionState = CharacterActionState.Walking;
                    }

                    break;
                case CharacterActionState.Walking:
                    if (current2DFacingDirection == Character2DFacingDirection.None)
                    {
                        CurrentCharacterActionState = CharacterActionState.Idle;
                    }

                    break;
                case CharacterActionState.Running:
                    break;
                case CharacterActionState.Jumping:
                    break;
                case CharacterActionState.Falling:
                    if (onGround)
                    {
                        CurrentCharacterActionState = current2DFacingDirection == Character2DFacingDirection.None
                            ? CharacterActionState.Idle
                            : CharacterActionState.Walking;
                    }

                    break;
                case CharacterActionState.WallSliding:
                    break;
                case CharacterActionState.Dashing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!onGround && currVelocity.y < 0)
                CurrentCharacterActionState = CharacterActionState.Falling;

        }

        protected virtual void VelocityConstrains(ref Vector2 currVelocity)
        {
            switch (currentActionState)
            {
                case CharacterActionState.Idle:
                    break;
                case CharacterActionState.Walking:
                    break;
                case CharacterActionState.Running:
                    break;
                case CharacterActionState.Jumping:
                    break;
                case CharacterActionState.Falling:
                    if (currVelocity.y < -maxFallSpeed) currVelocity.y = -maxFallSpeed;
                    break;
                case CharacterActionState.WallSliding:
                    break;
                case CharacterActionState.Dashing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (onGround && currVelocity.y < 0) currVelocity.y = 0;
        }
        
        #endregion

        #region Checks

        /// <summary>
        /// Checks called at the start of every FixedUpdate
        /// </summary>
        protected virtual void ExecuteEnvironmentChecks()
        {
            onGround = CheckIfOnGround();
        }
        
        protected virtual bool CheckIfOnGround()
        {
            return DoBoxCast(Vector2.down,castDistance,groundLayers).Length > 0;
        }

        /// <summary>
        /// shoots a physics2D boxCast with the dimensions of the box collider
        /// !filters out own colliders and trigger colliders 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <returns>filtered hits</returns>
        protected virtual RaycastHit2D[] DoBoxCast(Vector2 direction,float distance ,LayerMask layerMask)
        {
            var raycastHit2Ds = new RaycastHit2D[10];
            var size = Physics2D.BoxCastNonAlloc(transform.position + (Vector3)boxCollider2D.offset,
                boxCollider2D.size, 0, direction, raycastHit2Ds, distance, layerMask);
            var validResults = new List<RaycastHit2D>();
            foreach (var rayCastHit2D in raycastHit2Ds)
            {
                if (!rayCastHit2D || rayCastHit2D.collider.isTrigger || rayCastHit2D.transform.gameObject == gameObject)continue;
                validResults.Add(rayCastHit2D);
            }
            return validResults.ToArray();
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