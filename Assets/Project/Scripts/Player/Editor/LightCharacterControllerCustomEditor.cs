using ControllerPlugin.Scripts.Editor;
using UnityEditor;

namespace Project.Scripts.Player.Editor
{
    [CustomEditor(typeof(LightCharacterController))]
    public class LightCharacterControllerCustomEditor : BasicCharacterController2DCustomEditor
    {
    }
}