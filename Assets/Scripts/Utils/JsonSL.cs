using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public enum JsonTitle
    {
        // Custom Logs
        CustomLog = 0,
        
        // Player EquipItem Local file
        PlayerEquipItemObjects = 1,
        
        // Static Datas
        StaticDataVersion = 2,
        GatchaProbability = 3,
        EquipItemInfo = 4,
        EnhanceInfo = 5,
    }
    /// <summary>
    /// Json Read/Write를 지원하는 Util
    /// </summary>
    public class JsonSL
    {
        public static string basePath = Application.persistentDataPath;
        public static string LoadJson(JsonTitle path)
        {
            var filePath = Path.Combine(basePath, path.ToString() + ".json");
            if (!File.Exists(filePath))
                return string.Empty;
            return File.ReadAllText(filePath);
        }

        public static async UniTaskVoid SaveJson(JsonTitle path, string content)
        {
            var filePath = Path.Combine(basePath, path.ToString() + ".json");
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

        public static async UniTaskVoid DeleteJsonFile(JsonTitle path)
        {
            var filePath = Path.Combine(basePath, path.ToString() + ".json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}