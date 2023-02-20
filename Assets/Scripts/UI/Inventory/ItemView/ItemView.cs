using System;
using System.Collections.Generic;
using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory.ItemView
{
    public enum ItemViewMode
    {
        ItemSlotsWithInfo,
        ItemSlots,
        Hide
    }
    public class ItemView : View
    {
        private enum AnimParams
        {
            ItemInfoView,
            OnlySlots,
            Hide
        }
        
        [Header("Components")]
        [SerializeField] private List<ItemSlot> itemSlotList;
        [SerializeField] private Transform viewPortTransform;
        [SerializeField] private Toggle rareToggle;
        [SerializeField] private Toggle levelToggle;
        [SerializeField] private Toggle optionToggle;
        [SerializeField] private Button ascDescButton;
        [SerializeField] private Animator animator;

        [Header("Current Mode")]
        [SerializeField] private ItemViewMode currentMode;
        public ItemViewMode CurrentMode => currentMode;

        [Header("ItemViewMode Components")]
        [SerializeField] private GameObject itemInfoTransform;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image rareImage;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("EnhancementMode Components")]
        [SerializeField] private GameObject enhanceInfoTransform;
        
        [Header("Status")]
        [SerializeField] private bool isAsc;
        [SerializeField] private int beforeClicked;
        [SerializeField] private bool selectedBefore;
        [SerializeField] private List<int> equipedItemObjUid;

        public void ChangeAscDesc()
        {
            isAsc = !isAsc;
            ascDescButton.transform.Rotate(0, 0, 180);
            Sort();
        }

        private event SlotClickHandler SlotClickHandler;

        public void ChangeMode(ItemViewMode mode)
        {
            foreach (var slot in itemSlotList)
            {
                slot.LoadItemUI();
            }
            
            foreach (AnimParams parameter in Enum.GetValues(typeof(AnimParams)))
            {
                animator.SetBool(parameter.ToString(), false);
            }

            switch (mode)
            {
                case ItemViewMode.ItemSlotsWithInfo:
                    animator.SetBool(AnimParams.ItemInfoView.ToString(), true);
                    break;
                case ItemViewMode.ItemSlots:
                    animator.SetBool(AnimParams.OnlySlots.ToString(), true);
                    break;
                case ItemViewMode.Hide:
                    if (currentMode == ItemViewMode.ItemSlotsWithInfo)
                        animator.SetBool(AnimParams.OnlySlots.ToString(), true);
                    animator.SetBool(AnimParams.Hide.ToString(), true);
                    break;
            }
            
            currentMode = mode;
        }
        
        public void UpdateEquipedItem()
        {
            equipedItemObjUid = DataManager.Instance.playerDataManager.EquipedItemIdList;
            
            foreach (var itemSlot in itemSlotList)
            {
                itemSlot.SetEquiped(EquipNum.None);
                for (int i = 0; i < equipedItemObjUid.Count; i++)
                {
                    if (itemSlot.EquipItemObjectInfo.id == equipedItemObjUid[i])
                    {
                        itemSlot.SetEquiped((EquipNum)(i + 1));
                    }
                }
            }
        }
        
        public void InitHandler(SlotClickHandler handler, ItemViewMode mode)
        {
            SlotClickHandler = handler;
            SlotClickHandler += SlotClicked;

            SetItemDescription(-1);
            ChangeMode(mode);
        }

        private void SlotClicked(SlotClickArgs args)
        {
            itemSlotList[beforeClicked].Select(false);

            // Handle Click Same Block, Cancel Selected.
            if (beforeClicked == args.Index && selectedBefore)
            {
                SetItemDescription(-1);
                selectedBefore = false;
                return;
            }

            selectedBefore = true;
            beforeClicked = args.Index;
            
            itemSlotList[args.Index].Select(true);
            SetItemDescription(args.Index);
        }

        private void SetItemDescription(int index)
        {
            if (index < 0 || index >= itemSlotList.Count)
            {
                nameText.text = "";
                rareImage.color = DataManager.Instance.itemManager.RareColorList[1];
                levelText.text = "";
                descriptionText.text = "";
                return;
            }
            
            var itemInfo = itemSlotList[index].EquipItemObjectInfo;
            nameText.text = itemInfo.GetName(0);
            rareImage.color = DataManager.Instance.itemManager.RareColorList[itemInfo.rare];
            levelText.text = itemInfo.level.ToString();
            descriptionText.text = itemInfo.GetDescriptionText(1);
        }

        protected override void BeforeActivate()
        {
            animator.keepAnimatorControllerStateOnDisable = true;

            beforeClicked = 0;
            selectedBefore = false;

            // Get Items
            equipedItemObjUid = DataManager.Instance.playerDataManager.EquipedItemIdList;
            var itemList = DataManager.Instance.itemManager.ItemEquipments;

            for (var i = viewPortTransform.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(viewPortTransform.transform.GetChild(i).gameObject);
            }
        
            itemSlotList = new List<ItemSlot>();
            var prefab = Resources.Load("Prefabs/UI/Inventory/ItemSlot") as GameObject;
            for (int i = 0; i < itemList.Count; i++)
            {
                GameObject obj = Instantiate(prefab, viewPortTransform);
                obj.GetComponent<ItemSlot>().Init(i, itemList[i], SlotClickHandler, ItemViewMode.ItemSlotsWithInfo.ToString());
                itemSlotList.Add(obj.GetComponent<ItemSlot>());
            }

            UpdateEquipedItem();
            Sort();
        }
        protected override void AfterDeActivate()
        {
            
        }

        public void Sort()
        {
            var rareSort = rareToggle.isOn;
            var levelSort = levelToggle.isOn;
            var optionSort = optionToggle.isOn;

            if (!rareSort && !levelSort && !optionSort) return;

            QuickSort(0, itemSlotList.Count - 1, rareSort, levelSort, optionSort);
        }

        private void ChangeObjectIndex(int small, int large)
        {
            itemSlotList[large].transform.SetSiblingIndex(small);
            itemSlotList[small].transform.SetSiblingIndex(large);
            (itemSlotList[small], itemSlotList[large]) = (itemSlotList[large], itemSlotList[small]);
            itemSlotList[small].ChangeIndex(small);
            itemSlotList[large].ChangeIndex(large);

            if (small == beforeClicked || large == beforeClicked)
                beforeClicked = small + large - beforeClicked;
        }
        private void QuickSort(int minIndex, int maxIndex, bool rareSort, bool levelSort, bool optionSort)
        {
            if (minIndex >= maxIndex)
                return;
        
            var asc = isAsc ? 1 : -1;
        
            var pivot = itemSlotList[minIndex];
            var low = minIndex;
            var large = maxIndex;
            while (low <= large)
            {
                while ((rareSort && 
                        asc * itemSlotList[low].EquipItemObjectInfo.rare < asc * pivot.EquipItemObjectInfo.rare) ||
                       (
                           levelSort &&
                           (!rareSort || itemSlotList[low].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) && 
                           asc * itemSlotList[low].EquipItemObjectInfo.level < asc * pivot.EquipItemObjectInfo.level) ||
                       (
                           optionSort &&
                           (!rareSort || itemSlotList[low].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) && 
                           (!levelSort || itemSlotList[low].EquipItemObjectInfo.level == pivot.EquipItemObjectInfo.level) &&
                           asc * itemSlotList[low].EquipItemObjectInfo.option < asc * pivot.EquipItemObjectInfo.option) ||
                       (
                           !(!rareSort || itemSlotList[low].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) &&
                           !(!levelSort || itemSlotList[low].EquipItemObjectInfo.level == pivot.EquipItemObjectInfo.level) &&
                           !(!optionSort || itemSlotList[low].EquipItemObjectInfo.option == pivot.EquipItemObjectInfo.option) &&
                           asc * itemSlotList[low].EquipItemObjectInfo.id < asc * pivot.EquipItemObjectInfo.id)
                      )
                {
                    low++;
                }
        
                while ((rareSort && 
                        asc * itemSlotList[large].EquipItemObjectInfo.rare > asc * pivot.EquipItemObjectInfo.rare) ||
                       (
                           levelSort &&
                           (!rareSort || itemSlotList[large].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) && 
                           asc * itemSlotList[large].EquipItemObjectInfo.level > asc * pivot.EquipItemObjectInfo.level) ||
                       (
                           optionSort &&
                           (!rareSort || itemSlotList[large].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) && 
                           (!levelSort || itemSlotList[large].EquipItemObjectInfo.level == pivot.EquipItemObjectInfo.level) &&
                           asc * itemSlotList[large].EquipItemObjectInfo.option > asc * pivot.EquipItemObjectInfo.option) ||
                       (
                           !(!rareSort || itemSlotList[low].EquipItemObjectInfo.rare == pivot.EquipItemObjectInfo.rare) &&
                           !(!levelSort || itemSlotList[low].EquipItemObjectInfo.level == pivot.EquipItemObjectInfo.level) &&
                           !(!optionSort || itemSlotList[low].EquipItemObjectInfo.option == pivot.EquipItemObjectInfo.option) &&
                           asc * itemSlotList[low].EquipItemObjectInfo.id > asc * pivot.EquipItemObjectInfo.id)
                      )
                {
                    large--;
                }

                if (low > large)
                    continue;
                ChangeObjectIndex(low, large);
                low++;
                large--;
            }
            if (minIndex < large)
                QuickSort(minIndex, large, rareSort, levelSort, optionSort);
            
            if (low < maxIndex)
                QuickSort(low, maxIndex, rareSort, levelSort, optionSort);
        }
    }
}
