using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class GoogleMobileAdsScript : MonoBehaviour {

    private const string GallerySceneName = "Gallery";
    private const string SplashSceneName = "Splash";

    private const string AndroidTestAppId = "ca-app-pub-3940256099942544~3347511713";
    private const string IosTestAppId = "ca-app-pub-3940256099942544~1458002511";
    private const string AndroidTestBannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string IosTestBannerId = "ca-app-pub-3940256099942544/2934735716";
    private const string AndroidProductionAppId = "ca-app-pub-8846176967909254~4337911163";
    private const string IosProductionAppId = "ca-app-pub-8846176967909254~6797718233";
    private const string AndroidProductionBannerId = "ca-app-pub-8846176967909254/2922679727";
    private const string IosProductionBannerId = "ca-app-pub-8846176967909254/2123976058";
    private static bool sceneHookRegistered;
    private static bool mobileAdsInitialized;

    public bool showEditorBannerPlaceholder = true;
    public Vector2 editorBannerPlaceholderSize = new Vector2(320f, 50f);
    public InterstitialAd interstitial;
    public BannerView bannerView;

#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapSceneLocalBanner()
    {
        if (!sceneHookRegistered)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            sceneHookRegistered = true;
        }

        EnsureSceneLocalBannerManager(SceneManager.GetActiveScene());
    }
#endif

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureSceneLocalBannerManager(scene);
    }

    private static void EnsureSceneLocalBannerManager(Scene scene)
    {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
        if (ShouldSkipBannerInScene(scene.name))
            return;

        if (FindObjectOfType<GoogleMobileAdsScript>() != null)
            return;

        GameObject managerObject = new GameObject("GoogleMobileAdsScript");
        managerObject.AddComponent<GoogleMobileAdsScript>();
#endif
    }

    public void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
        if (ShouldSkipBannerInScene(SceneManager.GetActiveScene().name))
            return;

#if UNITY_EDITOR
        Debug.Log("AdMob Editor placeholder enabled for scene: " + SceneManager.GetActiveScene().name);
#else
        if (!mobileAdsInitialized)
        {
            MobileAds.Initialize(GetProductionAppId());
            mobileAdsInitialized = true;
            Debug.Log("AdMob initialized with production app id.");
        }

        RequestBanner();
#endif
#endif
    }

    private void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    private string GetProductionAppId()
    {
#if UNITY_ANDROID
        return AndroidProductionAppId;
#elif UNITY_IPHONE
        return IosProductionAppId;
#elif UNITY_EDITOR
        return AndroidTestAppId;
#else
        return "unexpected_platform";
#endif
    }

    private string GetProductionBannerAdUnitId()
    {
#if UNITY_ANDROID
        return AndroidTestBannerId;
#elif UNITY_IPHONE
        return IosProductionBannerId;
#elif UNITY_EDITOR
        return AndroidTestBannerId;
#else
        return "unexpected_platform";
#endif
    }

    private void RequestBanner()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (bannerView != null)
            return;

        Debug.Log("AdMob production banner requested for scene: " + SceneManager.GetActiveScene().name);
        bannerView = new BannerView(GetProductionBannerAdUnitId(), AdSize.SmartBanner, AdPosition.Bottom);
        bannerView.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("AdMob production banner loaded.");
        };
        bannerView.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.LogWarning("AdMob production banner failed to load: " + args.Message);
        };
        bannerView.LoadAd(BuildBannerRequest());
#endif
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (!showEditorBannerPlaceholder || ShouldSkipBannerInScene(SceneManager.GetActiveScene().name))
            return;

        float width = Mathf.Min(editorBannerPlaceholderSize.x, Screen.width);
        float height = editorBannerPlaceholderSize.y;
        Rect bannerRect = new Rect((Screen.width - width) * 0.5f, Screen.height - height, width, height);

        Color oldColor = GUI.color;
        GUI.color = new Color(0.08f, 0.08f, 0.08f, 0.92f);
        GUI.DrawTexture(bannerRect, Texture2D.whiteTexture);
        GUI.color = new Color(1f, 1f, 1f, 0.9f);
        GUI.Box(bannerRect, "AdMob Test Banner Placeholder 320x50");
        GUI.color = oldColor;
    }
#endif

    private AdRequest BuildBannerRequest()
    {
        if (GDPRPanelManager.isEuUser && GDPRPanelManager.personalized_ok.Equals("non_ok"))
        {
            Debug.Log("AdMob NON-personalized production banner request");
            return new AdRequest.Builder().AddExtra("npa", "1").Build();
        }

        Debug.Log("AdMob default production banner request");
        return new AdRequest.Builder().Build();
    }

    private static bool ShouldSkipBannerInScene(string sceneName)
    {
        return sceneName == GallerySceneName || sceneName == SplashSceneName;
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitIdInterstitial = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitIdInterstitial = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitIdInterstitial = "unexpected_platform";
#endif

        Debug.Log("INTERSTITIAL requested!");
        interstitial = new InterstitialAd(adUnitIdInterstitial);
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public void showInterstitialAdMob()
    {
        if (interstitial != null && interstitial.IsLoaded())
            interstitial.Show();
    }
}
