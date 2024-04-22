using System;
using UnityEngine;

namespace Project.Scripts.Utilities
{
    [DefaultExecutionOrder(-100)]
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) {return _instance;}
                
                var obj = new GameObject
                {
                    name = typeof(T).Name,
                    hideFlags = HideFlags.DontSave,
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
    
    public class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) {return _instance;}
                
                var obj = new GameObject
                {
                    name = typeof(T).Name,
                    hideFlags = HideFlags.DontSave,
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}