using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Data
{
    [Serializable]
    public class ItemEquipment
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
        [SerializeField] private int level;
        public EquipmentOption Option;
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