using System;

namespace ControllerPlugin.Scripts
{
    public enum CharacterActionState : byte
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        WallSliding,
        Dashing
    }

    public enum Character2DFacingDirection : byte
    {
        None,
        Right,
        Left
    }

    [Serializable]
    public struct FloatMinMax
    {
        public float min;
        public float max;
    }
}