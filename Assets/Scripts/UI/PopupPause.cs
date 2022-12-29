using NonDestroyObject;

namespace UI
{
    public class PopupPause : Popup
    {
        public void OpenAndPause()
        {
            base.Open();
            CombatManager.Instance.TimeBlocked = true;
        }

        /// <summary>
        /// 계정 초기화 하는 버튼
        /// </summary>
        public void ResetButton()
        {
            DataManager.Instance.PlayerDataManager.DeleteExistingUser();
            Close();
            CombatManager.Instance.TimeBlocked = false;
            UIManager.Instance.ShowPopupEnterYourNickname();
        }
    }
}