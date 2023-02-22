using NonDestroyObject;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PopupEnterName : Popup
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_Text warning_text;

        public async void OKButton()
        {
            var userName = nameInputField.text;
            if (userName.Length == 0 || userName.Length > 20)
            {
                warning_text.text = "Please Check Nickname's length";
                return;
            }

            warning_text.text = "Sending to Server...";

            var createNewUser = await NetworkManager.Instance.CreateNewUser(userName);
            if (createNewUser == CreateNewUserResult.Success)
            {
                Close();
            }
            else
            {
                DataManager.Instance.playerDataManager.ResetUser(-1, userName);
                Close();
            }
        }
    }
}