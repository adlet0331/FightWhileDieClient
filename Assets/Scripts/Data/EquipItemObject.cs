using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Data
{
    [Serializable]
    public class EquipItemObject
    {
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
        public int optionValue;
    }
}