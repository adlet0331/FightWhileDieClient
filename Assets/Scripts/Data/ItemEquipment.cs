using System;

namespace Data
{
    [Serializable]
    public class ItemEquipment : AItem
    {
        public EquipmentKind kind;
        public Rare rare;
        public uint level;
        public uint currExp;
    }
}