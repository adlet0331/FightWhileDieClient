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
        [SerializeField] private List<EquipItemObject> itemEquipmentList;
        [SerializeField] private List<Color> rareColorList; // TODO: Move to StaticDataManager 

        public List<EquipItemObject> ItemEquipments => new List<EquipItemObject>(itemEquipmentList);
        public List<Color> RareColorList => new List<Color>(rareColorList);

        public void Start()
        {
            var jsonString = JsonSL.LoadJson(JsonTitle.PlayerEquipItemObjects);
            if (jsonString == String.Empty)
            {
                itemEquipmentList = new List<EquipItemObject>();
            }
            else
            {
                itemEquipmentList = JsonConvert.DeserializeObject<List<EquipItemObject>>(jsonString);
            }
            rareColorList = new List<Color>();
            rareColorList.Add(new Color(0,0,0, 255));
            rareColorList.Add(new Color(137,137,137, 255));
            rareColorList.Add(new Color(0,19,255, 255));
            rareColorList.Add(new Color(195,0,255, 255));
            rareColorList.Add(new Color(255,0,0, 255));
            rareColorList.Add(new Color(255,229,0, 255));
            rareColorList.Add(new Color(0,0,0, 255));
        }

        public void Clear()
        {
            JsonSL.DeleteJsonFile(JsonTitle.PlayerEquipItemObjects).Forget();
            itemEquipmentList = new List<EquipItemObject>();
        }

        public void AddItems(List<EquipItemObject> itemEquipments)
        {
            foreach (var item in itemEquipments)
            {
                itemEquipmentList.Add(item);
            }
            var jsonString = JsonConvert.SerializeObject(itemEquipmentList);
            JsonSL.SaveJson(JsonTitle.PlayerEquipItemObjects, jsonString).Forget();
        }

        public bool DeleteItems(List<EquipItemObject> itemIdList)
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
