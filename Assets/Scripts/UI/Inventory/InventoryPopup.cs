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
        [Header("View Info")]
        [SerializeField] private List<string> upperViewStatusList;
        [Header("TopView")]
        [SerializeField] private EquipView equipView;
        [SerializeField] private EnhanceView enhanceView;
        [SerializeField] private DecomposeView decomposeView;

        [Header("DownView")]
        [SerializeField] private ItemView.ItemView itemView;

        public void OpenEquipView()
        {
            SwitchStatus(UpperViewStatus.Equip, ItemViewMode.WithItemInfoRow3);
        }

        public void OpenEnhanceView()
        {
            SwitchStatus(UpperViewStatus.Enhancement, ItemViewMode.OnlyItemSlotsRow3);
        }
        
        private void Awake()
        {
            upperViewStatus = UpperViewStatus.Equip;
            downViewStatus = ItemViewMode.WithItemInfoRow3;

            upperViewStatusList = new List<string>();

            foreach (var enumVar in Enum.GetValues(typeof(UpperViewStatus)))
            {
                upperViewStatusList.Add(enumVar.ToString());
            }
        }
        private void SwitchStatus(UpperViewStatus uViewStatus, ItemViewMode dViewStatus)
        {
            upperViewStatus = uViewStatus;
            downViewStatus = dViewStatus;
            
            equipView.DeActivate();
            enhanceView.DeActivate();
            //decompositionView.DeActivate();

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
            
            itemView.ChangeMode(dViewStatus);
        }

        private void ItemSlotClicked(SlotClickArgs args)
        {
            Debug.Log("Popup Clicked");
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
                        break;
                    case UpperViewStatus.Decomposition:
                        break;
                }
            }
            // 아래 View 터치 처리
            else
            {
                switch (upperViewStatus)
                {
                    case UpperViewStatus.Equip:
                        equipView.EquipEvent(args.EquipItemObject);
                        itemView.UpdateEquipedItem();
                        break;
                    case UpperViewStatus.Enhancement:
                        enhanceView.SelectEvent(args.EquipItemObject);
                        break;
                    case UpperViewStatus.Decomposition:
                        
                        break;
                }
            }
        }

        public new void Open()
        {
            equipView.Init(ItemSlotClicked);

            enhanceView.InitHandler(ItemSlotClicked);
            
            itemView.InitHandler(ItemSlotClicked, ItemViewMode.WithItemInfoRow3);
            itemView.Activate();

            upperViewStatus = UpperViewStatus.Equip;
            downViewStatus = ItemViewMode.WithItemInfoRow3;
            
            SwitchStatus(upperViewStatus, downViewStatus);
            
            base.Open();
        }
    }
}
