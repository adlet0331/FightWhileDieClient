using NonDestroyObject;
using UnityEngine;

namespace UI.Touch
{
    public class BackGroundTouch : ATouch
    {
        protected override void OnTouch()
        {
            UIManager.Instance.HideAllPopup();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                UIManager.Instance.HideAllPopup();
            }
        }
    }
}