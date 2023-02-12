using System;
using Data;
using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private IngredientBanner ingredientBanner;
        [SerializeField] private CoinUI playerCoinUI;
        [SerializeField] private CoinUI afterUseCoinUI;
        [SerializeField] private CoinUI priceCoinUI;
        [SerializeField] private TextMeshProUGUI currLevelVal;
        [SerializeField] private TextMeshProUGUI nextLevelVal;
        [SerializeField] private EnhanceTriggerObj enhanceTriggerObj;

        [Header("Buttons")]
        [SerializeField] private Image StartEnhanceButton;
        [SerializeField] private EnhanceIngredButton enhanceIngredButton;

        public void ButtonStartPressed()
        {
            
        }

        public void ButtonIngredientPressed()
        {
            
        }

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
            ingredientBanner.UpdateValues();
            
            var playerCoin = DataManager.Instance.playerDataManager.Coin;
            var price = DataManager.Instance.playerDataManager.GatchaCosts;
            playerCoinUI.SetCoinValue(playerCoin);
            priceCoinUI.SetCoinValue(price);
            afterUseCoinUI.SetCoinValue(playerCoin - price);
            if (playerCoin < price)
            {
                StartEnhanceButton.color = Color.gray;
            }
            else
            {
                StartEnhanceButton.color = Color.white;
            }
        }
        protected override void BeforeActivate()
        {
            ingredientBanner.Init(true, false);

            enhanceTriggerObj.InitHandler(SlotClickHandler);
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        protected override void AfterDeActivate()
        {
            
        }
    }
}