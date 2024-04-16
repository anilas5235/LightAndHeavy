using Project.Scripts.Attributes;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    public class AdvancedCharacterController2D : BasicCharacterController2D
    {
        [SerializeField, ReadOnly] private bool detectRightWall;
        [SerializeField, ReadOnly] private bool detectLeftWall;
        protected override void ExecuteEnvironmentChecks()
        {
            base.ExecuteEnvironmentChecks();
            detectRightWall = CheckForRightWall();
            detectLeftWall = CheckForLeftWall();
        }
        protected virtual bool CheckForRightWall()
        {
            return DoBoxCast(Vector2.right, castDistance, GroundLayers).Length > 0;
        }
        protected virtual bool CheckForLeftWall()
        {
            return DoBoxCast(Vector2.left, castDistance, GroundLayers).Length > 0;
        }
    }
}