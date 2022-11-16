using System;
using System.Collections;
using Managers;
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
    public class UIManager : Singleton<UIManager>
    {
        [FormerlySerializedAs("_updating")]
        [Header("UI Update")] 
        [SerializeField] private int _updateCount;
        [SerializeField] private float _updatingInterval;
        [SerializeField] private float _updatingEndValue;
        [Header("Components")] 
        public Transform titleTransform;
        public Transform stageHpTransform;
        public Button startButton;
        public Popup gatchaPopup;
        public Button gatchaButton;
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
        
        public void UpdateEnemyHp(float end)
        {
            Debug.Log(Time.fixedDeltaTime);
            _updateCount = (int) (0.5f / Time.fixedDeltaTime);
            _updatingInterval = (float) Math.Round((_enemyHp.value - end)/ _updateCount, 3);
            _updatingEndValue = end;
        }

        public void ShowButtons(bool show)
        {
            startButton.gameObject.SetActive(show);
            gatchaButton.gameObject.SetActive(show);
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