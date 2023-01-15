using System;
using UnityEngine;

namespace UI.Inventory
{
    public class TopItemSlotButtonClickEventArgs
    {
        public UpperViewStatus ViewStatus;
        public int Index;
    }
    public class DownItemSlotButtonClickEventArgs
    {
        public DownViewStatus ViewStatus;
        public int Index;
    }
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
        [Header("TopView")]
        [SerializeField] private EquipView equipView;
        [SerializeField] private View enhanceView;
        [SerializeField] private View decompositionView;
        [Header("DownView")]
        [SerializeField] private ItemView itemView;

        private void SwitchStatus(UpperViewStatus uViewStatus, DownViewStatus dViewStatus)
        {
            if (upperViewStatus != uViewStatus)
            {
                switch (upperViewStatus)
                {
                    case UpperViewStatus.Equip:
                        equipView.DeActivate();
                        break;
                    case UpperViewStatus.Enhancement:
                        enhanceView.DeActivate();
                        break;
                    case UpperViewStatus.Decomposition:
                        decompositionView.DeActivate();
                        break;
                }
            }
            if (downViewStatus != dViewStatus)
            {
                switch (downViewStatus)
                {
                    case DownViewStatus.ItemView:
                        itemView.DeActivate();
                        break;
                }
            }
            switch (uViewStatus)
            {
                case UpperViewStatus.Equip:
                    equipView.Activate();
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
                    itemView.BindSlotClickHandler(ItemViewSlotClicked);
                    itemView.Activate();
                    break;
            }
        }

        private void Start()
        {
            SwitchStatus(upperViewStatus, downViewStatus);
        }

        private void ItemViewSlotClicked(int index)
        {
            Debug.Log(index);
        }


        public new void Open()
        {
        

            base.Open();
        }
    }
}
