using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Utils;

namespace NonDestroyObject.DataManage
{
    [Serializable]
    public class ItemManager
    {
#if UNITY_EDITOR
        [ArrayElementTitle("Id")]
#endif
        [SerializeField] private List<ItemEquipment> ItemEquipmentList;

        public void Start()
        { 
            ItemEquipmentList = new List<ItemEquipment>();
        }

        public void AddNewItem()
        {
            
        }
    }
}
