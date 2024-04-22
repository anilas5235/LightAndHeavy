using System;
using UnityEngine;

namespace Project.Scripts.Utilities
{
    public abstract class MajorUIComponent : MonoBehaviour
    {
        [Header("MajorUIComponent"),SerializeField] protected Canvas mainCanvas;
        public bool Active { get; private set; }

        protected virtual void Awake()
        {
            mainCanvas.gameObject.SetActive(Active);
        }

        protected void OnDestroy()
        {
            Deactivate();
        }

        public virtual void Activate()
        {
            if(Active)return;
            Active = true;
            mainCanvas.gameObject.SetActive(Active);
        }
        public virtual void Deactivate()
        {
            if(!Active)return;
            Active = false;
            mainCanvas.gameObject.SetActive(Active);
        }
    }
}