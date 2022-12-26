using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace NonDestroyObject.Data
{
    
    [Serializable]
    public class ItemManager
    {
        [SerializeField] private List<Item> ItemList;
        [SerializeField] private List<ItemEquipment> ItemEquipmentList;
        [SerializeField] private List<ItemEnhancePiece> ItemEnhancePieceList;
        
        
    }
}
