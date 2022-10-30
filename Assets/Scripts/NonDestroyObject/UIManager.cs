using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
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
        [Header("Components")] 
        public Transform titleTransform;
        public Transform stageHpTransform;
        public Button startButton;
        [SerializeField] private TextMeshProUGUI _stage;
        [SerializeField] private TextMeshProUGUI _attackVal;
        [SerializeField] private TextMeshProUGUI _coinVal;
        [SerializeField] private Slider _enemyHp;

        private IEnumerator _sliderUpdate;
        private IEnumerator UpdateEnemyHpEnum(float end, float time)
        {
            int interval = (int)((_enemyHp.value - end)/0.01f);
            for (int i = 0; i < interval; i++)
            {
                yield return new WaitForSeconds(time / interval);
                _enemyHp.value -= 0.01f;
            }
            _enemyHp.value = end;
        }
        public void UpdateEnemyHp(float end)
        {
            if (_sliderUpdate != null)
            {
                StopCoroutine(_sliderUpdate);
            }
            _sliderUpdate = UpdateEnemyHpEnum(end, 0.5f);
            StartCoroutine(_sliderUpdate);
        }

        public void ShowButtons(bool show)
        {
            startButton.gameObject.SetActive(show);
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