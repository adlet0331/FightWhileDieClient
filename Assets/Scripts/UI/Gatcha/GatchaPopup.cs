using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gatcha
{
    public class GatchaPopup : Popup
    {
        [Header("Debugging")]
        [SerializeField] private int spendCoin;
        [SerializeField] private bool gatchaLoaded;
        [Header("GameObjects")]
        [SerializeField] private Popup NoInternetPopup;
        [SerializeField] private GameObject gatchaStartPage;
        [SerializeField] private GameObject gatchaStartingPage;
        [SerializeField] private GameObject gatchaOpeningPage;
        [SerializeField] private GridLayoutGroup gatchaOpeningPageTriggersParent;
        [SerializeField] private GameObject gatchaStartingGoButton;
        [Header("Components")]
        [SerializeField] private CoinUI playerCoinUI;
        [SerializeField] private CoinUI afterUseCoinUI;
        [SerializeField] private CoinUI priceCoinUI;
        [SerializeField] private GatchaTriggerObj inStartingPage;
        [SerializeField] private Image openGatchaImage;
        [SerializeField] private List<GatchaTriggerObj> gatchaTriggerObjs;
        //[SerializeField] private GatchaTriggerObj gatchaTriggerObjAnimator;

        public void StartGatchaButton()
        {
            var gatchaValue = DataManager.Instance.playerDataManager.GatchaCosts;
            if (DataManager.Instance.playerDataManager.Coin < gatchaValue)
                return;
            StartGatcha(10).Forget();
        }

        public void StartingGatchaButton()
        {
            StartOpening().Forget();
        }

        public void OpenAllGatchaTriggers()
        {
            for (int i = 0; i < gatchaTriggerObjs.Count; i++)
            {
                gatchaTriggerObjs[i].OpenAndShowItem();
            }
        }

        public void BackToStartPage()
        {
            UpdateUI();
            gatchaStartPage.SetActive(true);
            gatchaStartingPage.SetActive(false);
            gatchaOpeningPage.SetActive(false);
        }
        
        private async UniTaskVoid StartGatcha(int count)
        {
            if (!NetworkManager.Instance.Connectable)
            {
                NoInternetPopup.Open();
                return;
            }
            
            gatchaLoaded = false;
            gatchaStartPage.SetActive(false);
            gatchaStartingGoButton.SetActive(false);
            gatchaStartingPage.SetActive(true);

            var gatchaResult = await NetworkManager.Instance.AddRandomEquipItems(10);

            if (!gatchaResult.Success)
            {
                NoInternetPopup.Open();
            }

            var gatchaValue = DataManager.Instance.playerDataManager.GatchaCosts;
            
            DataManager.Instance.playerDataManager.SpendCoin(gatchaValue);
            DataManager.Instance.playerDataManager.GatchaIncrement();
            
            DataManager.Instance.itemManager.AddItems(gatchaResult.ItemEquipmentList);

            var highestRare = 1;
            foreach (var item in gatchaResult.ItemEquipmentList)
            {
                if (highestRare < item.rare)
                {
                    highestRare = item.rare;
                }
            }
            inStartingPage.Initiate(false, new EquipItemObject
            {
                rare = highestRare
            });
            
            Debug.AssertFormat(gatchaResult.ItemEquipmentList.Count == count, $"Gatcha Result Count is not matched: {0}", gatchaResult.ItemEquipmentList.Count.ToString());
            
            // Destroy Before Objects
            for (var i = gatchaOpeningPageTriggersParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(gatchaOpeningPageTriggersParent.transform.GetChild(i).gameObject);
            }

            // Resize Grid Layout
            float width = gatchaOpeningPageTriggersParent.gameObject.GetComponent<RectTransform>().rect.width;
            float height = gatchaOpeningPageTriggersParent.gameObject.GetComponent<RectTransform>().rect.height;
            float xmaxLen = (width - gatchaOpeningPageTriggersParent.spacing.x - 50) / 2;
            float ymaxLen = (height - gatchaOpeningPageTriggersParent.spacing.y * 4 - 50) / 5;
            float size = xmaxLen > ymaxLen ? ymaxLen : xmaxLen;
            
            gatchaOpeningPageTriggersParent.cellSize = new Vector2(size,size);
            
            var prefab = Resources.Load("Prefabs/UI/Gatcha/GatchaTrigger") as GameObject;
            gatchaTriggerObjs.Clear();
            for (var i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab, gatchaOpeningPageTriggersParent.transform);
                obj.GetComponent<GatchaTriggerObj>().Initiate(true, gatchaResult.ItemEquipmentList[i]);
                gatchaTriggerObjs.Add(obj.GetComponent<GatchaTriggerObj>());
            }
            gatchaStartingGoButton.SetActive(true);
            gatchaLoaded = true;
        }

        private async UniTaskVoid StartOpening()
        {
            await UniTask.WaitUntil(() => gatchaLoaded);
            gatchaStartingPage.SetActive(false);
            gatchaOpeningPage.SetActive(true);
        }

        private void UpdateUI()
        {
            var playerCoin = DataManager.Instance.playerDataManager.Coin;
            var price = DataManager.Instance.playerDataManager.GatchaCosts;
            playerCoinUI.SetCoinValue(playerCoin);
            priceCoinUI.SetCoinValue(price);
            afterUseCoinUI.SetCoinValue(playerCoin - price);
            if (playerCoin < price)
            {
                openGatchaImage.color = Color.gray;
            }
            else
            {
                openGatchaImage.color = Color.white;
            }
        }
        
        public new void Open()
        {
            UpdateUI();
            gatchaStartPage.SetActive(true);
            gatchaStartingPage.SetActive(false);
            gatchaOpeningPage.SetActive(false);
            base.Open();
        }

        public new void Close()
        {
            base.Close();
            NoInternetPopup.Close();
        }
    }
}