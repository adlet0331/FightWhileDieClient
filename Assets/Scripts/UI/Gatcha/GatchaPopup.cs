using System.Collections.Generic;
using UnityEngine;

namespace UI.Gatcha
{
    public enum PageStatus
    {
        StartPage,
        OpeningPage
    }
    public class GatchaPopup : Popup
    {
        [Header("Debugging")]
        //TODO: PlayerDataManager에 가챠한 횟수 추가하기. 
        //TODO: Server에는 안 보내도 되나? 안 보내도 되지 않나? 
        [SerializeField] private int spendCoin;
        [Header("GameObjects")]
        [SerializeField] private List<GameObject> particles;
        [SerializeField] private GameObject gatchaStartPage;
        [SerializeField] private GameObject gatchaOpeningPage;
        [Header("Components")]
        [SerializeField] private CoinUI playerCoinUI;
        [SerializeField] private CoinUI afterUseCoinUI;
        [SerializeField] private CoinUI valueCoinUI;
        [SerializeField] private GatchaTriggerObj gatchaTriggerObjAnimator;

        public void StartButton()
        {
            
        }
        
        public new void Open()
        {
            base.Open();
        }

        public new void Close()
        {
            base.Close();
        }

        private void SwitchPage(PageStatus pageStatus)
        {
            switch (pageStatus)
            {
                case PageStatus.StartPage:

                    return;
                case PageStatus.OpeningPage:

                    return;
            }
        }
    }
}