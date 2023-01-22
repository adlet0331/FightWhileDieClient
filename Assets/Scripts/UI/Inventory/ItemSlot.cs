using Data;
using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace UI.Inventory
{
    public delegate void SlotClickHandler(int var);
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("Item Info")]
        [SerializeField] private int index;
        [SerializeField] private EquipItemObject equipItemObjectInfo;
        [Header("Components")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private Image slotBorder;

        public EquipItemObject EquipItemObjectInfo => equipItemObjectInfo;
        
        private event SlotClickHandler SlotClicked;

        public void IndexChanged(int val)
        {
            index = val;
        }
        
        public void Init(int idx, EquipItemObject equipItemObject, SlotClickHandler slotClicked)
        {
            index = idx;
            
            equipItemObjectInfo = equipItemObject;
            // TODO: Load Image per option
            
            level.text = equipItemObject.level.ToString();
            slotBorder.color = DataManager.Instance.itemManager.RareColorList[equipItemObject.rare];
            
            SlotClicked = slotClicked;
        }

        private void Clicked()
        {
            SlotClicked?.Invoke(index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked();
        }
    }
}