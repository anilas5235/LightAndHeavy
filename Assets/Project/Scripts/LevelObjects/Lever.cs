using System;
using Project.Scripts.Attributes;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class Lever : BoolInteractable
    {
        [Space,SerializeField,Range(-90,0)] private float minAngle = -30;
        [SerializeField,Range(0,90)] private float maxAngle = 30;
        [SerializeField,Range(.5f,20f)] private float angleStep = .5f;
        [SerializeField,ReadOnly] private float currentAngleTarget;

        private Collider2D _collider2D;

        private const float AngleTolerance = .001f;

        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            currentAngleTarget = State ? minAngle : maxAngle;
        }

        private void Update()
        {
            if(Math.Abs(currentAngleTarget - transform.localRotation.eulerAngles.z) < AngleTolerance)return;
            transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(0,0,currentAngleTarget),.01f);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            MakeAngleStep(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            MakeAngleStep(other);
        }

        private void MakeAngleStep(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(other.transform.position.y > transform.position.y + _collider2D.offset.y) return;
                var direction = other.transform.position.x - (transform.position.x + _collider2D.offset.x);
                if (direction < 0) currentAngleTarget -= angleStep;
                else currentAngleTarget += angleStep;
                UpdateAngle();
            }
        }

        private void UpdateAngle()
        {
            currentAngleTarget = Mathf.Clamp(currentAngleTarget, minAngle, maxAngle);
            if (State)
            {
                if(Math.Abs(currentAngleTarget - maxAngle) < AngleTolerance) State = false;
            }
            else
            {
                if(Math.Abs(currentAngleTarget - minAngle) < AngleTolerance) State = true;
            }
        }
    }
}