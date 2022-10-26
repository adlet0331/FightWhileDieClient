using System;
using GoogleMobileAds.Api;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class AdsManager : Singleton<AdsManager>
    {
        private BannerView bannerView;
        private void Start()
        {
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });
            
            RequestBanner();
        }
        
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
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
            
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            
            // Load the banner with the request.
            this.bannerView.LoadAd(request);
        }
    }
}
