using System;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class ItemEquipment
    {
        public uint id;
        public Rare rare;
        public uint level;
        public EquipmentOption option;
        public uint optionValue;
    }
}