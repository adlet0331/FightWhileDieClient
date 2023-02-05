using Data;
using UnityEngine;

namespace UI.Inventory.Enhance
{
    public class EnhanceView : View
    {
        [Header("UI Components")]
        [SerializeField] private IngredientValUI[] ingredientInfoList;
        [SerializeField] private CoinUI playerCoin;
        [SerializeField] private CoinUI afterSpendCoin;
        [SerializeField] private CoinUI spendCoin;
        [SerializeField] private EnhanceTriggerObj enhanceTriggerObj;

        [Header("Status")]
        [SerializeField] private bool selected;

        public bool Selected => selected;
        
        [Header("Components")]
        [SerializeField] private Animator animator;

        private event SlotClickHandler SlotClickHandler;
        public void InitHandler(SlotClickHandler handler)
        {
            SlotClickHandler = handler;
            SlotClickHandler += SlotClicked;
        }

        public void SelectEvent(EquipItemObject itemObject)
        {
            if (selected) return;

            selected = true;
            enhanceTriggerObj.Select(itemObject);
        }

        private void SlotClicked(SlotClickArgs args)
        {
            if (selected)
            {
                selected = false;
                enhanceTriggerObj.Select(null);
                return;
            }
            else
            {
                
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
        }
        protected override void AfterDeActivate()
        {
            
        }
        
        private void Start()
        {
            animator = GetComponent<Animator>();
        }
    }
}