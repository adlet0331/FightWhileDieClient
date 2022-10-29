using System;
using GoogleMobileAds.Api;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class AdsManager : Singleton<AdsManager>
    {
        private BannerView _bannerView;
        private RewardedAd _rewardedAd;
        private void Start()
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });

            // RequestBanner();
        }

        public void RequestRewardAds()
        {
            RequestRewardAd();
        }
        
        // 베너 광고 띄우기
        private void RequestBanner()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif
            // Create a 320x50 banner at the top of the screen.
            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
            
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            
            // Load the banner with the request.
            this._bannerView.LoadAd(request);
        }
        
        // 보상형 광고 띄우기
        private void RequestRewardAd()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif
            this._rewardedAd = new RewardedAd(adUnitId);
            
            
            this._rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            this._rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            this._rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            this._rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            this._rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            this._rewardedAd.OnAdClosed += HandleRewardedAdClosed;
            
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            this._rewardedAd.LoadAd(request);
            this._rewardedAd.Show();
        }
        
        // Called when an ad request has successfully loaded.
        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdLoaded Loaded well");
        }

        // Called when an ad request failed to load.
        public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log(
                "HandleRewardedAdFailedToLoad event received with message: "
                + args.LoadAdError);
        }

        // Called when an ad is shown.
        public void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdOpening event received");
        }
        
        // Called when an ad request failed to show.
        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.LogAssertion(
                "HandleRewardedAdFailedToShow event received with message: "
                + args.AdError.GetMessage());
        }
        
        // Called when the ad is closed.
        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            Debug.Log("HandleRewardedAdClosed event received");
        }
        
        // Called when the user should be rewarded for interacting with the ad.
        public void HandleUserEarnedReward(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            Debug.Log(
                "HandleRewardedAdRewarded event received for "
                + amount.ToString() + " " + type);
        }
    }
}
