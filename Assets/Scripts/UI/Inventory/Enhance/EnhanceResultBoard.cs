using TMPro;
using UI.Touch;
using UnityEngine;

namespace UI.Inventory.Enhance
{
    public delegate void AfterEnhanceResultBoardTouched();
    public class EnhanceResultBoard : ATouch
    {
        [Header("Components")]
        [SerializeField] private GameObject[] successFailText;
        [SerializeField] private TextMeshProUGUI leftLevelText;
        [SerializeField] private TextMeshProUGUI resultLevelText;

        private event AfterEnhanceResultBoardTouched afterEnhanceResultBoardTouched;
        
        public void OpenWithData(bool success, int leftLevel, AfterEnhanceResultBoardTouched afterTouched)
        {
            successFailText[0].SetActive(success);
            successFailText[1].SetActive(!success);

            leftLevelText.text = leftLevel.ToString();
            resultLevelText.text = (leftLevel + (success ? 1 : -1)).ToString();

            afterEnhanceResultBoardTouched = afterTouched;
            
            gameObject.SetActive(true);
        }
        
        protected override void OnTouch()
        {
            gameObject.SetActive(false);
            afterEnhanceResultBoardTouched?.Invoke();
        }
    }
}