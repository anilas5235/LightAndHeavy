using System.Collections;
using AttributesLibrary.DrawIfAttribute;
using Project.Scripts.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
    public class BasicCharacterController2D : MonoBehaviour
    {
        [SerializeField] protected LayerMask groundLayers;
        
        [Header("Speed Settings"),SerializeField, Range(0f, 100f)] protected float maxSpeed = 4f;
        [SerializeField, Range(0f, 100f)] protected float maxFallSpeed = 6f;
        [SerializeField, Range(0f, 100f)] protected float maxAcceleration = 35f;
        [SerializeField, Range(0f, 100f)] protected float maxAirAcceleration = 20f;

        [Header("Jump Settings"),SerializeField] protected bool canJump = true;
        [DrawIf("canJump", true, ComparisonType.Equals),SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
        [DrawIf("canJump", true, ComparisonType.Equals),SerializeField, Range(0, 5)] private int maxAirJumps = 0;
        [DrawIf("canJump", true, ComparisonType.Equals),SerializeField, Range(0f, 5f)] protected float downwardMovementMultiplier = 3f;
        [DrawIf("canJump", true, ComparisonType.Equals),SerializeField, Range(0f, 5f)] protected float upwardMovementMultiplier = 1.7f;
        
        [Header("Info"),SerializeField, ReadOnly] private Vector2 velocity;
        [SerializeField, ReadOnly] private Vector2Int input;
        [SerializeField, ReadOnly] private bool onGround;
        [SerializeField, ReadOnly] private bool isJumping;

        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider2D;
        private float _defaultGravityScale = 1f;
        private bool _jumpInput;
        private bool _jumpCanceled;
        private int _airJumps;
        private bool _doneGroundCheck = true;

        protected virtual void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        protected void FixedUpdate()
        {
            velocity = _rigidbody2D.velocity;
            if(_doneGroundCheck) onGround = CheckIfOnGround();
            if (isJumping && onGround) isJumping = false;

            velocity.x = CalculateNewXVelocityFromInput(input,velocity);

            if (canJump&&_jumpInput)
            {
                _jumpInput = false;
                if (onGround)
                {
                    _doneGroundCheck = false;
                    StartCoroutine(ActivateGroundDetectAfter(.2f));
                    isJumping = true;
                    _airJumps = 0;
                    HandleJump(ref velocity);
                    onGround = false;
                }
                else if(isJumping && _airJumps < maxAirJumps)
                {
                    isJumping = true;
                    HandleJump(ref velocity); 
                }
            }

            /*if (isJumping && jumpCanceled && velocity.y >0)
            {
                jumpCanceled = false;
                JumpCanceled(ref velocity);
            }*/
                
            UpdateGravityScale();
            LastMinuteVelocityConstrains();
            _rigidbody2D.velocity = velocity;
        }

        protected virtual float CalculateNewXVelocityFromInput(Vector2Int inputVector, Vector2 currentVelocity)
        {
            var desiredVelocity = new Vector2(inputVector.x, 0f) * maxSpeed;
            var acceleration = onGround ? maxAcceleration : maxAirAcceleration;
            var maxSpeedChange = acceleration * Time.deltaTime;
            return Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, maxSpeedChange);
        }

        protected virtual void UpdateGravityScale()
        {
            _rigidbody2D.gravityScale = _rigidbody2D.velocity.y switch
            {
                > 0 => upwardMovementMultiplier,
                < 0 => downwardMovementMultiplier,
                0 => _defaultGravityScale,
                _ => _rigidbody2D.gravityScale
            };
        }

        protected virtual void HandleJump(ref Vector2 currentVelocity)
        {
            currentVelocity.y += jumpHeight;
        }

        protected virtual void LastMinuteVelocityConstrains()
        {
            if (onGround) velocity.y = 0;
            else if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        }
        protected virtual bool CheckIfOnGround()
        {
            var results = new RaycastHit2D[10];
            var size = Physics2D.BoxCastNonAlloc(transform.position + (Vector3)_boxCollider2D.offset,
                _boxCollider2D.size, 0, Vector2.down, results, .05f,groundLayers);

            for (var i = 0; i < size; i++)
            {
                var rayCastHit2D = results[i];
                if(rayCastHit2D.collider.isTrigger||rayCastHit2D.transform.gameObject == gameObject) continue;
                return true;
            }

            return false;
        }

        protected virtual void JumpCanceled(ref Vector2 currentVelocity)
        {
            currentVelocity.y = 0;
        }

        IEnumerator ActivateGroundDetectAfter(float sec)
        {
            yield return new WaitForSeconds(sec);
            _doneGroundCheck = true;
        }

        public virtual void JumpInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _jumpInput = true;
            }

            if (context.canceled)
            {
                _jumpCanceled = true;
            }
        }
        
        public virtual void HorizontalInput(InputAction.CallbackContext context)
        {
            input.x = Mathf.RoundToInt(context.ReadValue<float>());
        }
    }
}