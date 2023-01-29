using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UI.Inventory
{
    public enum UpperViewStatus
    {
        Equip,
        Enhancement,
        Decomposition
    }
    public enum DownViewStatus
    {
        ItemView
    }
    public class InventoryPopup : Popup
    {
        [Header("Status")]
        [SerializeField] private UpperViewStatus upperViewStatus;
        [SerializeField] private DownViewStatus downViewStatus;
        [Header("View Info")]
        [SerializeField] private List<string> upperViewStatusList;
        [SerializeField] private List<string> downViewStatusList;
        private void Awake()
        {
            upperViewStatus = UpperViewStatus.Equip;
            downViewStatus = DownViewStatus.ItemView;

            upperViewStatusList = new List<string>();
            downViewStatusList = new List<string>();

            foreach (var enumVar in Enum.GetValues(typeof(UpperViewStatus)))
            {
                upperViewStatusList.Add(enumVar.ToString());
            }
            foreach (var enumVar in Enum.GetValues(typeof(DownViewStatus)))
            {
                downViewStatusList.Add(enumVar.ToString());
            }
        }
        private void SwitchStatus(UpperViewStatus uViewStatus, DownViewStatus dViewStatus)
        {
            equipView.DeActivate();
            //enhanceView.DeActivate();
            //decompositionView.DeActivate();

            itemView.DeActivate();
            
            switch (uViewStatus)
            {
                case UpperViewStatus.Equip:
                    equipView.OpenWithBindingEvent(ItemObjectClicked);
                    break;
                case UpperViewStatus.Enhancement:
                    enhanceView.Activate();
                    break;
                case UpperViewStatus.Decomposition:
                    decompositionView.Activate();
                    break;
            }
            switch (dViewStatus)
            {
                case DownViewStatus.ItemView:
                    itemView.OpenWithBindingEvent(ItemObjectClicked);
                    break;
            }
        }
        [Header("TopView")]
        [SerializeField] private EquipView equipView;
        [SerializeField] private View enhanceView;
        [SerializeField] private View decompositionView;

        [Header("DownView")]
        [SerializeField] private ItemView itemView;
        

        private void ItemObjectClicked(SlotClickArgs args)
        {
            if (upperViewStatusList.Contains(args.SourceView))
            {
                switch (downViewStatus)
                {
                    case DownViewStatus.ItemView:
                        break;
                }
            }
            else if (downViewStatusList.Contains(args.SourceView))
            {
                switch (upperViewStatus)
                {
                    case UpperViewStatus.Equip:
                        equipView.EquipEvent(args.EquipItemObject);
                        break;
                    case UpperViewStatus.Enhancement:
                        enhanceView.Activate();
                        break;
                    case UpperViewStatus.Decomposition:
                        decompositionView.Activate();
                        break;
                }
            }
            else
            {
                Debug.LogAssertion($"{args.SourceView} is Not Contained in Any View Status");
            }
        }

        public new void Open()
        {
            SwitchStatus(upperViewStatus, downViewStatus);

            base.Open();
        }
    }
}
