using TMPro;
using UnityEngine;

namespace UI
{
    public class ValueText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selfText;

        public void SetValue(float val)
        {
            selfText.text = ((int)val).ToString();
        }
    }
}