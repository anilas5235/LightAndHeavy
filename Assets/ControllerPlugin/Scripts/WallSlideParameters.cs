using System;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    [Serializable]
    public class WallSlideParameters
    {
        public bool canWallSlide = true;
        [Range(0.01f, 50f)] public float maxWallSlidingSpeed = 1.5f;
    }
}