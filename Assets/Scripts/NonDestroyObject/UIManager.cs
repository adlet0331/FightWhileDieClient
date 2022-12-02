using System;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NonDestroyObject
{
    public enum UIStatus
    {
        Idle = 0,
        Starting = 1,
        Fighting = 2,
        Shop = 3,
        Equip = 4,
        Enforcement = 5,
    }

    public enum PopupType
    {
        Gatcha = 0,
    }
    public class UIManager : Singleton<UIManager>
    {
        [FormerlySerializedAs("_updating")]
        [Header("UI Update")] 
        [SerializeField] private int _updateCount;
        [SerializeField] private float _updatingInterval;
        [SerializeField] private float _updatingEndValue;
        [Header("Popup")]
        public Popup gatchaPopup;
        public Popup enterYourNamePopup;
        public CoinEffect coinEffect;
        [Header("Components")] 
        public Transform titleTransform;
        public Transform stageHpTransform;
        public Button startButton;
        public Button gatchaButton;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _stage;
        [SerializeField] private TextMeshProUGUI _attackVal;
        [SerializeField] private TextMeshProUGUI _coinVal;
        [SerializeField] private Slider _enemyHp;

        
        
        private void FixedUpdate()
        {
            if (_updateCount > 0)
            {
                if (_updateCount == 1)
                {
                    _enemyHp.value = _updatingEndValue;
                }
                else
                {
                    _enemyHp.value -= _updatingInterval;
                }
                _updateCount -= 1;
            }
        }

        public void ShowCoinEffect()
        {
            coinEffect.ShowCoinEffect(SLManager.Instance.ClearCoin, 0.3f);
        }

        public void ShowPopupEnterYourNickname()
        {
            enterYourNamePopup.Open();
        }

        public void HideAllPopup()
        {
            gatchaPopup.Close();
        }
        
        public void ShowHideButtons(bool show)
        {
            startButton.gameObject.SetActive(show);
            gatchaButton.gameObject.SetActive(show);
        }
        
        public void UpdateUserName(string name)
        {
            if (name == String.Empty)
            {
                _name.text = "guest";
                _name.color = Color.gray;
                return;
            }
            _name.text = name;
            _name.color = Color.black;
        }
        
        public void UpdateEnemyHp(float end)
        {
            _updateCount = (int) (0.5f / Time.fixedDeltaTime);
            _updatingInterval = (float) Math.Round((_enemyHp.value - end)/ _updateCount, 3);
            _updatingEndValue = end;
        }

        public void UpdateStage(int stage)
        {
            _stage.text = stage.ToString();
        }

        public void UpdateAttackVal(int atk)
        {
            _attackVal.text = atk.ToString();
        }

        public void UpdateCoinVal(int coin)
        {
            _coinVal.text = coin.ToString();
        }
    }
}