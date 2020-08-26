using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*
 Class for Handling all btn and link inside credits
*/
public class GUIManager : MonoBehaviour {


    public void LoadCredits(){
        SceneManager.LoadScene("Credits");
    }

    public void LoadConsent(){
        PlayerPrefs.SetInt("personalized_ok", -1);
        SceneManager.LoadScene("Start");
    }

    public void showUnityAds(){
        AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
    }


    public void OpenMySite(){
        Application.OpenURL("http://www.indie-walkabout.eu/");
    }


    public void OpenPrivacyPolicy(){
        Application.OpenURL("http://www.indie-walkabout.eu/privacy-policy-app/");
    }
}
