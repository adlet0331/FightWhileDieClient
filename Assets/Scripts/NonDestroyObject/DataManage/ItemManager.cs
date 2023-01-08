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
        [SerializeField] private List<ItemEquipment> itemEquipmentList;
        [SerializeField] private List<Color> rareColorList;

        public List<Color> RareColorList => rareColorList;

        public void Start()
        { 
            itemEquipmentList = new List<ItemEquipment>();
        }

        public void AddNewItem()
        {
            
        }
    }
}
