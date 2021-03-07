using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : MonoBehaviour {
    private static AppInit _instance;

    // public List<string> HomeButtNames;
    // public List<string> HomeButtImages;
    public List<string> category;
    public List<string> mainImg ;
    public List<ImagePath> editedSubImg; // img list for editing: copied from the original ones in origResImg
    public List<string> origResImg;
    public List<int> subImgCount;  // img number for each category

    GDPRPanelManager gdprPanelManager = null;

    private void Awake() {
        SetUpSingleton();
        AppSetup();
    }


    private void SetUpSingleton() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject.GetComponent(_instance.GetType()));
        DontDestroyOnLoad(gameObject);
    }



    // Start is called before the first frame update
    void AppSetup() {
        Debug.Log("====================== AppInit : START ======================");
        gdprPanelManager = FindObjectOfType<GDPRPanelManager>();
        StartCoroutine(setupForMobile());
        loadCategories();
        loadImages();
        if (gdprPanelManager != null)
            gdprPanelManager.GDPRInit();
        Debug.Log("====================== AppInit : END ======================");
    }



    /*
     * NB : this is the most costly part : it rewrites all the png 
     * as file of bytes .txt in resources disk storage of mobile devices; 
     * it's maybe the reason why on ios it became so huge
     * 
     * BUT WHY DOING  THIS ?
     * 
     * Maybe for saving them there after coloring, infact they are changed after that.
     * But why do not save them only when needed?
     * 
     * Excluding this lead to not showing categories at all: 
     * maybe something fails  in  GenerateMainCategoryList(), even if there is no crash
     * 
     */
    IEnumerator setupForMobile() {
        if (!PlayerPrefsX.GetBool("ImagesStoredMobile")) {
        #if UNITY_ANDROID || UNITY_IOS  // on mobile only: it's the only platform target of the game
            Debug.Log("====================== Generate files with data for Mobile: START ======================");
            List<string> filePath = new List<string>();
            List<ImagePath> filePathInAsset = ImagePathHolder.LoadSubImagePathFromUnityAsset();
        
            foreach (ImagePath i in filePathInAsset)
                filePath.Add(i.imagePath);

            ImagePathHolder.ForMobileDeviceOnly(ImagePathHolder.LoadSubImageResourcePathFromUnityAsset(), filePath);
            Debug.Log("====================== Generate files with data for Mobile: END ======================");
            yield return null;
        #endif
        }
    }


    void loadCategories() {
        category = ImagePathHolder.LoadCategoryFromAsset();
        mainImg = ImagePathHolder.LoadMainImagePathFromAsset();
    }

    void loadImages() {
        editedSubImg = ImagePathHolder.LoadSubImagePathFromAsset();
        origResImg = ImagePathHolder.LoadSubImageResourcePathFromAsset();
        subImgCount = ImagePathHolder.LoadSubImageCountFromAsset();
    }
}
