using System;
using AttributesLibrary.ReadOnly;
using ControllerPlugin.Scripts;
using UnityEngine;
using static ControllerPlugin.Scripts.Character2DFacingDirection;

namespace Project.Scripts.LevelObjects
{
    public class Lever : BoolInteractable
    {
        [Space,SerializeField,Range(-90,0)] private float minAngle = -30;
        [SerializeField,Range(0,90)] private float maxAngle = 30;
        [SerializeField,Range(.5f,20f)] private float angleStep = .5f;
        [SerializeField,ReadOnly] private float currentAngleTarget;
        [SerializeField,ReadOnly]private float originalAngle;
        [SerializeField,ReadOnly]private float usedMinAngle;
        [SerializeField,ReadOnly]private float usedMaxAngle;

        private Collider2D _collider2D;

        private const float AngleTolerance = .001f;

        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            originalAngle = transform.localRotation.eulerAngles.z;
            usedMinAngle = minAngle + originalAngle;
            usedMaxAngle = maxAngle + originalAngle;
            currentAngleTarget = State ? usedMinAngle : usedMaxAngle;
        }

        private void Update()
        {
            if(Math.Abs(currentAngleTarget - transform.localRotation.eulerAngles.z) < AngleTolerance)return;
            transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(0,0,currentAngleTarget),.01f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MakeAngleStep(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            MakeAngleStep(other);
        }

        private void MakeAngleStep(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //if(other.transform.position.y > transform.position.y + _collider2D.offset.y) return;
                var direction = other.transform.position.x - (transform.position.x + _collider2D.offset.x);
                var playerDirection = other.gameObject.GetComponent<AdvancedCharacterController2D>()
                    .CurrentCharacter2DFacingDirection;
                if (Mathf.Sign(direction) > 0 && playerDirection == Left)
                {
                    currentAngleTarget += angleStep;
                }
                else if (Mathf.Sign(direction) < 0 && playerDirection == Right)
                {
                    currentAngleTarget -= angleStep;
                }
                UpdateAngle();
            }
        }

        private void UpdateAngle()
        {
            currentAngleTarget = Mathf.Clamp(currentAngleTarget, usedMinAngle, usedMaxAngle);
            if (State)
            {
                if(Math.Abs(currentAngleTarget - usedMaxAngle) < AngleTolerance) State = false;
            }
            else
            {
                if(Math.Abs(currentAngleTarget - usedMinAngle) < AngleTolerance) State = true;
            }
        }
    }
}