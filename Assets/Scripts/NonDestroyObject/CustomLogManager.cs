using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace NonDestroyObject
{
    /// <summary>
    /// 게임의 오프라인에서 발생하는 모든 Log를 관리하는 매니저입니다
    /// 인터넷이 연결된 경우, 그동안 쌓인 Log를 모두 서버로 보내 무결성 체크를 하는 역할을 합니다
    /// </summary>
    public class CustomLogManager : Singleton<CustomLogManager>
    {
        [SerializeField] private List<CustomLog> CustomLogList;

        private void Start()
        {
            LoadLog();
        }

        private void LoadLog()
        {
            var logJsonString = JsonSL.LoadJson(JsonTitle.CustomLog);
            try
            {
                var wrapper = JsonUtility.FromJson<CustomLogListWrapper>(logJsonString);
                CustomLogList = wrapper != null ? wrapper.items : new List<CustomLog>();
            }
            catch (Exception e)
            {
                Debug.LogAssertion(e.ToString());
                CustomLogList = new List<CustomLog>();
            }
        }

        public void AddLog(CustomLogType type, uint id, uint value)
        {
            CustomLogList.Add(new CustomLog
            {
                Type = type,
                Content = new CustomLogContent
                {
                    Id = id,
                    Value = value,
                    Time = DateTime.UtcNow.ToString("@yyyy-MM-dd HH:mm:ss")
                }
            });
        }
        
        public void SaveLog(bool sendServer)
        {
            var wrapper = new CustomLogListWrapper { items = CustomLogList };
            var jsonString = JsonUtility.ToJson(wrapper);
            JsonSL.SaveJson(JsonTitle.CustomLog, jsonString).Forget();
            if (sendServer)
            {
                // TODO: Send Log To Server
            }
        }
    }
}
