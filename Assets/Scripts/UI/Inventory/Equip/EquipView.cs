using System.Collections.Generic;
using Data;
using NonDestroyObject;
using UnityEngine;

namespace UI.Inventory.Equip
{
    public class EquipView : View
    {
        [Header("Components")]
        [SerializeField] private List<EquipSlot> equipSlots;
        [Header("Debugging")]
        [SerializeField] private List<EquipItemObject> equipItemObjects;
        [SerializeField] private bool equipAvailable;
        [SerializeField] private int beforeClicked;
        [SerializeField] private bool selectedBefore;

        private event SlotClickHandler SlotClickHandler;

        public void Init(SlotClickHandler handler)
        {
            SlotClickHandler = SlotClicked;
            SlotClickHandler += handler;
        }
        
        public void EquipEvent(EquipItemObject itemObject)
        {
            if (!equipAvailable) return;
            
            foreach (var equipItemObject in equipItemObjects)
            {
                if (equipItemObject?.id == itemObject.id) return;
            }

            equipItemObjects[beforeClicked] = itemObject;
            equipSlots[beforeClicked].SetNewItem(itemObject);
            equipSlots[beforeClicked].Select(false);

            equipAvailable = false;
            selectedBefore = false;
            
            // Update Datas Logic
            DataManager.Instance.playerDataManager.UpdateEquipItem(beforeClicked, itemObject.id);
        }

        private void SlotClicked(SlotClickArgs args)
        {
            // Reset Before Clicked
            equipSlots[beforeClicked].Select(false);

            // Handle if Click Same : UnEquip Slot
            if (beforeClicked == args.Index && selectedBefore)
            {
                equipAvailable = false;
                selectedBefore = false;
                
                equipItemObjects[beforeClicked] = null;
                equipSlots[beforeClicked].SetNewItem(null);
                // Update Datas Logic
                DataManager.Instance.playerDataManager.UpdateEquipItem(beforeClicked, -1);
                return;
            }

            selectedBefore = true;
            equipSlots[args.Index].Select(true);
            
            equipAvailable = true;
            beforeClicked = args.Index;
        }

        protected override void BeforeActivate()
        {
            beforeClicked = 0;
            equipAvailable = false;
            
            equipItemObjects = new List<EquipItemObject>();
            var equipedItemIdList = DataManager.Instance.playerDataManager.EquipedItemIdList;
            for (int i = 0; i < equipSlots.Count; i++)
            {
                var itemObject = DataManager.Instance.itemManager.GetEquipItemObjectWithId(equipedItemIdList[i]);
                equipItemObjects.Add(itemObject);

                equipSlots[i].Init(i, itemObject, SlotClickHandler, itemObject != null ? ItemSlotMode.ItemSlotView : ItemSlotMode.HideAll);
            }
        }
        protected override void AfterDeActivate()
        {
            
        }
    }
}
