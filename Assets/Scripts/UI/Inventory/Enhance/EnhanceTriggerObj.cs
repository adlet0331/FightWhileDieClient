using Data;
using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory.Enhance
{
    public enum EnhanceTriggerStatus
    {
        NotSelected,
        Selected,
        Enhancing,
        Result
    }
    public class EnhanceTriggerObj: MonoBehaviour
    {
        private enum AnimParams
        {
            SelectedBool,
            EnhancingBool,
            EnhanceEndBool
        }
        
        [Header("Components")]
        [SerializeField] private Image itemBorder;
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private Animator animator;

        [Header("States")]
        [SerializeField] private bool initialized;
        [SerializeField] private EnhanceTriggerStatus currentStatus;

        private event SlotClickHandler SlotClickHandler;

        public void SwitchStatus(EnhanceTriggerStatus status)
        {
            currentStatus = status;

            animator.SetBool(AnimParams.SelectedBool.ToString(), false);
            animator.SetBool(AnimParams.EnhancingBool.ToString(), false);
            animator.SetBool(AnimParams.EnhanceEndBool.ToString(), false);

            switch (status)
            {
                case EnhanceTriggerStatus.NotSelected:
                    animator.SetBool(AnimParams.SelectedBool.ToString(), false);
                    break;
                case EnhanceTriggerStatus.Selected:
                    animator.SetBool(AnimParams.SelectedBool.ToString(), true);
                    break;
                case EnhanceTriggerStatus.Enhancing:
                    animator.SetBool(AnimParams.EnhancingBool.ToString(), true);
                    break;
                case EnhanceTriggerStatus.Result:
                    animator.SetBool(AnimParams.EnhanceEndBool.ToString(), true);
                    break;
            }
        }

        public void Select(EquipItemObject itemObject)
        {
            if (itemObject == null)
                itemBorder.color = Color.white;
            else
                itemBorder.color = DataManager.Instance.itemManager.RareColorList[itemObject.rare];
            
            itemSlot.UpdateItemObjectAndMode(itemObject, ItemSlotMode.OnlyItem);
        }

        public void InitHandler(SlotClickHandler clickHandler)
        {
            if (initialized) return;
            
            SlotClickHandler = clickHandler;
            animator.keepAnimatorControllerStateOnDisable = true;

            itemSlot.Init(0, null, clickHandler, UpperViewStatus.Enhancement.ToString(), ItemSlotMode.OnlyItem);

            initialized = true;
        }
    }
}