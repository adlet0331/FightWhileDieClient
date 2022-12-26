using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace NonDestroyObject
{
    /// <summary>
    /// Item을 관리하는 싱글톤 매니저입니다
    /// 
    /// </summary>
    public class ItemManager : Singleton<ItemManager>
    {
        [SerializeField] private List<Item> ItemList;
        [SerializeField] private List<ItemEquipment> ItemEquipmentList;
        [SerializeField] private List<ItemEnhancePiece> ItemEnhancePieceList;
        
        
    }
}
