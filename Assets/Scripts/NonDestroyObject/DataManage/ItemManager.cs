using System;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using Unity.VisualScripting;
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
        [SerializeField] private List<Color> rareColorList; // TODO: Move to StaticDataManager 

        public List<ItemEquipment> ItemEquipments => new List<ItemEquipment>(itemEquipmentList);
        public List<Color> RareColorList => rareColorList;

        public void Start()
        {
            var jsonString = JsonSL.LoadJson(JsonTitle.ItemEquipment);
            if (jsonString == String.Empty)
            {
                itemEquipmentList = new List<ItemEquipment>();
            }
            else
            {
                itemEquipmentList = JsonConvert.DeserializeObject<List<ItemEquipment>>(jsonString);
            }
        }

        public void Clear()
        {
            JsonSL.DeleteJsonFile(JsonTitle.ItemEquipment).Forget();
            itemEquipmentList = new List<ItemEquipment>();
        }

        public void AddItems(List<ItemEquipment> itemEquipments)
        {
            foreach (var item in itemEquipments)
            {
                itemEquipmentList.Add(item);
            }
            var jsonString = JsonConvert.SerializeObject(itemEquipmentList);
            JsonSL.SaveJson(JsonTitle.ItemEquipment, jsonString).Forget();
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
