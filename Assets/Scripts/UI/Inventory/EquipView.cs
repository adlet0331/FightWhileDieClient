using System.Collections.Generic;
using Data;
using NonDestroyObject;
using UnityEngine;

namespace UI.Inventory
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

        public void OpenWithBindingEvent(SlotClickHandler handler)
        {
            SlotClickHandler = handler;
            SlotClickHandler += SlotClicked;
            Activate();
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
        }

        private void SlotClicked(SlotClickArgs args)
        {
            // Reset Before Clicked
            equipSlots[beforeClicked].Select(false);

            // Handle if Click Same : Cancel Selection
            Debug.Log(args.Index);
            if (beforeClicked == args.Index && selectedBefore)
            {
                equipAvailable = false;
                selectedBefore = false;
                return;
            }

            selectedBefore = true;
            equipSlots[args.Index].Select(true);
            
            equipAvailable = true;
            beforeClicked = args.Index;
        }
    
        protected override void Init()
        {
            beforeClicked = 0;
            equipAvailable = false;
            
            equipItemObjects = new List<EquipItemObject>();
            var equipedItemIdList = DataManager.Instance.playerDataManager.EquipedItem1Id;
            for (int i = 0; i < equipSlots.Count; i++)
            {
                var itemObject = DataManager.Instance.itemManager.GetEquipItemObjectWithId(equipedItemIdList[i]);
                equipItemObjects.Add(itemObject);
                
                equipSlots[i].Init(i, itemObject, SlotClickHandler);
            }
        }
        protected override void Clean()
        {
            
        }
    }
}
