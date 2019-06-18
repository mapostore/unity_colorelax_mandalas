using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour {


    GameObject eu_panel;

    // Use this for initialization
    void Start()
    {
        
        eu_panel = GameObject.Find("Eu_Panel");

        if (GDPRPanelManager.isEuUser==false) {
            eu_panel.SetActive(false);
        }

        print("CREDITS LOADED");

    }


    public void LoadCredits()
    {      
        print("CREDITS LOADING CALL.");
        SceneManager.LoadScene("Credits");
    }

    public void LoadGallery()
    {
        print("Gallery LOADING CALL.");
        int rnd = Random.Range(1, 101);
        if (rnd <= 50)
        {
            AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
            // adsManager.showInterstitialAdMob();
        }
        SceneManager.LoadScene("Gallery");
    }

    public void LoadConsent()
    {
        print("BCK TO CONSENT REQUEST");
        //PlayerPrefs.SetInt("personalized_ok", -1);
        PlayerPrefs.SetString("personalized_ok", ""); // reinit the flag to allow consent request
        SceneManager.LoadScene("Start");
    }

    public void showUnityAds()
    {
        AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
    }



    public void OpenMySite()
    {
        print("OpenMySite");
        Application.OpenURL("http://www.indie-walkabout.eu/");
    }


    public void OpenPrivacyPolicy()
    {
        print("OpenPrivacyPolicy");
        Application.OpenURL("http://www.indie-walkabout.eu/privacy-policy-app/");
    }


    public void OpenAndroidGameUrl()
    {
        print("OpenAndroidGameUrl");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.mapo.indiewalkabout.coloRelax");
    }


    public void OpenFuturisticMandalaGooglePlayStore()
    {
        print("OpenAndroidGameUrl");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.mapo.indiewalkabout.coloRelax.futuristic.mandalas");
    }

}
