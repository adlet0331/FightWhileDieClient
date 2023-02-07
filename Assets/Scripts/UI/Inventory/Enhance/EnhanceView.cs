using System;
using Data;
using UnityEngine;

namespace UI.Inventory.Enhance
{
    
    public class EnhanceView : View
    {
        private enum EnhanceViewMode
        {
            ItemNotSelected,
            ItemSelected,
        }

        private enum AnimatorParams
        {
            ItemSelectedBool
        }
        
        [Header("UI Components")]
        [SerializeField] private IngredientValUI[] ingredientInfoList;
        [SerializeField] private CoinUI playerCoin;
        [SerializeField] private CoinUI afterSpendCoin;
        [SerializeField] private CoinUI spendCoin;
        [SerializeField] private EnhanceTriggerObj enhanceTriggerObj;

        [Header("Status")]
        [SerializeField] private EnhanceViewMode currentMode;
        [SerializeField] private EquipItemObject currentObject;
        [SerializeField] private bool selected;

        public bool Selected => selected;
        
        [Header("Components")]
        [SerializeField] private Animator animator;

        private event SlotClickHandler SlotClickHandler;
        public void InitHandler(SlotClickHandler handler)
        {
            animator.keepAnimatorControllerStateOnDisable = true;
            SlotClickHandler = handler;
            SlotClickHandler += SlotClicked;
        }

        public void SelectEvent(EquipItemObject itemObject)
        {
            if (selected) return;

            SetItem(itemObject);
        }

        private void SlotClicked(SlotClickArgs args)
        {
            if (!selected) return;
            
            HideItem();
        }

        private void SetItem(EquipItemObject itemObject)
        {
            selected = true;
            currentObject = itemObject;
            
            SwitchMode(EnhanceViewMode.ItemSelected);
            
            enhanceTriggerObj.Select(itemObject);
            enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.Selected);
        }

        private void HideItem()
        {
            selected = false;
            currentObject = null;
            
            SwitchMode(EnhanceViewMode.ItemNotSelected);
            
            enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.NotSelected);
            enhanceTriggerObj.Select(null);
        }

        private void SwitchMode(EnhanceViewMode mode)
        {
            currentMode = mode;

            animator.SetBool(AnimatorParams.ItemSelectedBool.ToString(), false);
            
            switch (mode)
            {
                case EnhanceViewMode.ItemNotSelected:
                    animator.SetBool(AnimatorParams.ItemSelectedBool.ToString(), false);
                    break;
                case EnhanceViewMode.ItemSelected:
                    animator.SetBool(AnimatorParams.ItemSelectedBool.ToString(), true);
                    break;
            }
        }

        private void UpdateAllUI()
        {
            for (int i = 1; i <= 6; i++)
            {
                ingredientInfoList[i].UpdateValue();
            }
        }
        protected override void BeforeActivate()
        {
            UpdateAllUI();

            enhanceTriggerObj.InitHandler(SlotClickHandler);
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        protected override void AfterDeActivate()
        {
            
        }
    }
}