using UnityEngine;
using UnityEngine.Advertisements;

public static class UnityAdsPrivacy
{
    private const string ConsentPreferenceKey = "personalized_ok";
    private const string ConsentGrantedValue = "ok";
    private const string ConsentDeniedValue = "non_ok";

    public static bool CanInitializeAds()
    {
        if (!GDPRPanelManager.IsConsentStatusResolved)
            return false;

        string choice = PlayerPrefs.GetString("personalized_ok", "");
        return !GDPRPanelManager.isEuUser ||
               choice == ConsentGrantedValue ||
               choice == ConsentDeniedValue;
    }

    public static void ApplyConsentMetadata()
    {
        string choice = PlayerPrefs.GetString(ConsentPreferenceKey, "");
        if (choice != ConsentGrantedValue && choice != ConsentDeniedValue)
            return;

        bool consentGranted = choice == ConsentGrantedValue;
        MetaData metadata = new MetaData("gdpr");
        metadata.Set("consent", consentGranted);
        Advertisement.SetMetaData(metadata);
    }
}
