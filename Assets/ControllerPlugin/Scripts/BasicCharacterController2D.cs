using System;
using AttributesLibrary.DrawIfAttribute;
using Project.Scripts.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class BasicCharacterController2D : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayers;
        [SerializeField, Range(0.01f, 10f)] private float gravityScale = 1f;

        [SerializeField, Range(0f, 100f)] protected float maxSpeed = 4f;
        [SerializeField, Range(0f, 100f)] private float maxFallSpeed = 6f;
        [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
        [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;

        [SerializeField] private bool canJump = true;
        [SerializeField] private FloatMinMax jumpHeight = new (){max = 3f,min = .25f};
        [SerializeField, Range(0f, 3f)] private float timeTillApex = 1f;

        [SerializeField] private bool canAirJump;
        [SerializeField, Range(0, 5)] private int maxAirJumps;

        [SerializeField, ReadOnly] private Vector2Int input;
        [SerializeField, ReadOnly] private CharacterActionState currentCharacterActionState = CharacterActionState.Idle;
        [SerializeField, ReadOnly] private Character2DFacingDirection currentCharacter2DFacingDirection = Character2DFacingDirection.None;
        [SerializeField, ReadOnly] private bool onGround;
        [SerializeField, ReadOnly] private float gravity;
        [SerializeField, ReadOnly] private float maxJumpVelocity;
        [SerializeField, ReadOnly] private float minJumpVelocity;
        
        private Rigidbody2D _rigidBody2D;
        private BoxCollider2D _boxCollider2D;
        
        private bool _jumpInput;
        private bool _jumpCanceledInput;
        
        private int _airJumps;
        
        public CharacterActionState CurrentCharacterActionState 
        {
            get => currentCharacterActionState;
            protected set
            {
                if (currentCharacterActionState == value) return;
                currentCharacterActionState = value;
                onCharacterStateChanged?.Invoke(currentCharacterActionState);
            }
        }

        public Character2DFacingDirection CurrentCharacter2DFacingDirection
        {
            get => currentCharacter2DFacingDirection;
            protected set
            {
                if(currentCharacter2DFacingDirection == value)return;
                currentCharacter2DFacingDirection = value;
                onCharacter2DFacingDirectionChanged?.Invoke(currentCharacter2DFacingDirection);
            }
        }

        public UnityEvent<CharacterActionState> onCharacterStateChanged;
        public UnityEvent<Character2DFacingDirection> onCharacter2DFacingDirectionChanged;
        protected virtual void Awake()
        {
            _rigidBody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _rigidBody2D.gravityScale = 0;
            CalculateJumpValues();
        }

        private void OnEnable()
        {
            CurrentCharacter2DFacingDirection = Character2DFacingDirection.None;
            CurrentCharacterActionState = CharacterActionState.Idle;
        }

        protected void FixedUpdate()
        {
            var currentVelocity = _rigidBody2D.velocity;
            onGround = CheckIfOnGround();

            ApplyMovement(ref currentVelocity,input);

            if (canJump)HandleJumpInput(ref currentVelocity);

            ApplyGravity(ref currentVelocity);
            UpdateStateValues(currentVelocity);
            LastMinuteVelocityConstrains(ref currentVelocity);
            _rigidBody2D.velocity = currentVelocity;
        }

        private void ApplyGravity(ref Vector2 currentVelocity)
        {
            currentVelocity.y -= gravity*Time.fixedDeltaTime;
        }

        private void CalculateJumpValues()
        {
            gravity = 2 * jumpHeight.max / Mathf.Pow(timeTillApex,2) * gravityScale;
            maxJumpVelocity = Mathf.Abs(gravity) * timeTillApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight.min);
        }

        private void HandleJumpInput(ref Vector2 currentVelocity)
        {
            if (_jumpInput)
            {
                _jumpInput = false;
                switch (currentCharacterActionState)
                {
                    case CharacterActionState.Jumping:
                    case CharacterActionState.Falling:
                        if (_airJumps >= maxAirJumps) break;
                        currentCharacterActionState = CharacterActionState.Jumping;
                        HandleAirJump(ref currentVelocity);
                        break;
                    case CharacterActionState.WallSliding:
                        break;
                    case CharacterActionState.Dashing:
                        break;
                    default:
                        if (!onGround) break;
                        currentCharacterActionState = CharacterActionState.Jumping;
                        _airJumps = 0;
                        HandleNewJump(ref currentVelocity);
                        break;
                }
            }
            else if (canAirJump && _jumpCanceledInput && currentCharacterActionState == CharacterActionState.Jumping)
            {
                _jumpCanceledInput = false;
                HandleJumpCanceled(ref currentVelocity);
            }
        }

        private void UpdateStateValues(Vector2 currentVelocity)
        {
            if (!onGround && currentVelocity.y < 0 && currentCharacterActionState != CharacterActionState.WallSliding)
                CurrentCharacterActionState = CharacterActionState.Falling;
            
            currentCharacter2DFacingDirection = currentVelocity.x switch
            {
                < 0 => Character2DFacingDirection.Left,
                > 0 => Character2DFacingDirection.Right,
                _ => Character2DFacingDirection.None
            };
        }

        protected virtual void ApplyMovement(ref Vector2 currentVelocity,Vector2Int inputVector)
        {
            var desiredVelocity = Vector2.right * (Mathf.Sign(inputVector.x) * maxSpeed);
            var acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, acceleration * Time.fixedDeltaTime);
        }

        protected virtual void HandleNewJump(ref Vector2 currentVelocity)
        {
            currentVelocity.y = maxJumpVelocity;
        }
        
        protected virtual void HandleAirJump(ref Vector2 currentVelocity)
        {
            currentVelocity.y = maxJumpVelocity;
        }
        
        protected virtual void HandleJumpCanceled(ref Vector2 currentVelocity)
        {
            if (currentVelocity.y > minJumpVelocity) currentVelocity.y = minJumpVelocity;
        }

        private void LastMinuteVelocityConstrains(ref Vector2 currentVelocity)
        {
            switch (currentCharacterActionState)
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
                    if (currentVelocity.y < -maxFallSpeed) currentVelocity.y = -maxFallSpeed;
                    break;
                case CharacterActionState.WallSliding:
                    break;
                case CharacterActionState.Dashing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if(onGround && currentVelocity.y < 0) currentVelocity.y = 0;
        }

        protected virtual bool CheckIfOnGround()
        {
            var results = new RaycastHit2D[10];
            var size = Physics2D.BoxCastNonAlloc(transform.position + (Vector3)_boxCollider2D.offset,
                _boxCollider2D.size, 0, Vector2.down, results, .05f, groundLayers);

            for (var i = 0; i < size; i++)
            {
                var rayCastHit2D = results[i];
                if (rayCastHit2D.collider.isTrigger || rayCastHit2D.transform.gameObject == gameObject) continue;
                return true;
            }

            return false;
        }

        public virtual void JumpInput(InputAction.CallbackContext context)
        {
            if (!_jumpInput && context.started) _jumpInput = true;
            if (!_jumpCanceledInput && context.canceled) _jumpCanceledInput = true;
        }

        public virtual void HorizontalInput(InputAction.CallbackContext context)
        {
            input.x = Mathf.RoundToInt(context.ReadValue<float>());
        }
    }

    public enum CharacterActionState : byte
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        WallSliding,
        Dashing
    }

    public enum Character2DFacingDirection : byte
    {
        None,
        Right,
        Left
    }

    [Serializable]
    public struct FloatMinMax
    {
        public float min;
        public float max;
    }
}