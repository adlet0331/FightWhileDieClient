using System;
using Data;
using NonDestroyObject.DataManage;
using UnityEngine;

namespace UI.Inventory.Enhance
{
    public class EnhanceTriggerObj: MonoBehaviour
    {
        private enum AnimatorParams
        {
            SelectedBool,
            EnhancingBool
        }
        
        [Header("Components")]
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private Animator animator;

        [Header("States")]
        [SerializeField] private bool selected;
        
        
        private event SlotClickHandler SlotClickHandler;

        public void Select(EquipItemObject itemObject)
        {
            itemSlot.SetNewItemObject(itemObject);
        }

        public void InitHandler(SlotClickHandler clickHandler)
        {
            SlotClickHandler = clickHandler;
            SlotClickHandler += SlotClicked;
            
            itemSlot.Init(0, null, clickHandler, UpperViewStatus.Enhancement.ToString());
        }
        private void SlotClicked(SlotClickArgs args)
        {
            
        }
    }
}