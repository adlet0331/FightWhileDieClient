using System;
using NonDestroyObject;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class EquipItemObject
    {
        public string GetName(int locale)
        {
            var itemStaticInfo = DataManager.Instance.staticDataManager.GetEquipItemInfo(rare, option);
            return itemStaticInfo.nameList[locale];
        }
        public string GetDescriptionText(int locale)
        {
            var itemStaticInfo = DataManager.Instance.staticDataManager.GetEquipItemInfo(rare, option);
            return string.Format(itemStaticInfo.descriptionList[locale], itemStaticInfo.optionValuePerLevelList[level]);
        }

        public int GetOptionValue()
        {
            var itemStaticInfo = DataManager.Instance.staticDataManager.GetEquipItemInfo(rare, option);
            return itemStaticInfo.optionValuePerLevelList[level];
        }
        
        public int id;
        [SerializeField] private Rare Rare;
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
        public int level;
        [SerializeField]public EquipmentOption Option;
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
    }
}