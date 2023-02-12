using System;
using Cysharp.Threading.Tasks;
using Data;
using NonDestroyObject;
using TMPro;
using UI.Touch;
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
        [SerializeField] private TextMeshProUGUI basicProbabilityVal;
        [SerializeField] private TextMeshProUGUI additionalProbabilityVal;
        [SerializeField] private TextMeshProUGUI totalProbabilityVal;
        [SerializeField] private EnhanceTriggerObj enhanceTriggerObj;
        [SerializeField] private NoResponseTouch noResponseTouchBoard;
        
        [Header("Buttons")]
        [SerializeField] private Image startEnhanceButton;
        [SerializeField] private EnhanceIngredButton addIngredientButton;


        public void ButtonStartPressed()
        {
            ButtonStartPressedAsync().Forget();
        }

        public async UniTaskVoid ButtonStartPressedAsync()
        {
            // Enable Block Touch Event
            noResponseTouchBoard.gameObject.SetActive(true);
            
            

            UniTask.SwitchToMainThread();
            noResponseTouchBoard.gameObject.SetActive(false);
        }

        public void ButtonIngredientPressed()
        {
            if (!itemSelected) return;

            // 이미 선택 되어 있을 때
            if (ingredientSelected)
            {
                ingredientSelected = false;
                addIngredientButton.SelectIngredient(false);
            }
            // 선택 안 되어 있을 때
            else
            {
                if (DataManager.Instance.playerDataManager.EnhanceIngredientList[selectedObject.rare] <= 0) return;

                ingredientSelected = true;
                addIngredientButton.SelectIngredient(true);
            }
            
            UpdateAllUI();
        }

        [Header("Status")]
        [SerializeField] private EnhanceViewMode currentMode;
        [SerializeField] private EquipItemObject selectedObject;
        [SerializeField] private bool itemSelected;
        [SerializeField] private bool ingredientSelected;

        public bool ItemSelected => itemSelected;
        
        [Header("Components")]
        [SerializeField] private Animator animator;

        private event SlotClickHandler SlotClickHandler;
        public void InitHandler(SlotClickHandler handler)
        {
            animator.keepAnimatorControllerStateOnDisable = true;
            SlotClickHandler = handler;
            SlotClickHandler += SlotClicked;
        }

        // Item View에서 눌렸을 때
        // SetItem
        public void SelectEvent(EquipItemObject itemObject)
        {
            if (itemSelected) return;

            itemSelected = true;
            selectedObject = itemObject;
            
            SwitchMode(EnhanceViewMode.ItemSelected);
            
            enhanceTriggerObj.Select(itemObject);
            enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.Selected);
            
            addIngredientButton.InitRare(itemObject.rare, false);

            UpdateAllUI();
        }

        // 현재 View에서 눌렸을 때
        // Cancel
        private void SlotClicked(SlotClickArgs args)
        {
            if (!itemSelected) return;
            
            itemSelected = false;
            selectedObject = null;
            
            SwitchMode(EnhanceViewMode.ItemNotSelected);
            
            enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.NotSelected);
            enhanceTriggerObj.Select(null);
            
            addIngredientButton.SelectIngredient(false);

            UpdateAllUI();
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
            playerCoinUI.SetCoinValue(playerCoin);

            if (itemSelected)
            {
                int rare = selectedObject.rare;
                int level = selectedObject.level;

                var enhanceInfo = DataManager.Instance.staticDataManager.GetEnhanceInfo(rare);
                var price = enhanceInfo.coinPerLevelList[level];
                var basicProbability = enhanceInfo.probabilityPerlevelList[level];
                var additionalProbability = ingredientSelected ? enhanceInfo.addProbabilityPerLevelList[level] : 0;
                
                priceCoinUI.SetCoinValue(price);
                afterUseCoinUI.SetCoinValue(playerCoin - price);
                if (playerCoin < price)
                {
                    startEnhanceButton.color = Color.gray;
                }
                else
                {
                    startEnhanceButton.color = Color.white;
                }

                currLevelVal.text = level.ToString();
                nextLevelVal.text = (level + 1).ToString();

                basicProbabilityVal.text = basicProbability.ToString();
                additionalProbabilityVal.text = additionalProbability.ToString();
                totalProbabilityVal.text = (basicProbability + additionalProbability).ToString();
                
                ingredientBanner.SelectRare(rare);
            }
            else
            {
                ingredientBanner.ClearSelect();
            }
        }
        protected override void BeforeActivate()
        {
            noResponseTouchBoard.gameObject.SetActive(false);
            
            ingredientBanner.Init(true, false);
            addIngredientButton.InitRare(6, false);

            enhanceTriggerObj.InitHandler(SlotClickHandler);
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        protected override void AfterDeActivate()
        {
            
        }
    }
}