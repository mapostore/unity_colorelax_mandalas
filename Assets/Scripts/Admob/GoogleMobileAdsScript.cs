using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class GoogleMobileAdsScript : MonoBehaviour, IUnityAdsInitializationListener {

    private const string GallerySceneName = "Gallery";
    private const string SplashSceneName = "Splash";

#if UNITY_IOS
    [SerializeField] private string gameId = "1724256";
#elif UNITY_ANDROID
    [SerializeField] private string gameId = "1724257";
#else
    [SerializeField] private string gameId = "";
#endif

    [SerializeField] private string bannerAdUnitId = "banner";
    [SerializeField] private bool testMode = false;

    private static bool sceneHookRegistered;
    private static bool unityAdsInitRequested;
    private bool bannerLoadRequested;

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
        Debug.Log("Unity Ads banner skipped in Editor. Legacy Unity Ads banners load only on device builds.");
#else
        if (!Advertisement.isSupported)
            return;

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        if (Advertisement.isInitialized)
        {
            LoadUnityBanner();
            return;
        }

        if (!unityAdsInitRequested)
        {
            unityAdsInitRequested = true;
            Advertisement.Initialize(gameId, testMode, this);
        }
#endif
#endif
    }

    private void OnDestroy()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        Advertisement.Banner.Hide(false);
#endif
    }

    private void LoadUnityBanner()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (bannerLoadRequested || ShouldSkipBannerInScene(SceneManager.GetActiveScene().name))
            return;

        bannerLoadRequested = true;
        Advertisement.Banner.Load(bannerAdUnitId, new BannerLoadOptions
        {
            loadCallback = OnUnityAdsBannerLoaded,
            errorCallback = OnUnityAdsBannerError
        });
#endif
    }

    private static bool ShouldSkipBannerInScene(string sceneName)
    {
        return sceneName == GallerySceneName || sceneName == SplashSceneName;
    }

    public void OnInitializationComplete()
    {
        unityAdsInitRequested = false;
        LoadUnityBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        unityAdsInitRequested = false;
        Debug.LogWarning("Unity Ads banner initialization failed: " + error + " - " + message);
    }

    public void OnUnityAdsBannerLoaded()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        Advertisement.Banner.Show(bannerAdUnitId, new BannerOptions
        {
            clickCallback = OnUnityAdsBannerClick,
            hideCallback = OnUnityAdsBannerHide,
            showCallback = OnUnityAdsBannerShown
        });
#endif
    }

    public void OnUnityAdsBannerError(string message)
    {
        bannerLoadRequested = false;
        Debug.LogWarning("Unity Ads banner failed to load: " + message);
    }

    public void OnUnityAdsBannerClick()
    {
    }

    public void OnUnityAdsBannerShown()
    {
    }

    public void OnUnityAdsBannerHide()
    {
    }
}
