using System;
using UnityEngine.InputSystem;

namespace Project.Scripts.Utilities
{
    public class InputSystemUtility
    {
        public static void SubFunctionToAllInputEvents(InputAction inputAction, Action<InputAction.CallbackContext> subscriber)
        {
            inputAction.started += subscriber;
            inputAction.performed += subscriber;
            inputAction.canceled += subscriber;
        }
        
        public static void UnSubFunctionToAllInputEvents(InputAction inputAction, Action<InputAction.CallbackContext> subscriber)
        {
            inputAction.started -= subscriber;
            inputAction.performed -= subscriber;
            inputAction.canceled -= subscriber;
        }
    }
}