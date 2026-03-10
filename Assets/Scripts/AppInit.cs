using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : MonoBehaviour {
    private static AppInit _instance;
    [Header("UI")]
    [Range(0f, 64f)]
    public float imagePreviewCornerRadiusDp = 16f;

    // public List<string> HomeButtNames;
    // public List<string> HomeButtImages;
    public List<string> category;        // category name as shown in list : CATEGORY Calm,Contemplation,
    public List<string> mainImg;         // category image path and name list : MAINIMAGEPATH : Category/0_CATEGORY ICONS/calm,Category/0_CATEGORY ICONS/contemplation,
    public List<ImagePath> editedSubImg; // images list for editing: copied from the original ones in origResImg ?
    public List<string> origResImg;      // images name list from unity asset : IMAGEPATHINRESOURCE Category/CALM/OK_comp/calm_573,Category/CALM/OK_comp/calm_578,
    public List<int> subImgCount;        // images number for each category
    public List<string> subImgResPath;   // IMAGEPATHINRESOURCE file name list : Category/CALM/OK_comp/calm_573
    public List<string> subImgFilePath;  // EDITEDSUBIMAGEPATH ImagePath list: Calm1D

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

    public static float GetImagePreviewCornerRadiusDp() {
        if (_instance == null) {
            return 16f;
        }
        return _instance.imagePreviewCornerRadiusDp;
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
            subImgFilePath = new List<string>();
            List<ImagePath> filePathInAsset = ImagePathHolder.LoadSubImagePathFromUnityAsset();

            foreach (ImagePath i in filePathInAsset) {
                subImgFilePath.Add(i.imagePath);
            }

            subImgResPath = ImagePathHolder.LoadSubImageResourcePathFromUnityAsset();
            // param :
            // - IMAGEPATHINRESOURCE file name list : Category/CALM/OK_comp/calm_573 (image sprite name in unity assets)
            // - EDITEDSUBIMAGEPATH image name (ON DEVICE) list: Calm1D
            ImagePathHolder.ForMobileDeviceOnly(subImgResPath, subImgFilePath);
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
