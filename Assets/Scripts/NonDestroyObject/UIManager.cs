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
        public UIStatus CurrentStatus => _currentStatus;

        [Header("UI Status")]
        [SerializeField] private UIStatus _currentStatus;

        [Header("Components")] 
        [SerializeField] private TextMeshProUGUI _stage;
        [SerializeField] private TextMeshProUGUI _attackVal;
        [SerializeField] private Slider _enemyHp;
        public void ChangeUIStatus(UIStatus status)
        {
            _currentStatus = status;
        }
        
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
        
        public void UpdateStage(int stage)
        {
            _stage.text = stage.ToString();
        }

        public void UpdateAttackVal(int atk)
        {
            _attackVal.text = atk.ToString();
        }
        
    }
}