using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SliderWithValue : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueText;

        public void InitValue(float val)
        {
            slider.value = val;
            valueText.text = ((int)val).ToString();
        }
    }
}