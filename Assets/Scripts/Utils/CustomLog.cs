using System;

namespace Utils
{
    [Serializable]
    public enum CustomLogType
    {
        GetCoin,
        DeleteCoin,
        GetItem,
        DeleteItem,
        EnhanceSuccess,
        EnhanceFail,
    }
    [Serializable]
    public class CustomLogContent
    {
        public string Time;
        /// <summary>
        /// Item Id
        /// </summary>
        public uint Id;
        /// <summary>
        /// 1. Get, DeleteCoin: 코인 값
        /// 2. Get, Delete Item: 더미
        /// 3. EnhanceSuccess, Fail: 강화를 시도한 레벨
        /// </summary>
        public uint Value;
    }
    [Serializable]
    public class CustomLog
    {
        public CustomLogType Type;
        public CustomLogContent Content;
    }
}