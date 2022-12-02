using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class CoinEffect : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image CoinImage;
        [SerializeField] private TextMeshProUGUI PlusText;
        [SerializeField] private TextMeshProUGUI CoinValText;
        [SerializeField] private Transform _initTransform;
        [Header("Updating")] 
        [SerializeField] private float movingInterval;
        [SerializeField] private bool updating;
        [SerializeField] private float updateTotalTime;
        [SerializeField] private float updateTime;

        private void FixedUpdate()
        {
            if (!updating) return;
            
            gameObject.transform.localPosition += Vector3.up * movingInterval;
            CoinImage.color = new Color(CoinImage.color.r, CoinImage.color.g, CoinImage.color.b, updateTime / updateTotalTime);
            PlusText.color = new Color(PlusText.color.r, PlusText.color.g, PlusText.color.b, updateTime / updateTotalTime);
            CoinValText.color = new Color(CoinValText.color.r, CoinValText.color.g, CoinValText.color.b, updateTime / updateTotalTime);

            updateTime -= Time.deltaTime * 0.5f;

            if (updateTime < 0f)
            {
                updateTime = 0f;
                updating = false;
                CoinImage.color = new Color(CoinImage.color.r, CoinImage.color.g, CoinImage.color.b, updateTime / updateTotalTime);
                PlusText.color = new Color(PlusText.color.r, PlusText.color.g, PlusText.color.b, updateTime / updateTotalTime);
                CoinValText.color = new Color(CoinValText.color.r, CoinValText.color.g, CoinValText.color.b, updateTime / updateTotalTime);
                gameObject.SetActive(false);
                gameObject.transform.position = _initTransform.position;
            }
        }

        public void ShowCoinEffect(int coinval, float time)
        {
            gameObject.SetActive(true);
            CoinValText.text = coinval.ToString();
            updating = true;
            updateTime = time;
            updateTotalTime = time;
        }
    }
}