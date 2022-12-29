using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Json Read/Write를 지원하는 Util
    /// </summary>
    public class JsonSL
    {
        public static string basePath = Application.dataPath;
        public static string LoadJson(string path)
        {
            var filePath = Path.Combine(basePath, path, ".json");
            if (!File.Exists(filePath))
                return string.Empty;
            string logJsonString;
#if UNITY_EDITOR
            logJsonString = File.ReadAllText(filePath);
#elif UNITY_ANDROID
            logJsonString = File.ReadAllText(filePath);
#elif UNITY_IPHONE
            logJsonString = File.ReadAllText(filePath);
#endif
            return logJsonString;
        }

        public static async UniTaskVoid SaveJson(string path, string content)
        {
            var filePath = Path.Combine(basePath, path, ".json");
            try
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    File.WriteAllText(filePath, content);
                });
            }
            catch (Exception e)
            {
                Debug.LogAssertion(e);
            }
        }
    }
}