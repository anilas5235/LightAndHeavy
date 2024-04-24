using System;
using System.Collections;
using AttributesLibrary.ReadOnly;
using ControllerPlugin.Scripts;
using Project.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using static Project.Scripts.Utilities.InputSystemUtility;

namespace Project.Scripts.LevelObjects
{
    public class Canon : BoolInteractable
    {
        private const float AngleTolerance = .001f;
        
        [Space,SerializeField,Range(-90,0)] private float minAngle = -30;
        [SerializeField,Range(0,90)] private float maxAngle = 30;
        [SerializeField,Range(.5f,20f)] private float angleStep = .5f;
        [SerializeField,Range(1, 30)] private float canonStrength = 8;
        [SerializeField,Range(1, 30)] private float shootDuration = 2;
        [SerializeField] private Transform tipTransform;
        [SerializeField] private SpriteRenderer tippLine;
        
        
        [Header("info"),SerializeField,ReadOnly] private float currentAngleTarget;
        [SerializeField,ReadOnly] private float originalAngle;
        [SerializeField,ReadOnly] private float usedMinAngle;
        [SerializeField,ReadOnly] private float usedMaxAngle;
        [SerializeField, ReadOnly] private AdvancedCharacterController2D user;
        [SerializeField, ReadOnly] private bool readyForUse = true;
        
        private MainInput _mainInput;
        private Vector2Int _input;
        private const float coolDown = 1;
        private float _lineLength;
        
        private static readonly int Tiling = Shader.PropertyToID("_Tiling");

        private void Awake()
        {
            originalAngle = transform.localRotation.eulerAngles.z;
            usedMinAngle = minAngle + originalAngle;
            usedMaxAngle = maxAngle + originalAngle;
            _mainInput = new MainInput();
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyXAxis, InputAxis);
            _lineLength = canonStrength * (shootDuration / 10f);
            var lineTransform = tippLine.transform;
            var scale = lineTransform.localScale;
            scale.x = _lineLength / 2f;
            lineTransform.localScale = scale;
            tippLine.material.SetFloat(Tiling,_lineLength*3);
            tippLine.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && readyForUse && user == null)
            {
                var comp = other.gameObject.GetComponent<HeavyCharacterController>();
                if (comp)
                {
                    _mainInput.Enable();
                    user = comp;
                    comp.enabled = false;
                    comp.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    comp.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    tippLine.enabled = true;
                }
                readyForUse = true;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _mainInput?.Disable();
        }

        protected override void StateChanged(bool newState)
        {
            if (newState && user != null && readyForUse)
            {
                Shoot();
                _mainInput.Disable();
                tippLine.enabled = false;
            }
        }

        private void Shoot()
        {
            var player = user.gameObject;
            var tipPosition = tipTransform.position;
            user.enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.transform.position = tipPosition;
            var dashParams = new DashParams()
            {
                direction = (tipPosition - transform.position).normalized,
                dashSpeed = canonStrength,
                dashDuration = shootDuration/10f,
                useDashCoolDown = false,
            };
            user.DashInput(dashParams);
            readyForUse = false;
            user = null;
            StartCoroutine(CoolDown(coolDown));
        }

        private void InputAxis(InputAction.CallbackContext obj)
        {
            _input.x = Mathf.RoundToInt(obj.ReadValue<float>());
        }
        private void Update()
        {
            if(Math.Abs(currentAngleTarget - transform.localRotation.eulerAngles.z) < AngleTolerance)return;
            transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(0,0,currentAngleTarget),.01f);
        }
        private void FixedUpdate()
        {
            currentAngleTarget += angleStep * -_input.x;
            currentAngleTarget = Mathf.Clamp(currentAngleTarget, usedMinAngle, usedMaxAngle);
        }

        private IEnumerator CoolDown(float duration)
        {
            yield return new WaitForSeconds(duration);
            readyForUse = true;
        }

        private void OnDrawGizmos()
        {
            var tipPosition = tipTransform.position;
            Gizmos.DrawRay(tipPosition,(tipPosition - transform.position).normalized * _lineLength);
        }
    }
}
