using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using NonDestroyObject;
using Unity.Mathematics;
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
        [SerializeField] private GameObject gatchaStartinbGoButton;
        [Header("Components")]
        [SerializeField] private CoinUI playerCoinUI;
        [SerializeField] private CoinUI afterUseCoinUI;
        [SerializeField] private CoinUI priceCoinUI;
        [SerializeField] private GatchaTriggerObj inStartingPage;
        [SerializeField] private Image buttonImage;
        //[SerializeField] private GatchaTriggerObj gatchaTriggerObjAnimator;

        public void StartGatchaButton()
        {
            var gatchaValue = DataManager.Instance.PlayerDataManager.GatchaCosts;
            if (DataManager.Instance.PlayerDataManager.Coin < gatchaValue)
                return;
            StartGatcha(10).Forget();
        }

        public void StartingGatchaButton()
        {
            StartOpening().Forget();
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
            gatchaStartinbGoButton.SetActive(false);
            gatchaStartingPage.SetActive(true);

            var gatchaResult = await NetworkManager.Instance.AddRandomEquipItems(10);

            var gatchaValue = DataManager.Instance.PlayerDataManager.GatchaCosts;
            
            DataManager.Instance.PlayerDataManager.SpendCoin(gatchaValue);
            DataManager.Instance.PlayerDataManager.GatchaIncrement();
            
            DataManager.Instance.ItemManager.AddItems(gatchaResult.ItemEquipmentList);

            var highestRare = 1;
            foreach (var item in gatchaResult.ItemEquipmentList)
            {
                if (highestRare < item.rare)
                {
                    highestRare = item.rare;
                }
            }
            inStartingPage.Initiate(false, new ItemEquipment
            {
                rare = highestRare
            });
            
            Debug.AssertFormat(gatchaResult.ItemEquipmentList.Count == count, $"Gatcha Result Count is not matched: {0}", gatchaResult.ItemEquipmentList.Count.ToString());
            
            // Destroy Before Objects
            for (var i = gatchaOpeningPage.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(gatchaOpeningPage.transform.GetChild(i).gameObject);
            }

            var prefab = Resources.Load("Prefabs/UI/Gatcha/GatchaTrigger") as GameObject;
            for (var i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab, gatchaOpeningPage.transform);
                obj.GetComponent<GatchaTriggerObj>().Initiate(true, gatchaResult.ItemEquipmentList[i]);
            }
            gatchaStartinbGoButton.SetActive(true);
            gatchaLoaded = true;
        }

        private async UniTaskVoid StartOpening()
        {
            await UniTask.WaitUntil(() => gatchaLoaded);
            gatchaStartingPage.SetActive(false);
            gatchaOpeningPage.SetActive(true);
        }
        
        public new void Open()
        {
            NetworkManager.Instance.CheckConnection().Forget();
            
            gatchaStartPage.SetActive(true);
            gatchaStartingPage.SetActive(false);
            gatchaOpeningPage.SetActive(false);
            var playerCoin = DataManager.Instance.PlayerDataManager.Coin;
            var price = DataManager.Instance.PlayerDataManager.GatchaCosts;
            playerCoinUI.SetCoinValue(playerCoin);
            priceCoinUI.SetCoinValue(price);
            afterUseCoinUI.SetCoinValue(playerCoin - price);
            if (playerCoin < price)
            {
                buttonImage.color = Color.gray;
            }
            else
            {
                buttonImage.color = Color.white;
            }
            base.Open();
        }

        public new void Close()
        {
            base.Close();
        }
    }
}