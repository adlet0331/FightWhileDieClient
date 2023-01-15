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
        [SerializeField] private ItemEquipment itemEquipmentInfo;
        [Header("Components")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private Image slotBorder;

        public ItemEquipment ItemEquipmentInfo => itemEquipmentInfo;
        
        private event SlotClickHandler SlotClicked;

        public void IndexChanged(int val)
        {
            index = val;
        }
        
        public void Init(int idx, ItemEquipment itemEquipment, SlotClickHandler slotClicked)
        {
            index = idx;
            
            itemEquipmentInfo = itemEquipment;
            // TODO: Load Image per option
            
            level.text = itemEquipment.level.ToString();
            slotBorder.color = DataManager.Instance.ItemManager.RareColorList[itemEquipment.rare];
            
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