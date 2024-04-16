using ControllerPlugin.Scripts;
using Project.Scripts.Interfaces;
using Project.Scripts.LevelObjects;
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
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyJump,JumpInput);
            SubFunctionToAllInputEvents(_mainInput.KeyBoardPlayer.HeavyXAxis,HorizontalInput);
        }

        private void OnEnable()
        {
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
    }
}