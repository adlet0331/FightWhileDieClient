
using NonDestroyObject;

namespace UI.Touch
{
    public class RestartTouch: ATouch
    {
        protected override void OnTouch()
        {
            UIManager.Instance.HideAllPopup();
            CombatManager.Instance.TimeBlocked = false;
        }
    }
}