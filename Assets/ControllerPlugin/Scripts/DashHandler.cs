using System;
using System.Collections;
using ControllerPlugin.ReadOnly;
using UnityEngine;

namespace ControllerPlugin.Scripts
{
    [Serializable]
    public class DashHandler
    {
        public bool canDash = true;
        [SerializeField] public DashParams dashParams;
        [ReadOnly] public DashState currentDashState;

        [HideInInspector] public bool useOverrideParams = false;
        [HideInInspector] public DashParams overrideParams;

        public Vector2 DashVector => useOverrideParams ? overrideParams.DashVector : dashParams.DashVector;
        public Vector2 DashDirection => useOverrideParams ? overrideParams.direction : dashParams.direction;
        public float DashSpeed => useOverrideParams ? overrideParams.dashSpeed : dashParams.dashSpeed;
        public float DashDuration => useOverrideParams ? overrideParams.dashDuration : dashParams.dashDuration; 
        public bool UseDashCoolDown => useOverrideParams ? overrideParams.useDashCoolDown : dashParams.useDashCoolDown;
        public float DashCoolDown => useOverrideParams ? overrideParams.dashCoolDown : dashParams.dashCoolDown; 

        public bool CanDashNow => canDash && currentDashState == DashState.Ready;
    }
    
    [Serializable]
    public class DashParams
    {
        [Range(0.01f,50)] public float dashSpeed = 6;
        [Range(0.01f, 5f)] public float dashDuration = .3f;
        public bool useDashCoolDown = true;
        [Range(0.01f, 20f)] public float dashCoolDown = 1f;
        [HideInInspector]public Vector2 direction;

        public Vector2 DashVector => direction.normalized * dashSpeed;
    }

    public enum DashState : byte
    {
        Ready,
        Dashing,
        CoolDown,
    }
}