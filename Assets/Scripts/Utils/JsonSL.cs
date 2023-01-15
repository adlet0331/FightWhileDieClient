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
        public static string basePath = Application.persistentDataPath;
        public static string LoadJson(string path)
        {
            var filePath = Path.Combine(basePath, path + ".json");
            if (!File.Exists(filePath))
                return string.Empty;
            string jsonString;
#if UNITY_EDITOR
            jsonString = File.ReadAllText(filePath);
#elif UNITY_ANDROID
            jsonString = File.ReadAllText(filePath);
#elif UNITY_IPHONE
            jsonString = File.ReadAllText(filePath);
#endif
            return jsonString;
        }

        public static async UniTaskVoid SaveJson(string path, string content)
        {
            var filePath = Path.Combine(basePath, path + ".json");
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