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
    public enum EquipNum
    {
        None = 0,
        One = 1,
        Two = 2,
    }
    public enum ItemSlotMode
    {
        HideAll,
        OnlyFrame,
        OnlyItem,
        ItemSlotView,
        ItemSlotViewSelected,
    }
    public delegate void SlotClickHandler(SlotClickArgs var);
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("Infos")]
        [SerializeField] private ItemSlotMode currentMode;
        [SerializeField] private int index;
        [SerializeField] private bool isSelected;
        [SerializeField] private EquipItemObject equipItemObjectInfo;
        [Header("Components")]
        [SerializeField] private Image itemImage;
        [SerializeField] private Image backGround;
        [SerializeField] private GameObject levelObject;
        [SerializeField] private TextMeshProUGUI level;
        [SerializeField] private Image slotBorder;
        [SerializeField] private TextMeshProUGUI equiped;
        [Header("For Event")]
        [SerializeField] private string sourceView;

        public EquipItemObject EquipItemObjectInfo => equipItemObjectInfo;

        public void ChangeIndex(int val)
        {
            index = val;
        }

        public void SetEquiped(EquipNum equipNum)
        {
            if (equipNum == EquipNum.None)
            {
                equiped.text = "";
            }
            else
            {
                equiped.text = ((int) equipNum).ToString();
            }
        }

        public void UpdateItemObjectAndMode(EquipItemObject itemObject, ItemSlotMode mode)
        {
            equipItemObjectInfo = itemObject;
            currentMode = mode;
            LoadItemUI();
        }
        
        public void Init(int idx, EquipItemObject equipItemObject, SlotClickHandler slotClicked, string sourceViewString, ItemSlotMode mode = ItemSlotMode.ItemSlotView)
        {
            index = idx;
            
            UpdateItemObjectAndMode(equipItemObject, mode);

            SlotClicked = slotClicked;
            sourceView = sourceViewString;
            isSelected = false;
        }

        public void Select(bool select)
        {
            isSelected = select;
            UpdateItemObjectAndMode(equipItemObjectInfo, select ? ItemSlotMode.ItemSlotViewSelected : ItemSlotMode.ItemSlotView);
        }

        private void LoadItemUI()
        {
            itemImage.gameObject.SetActive(false);
            backGround.gameObject.SetActive(false);
            slotBorder.gameObject.SetActive(false);
            levelObject.SetActive(false);
            equiped.gameObject.SetActive(false);

            bool isNull = equipItemObjectInfo == null;
            int lev = equipItemObjectInfo?.level ?? 0; 
            int rare = equipItemObjectInfo?.rare ?? 1;
            int option = equipItemObjectInfo?.option ?? 0;
            
            switch (currentMode)
            {
                case ItemSlotMode.HideAll:
                    break;
                
                case ItemSlotMode.OnlyFrame:
                    slotBorder.gameObject.SetActive(true);
                    slotBorder.color = DataManager.Instance.itemManager.RareColorList[rare];

                    break;

                case ItemSlotMode.OnlyItem:
                    itemImage.gameObject.SetActive(true);
                    Debug.Log(isNull);
                    itemImage.sprite = !isNull ? ResourcesLoad.LoadEquipmentSprite(rare, option) : null;
                    itemImage.color = !isNull ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);
                    break;
                
                case ItemSlotMode.ItemSlotView:
                    itemImage.gameObject.SetActive(true);
                    itemImage.sprite = !isNull ? ResourcesLoad.LoadEquipmentSprite(rare, option) : null;
                    itemImage.color = !isNull ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);
                    
                    backGround.gameObject.SetActive(true);
                    backGround.color = new Color(0.85f, 0.85f, 0.85f, 1);
                    
                    levelObject.SetActive(true);
                    level.text = !isNull ? lev.ToString() : "";
                    
                    slotBorder.gameObject.SetActive(true);
                    slotBorder.color = DataManager.Instance.itemManager.RareColorList[rare];
                    
                    equiped.gameObject.SetActive(true);
                    break;
                
                case ItemSlotMode.ItemSlotViewSelected:
                    itemImage.gameObject.SetActive(true);   
                    itemImage.sprite = !isNull ? ResourcesLoad.LoadEquipmentSprite(rare, option) : null;
                    itemImage.color = !isNull ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);
                    
                    backGround.gameObject.SetActive(true);
                    backGround.color = new Color(0.7f, 0.7f, 0.7f, 1);
                    
                    levelObject.SetActive(true);
                    level.text = lev != 0 ? lev.ToString() : "";
                    
                    slotBorder.gameObject.SetActive(true);
                    slotBorder.color = DataManager.Instance.itemManager.RareColorList[rare];
                    
                    equiped.gameObject.SetActive(true);
                    break;
            }
        }
        
        private event SlotClickHandler SlotClicked;

        private void Start()
        {
            isSelected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlayClip(0);
            SlotClicked?.Invoke(new SlotClickArgs
            {
                SourceView = sourceView,
                EquipItemObject = equipItemObjectInfo,
                Index = index
            });
        }
    }
}