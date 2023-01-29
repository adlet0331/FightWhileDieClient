using System;
using Data;
using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Image = UnityEngine.UI.Image;

namespace UI.Inventory
{
    public class SlotClickArgs
    {
        public string SourceView;
        public int Index;
        public EquipItemObject EquipItemObject;
    }
    public delegate void SlotClickHandler(SlotClickArgs var);
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("Item Info")]
        [SerializeField] private int index;
        [SerializeField] private bool isSelected;
        [SerializeField] private EquipItemObject equipItemObjectInfo;
        [Header("Components")]
        [SerializeField] private Image itemImage;
        [SerializeField] private Image backGround;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private Image slotBorder;
        [Header("For Event")]
        [SerializeField] private string sourceView;

        public EquipItemObject EquipItemObjectInfo => equipItemObjectInfo;

        public void ChangeIndex(int val)
        {
            index = val;
        }

        public void SetNewItemObject(EquipItemObject itemObject)
        {
            equipItemObjectInfo = itemObject;
            LoadItemUI();
        }
        
        public void Init(int idx, EquipItemObject equipItemObject, SlotClickHandler slotClicked, string sourceViewString)
        {
            index = idx;
            
            equipItemObjectInfo = equipItemObject;
            LoadItemUI();
            
            SlotClicked = slotClicked;
            sourceView = sourceViewString;
            isSelected = false;
        }

        public void Select(bool select)
        {
            isSelected = select;
            if (select)
                backGround.color = new Color(0.7f, 0.7f, 0.7f, 1);
            else
                backGround.color = new Color(0.85f, 0.85f, 0.85f, 1);
        }

        private void LoadItemUI()
        {
            if (equipItemObjectInfo == null) return;
            
            itemImage.sprite = ResourcesLoad.LoadEquipmentSprite(equipItemObjectInfo.rare, equipItemObjectInfo.option);
            level.text = equipItemObjectInfo.level.ToString();
            slotBorder.color = DataManager.Instance.itemManager.RareColorList[equipItemObjectInfo.rare];
        }
        
        private event SlotClickHandler SlotClicked;

        private void Start()
        {
            isSelected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SlotClicked?.Invoke(new SlotClickArgs
            {
                SourceView = sourceView,
                EquipItemObject = equipItemObjectInfo,
                Index = index
            });
        }
    }
}