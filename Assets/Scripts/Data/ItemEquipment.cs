using System;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class ItemEquipment
    {
        public uint Id;
        public Rare Rare;
        public uint Level;
        public EquipmentOption Option;
        public uint OptionValue;
    }
}