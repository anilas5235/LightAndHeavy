using System;
using UnityEngine;

namespace Project.Scripts.Utilities
{
    public abstract class SaveHandler<TSaveType,TClass> : Singleton<TClass> where TSaveType : class, new() where TClass : Component
    {
        protected TSaveType currentSave;
        public TSaveType CurrentSave {
            get
            {
               if(currentSave == null) LoadDataFromFile();
               return currentSave;  
            }
            set => currentSave = value;
        }

        protected override void Awake()
        {
            base.Awake();
            UpdateFilePath();
            LoadDataFromFile();
        }

        protected override void OnDestroy()
        {
            SaveDataToFile();
            base.OnDestroy();
        }

        protected string PathString { get; private set; }

        public void LoadDataFromFile()
        {
            CheckPath();
            currentSave = SaveSystem.Load<TSaveType>(PathString) ?? GenerateNewSave();
        }

        protected virtual TSaveType GenerateNewSave()
        {
            return new TSaveType();
        }

        public void UpdateFilePath()
        {
            PathString = GetFilePath();
        }

        protected abstract string GetFilePath();

        public void SaveDataToFile()
        {
            CheckPath();
            SaveSystem.Save(PathString,currentSave);
        }

        private void CheckPath()
        {
            if (PathString is { Length: < 1 })
            {
                throw new ArgumentException("There was no path set, unable to save or load");
            }
        }
    }
}