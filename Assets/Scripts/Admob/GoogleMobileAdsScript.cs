using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleMobileAdsScript : MonoBehaviour {

    public InterstitialAd interstitial;

    public BannerView bannerView;

    public void Start()
        {
            #if UNITY_ANDROID
        string appId =  "ca-app-pub-8846176967909254~4337911163"; //TEST : "ca-app-pub-3940256099942544~3347511713"; // PROD 
            #elif UNITY_IPHONE
        string appId = "ca-app-pub-8846176967909254~6797718233"; //TEST : "ca-app-pub-3940256099942544~1458002511"; // PROD
            #else
                string appId = "unexpected_platform";
            #endif

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);
            Debug.Log("BANNER initialized!");

            this.RequestBanner();
            //this.RequestInterstitial();
        }


    private void RequestBanner()
    {
        #if UNITY_ANDROID
        //google play      ->****ACTIVE
        string adUnitId =  "ca-app-pub-8846176967909254/2922679727"; //TEST : "ca-app-pub-3940256099942544/1033173712"; // PROD 
        // amazon
        // string adUnitId    = "ca-app-pub-8846176967909254/7535336407";  //TEST :  // PROD "ca-app-pub-8846176967909254/2922679727";
        // SlideMe 
        // string adUnitId = "ca-app-pub-8846176967909254/5652366953";  //TEST :  // PROD "ca-app-pub-8846176967909254/2922679727";
        // GetJar    
        // string adUnitId = "ca-app-pub-8846176967909254/4486926268";  //TEST :  // PROD "ca-app-pub-8846176967909254/2922679727";
        #elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-8846176967909254/2123976058"; //TEST : "ca-app-pub-3940256099942544/4411468910"; // PROD
        #else
                    string adUnitId = "unexpected_platform";
        #endif


        Debug.Log("BANNER requested!");
        // Create a 320x50 banner at the Bottom of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);



        // AdRequest request = new AdRequest.Builder().Build();

        // Create an empty ad request.
        AdRequest request = null;
        // check if Eu user or not
        Debug.Log("GDPRPanelManager.isEuUser : " + GDPRPanelManager.isEuUser);
        Debug.Log("GDPRPanelManager.personalized_ok : " + GDPRPanelManager.personalized_ok.ToString());
        if (GDPRPanelManager.isEuUser == true)
        {
            // if (GDPRPanelManager.personalized_ok == 0)
            if (GDPRPanelManager.personalized_ok.Equals("non_ok"))
            {
                request = new AdRequest.Builder().AddExtra("npa", "1").Build();
                Debug.Log("NON_PersonalizedAds");

            }
            // else if (GDPRPanelManager.personalized_ok == 1)
            else if (GDPRPanelManager.personalized_ok.Equals("ok"))
            {
                request = new AdRequest.Builder().Build();
                Debug.Log("EUropeo and PersonalizedAds");
            }
        }
        else
        {
            request = new AdRequest.Builder().Build();
            Debug.Log("NO EUropeo,  PersonalizedAds");
        }



        // PlayerPrefs.SetInt("personalized_ok", -1);  //DEBUG
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }


    private void RequestInterstitial()
    {
        #if UNITY_ANDROID
        string adUnitIdInterstitial =  "ca-app-pub-3940256099942544/6300978111"; //TEST :  // PROD "ca-app-pub-8846176967909254/3485159189";
        #elif UNITY_IPHONE                
        string adUnitIdInterstitial = "ca-app-pub-8846176967909254/7777549993";  //TEST: "ca-app-pub-3940256099942544/4411468910"; // PROD
        #else
             string adUnitId = "unexpected_platform";
        #endif


        Debug.Log("INTERSTITIAL requested!");
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitIdInterstitial);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        interstitial.LoadAd(request);

        /* to show :
         *   if (interstitial.IsLoaded()) {
            interstitial.Show();
        }
         */
    }


    public void showInterstitialAdMob(){
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
    }

            
}
