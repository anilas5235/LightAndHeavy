using System.IO;
using UnityEngine;

namespace Project.Scripts.Utilities
{
    public static class SaveSystem
    {
        public static void Save<T>(string path, T saveData) where T : class, new()
        {
            path = GetPathPlusApplicationPath(path);
            File.WriteAllText(path, JsonUtility.ToJson(saveData, true));
        }

        public static T Load<T>(string path) where T : class, new()
        {
            path = GetPathPlusApplicationPath(path);
            Debug.Log(path);

            return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null;
        }

        public static void DeleteSaveData(string path)
        {
            path = GetPathPlusApplicationPath(path);
            if (File.Exists(path)) File.Delete(path);
        }

        private static string GetPathPlusApplicationPath(string path)
        {
            return  Application.persistentDataPath + "/" + path;
        }
    }
}

    