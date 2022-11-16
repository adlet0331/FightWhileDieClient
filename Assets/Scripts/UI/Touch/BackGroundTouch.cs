using NonDestroyObject;

namespace UI.Touch
{
    public class BackGroundTouch : ATouch
    {
        protected override void OnTouch()
        {
            UIManager.Instance.HideAllPopup();
        }
    }
}