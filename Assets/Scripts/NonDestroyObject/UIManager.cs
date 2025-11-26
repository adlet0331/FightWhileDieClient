using System;
using System.Collections;
using TMPro;
using UI;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

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
    public delegate void UIUpdateVoid();
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Enemy Hp Update")]
        [SerializeField] private float updateIntervalConstant;
        private IEnumerator _hpUpdateCoroutine;
        [Header("Popup")]
        public InventoryPopup inventoryPopup;
        public Popup rankingPopup;
        public Popup pausePopup;
        public PopupEnterName enterYourNamePopup;
        public LoadingPopup loadingPopup;
        public SimpleTextPopup simpleTextPopup;
        [Header("Effect")]
        public CoinEffect coinEffect;
        [Header("Components")] 
        public Transform titleTransform;
        public Transform stageHpTransform;
        public Button startButton;
        public Button inventoryButton;
        public Slider attackChargeGageSlider;
        public GameObject attackPerfectEffectObject;
        [Header("Transforms")] 
        [SerializeField] private float uiMovingTime;
        [SerializeField] private Transform titleShowPosition;
        [SerializeField] private Transform titleHidePosition;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _stage;
        [SerializeField] private TextMeshProUGUI _attackVal;
        [SerializeField] private TextMeshProUGUI _coinVal;
        [SerializeField] private Slider _enemyHp;
        [SerializeField] private TextMeshProUGUI currentHpText;
        [SerializeField] private TextMeshProUGUI maxHpText;
        
        public event UIUpdateVoid UpdateAllUIEvent;

        public void UpdateAllUIInGame()
        {
            UpdateAllUIEvent?.Invoke();
        }

        private void Start()
        {
            _stageHpMoveCoroutine = null;
            _titleMoveCoroutine = null;

            UpdateAllUIEvent += UpdateMainUI;
            UpdateAllUIEvent += UpdateCombatUI;
            
            UpdateMainUI();
        }

        public void ShowCoinEffect()
        {
            coinEffect.ShowCoinEffect(DataManager.Instance.playerDataManager.ClearCoin, 0.3f);
        }

        public void ShowPopupEnterYourNickname()
        {
            enterYourNamePopup.Open();
        }

        public void HideAllPopup()
        {
            inventoryPopup.Close();
            rankingPopup.Close();
            pausePopup.Close();
            enterYourNamePopup.Close();
            simpleTextPopup.Close();
        }

        public void ActiveCombatUIs(bool show)
        {
            attackChargeGageSlider.gameObject.SetActive(show);
        }

        public void ActivePerfectEffect(bool show)
        {
            attackPerfectEffectObject.SetActive(show);
        }
        
        public void ActiveMainPagesButtons(bool show)
        {
            startButton.gameObject.SetActive(show);
            inventoryButton.gameObject.SetActive(show);
        }

        public void UpdateMainUI()
        {
            var name = DataManager.Instance.playerDataManager.UserName;
            if (name == String.Empty)
            {
                _name.text = "guest";
                _name.color = Color.gray;
                return;
            }
            else
            {
                _name.text = name;
                _name.color = Color.black;
            }
            _stage.text = IntToUnitString.ToString(DataManager.Instance.playerDataManager.Stage);
            _attackVal.text = IntToUnitString.ToString(DataManager.Instance.playerDataManager.Atk);
            _coinVal.text = IntToUnitString.ToString(DataManager.Instance.playerDataManager.Coin);
        }

        public void UpdateCombatUI()
        {
            UpdateEnemyHp(DataManager.Instance.playerDataManager.CurrentEnemyHp);
            currentHpText.text = IntToUnitString.ToString(DataManager.Instance.playerDataManager.CurrentEnemyHp);
            maxHpText.text = IntToUnitString.ToString(DataManager.Instance.playerDataManager.CurrentEnemyHp);
        }

        private void CancelUpdateEnemyHpCoroutine()
        {
            if (_hpUpdateCoroutine != null)
            {
                StopCoroutine(_hpUpdateCoroutine);
                _hpUpdateCoroutine = null;
            }
        }

        IEnumerator UpdateHpSliderIEnumerator(float endval)
        {
            float elapsedTime = 0;
            float startval = _enemyHp.value;
            while(elapsedTime < updateIntervalConstant)
            {
                elapsedTime += Time.deltaTime;
                _enemyHp.value = startval * (1.0f - elapsedTime / updateIntervalConstant) + endval * (elapsedTime / updateIntervalConstant);
                yield return null;
            }
            _enemyHp.value = endval;
            _hpUpdateCoroutine = null;
        }
        
        public void UpdateEnemyHp(float end)
        {
            CancelUpdateEnemyHpCoroutine();
            _hpUpdateCoroutine = UpdateHpSliderIEnumerator(end);
            StartCoroutine(_hpUpdateCoroutine);
        }

        public void UpdateAttackGageSlider(float val)
        {
            attackChargeGageSlider.value = val;
        }
        
        private IEnumerator _stageHpMoveCoroutine;
        private IEnumerator _titleMoveCoroutine;
        
        public void SwitchMainPage2CombatUI(bool isCombat)
        {
            if (_stageHpMoveCoroutine != null)
            {
                StopCoroutine(_stageHpMoveCoroutine);
            }
            if (_titleMoveCoroutine != null)
            {
                StopCoroutine(_titleMoveCoroutine);
            }
            
            if (isCombat)
            {
                stageHpTransform.gameObject.SetActive(true);
                _stageHpMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, stageHpTransform, titleShowPosition, () => { _stageHpMoveCoroutine = null; });
                _titleMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, titleTransform, titleHidePosition, () => { _titleMoveCoroutine = null; });
                
                ActiveMainPagesButtons(false);
                ActiveCombatUIs(true);
            }
            else
            {
                stageHpTransform.gameObject.SetActive(false);
                _stageHpMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, stageHpTransform, titleHidePosition, () => { _stageHpMoveCoroutine = null; });
                _titleMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, titleTransform, titleShowPosition, () => { _titleMoveCoroutine = null; });
                
                ActiveMainPagesButtons(true);
                ActiveCombatUIs(false);
            }
            
            StartCoroutine(_stageHpMoveCoroutine);
            StartCoroutine(_titleMoveCoroutine);
        }
    }
}