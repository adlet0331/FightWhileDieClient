using System;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace NonDestroyObject.DataManage
{
    [Serializable]
    public class ItemManager
    {
#if UNITY_EDITOR
        [ArrayElementTitle("id")]
#endif
        [SerializeField] private List<ItemEquipment> itemEquipmentList;
        [SerializeField] private List<Color> rareColorList;

        public List<ItemEquipment> ItemEquipments => new List<ItemEquipment>(itemEquipmentList);
        public List<Color> RareColorList => rareColorList;

        public void Start()
        {
            var jsonString = JsonSL.LoadJson("ItemEquipmentList");
            itemEquipmentList = JsonConvert.DeserializeObject<List<ItemEquipment>>(jsonString);
        }

        public void AddItems(List<ItemEquipment> itemEquipments)
        {
            foreach (var item in itemEquipments)
            {
                itemEquipmentList.Add(item);
            }
            var jsonString = JsonConvert.SerializeObject(itemEquipmentList);
            JsonSL.SaveJson("ItemEquipmentList", jsonString).Forget();
        }

        public bool DeleteItems(List<ItemEquipment> itemIdList)
        {
            var flag = true;
            foreach (var item in itemIdList)
            {
                flag = flag && itemEquipmentList.Remove(item);
            }
            return flag;
        }
    }
}
