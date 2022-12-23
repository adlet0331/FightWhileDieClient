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
    }
}