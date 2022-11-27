using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupEnterName : Popup
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_Text warning_text;

        public void OKButton()
        {
            if (nameInputField.text.Length == 0 || nameInputField.text.Length > 20)
            {
                warning_text.text = "Please Check Nickname's length";
                return;
            }
            var success = NetworkManager.Instance.CreateNewUser(nameInputField.text);
            if (success)
            {
                Close();
            }
        }
    }
}