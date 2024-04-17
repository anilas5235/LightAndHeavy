using Project.Scripts.Attributes;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class CapsuleRayCaster2D : MonoBehaviour
    {
        [SerializeField] private float rayRange = 10;
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

        private Vector2 _feetRelativeOrigin;
        private Vector2 _kneeRelativeOrigin;
        private Vector2 _hipRelativeOrigin;

        private Vector2 _feetWorldOrigin;
        private Vector2 _kneeWorldOrigin;
        private Vector2 _hipWorldOrigin;

        private float _detectDistance = 0.05f;

        #region Properties

        public float RayRange
        {
            get => rayRange;
            set => rayRange = Mathf.Clamp(value,0,float.MaxValue);
        }

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

        public Character2DFacingDirection WallDetection
        {
            get => wallDetection;
            private set
            {
                if(wallDetection == value)return;
                wallDetection = value;
            }
        }

        public Vector2 GroundNormal
        {
            get => groundNormal;
            private set
            {
                if(groundNormal == value) return;
                groundNormal = value;
                slopeAngle = Vector2.Angle(groundNormal, Vector2.up);
                groundNormalPerp = Vector2.Perpendicular(groundNormal);
            }
        }

        public float SlopeAngle => slopeAngle;

        public Vector2 GroundNormalPerpendicular => groundNormalPerp;

        #endregion
        
        private void Awake()
        {
            _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            CalculateRelativeOrigins();
        }

        private void CalculateRelativeOrigins()
        {
            var size = _capsuleCollider2D.size;
            _feetRelativeOrigin = new Vector2(0, size.y / 2f - .01f);
            _kneeRelativeOrigin = new Vector2(0, size.y / 4f);
            _hipRelativeOrigin = Vector2.zero;
        }

        private void CalculateWorldOrigins()
        {
            var worldPosition = (Vector2)_capsuleCollider2D.transform.position + _capsuleCollider2D.offset;
            _feetWorldOrigin = worldPosition + _feetRelativeOrigin;
            _kneeWorldOrigin = worldPosition + _kneeRelativeOrigin;
            _hipWorldOrigin = worldPosition + _hipRelativeOrigin;
        }

        public void UpdateAll()
        {
            CalculateWorldOrigins();

            onGround = ShootCapsuleCast(_hipWorldOrigin, Vector2.down, _detectDistance, out var groundResult);
            inAir = !onGround;
            onSlope = onGround && slopeAngle != 0;
            
            ShootRayCast(_feetWorldOrigin, Vector2.down, rayRange, out var feetDownResult);
            ShootRayCast(_feetWorldOrigin, Vector2.right, rayRange, out var feetRightResult);
            ShootRayCast(_feetWorldOrigin, Vector2.left, rayRange, out var feetLeftResult);
            
            ShootRayCast(_kneeWorldOrigin, Vector2.right, rayRange, out var kneeRightResult);
            ShootRayCast(_kneeWorldOrigin, Vector2.left, rayRange, out var kneeLeftResult);
            
            ShootRayCast(_hipWorldOrigin, Vector2.right, rayRange, out var hipRightResult);
            ShootRayCast(_hipWorldOrigin, Vector2.left, rayRange, out var hipLeftResult);

            var wallDetectDist = _capsuleCollider2D.size.x / 2f + _detectDistance;
            if (kneeRightResult && kneeRightResult.distance<= wallDetectDist 
                                && hipRightResult&& hipRightResult.distance<= wallDetectDist )
            {
                WallDetection = Character2DFacingDirection.Right;
            }
            else if(kneeLeftResult && kneeLeftResult.distance<= wallDetectDist 
                                    && hipLeftResult&& hipLeftResult.distance<= wallDetectDist)
            {
                WallDetection = Character2DFacingDirection.Left;
            }
            else
            {
                WallDetection = Character2DFacingDirection.None;
            }

            GroundNormal = feetDownResult.normal;
#if UNITY_EDITOR
            Debug.DrawRay(feetDownResult.point,GroundNormal,Color.yellow);
            Debug.DrawRay(feetDownResult.point,GroundNormalPerpendicular,Color.red);
#endif
        }

        private bool ShootRayCast(Vector2 position,Vector2 direction,float rayDistance, out RaycastHit2D resultHit)
        {
            resultHit = default;
            var hits = new RaycastHit2D[maxHitChecks];
            Physics2D.RaycastNonAlloc(position,direction,hits,rayDistance,layerMask);
            foreach (var rayCastHit2D in hits)
            {
                if (!rayCastHit2D || rayCastHit2D.collider.isTrigger || rayCastHit2D.collider.gameObject == gameObject)continue;
                resultHit = rayCastHit2D;
                break;
            }
#if UNITY_EDITOR
            
            if(resultHit) Debug.DrawLine(position,resultHit.point,Color.green);
#endif
                
            return resultHit;
        }

        private bool ShootCapsuleCast(Vector2 position, Vector2 direction, float castDistance,
            out RaycastHit2D resultHit)
        {
            resultHit = default;
            var hits = new RaycastHit2D[maxHitChecks];
            var size = _capsuleCollider2D.size;
            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y)) size.x -= .05f;
            else size.y -= .05f;
            Physics2D.CapsuleCastNonAlloc(position, size, _capsuleCollider2D.direction, 0, direction, hits,
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
    }
}