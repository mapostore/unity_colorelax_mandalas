using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : MonoBehaviour {
    private static AppInit _instance;

    // public List<string> HomeButtNames;
    // public List<string> HomeButtImages;
    public List<string> category;
    public List<string> mainImg ;
    public List<ImagePath> editedSubImg;
    public List<string> origResImg;
    public List<int> subImgCount;

    GDPRPanelManager gdprPanelManager = null;

    private void Awake() {
        SetUpSingleton();
    }


    private void SetUpSingleton() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject.GetComponent(_instance.GetType()));
        DontDestroyOnLoad(gameObject);
    }



    // Start is called before the first frame update
    void Start() {
        Debug.Log("====================== AppInit : START ======================");
        gdprPanelManager = FindObjectOfType<GDPRPanelManager>();
        setupForMobile();
        // loadIcons();
        loadCategories();
        loadImages();
        if (gdprPanelManager != null)
            gdprPanelManager.GDPRInit();
        Debug.Log("====================== AppInit : END ======================");
    }



    void setupForMobile() {
        if (!PlayerPrefsX.GetBool("ImagesStoredMobile")) {
        #if UNITY_ANDROID || UNITY_IOS  // on mobile only: it's the only platform target of the game
            Debug.Log("Generate files with data for Mobile");
            List<string> filePath = new List<string>();
            List<ImagePath> filePathInAsset = ImagePathHolder.LoadSubImagePathFromUnityAsset();
        
            foreach (ImagePath i in filePathInAsset)
                filePath.Add(i.imagePath);
        
            ImagePathHolder.ForMobileDeviceOnly(ImagePathHolder.LoadSubImageResourcePathFromUnityAsset(), filePath);
        #endif
        }
    }


    /*
    void loadIcons() {
        HomeButtNames = new List<string>();
        HomeButtImages = new List<string>();
    }
    */

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
