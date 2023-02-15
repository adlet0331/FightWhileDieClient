using System;
using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AutoButton : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image autoText;
        [SerializeField] private Image border;
        [SerializeField] private bool isAuto;

        private void Start()
        {
            isAuto = false;
            AutoManager.Instance.AutoChanged += UpdateUI;
        }

        public void AutoButtonClicked()
        {
            AutoManager.Instance.IsAuto = !AutoManager.Instance.IsAuto;
        }

        public void UpdateUI()
        {
            isAuto = AutoManager.Instance.IsAuto;
                    
            if (isAuto)
            {
                background.color = new Color(1.0f, 1.0f,0.0f,1.0f);
                autoText.color = new Color(0.1f, 0.1f,0.1f,1.0f);
                border.color = new Color(1.0f, 0.0f,0.0f,1.0f);
            }
            else
            {
                background.color = new Color(1.0f, 1.0f,1.0f,1.0f);
                autoText.color = new Color(0.0f, 0.0f,0.0f,1.0f);
                border.color = new Color(0.0f, 0.0f,0.0f,1.0f);
            }
        }
    }
}