using ControllerPlugin.Scripts.Editor;
using UnityEditor;

namespace Project.Scripts.Player.Editor
{
    [CustomEditor(typeof(HeavyCharacterController))]
    public class HeavyCharacterControllerCustomEditor : BasicCharacterController2DCustomEditor
    {
    }
}