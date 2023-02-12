using System;
using System.Collections.Generic;
using NonDestroyObject;
using UnityEditor.Presets;
using UnityEngine;

namespace UI.Inventory
{
    public class IngredientBanner : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private bool usePlayerData;
        [SerializeField] private bool hideIfZero;
        [SerializeField] private List<int> ingredientValues;
        
        [Header("Components")]
        [SerializeField] private IngredientValUI[] ingredientInfoList;
        
        public void Init(bool player, bool hide)
        {
            usePlayerData = player;
            hideIfZero = hide;
            
            ClearValues();
            UpdateValues();
        }

        public void ClearSelect()
        {
            for (int i = 1; i <= 6; i++)
            {
                ingredientInfoList[i].SetSelected(false);
            }
        }

        public void SelectRare(int rare)
        {
            for (int i = 1; i <= 6; i++)
            {
                ingredientInfoList[i].SetSelected(i == rare);
            }
        }

        public void ClearValues()
        {
            ingredientValues = new List<int>();
            for (int i = 0; i < 7; i ++) 
                ingredientValues.Add(i);
        }

        public void ApplyChangeValue(bool isPlus, int rare, int val = 1)
        {
            if (usePlayerData)
            {
                Debug.LogAssertion("This Banner use player Data. Do not Use Increase/Decrease Value Function.");
                return;
            }

            ingredientValues[rare] += (isPlus ? 1 : -1) * val;
            UpdateValues();
        }
        
        public void UpdateValues()
        {
            for (int i = 1; i <= 6; i++)
            {
                var color = DataManager.Instance.itemManager.RareColorList[i];
                
                if (usePlayerData)
                {
                    ingredientValues[i] = DataManager.Instance.playerDataManager.EnhanceIngredientList[i];
                }

                if (hideIfZero && ingredientValues[i] == 0)
                {
                    ingredientInfoList[i].gameObject.SetActive(false);
                    continue;
                }
                
                ingredientInfoList[i].gameObject.SetActive(true);
                ingredientInfoList[i].UpdateUI(color, ingredientValues[i]);
            }
        }
    }
}