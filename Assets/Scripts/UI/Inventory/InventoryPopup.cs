using System;
using System.Collections.Generic;
using Data;
using UI.Inventory.Decompose;
using UI.Inventory.Enhance;
using UI.Inventory.Equip;
using UI.Inventory.ItemView;
using UnityEngine;

namespace UI.Inventory
{
    public enum UpperViewStatus
    {
        Equip,
        Enhancement,
        Decomposition
    }
    public class InventoryPopup : Popup
    {
        [Header("Status")]
        [SerializeField] private UpperViewStatus upperViewStatus;
        [SerializeField] private ItemViewMode downViewStatus;
        [SerializeField] private bool initialized;
        [Header("View Info")]
        [SerializeField] private List<string> upperViewStatusList;
        [SerializeField] private List<string> downViewStatusList;
        [Header("TopView")]
        [SerializeField] private EquipView equipView;
        [SerializeField] private EnhanceView enhanceView;
        [SerializeField] private DecomposeView decomposeView;

        [Header("DownView")]
        [SerializeField] private ItemView.ItemView itemView;

        public void OpenEquipView()
        {
            SwitchStatus(UpperViewStatus.Equip,  ItemViewMode.ItemSlotsWithInfo);
        }

        public void OpenEnhanceView()
        {
            SwitchStatus(UpperViewStatus.Enhancement, enhanceView.ItemSelected ? ItemViewMode.Hide : ItemViewMode.ItemSlots);
        }

        public void OpenDecomposeView()
        {
            SwitchStatus(UpperViewStatus.Decomposition, ItemViewMode.ItemSlotsWithInfo);
        }
        
        private void Awake()
        {
            upperViewStatusList = new List<string>();
            downViewStatusList = new List<string>();

            foreach (var enumVar in Enum.GetValues(typeof(UpperViewStatus)))
            {
                upperViewStatusList.Add(enumVar.ToString());
            }

            foreach (var enumVar in Enum.GetValues(typeof(ItemViewMode)))
            {
                downViewStatusList.Add(enumVar.ToString());
            }
        }
        private void SwitchStatus(UpperViewStatus uViewStatus, ItemViewMode dViewStatus)
        {
             downViewStatus = dViewStatus;
             upperViewStatus = uViewStatus;

            equipView.DeActivate();
            enhanceView.DeActivate();
            decomposeView.DeActivate();

            switch (uViewStatus)
            {
                case UpperViewStatus.Equip:
                    equipView.Activate();
                    break;
                case UpperViewStatus.Enhancement:
                    enhanceView.Activate();
                    break;
                case UpperViewStatus.Decomposition:
                    decomposeView.Activate();
                    break;
            }
            
            itemView.ChangeMode(downViewStatus);
        }

        private void ItemSlotClicked(SlotClickArgs args)
        {
            // 위의 View 터치 처리
            if (upperViewStatusList.Contains(args.SourceView))
            {
                switch (upperViewStatus)
                {
                    // 장착 해제 처리
                    case UpperViewStatus.Equip:
                        itemView.UpdateEquipedItem();
                        break;
                    // 강화 할 아이템 변경 이벤트
                    case UpperViewStatus.Enhancement:
                        // 아이템이 선택이 안되어있으면 터치 X
                        if (!enhanceView.ItemSelected) return;
                        
                        SwitchStatus(UpperViewStatus.Enhancement, ItemViewMode.ItemSlots);
                        break;
                    case UpperViewStatus.Decomposition:
                        break;
                }
            }
            // 아래 View 터치 처리
            else if (downViewStatusList.Contains(args.SourceView))
            {
                switch (upperViewStatus)
                {
                    case UpperViewStatus.Equip:
                        equipView.EquipEvent(args.EquipItemObject);
                        itemView.UpdateEquipedItem();
                        break;
                    case UpperViewStatus.Enhancement:
                        enhanceView.SelectEvent(args.EquipItemObject);
                        SwitchStatus(UpperViewStatus.Enhancement, ItemViewMode.Hide);
                        break;
                    case UpperViewStatus.Decomposition:
                        
                        break;
                }
            }
            else
            {
                Debug.LogAssertion(args.SourceView);
            }
        }

        public new void Open()
        {
            base.Open();

            if (!initialized)
            {
                equipView.Init(ItemSlotClicked);

                enhanceView.InitHandler(ItemSlotClicked);
            
                itemView.InitHandler(ItemSlotClicked, ItemViewMode.ItemSlotsWithInfo);
                itemView.Activate();

                initialized = true;
            }
            
            SwitchStatus(upperViewStatus, downViewStatus);
        }
    }
}
