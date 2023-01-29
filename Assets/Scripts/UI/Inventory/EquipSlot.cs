﻿using Data;
using TMPro;
using UnityEngine;

namespace UI.Inventory
{
    public class EquipSlot : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private GameObject description;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject selected;

        [Header("Debugging")]
        [SerializeField] private bool isSelected;

        public void SetNewItem(EquipItemObject newItem)
        {
            itemSlot.SetNewItemObject(newItem);
            descriptionText.text = newItem.GetDescriptionText(1);
        }
        
        public void Init(int idx, EquipItemObject equipItemObject, SlotClickHandler slotClicked)
        {
            isSelected = false;
            itemSlot.Init(idx, equipItemObject, slotClicked, UpperViewStatus.Equip.ToString());
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