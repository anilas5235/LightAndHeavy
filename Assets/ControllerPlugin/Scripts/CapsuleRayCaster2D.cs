using System;
using AttributesLibrary.ReadOnly;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class CapsuleRayCaster2D : MonoBehaviour
    {
        [SerializeField]private float detectDistance = 0.05f;
        [SerializeField] private LayerMask layerMask = int.MaxValue;
        [SerializeField] private int maxHitChecks = 5;
        
        [Header("Info")]
        [SerializeField, ReadOnly] private bool onGround;
        [SerializeField, ReadOnly] private bool inAir;
        [SerializeField, ReadOnly] private bool onSlope;
        [SerializeField, ReadOnly] private Vector2 groundNormal;
        [SerializeField, ReadOnly] private Vector2 groundNormalPerp;
        [SerializeField, ReadOnly] private float slopeAngle;
        [SerializeField, ReadOnly] private Character2DFacingDirection wallDetection;
        
        private CapsuleCollider2D _capsuleCollider2D;
        
        private Vector2 _hipRelativeOrigin;
        private Vector2 _lowerCircleCenterRelative;
        
        private Vector2 _hipWorldOrigin;
        private Vector2 _lowerCircleCenterWorld;


        private Vector2 _trueCapsuleSize;
        private float _trueCapsuleXHalfSize;
        private float _trueCapsuleYHalfSize;

        #region Properties
        public LayerMask LayerMask
        {
            get => layerMask;
            set => layerMask = value;
        }

        public int MaxHitChecks
        {
            get => maxHitChecks;
            set => maxHitChecks = Mathf.Clamp(value,2,int.MaxValue);
        }

        public bool OnGround => onGround;
        public bool InAir => inAir;
        public bool OnSlope => onSlope;

        public Vector2 GroundHitPoint { get; private set; }

        public Character2DFacingDirection WallDetection
        {
            get => wallDetection;
            private set => wallDetection = value;
        }

        public Vector2 GroundNormal
        {
            get => groundNormal;
            private set
            {
                if(groundNormal == value) return;
                groundNormal = value;
                slopeAngle = Vector2.Angle(groundNormal, Vector2.up);
                groundNormalPerp = -Vector2.Perpendicular(groundNormal);
            }
        }

        public float SlopeAngle => slopeAngle;

        public Vector2 GroundNormalPerpendicular => groundNormalPerp;

        #endregion
        
        private void Awake()
        {
            SetValues();
        }

        private void SetValues()
        {
            _capsuleCollider2D ??= GetComponent<CapsuleCollider2D>();
           _trueCapsuleSize = _capsuleCollider2D.size * transform.localScale;
           _trueCapsuleXHalfSize = _trueCapsuleSize.x / 2f;
           _trueCapsuleYHalfSize = _trueCapsuleSize.y / 2f;
           
           CalculateRelativeOrigins();
           CalculateWorldOrigins();
        }

        private void CalculateRelativeOrigins()
        {
            _hipRelativeOrigin = Vector2.zero;
            _lowerCircleCenterRelative = new Vector2(0, _trueCapsuleXHalfSize - _trueCapsuleYHalfSize);
        }

        private void CalculateWorldOrigins()
        {
            var worldPosition = (Vector2)_capsuleCollider2D.transform.position + _capsuleCollider2D.offset;
            _hipWorldOrigin = worldPosition + _hipRelativeOrigin;
            _lowerCircleCenterWorld = worldPosition + _lowerCircleCenterRelative;
        }

        public void UpdateAll()
        {
            CalculateWorldOrigins();

            onGround = ShootCapsuleCast(_hipWorldOrigin, Vector2.down, detectDistance,out var groundResult);
            inAir = !onGround;
            GroundNormal = groundResult.normal;
            GroundHitPoint = groundResult.point;
            onSlope = onGround && groundResult.point.y <= _lowerCircleCenterWorld.y && slopeAngle != 0;
            
            if (ShootCapsuleCast(_hipWorldOrigin,Vector2.right,detectDistance,out var rightWallCheckResult))
            {
                WallDetection = Character2DFacingDirection.Right;
            }
            else if(ShootCapsuleCast(_hipWorldOrigin,Vector2.left,detectDistance,out var leftWallCheckResult))
            {
                WallDetection = Character2DFacingDirection.Left;
            }
            else
            {
                WallDetection = Character2DFacingDirection.None;
            }
        }
        private bool ShootCapsuleCast(Vector2 position, Vector2 direction, float castDistance,
            out RaycastHit2D resultHit)
        {
            resultHit = default;
            var hits = new RaycastHit2D[maxHitChecks];
            Physics2D.CapsuleCastNonAlloc(position, _trueCapsuleSize, _capsuleCollider2D.direction, 0, direction, hits,
                castDistance, layerMask);
            foreach (var rayCastHit2D in hits)
            {
                if (!rayCastHit2D || rayCastHit2D.collider.isTrigger ||
                    rayCastHit2D.collider.gameObject == gameObject) continue;
                resultHit = rayCastHit2D;
                break;
            }
            return resultHit;
        }

        private void OnDrawGizmos()
        {
            var oldColor = Gizmos.color;
            
            Gizmos.color =Color.magenta;
            Gizmos.DrawSphere(GroundHitPoint,.05f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(GroundHitPoint,GroundNormal);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(GroundHitPoint,GroundNormalPerpendicular);
            
            Gizmos.color = oldColor;
        }
    }
}