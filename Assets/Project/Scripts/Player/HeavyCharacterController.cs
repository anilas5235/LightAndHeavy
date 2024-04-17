using ControllerPlugin.Scripts;
using Project.Scripts.Interfaces;
using Project.Scripts.LevelObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using static Project.Scripts.Utilities.InputSystemUtility;

namespace Project.Scripts.Player
{
    public class HeavyCharacterController : AdvancedCharacterController2D, IHaveElementType
    {
        private MainInput _mainInput;

        protected override void Awake()
        {
            base.Awake();
            _mainInput = new MainInput();
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyXAxis,HorizontalInput);
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyLeftDash,DashLeft);
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyRightDash,DashRight);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _mainInput.Enable();
        }

        private void OnDisable()
        {
           _mainInput.Disable();
        }

        public ElementType GetElementType()
        {
            return ElementType.Heavy;
        }
        
        private void DashRight(InputAction.CallbackContext obj)
        {
            if(obj.performed) DashInput(Vector2.right);
        }

        private void DashLeft(InputAction.CallbackContext obj)
        {
            if(obj.performed) DashInput(Vector2.left);
        }
    }
}