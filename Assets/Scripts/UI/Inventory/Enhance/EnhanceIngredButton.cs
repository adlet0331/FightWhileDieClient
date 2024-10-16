using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory.Enhance
{
    public class EnhanceIngredButton : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image plusImage;
        [Header("Status")]
        [SerializeField] private int currentRare;
        [SerializeField] private Color currentColor;
        [SerializeField] private bool isSelected;

        public void InitRare(int rare, bool select)
        {
            currentRare = rare;
            currentColor = DataManager.Instance.staticDataManager.RareColorList[rare];
            SelectIngredient(select);
        }
        
        public void SelectIngredient(bool select)
        {
            isSelected = select;
            
            if (isSelected)
            {
                plusImage.color = new Color(0, 0, 0, 0);
                background.color = currentColor;
            }
            else
            {
                background.color = Color.white;
                plusImage.color = Color.black;
            }

        }
    }
}