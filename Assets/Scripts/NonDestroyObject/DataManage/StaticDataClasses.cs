using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace NonDestroyObject.DataManage
{
    [Serializable]
    public class StaticDataVersion
    {
        public string title;
        public int versionNum;
        public int updatedDate;
    }

    public enum GatchaProbabilityType
    {
        Rare = 0,
        Option = 1
    }

    [Serializable]
    public class GatchaProbability
    {
        public int idx;
        [SerializeField] private GatchaProbabilityType Type;
        // Property
        public int type
        {
            get
            {
                return (int)Type;
            }
            set
            {
                Type = (GatchaProbabilityType)value;
            }
        }
        public List<int> probabilityList;
        public int maxIndex;
        public int totalProbabilityNum;
    }

    [Serializable]
    public class EquipItemInfo
    {
        public int idx;
        [SerializeField] private Rare Rare;
        // Property
        public int rare
        {
            get
            {
                return (int)Rare;
            }
            set
            {
                Rare = (Rare)value;
            }
        }
        [SerializeField] public EquipmentOption Option;
        // Property
        public int option
        {
            get
            {
                return (int)Option;
            }
            set
            {
                Option = (EquipmentOption)value;
            }
        }
        public List<string> nameList;
        public List<string> descriptionList;
        public string spriteName;
        public int maxLevel;
        public List<int> optionValuePerLevelList;
        public List<int> coinPerLevelList;
    }

    [Serializable]
    public class EnhanceProbability
    {
        
    }
}