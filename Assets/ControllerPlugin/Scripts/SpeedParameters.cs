using System;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    [Serializable]
    public class SpeedParameters
    {
        [Range(0.01f, 50f)] public float maxSpeed = 4f;
        [Range(0.01f, 50f)] public float maxFallSpeed = 20f;
        [Range(0.01f, 50f)] public float maxAcceleration = 15f;
        [Range(0.01f, 50f)] public float maxAirAcceleration = 8f;

        public float GetAcceleration(bool onGround)
        {
            return onGround ? maxAcceleration : maxAirAcceleration;
        }
    }
}