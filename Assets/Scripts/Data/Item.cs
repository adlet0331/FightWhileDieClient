using System;

namespace Data
{
    [Serializable]
    // 모든 Item을 가지고 있음.
    // uid와 type으로 type에 맞추어서 클래스로 가져옴.
    public class Item
    {
        public uint Id;
        public ItemType Type;
    }
}