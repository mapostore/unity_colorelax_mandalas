using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GDPRPanelManager : MonoBehaviour {

    public static string userCountryCode="";
    public static bool isEuUser = false;
    GameObject canvas; //, splashLoading;
    //public static int personalized_ok = -1;
    public static string personalized_ok = "";
    //MeshRenderer renderSplashImg;
    private bool choiceMade = false;


    void Awake(){
        // get preferences for result_gdpr
        // personalized_ok = PlayerPrefs.GetInt("personalized_ok");
        personalized_ok = PlayerPrefs.GetString("personalized_ok");
        Debug.Log("personalized_ok : " + personalized_ok.ToString());

        // eu user check
        userCountryCode = PreciseLocale.GetRegion();

        //COMMENT/UNCOMMENT FOR DEBUG
        // userCountryCode = "IT"; //DEBUG
        // userCountryCode = "US"; //DEBUG
        // PlayerPrefs.SetInt("personalized_ok", -1);  //DEBUG
        // PlayerPrefs.SetInt("personalized_ok", "");  //DEBUG

        Debug.Log("userCountryCode : " + userCountryCode);
        isEuUser = CheckEu.isEuCountry(userCountryCode);
        
    }
    // Use this for initialization
    void Start(){
        //isEuUser = true; //DEBUG
        canvas = GameObject.Find("Canvas");


        /*
        if ((isEuUser == true) && (personalized_ok < 0)){
	


            Debug.Log("Request ok");
            //canvas.SetActive(true);
        }
        else if ((isEuUser == false) || (personalized_ok >= 0)){
	


            choiceMade = true;
            Debug.Log("Canvas deactivated");
            //canvas.SetActive(false);
            SceneManager.LoadScene("Gallery");
        }
        */

        // non mostrare  la richiesta nel caso  NON sia europeo e il consenso sia già stato chiesto, cioè
        // personalized_ok uguale a "ok" o "non_ok", unici valori ammessi
        if ( 
            (isEuUser == false) || ( (personalized_ok.Equals("ok")) || (personalized_ok.Equals("non_ok")) )
           ) {
            choiceMade = true;
            Debug.Log("Skip compliance request, userCountryCode : " + userCountryCode + " personalized_ok :" +personalized_ok);

            // SceneManager.LoadScene("Gallery");
            // LoadGallery(); // not async, it's in the main thread
            // Use a coroutine to load the Scene in the background
            StartCoroutine(LoadGalleryAsyncScene());
        }




    }

        

    void drawLoadingImage(){
        Texture2D loadingImage;
        Rect loadingImageRect;
        // loadingImageRect = new Rect((float)(Screen.width * 0.1*(-1)), 0, (float)(Screen.width*1.2), (float) (Screen.height));
        // loadingImageRect = new Rect((float)(Screen.width * 0.15) ,(float)(Screen.height * 0.25 ), (float)(Screen.width * 0.7), (float)(Screen.height*0.5));
        loadingImageRect = new Rect((float)(Screen.width * 0.05), (float)(Screen.height * 0.13), (float)(Screen.width * 0.9), (float)(Screen.height * 0.72));
        loadingImage = Resources.Load<Texture2D>("Graphics/UIIcons/Splash/splash_still_loading");
        GUI.DrawTexture(loadingImageRect, loadingImage);
    }

    private void OnGUI(){
        if (choiceMade==true) { drawLoadingImage(); }
    }


    public void LoadApp_GdprYes(){
        Debug.Log("Yes pressed");
        choiceMade = true;

        // personalized_ok = 1;
        // PlayerPrefs.SetInt("personalized_ok", 1);

        personalized_ok = "ok";
        PlayerPrefs.SetString("personalized_ok", "ok");
        Invoke("LoadGallery", 2.0f);

    }

    public void LoadApp_GdprNo(){
        Debug.Log("No pressed");
        choiceMade = true;

        // personalized_ok = 0;
        // PlayerPrefs.SetInt("personalized_ok", 0);

        personalized_ok = "non_ok";
        PlayerPrefs.SetString("personalized_ok", "non_ok");
        Invoke("LoadGallery", 2.0f);
    }

    public void OpenPrivacyPolicy(){
        Application.OpenURL("http://www.indie-walkabout.eu/privacy-policy-app/");
    }

    /**
     *  Load main menu with galleries
     */
    void LoadGallery(){
        Debug.Log("=============================================> GDPR CHECK : END.");
        SceneManager.LoadScene("Gallery");

    }


    /**
     *  Load main screen with galleries in Async mode
     */
    IEnumerator LoadGalleryAsyncScene(){
        Debug.Log("=============================================> GDPR CHECK : END.");
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Gallery");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone){
	


            yield return null;
        }
    }



}
