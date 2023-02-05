using NonDestroyObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class IngredientValUI: MonoBehaviour
    {
        [Header("Need To Be Init")]
        [SerializeField] private int rare;
        
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image ingredientColor;
        [SerializeField] private TextMeshProUGUI valueText;

        public void SetSelected(bool selected)
        {
            if (selected)
                background.color = Color.grey;
            else
                background.color = Color.white;
        }
        
        public void UpdateValue()
        {
            var val = DataManager.Instance.playerDataManager.EnhanceIngredientList[rare];
            var color = DataManager.Instance.itemManager.RareColorList[rare];
            
            valueText.text = val.ToString();
            ingredientColor.color = color;
            valueText.color = color;
        }
    }
}