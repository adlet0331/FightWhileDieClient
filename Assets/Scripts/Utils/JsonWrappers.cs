using System;
using System.Collections.Generic;
using Data;
using NonDestroyObject.DataManage;

namespace Utils
{
    /// <summary>
    /// Wrapper classes for Unity JsonUtility serialization
    /// Unity's JsonUtility cannot directly serialize Lists, so we need wrapper classes
    /// </summary>
    
    [Serializable]
    public class EquipItemObjectListWrapper
    {
        public List<EquipItemObject> items;
    }
    
    [Serializable]
    public class StaticDataVersionListWrapper
    {
        public List<StaticDataVersion> items;
    }
    
    [Serializable]
    public class GatchaProbabilityListWrapper
    {
        public List<GatchaProbability> items;
    }
    
    [Serializable]
    public class EquipItemInfoListWrapper
    {
        public List<EquipItemInfo> items;
    }
    
    [Serializable]
    public class EnhanceInfoListWrapper
    {
        public List<EnhanceInfo> items;
    }
    
    [Serializable]
    public class CustomLogListWrapper
    {
        public List<CustomLog> items;
    }
}
