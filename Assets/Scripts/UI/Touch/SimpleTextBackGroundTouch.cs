using NonDestroyObject;

namespace UI.Touch
{
    public class SimpleTextBackGroundTouch : ATouch
    {

        protected override void OnTouch()
        {
            UIManager.Instance.simpleTextPopup.Close();
        }
    }
}