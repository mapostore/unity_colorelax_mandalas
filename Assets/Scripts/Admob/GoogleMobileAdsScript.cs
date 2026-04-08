using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class GoogleMobileAdsScript : MonoBehaviour
{
    const string AndroidTestAppId = "ca-app-pub-3940256099942544~3347511713";
    const string IosTestAppId = "ca-app-pub-3940256099942544~1458002511";
    const string AndroidTestBannerId = "ca-app-pub-3940256099942544/1033173712";
    const string IosTestBannerId = "ca-app-pub-3940256099942544/2934735716";

    static GoogleMobileAdsScript instance;
    static bool bootstrapRequested;

    public bool useTestAds = true;
    public bool loadBannerOnStart = true;

    public InterstitialAd interstitial;
    public BannerView bannerView;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureBannerBootstrap()
    {
        if (bootstrapRequested)
            return;

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        bootstrapRequested = true;
        if (FindObjectOfType<GoogleMobileAdsScript>() != null)
            return;

        GameObject adObject = new GameObject("GoogleMobileAdsScript");
        DontDestroyOnLoad(adObject);
        adObject.AddComponent<GoogleMobileAdsScript>();
#endif
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeAds();
        RefreshBannerVisibility();
#endif
    }

    void OnDestroy()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        if (instance == this)
            instance = null;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshBannerVisibility();
    }

    bool ShouldShowBannerInActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return activeScene.IsValid() && activeScene.name != "Gallery";
    }

    void RefreshBannerVisibility()
    {
        if (!ShouldShowBannerInActiveScene())
        {
            if (bannerView != null)
                bannerView.Hide();
            return;
        }

        if (bannerView == null)
        {
            RequestBanner();
            return;
        }

        bannerView.Show();
    }

    void InitializeAds()
    {
        string appId = GetAppId();
        if (string.IsNullOrEmpty(appId))
            return;

        MobileAds.Initialize(appId);
        Debug.Log("AdMob initialized in test banner mode.");

        if (loadBannerOnStart)
            RequestBanner();
    }

    string GetAppId()
    {
#if UNITY_ANDROID
        return useTestAds ? AndroidTestAppId : "ca-app-pub-8846176967909254~4337911163";
#elif UNITY_IPHONE
        return useTestAds ? IosTestAppId : "ca-app-pub-8846176967909254~6797718233";
#else
        return string.Empty;
#endif
    }

    string GetBannerAdUnitId()
    {
#if UNITY_ANDROID
        return useTestAds ? AndroidTestBannerId : "ca-app-pub-8846176967909254/2922679727";
#elif UNITY_IPHONE
        return useTestAds ? IosTestBannerId : "ca-app-pub-8846176967909254/2123976058";
#else
        return string.Empty;
#endif
    }

    public void RequestBanner()
    {
#if !(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR
        return;
#else
        string adUnitId = GetBannerAdUnitId();
        if (string.IsNullOrEmpty(adUnitId))
            return;

        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request = BuildBannerRequest();
        bannerView.LoadAd(request);
#endif
    }

    AdRequest BuildBannerRequest()
    {
        AdRequest.Builder builder = new AdRequest.Builder();

        if (useTestAds)
            return builder.Build();

        if (GDPRPanelManager.isEuUser)
        {
            if (GDPRPanelManager.personalized_ok.Equals("non_ok"))
                return builder.AddExtra("npa", "1").Build();

            if (GDPRPanelManager.personalized_ok.Equals("ok"))
                return builder.Build();
        }

        return builder.Build();
    }

    public void showInterstitialAdMob()
    {
        if (interstitial != null && interstitial.IsLoaded())
            interstitial.Show();
    }
}
