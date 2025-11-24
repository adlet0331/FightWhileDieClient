using System;
using System.Collections.Generic;
using System.ComponentModel;
using Data;
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
        public bool ItemAddedOrDeleted
        {
            get
            {
                // 인식 했으면 조치 했을거라 믿음
                if (!itemAddOrDeleted)
                    return false;
                itemAddOrDeleted = false;
                return true;
            }
            private set => itemAddOrDeleted = value;
        }
        [SerializeField] private bool itemAddOrDeleted;
        public List<EquipItemObject> ItemEquipments => itemEquipmentList;

        public EquipItemObject GetEquipItemObjectWithId(int id)
        {
            foreach (var itemObject in itemEquipmentList)
            {
                if (itemObject.id == id)
                    return itemObject;
            }
            return null;
        }

        public void UpdateEquipItemObject(EquipItemObject equipItemObject)
        {
            var index = itemEquipmentList.FindIndex((val) => val.id == equipItemObject.id);
            itemEquipmentList[index] = equipItemObject;

            var wrapper = new EquipItemObjectListWrapper { items = itemEquipmentList };
            var jsonString = JsonUtility.ToJson(wrapper);
            JsonSL.SaveJson(JsonTitle.PlayerEquipItemObjects, jsonString).Forget();
        }
        
        public void Start()
        {
            var jsonString = JsonSL.LoadJson(JsonTitle.PlayerEquipItemObjects);
            if (jsonString == String.Empty)
            {
                itemEquipmentList = new List<EquipItemObject>();
            }
            else
            {
                var wrapper = JsonUtility.FromJson<EquipItemObjectListWrapper>(jsonString);
                itemEquipmentList = wrapper != null ? wrapper.items : new List<EquipItemObject>();
            }
            itemAddOrDeleted = false;
        }

        public void Clear()
        {
            JsonSL.DeleteJsonFile(JsonTitle.PlayerEquipItemObjects).Forget();
            itemEquipmentList = new List<EquipItemObject>();
            itemAddOrDeleted = true;
        }

        public void AddItems(List<EquipItemObject> itemEquipments)
        {
            foreach (var item in itemEquipments)
            {
                itemEquipmentList.Add(item);
            }
            var wrapper = new EquipItemObjectListWrapper { items = itemEquipmentList };
            var jsonString = JsonUtility.ToJson(wrapper);
            JsonSL.SaveJson(JsonTitle.PlayerEquipItemObjects, jsonString).Forget();
            DataManager.Instance.playerDataManager.CallFetchAllStatus(true);
            itemAddOrDeleted = true;
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
