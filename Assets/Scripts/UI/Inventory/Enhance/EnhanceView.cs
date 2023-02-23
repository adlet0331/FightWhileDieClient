using Data;
using NonDestroyObject;
using TMPro;
using UI.Touch;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = System.Random;

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
        [SerializeField] private CoinUI priceCoinUI;
        [SerializeField] private TextMeshProUGUI currLevelVal;
        [SerializeField] private TextMeshProUGUI nextLevelVal;
        [SerializeField] private TextMeshProUGUI basicProbabilityVal;
        [SerializeField] private TextMeshProUGUI additionalProbabilityVal;
        [SerializeField] private TextMeshProUGUI totalProbabilityVal;
        [SerializeField] private TextMeshProUGUI beforeOptionVal;
        [SerializeField] private TextMeshProUGUI afterOptionVal;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private EnhanceTriggerObj enhanceTriggerObj;
        [SerializeField] private NoResponseTouch noResponseTouchBoard;
        [SerializeField] private GameObject[] disableWhileEnhancing;
        [SerializeField] private EnhanceResultBoard enhanceResultBoard;

        [SerializeField] private GameObject whenItemNotSelectedBoard;
        [SerializeField] private GameObject whenItemSelectedBoard;
        
        
        [Header("Buttons")]
        [SerializeField] private Image startEnhanceButton;
        [SerializeField] private EnhanceIngredButton addIngredientButton;
        
        public void ButtonStartPressed()
        {
            int rare = selectedObject.rare;
            int level = selectedObject.level;

            var enhanceInfo = DataManager.Instance.staticDataManager.GetEnhanceInfo(rare);
            var price = enhanceInfo.coinPerLevelList[level];
            var playerCoin = DataManager.Instance.playerDataManager.Coin;
            
            // Check If There is Enough Coin
            if (price > playerCoin) return;
            
            // Enable Block Touch Event
            noResponseTouchBoard.gameObject.SetActive(true);
            
            // Calculate If Enhance Success Or Fail
            Random random = new Random();
            var successProb = enhanceInfo.probabilityPerlevelList[level];
            var randResult = random.Next(1, 101);
            // 1 ~ 100
            // 1 퍼센트면 1일 때만 성공
            var success = randResult <= successProb;
            enhanceTriggerObj.SetEnhanceResult(success);
            var enhancingTime = enhanceTriggerObj.GetEnhancingTime();

            // Spend Coin
            DataManager.Instance.playerDataManager.SpendCoin(price);
            
            // Fetch Data
            if (success) selectedObject.level += 1;
            else if (selectedObject.level > 1) selectedObject.level -= 1;
            DataManager.Instance.itemManager.UpdateEquipItemObject(selectedObject);
            DataManager.Instance.playerDataManager.CallFetchAllStatus(true);
            
            // UI
            foreach (var disableGameObject in disableWhileEnhancing)
            {
                disableGameObject.SetActive(false);
            }
            beforeOptionVal.text = selectedObject.GetOptionValue().ToString();
            afterOptionVal.text = selectedObject.GetNextOptionValue().ToString();
            descriptionText.text = selectedObject.GetDescriptionText(1);
            
            // Sound
            SoundManager.Instance.PlayClip((int) ClipName.GatchaOpen);

            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(enhancingTime, () =>
            {
                // Sound
                SoundManager.Instance.PlayClip(success ? (int) ClipName.EnhanceSuccess : (int) ClipName.EnhanceFail);
                
                noResponseTouchBoard.gameObject.SetActive(false);
                
                enhanceResultBoard.OpenWithData(success, level, () =>
                {
                    enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.Selected);
                    foreach (var disableGameObject in disableWhileEnhancing)
                    {
                        disableGameObject.SetActive(true);
                    }
                    UpdateAllUI();
                });
            }));
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
        public bool SelectEvent(EquipItemObject itemObject)
        {
            if (itemSelected) return false;

            itemSelected = true;
            selectedObject = itemObject;
            
            SwitchMode(EnhanceViewMode.ItemSelected);
            
            enhanceTriggerObj.Select(itemObject);
            enhanceTriggerObj.SwitchStatus(EnhanceTriggerStatus.Selected);
            
            beforeOptionVal.text = itemObject.GetOptionValue().ToString();
            afterOptionVal.text = itemObject.GetNextOptionValue().ToString();
            descriptionText.text = itemObject.GetDescriptionText(1);
            
            addIngredientButton.InitRare(itemObject.rare, false);

            UpdateAllUI();

            return true;
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
            switch (mode)
            {
                case EnhanceViewMode.ItemNotSelected:
                    animator.SetBool(AnimatorParams.ItemSelectedBool.ToString(), false);
                    whenItemSelectedBoard.SetActive(false);
                    whenItemNotSelectedBoard.SetActive(true);
                    break;
                case EnhanceViewMode.ItemSelected:
                    animator.SetBool(AnimatorParams.ItemSelectedBool.ToString(), true);
                    whenItemSelectedBoard.SetActive(true);
                    whenItemNotSelectedBoard.SetActive(false);
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
                // Check Selected Object Is Still in Inventory
                if (DataManager.Instance.itemManager.GetEquipItemObjectWithId(selectedObject.id) == null)
                {
                    itemSelected = false;
                    selectedObject = null;
                    SwitchMode(EnhanceViewMode.ItemNotSelected);

                    return;
                }

                int rare = selectedObject.rare;
                int level = selectedObject.level;

                var enhanceInfo = DataManager.Instance.staticDataManager.GetEnhanceInfo(rare);
                var price = enhanceInfo.coinPerLevelList[level];
                var basicProbability = enhanceInfo.probabilityPerlevelList[level];
                var additionalProbability = ingredientSelected ? enhanceInfo.addProbabilityPerLevelList[level] : 0;
                
                priceCoinUI.SetCoinValue(price);
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

            UpdateAllUI();
        }
        protected override void AfterDeActivate()
        {
            
        }
    }
}