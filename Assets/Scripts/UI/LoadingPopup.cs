using NonDestroyObject;

namespace UI
{
    public class LoadingPopup : Popup
    {
        public new void Open()
        {
            CombatManager.Instance.TimeBlocked = true;
            base.Open();
        }

        public new void Close()
        {
            CombatManager.Instance.TimeBlocked = false;
            base.Close();
        }
    }
}
