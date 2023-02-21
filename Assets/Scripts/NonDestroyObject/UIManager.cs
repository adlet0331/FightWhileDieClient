using System;
using System.Collections;
using TMPro;
using UI;
using UI.Gatcha;
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

    public enum PopupType
    {
        Gatcha = 0,
    }
    public delegate void UIUpdateVoid();
    public class UIManager : Singleton<UIManager>
    {
        [Header("UI Enemy Hp Update")] 
        [SerializeField] private int updateCount;
        [SerializeField] private float updatingInterval;
        [SerializeField] private float updatingEndValue;
        [Header("Popup")]
        public GatchaPopup gatchaPopup;
        public Popup noInternetInGatchaPopup;
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
        public Button gatchaButton;
        public Button inventoryButton;
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

        private void FixedUpdate()
        {
            if (updateCount > 0)
            {
                if (updateCount == 1)
                {
                    _enemyHp.value = updatingEndValue;
                }
                else
                {
                    _enemyHp.value -= updatingInterval;
                    currentHpText.text = IntToUnitString.ToString((int)(DataManager.Instance.playerDataManager.CurrentEnemyHp * _enemyHp.value ));
                }
                updateCount -= 1;
            }
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
            gatchaPopup.Close();
            noInternetInGatchaPopup.Close();
            inventoryPopup.Close();
            rankingPopup.Close();
            pausePopup.Close();
            enterYourNamePopup.Close();
            simpleTextPopup.Close();
        }
        
        public void ShowHideButtons(bool show)
        {
            startButton.gameObject.SetActive(show);
            gatchaButton.gameObject.SetActive(show);
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

        public void UpdateEnemyHp(float end)
        {
            updateCount = (int) (0.5f / Time.fixedDeltaTime);
            updatingInterval = (float) Math.Round((_enemyHp.value - end)/ updateCount, 3);
            updatingEndValue = end;
        }
        
        private IEnumerator _stageHpMoveCoroutine;
        private IEnumerator _titleMoveCoroutine;
        
        public void TitleEnemyHpSwitch(bool showHp)
        {
            if (_stageHpMoveCoroutine != null)
            {
                StopCoroutine(_stageHpMoveCoroutine);
            }
            if (_titleMoveCoroutine != null)
            {
                StopCoroutine(_titleMoveCoroutine);
            }
            
            if (showHp)
            {
                stageHpTransform.gameObject.SetActive(true);
                _stageHpMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, stageHpTransform, titleShowPosition, () => { _stageHpMoveCoroutine = null; });
                _titleMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, titleTransform, titleHidePosition, () => { _titleMoveCoroutine = null; });
                
                ShowHideButtons(false);
            }
            else
            {
                stageHpTransform.gameObject.SetActive(false);
                _stageHpMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, stageHpTransform, titleHidePosition, () => { _stageHpMoveCoroutine = null; });
                _titleMoveCoroutine = CoroutineUtils.TransformMove(uiMovingTime, titleTransform, titleShowPosition, () => { _titleMoveCoroutine = null; });
                
                ShowHideButtons(true);
            }
            
            StartCoroutine(_stageHpMoveCoroutine);
            StartCoroutine(_titleMoveCoroutine);
        }
    }
}