using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_IOS
    [SerializeField] private string gameId = "1757958";
    [SerializeField] private string interstitialAdUnitId = "video";
#elif UNITY_ANDROID
    [SerializeField] private string gameId = "1757957";
    [SerializeField] private string interstitialAdUnitId = "video";
#else
    [SerializeField] private string gameId = "";
    [SerializeField] private string interstitialAdUnitId = "video";
#endif

    [SerializeField] private bool testMode = false;

    private bool isInitialized;
    private bool initAttemptCompleted;
    private bool isAdLoaded;
    private bool loadAttemptCompleted;

    public static AdManager myInstance;
    public static AdManager Instance
    {
        get
        {
            if (myInstance == null)
            {
                myInstance = FindObjectOfType(typeof(AdManager)) as AdManager;
            }

            return myInstance;
        }
    }

    private void Awake()
    {
        Debug.Log("====================== AdManager Awaken ======================");

        if (myInstance == null)
        {
            myInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public IEnumerator ShowAd()
    {
        if (!Advertisement.isSupported)
        {
            yield break;
        }

        if (!isInitialized)
        {
            initAttemptCompleted = false;
            Advertisement.Initialize(gameId, testMode, this);
            while (!initAttemptCompleted)
            {
                yield return null;
            }

            if (!isInitialized)
            {
                yield break;
            }
        }

        isAdLoaded = false;
        loadAttemptCompleted = false;
        Advertisement.Load(interstitialAdUnitId, this);

        while (!loadAttemptCompleted)
        {
            yield return null;
        }

        if (!isAdLoaded)
        {
            yield break;
        }

        Advertisement.Show(interstitialAdUnitId, this);
    }

    public void OnInitializationComplete()
    {
        isInitialized = true;
        initAttemptCompleted = true;
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        initAttemptCompleted = true;
        Debug.LogError("Unity Ads initialization failed: " + error + " - " + message);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        if (adUnitId == interstitialAdUnitId)
        {
            isAdLoaded = true;
            loadAttemptCompleted = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        if (adUnitId == interstitialAdUnitId)
        {
            loadAttemptCompleted = true;
        }

        Debug.LogError("Unity Ads load failed for " + adUnitId + ": " + error + " - " + message);
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError("Unity Ads show failed for " + adUnitId + ": " + error + " - " + message);
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
    }

    public void OnUnityAdsShowClick(string adUnitId)
    {
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
    }
}
