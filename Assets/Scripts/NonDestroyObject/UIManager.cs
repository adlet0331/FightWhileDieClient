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
    public class UIManager : Singleton<UIManager>
    {
        [FormerlySerializedAs("_updating")]
        [Header("UI Enemy Hp Update")] 
        [SerializeField] private int _updateCount;
        [SerializeField] private float _updatingInterval;
        [SerializeField] private float _updatingEndValue;
        [Header("Popup")]
        public GatchaPopup gatchaPopup;
        public Popup NoInternetInGatchaPopup;
        public InventoryPopup inventoryPopup;
        public Popup rankingPopup;
        public Popup pausePopup;
        public PopupEnterName enterYourNamePopup;
        public LoadingPopup LoadingPopup;
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
        public float UiMovingTime => uiMovingTime;
        [SerializeField] private Transform titleShowPosition;
        [SerializeField] private Transform titleHidePosition;
        
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _stage;
        [SerializeField] private TextMeshProUGUI _attackVal;
        [SerializeField] private TextMeshProUGUI _coinVal;
        [SerializeField] private Slider _enemyHp;

        private void Start()
        {
            _stageHpMoveCoroutine = null;
            _titleMoveCoroutine = null;
        }

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
            coinEffect.ShowCoinEffect(DataManager.Instance.playerDataManager.ClearCoin, 0.3f);
        }

        public void ShowPopupEnterYourNickname()
        {
            enterYourNamePopup.Open();
        }

        public void HideAllPopup()
        {
            gatchaPopup.Close();
            NoInternetInGatchaPopup.Close();
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