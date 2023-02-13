using TMPro;
using UnityEngine;

namespace UI
{
    public class SimpleTextPopup : Popup
    {
        [SerializeField] private TextMeshProUGUI textField;
        public void OpenWithText(string str)
        {
            textField.text = str;
            base.Open();
        }
    }
}
