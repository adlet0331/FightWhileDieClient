using Data;
using TMPro;
using UnityEngine;

namespace UI.Inventory.Equip
{
    public class EquipSlot : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private GameObject description;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject selected;

        [Header("Status")]
        [SerializeField] private bool isSelected;

        public void SetNewItem(EquipItemObject newItem)
        {
            itemSlot.UpdateItemObjectAndMode(newItem, ItemSlotMode.ItemSlotView);
            if (newItem != null)
                descriptionText.text = newItem.GetDescriptionText(1);
            else
                descriptionText.text = "No Ability Applied";
        }
        
        public void Init(int idx, EquipItemObject equipItemObject, SlotClickHandler slotClicked, ItemSlotMode mode)
        {
            isSelected = false;
            itemSlot.Init(idx, equipItemObject, slotClicked, UpperViewStatus.Equip.ToString(), mode);
            SetNewItem(equipItemObject);
        }

        public void Select(bool select)
        {
            isSelected = select;
            
            itemSlot.Select(select);
            description.SetActive(!select);
            selected.SetActive(select);
        }
    }
}