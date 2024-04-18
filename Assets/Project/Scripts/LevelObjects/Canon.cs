using System;
using AttributesLibrary.ReadOnly;
using ControllerPlugin.Scripts;
using Project.Scripts.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts.LevelObjects
{
    public class Canon : BoolInteractable
    {
        private const float AngleTolerance = .001f;
        
        [Space,SerializeField,Range(-90,0)] private float minAngle = -30;
        [SerializeField,Range(0,90)] private float maxAngle = 30;
        [SerializeField,Range(.5f,20f)] private float angleStep = .5f;
        [SerializeField,ReadOnly] private float currentAngleTarget;
        [SerializeField,ReadOnly] private float originalAngle;
        [SerializeField,ReadOnly] private float usedMinAngle;
        [SerializeField,ReadOnly] private float usedMaxAngle;
        [SerializeField, ReadOnly] private AdvancedCharacterController2D user;
        
        public AdvancedCharacterController2D User
        {
            get => user;
            set => user = value;
        }
        
        private MainInput _mainInput;
        private Vector2Int input;

        private void Awake()
        {
            originalAngle = transform.localRotation.eulerAngles.z;
            usedMinAngle = minAngle + originalAngle;
            usedMaxAngle = maxAngle + originalAngle;
            _mainInput = new MainInput();
            InputSystemUtility.SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyXAxis, InputAxis);
        }
        public void EnableInput()
        {
            _mainInput?.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _mainInput?.Disable();
        }

        protected override void StateChanged(bool newState)
        {
            if (newState && user != null)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            print("shoot");
        }

        private void InputAxis(InputAction.CallbackContext obj)
        {
            input.x = Mathf.RoundToInt(obj.ReadValue<float>());
        }
        private void Update()
        {
            if(Math.Abs(currentAngleTarget - transform.localRotation.eulerAngles.z) < AngleTolerance)return;
            transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(0,0,currentAngleTarget),.01f);
        }
        private void FixedUpdate()
        {
            currentAngleTarget += angleStep * -input.x;
            currentAngleTarget = Mathf.Clamp(currentAngleTarget, usedMinAngle, usedMaxAngle);
        }
    }
}
