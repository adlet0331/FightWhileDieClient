using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupPause : Popup
    {
        [SerializeField] private Image resetButton;
        
        public void OpenAndPause()
        {
            base.Open();
            CombatManager.Instance.TimeBlocked = true;

            if (CombatManager.Instance.isInCombat)
            {
                resetButton.color = Color.gray;
            }
            else
            {
                resetButton.color = Color.white;
            }
        }

        /// <summary>
        /// 계정 초기화 하는 버튼
        /// </summary>
        public void ResetButton()
        {
            if (CombatManager.Instance.isInCombat)
            {
                UIManager.Instance.simpleTextPopup.OpenWithText("Reset Is Not Avaliable During Combat.");
                return;
            }
            
            DataManager.Instance.DeleteUser();
            Close();
            CombatManager.Instance.TimeBlocked = false;
            UIManager.Instance.ShowPopupEnterYourNickname();
        }
    }
}