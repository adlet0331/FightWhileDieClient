using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public class JsonSL
    {
        public static string LoadJson(string path)
        {
            var filePath = Path.Combine(Application.dataPath, path, ".json");
            if (!File.Exists(filePath))
                return string.Empty;
            string logJsonString;
#if UNITY_EDITOR
            logJsonString = File.ReadAllText(path);
#elif UNITY_ANDROID
            logJsonString = File.ReadAllText(path);
#elif UNITY_IPHONE
            logJsonString = File.ReadAllText(path);
#endif
            return logJsonString;
        }

        public static async UniTaskVoid SaveJson(string path, string content)
        {
            var filePath = Path.Combine(Application.dataPath, path, ".json");
            try
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    File.WriteAllText(path, content);
                });
            }
            catch (Exception e)
            {
                Debug.LogAssertion(e);
            }
        }
    }
}