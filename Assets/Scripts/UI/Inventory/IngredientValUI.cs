using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class IngredientValUI: MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image ingredientColor;
        [SerializeField] private TextMeshProUGUI valueText;

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
                background.color = Color.grey;
            else
                background.color = Color.white;
        }
        
        public void UpdateUI(Color color, int val)
        {
            valueText.text = val.ToString();
            ingredientColor.color = color;
            valueText.color = color;
        }
    }
}