using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GalleryHandler : MonoBehaviour {

    public float categoryListStartPos, subCategoryListStartPos;
    public GameObject mainCategoryItem, categoryPanel,
        subCategoryPanel,
        subCategoryItem,
        subCategoryRect,
        lockCategory, Header, IAPPanel;
    //// public GameObject imageItem; // simo : ERRORE ! must be commented
    public List<GameObject> Footer, FooterText;   // Footer list : CONTAINS BUTTONS !!!!
    public List<GameObject> subCategoryItemList; //,IAPPanelButts;
    public static GalleryHandler myInstance;


    [HideInInspector]
    public AppInit appInit;

    // GalleryHandler Singleton
    public static GalleryHandler Instance {
        get {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(GalleryHandler)) as GalleryHandler;
            return myInstance;
        }
    }


    // Use this for initialization
    void Start() {
        Debug.Log("====================== GalleryHandler Started ======================");
        appInit = FindObjectOfType<AppInit>();           
        GenerateMainCategoryList();
        SetHomeScreen();
        subCategoryListStartPos = categoryListStartPos;
    }



    void SetHomeScreen() {
        Debug.Log("DRAW HOME SCREEN");
        // MOVED TO APPMANAGER
        List<string> HomeButtNames = new List<string>();
        List<string> HomeButtImages = new List<string>();
        HomeButtImages = ImagePathHolder.LoadHomeButtImages();
        HomeButtNames = ImagePathHolder.LoadHomeButtNames();

        Header.GetComponent<Text>().text = HomeButtNames[0];

        foreach (GameObject g in FooterText)
            g.GetComponent<Text>().text = HomeButtNames[FooterText.IndexOf(g)];

        foreach (GameObject g in Footer)
            g.GetComponent<Image>().sprite = Resources.Load<Sprite>(HomeButtImages[Footer.IndexOf(g)]);
        //		foreach (GameObject g in Footer) 
        //		{
        //			g.GetComponent<UnityEngine.UI.Button>().transition=UnityEngine.UI.Selectable.Transition.ColorTint;
        //			ColorBlock c=g.GetComponent<UnityEngine.UI.Button>().colors;
        //			c.normalColor=Color.white;
        //			c.disabledColor=Color.white;
        //			c.pressedColor= GetColorFromString(HomeButtColors [Footer.IndexOf (g)]);
        //			c.highlightedColor=Color.white;
        //			g.GetComponent<UnityEngine.UI.Button>().colors=c;
        //		}

        // simo : disable gallery button in main screen
        Footer[0].SetActive(false);
    }




    // Load categories names with image
    void GenerateMainCategoryList() {
        Debug.Log("====================== GalleryHandler --> GenerateMainCategoryList ======================");
        mainCategoryItem.SetActive(true);

        // Instantiate each category item in list, based on the ones in resources
        foreach (string s in appInit.category) {
            GameObject mainCategoryItemImage = GameObject.Instantiate(mainCategoryItem,
                new Vector3(0, categoryListStartPos, 0), Quaternion.identity) as GameObject;

            mainCategoryItemImage.transform.SetParent(categoryPanel.transform, false);

            mainCategoryItemImage.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            mainCategoryItemImage.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, categoryListStartPos, 0f);

            // imageItem.transform.FindChild("Image").GetComponent<Image>().sprite=Sprite.Create(image.LoadImage(
            // 	ImagePathHolder.Instance.mainCategoryImages[ImagePathHolder.Instance.LoadCategoryFromAsset().IndexOf(s)]));
            // Debug.Log(mainImg[category.IndexOf(s)]);

            // img and text are both children of mainCategoryItem
            mainCategoryItemImage.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(appInit.mainImg[appInit.category.IndexOf(s)]);
            Debug.Log("     ---> Sprite path : " + appInit.mainImg[appInit.category.IndexOf(s)]);
            mainCategoryItemImage.transform.GetChild(0).GetComponent<Text>().text = s;
            mainCategoryItemImage.AddComponent<ImageDetails>();
            mainCategoryItemImage.GetComponent<ImageDetails>().CategoryName = s;
            // imageItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {StartCoroutine(GenerateSubCategoryList(imageItem));});
            // imageItem.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {StartCoroutine(GenerateSubCategoryList(imageItem));});


            // register button click on category image 
            mainCategoryItemImage.GetComponent<UnityEngine.UI.Button>()
                .onClick.AddListener(delegate {
                    StartCoroutine(GenerateSubImages(mainCategoryItemImage));
                });


            // TODO : register button click on what !??!
            mainCategoryItemImage.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>()
                .onClick.AddListener(delegate {
                    StartCoroutine(GenerateSubImages(mainCategoryItemImage));
                });

            categoryListStartPos -= 700f;
        }
        mainCategoryItem.SetActive(false);
    }




    // Generate images in each category: use WaitForSeconds for allow loading it
    public IEnumerator GenerateSubImages(GameObject mainCategoryItemImage) {
        CircularLoader.Instance.Loader();
        yield return new WaitForSeconds(1.5f);
        GenerateSubCategoryList(mainCategoryItemImage);
    }




    // TODO : Generate images for each category 
    public void GenerateSubCategoryList(GameObject mainCategoryItemImage) {
        Debug.Log("Image category hit, GenerateSubCategoryList");
        subCategoryItem.SetActive(true);

        categoryPanel.SetActive(false); // simo: moved from the bottom of the method; delete and decomment below if necessary
        // simo : enable gallery button in main screen
        Footer[0].SetActive(true);

        int startingImgIndex = 0;
        string categoryName = mainCategoryItemImage.GetComponent<ImageDetails>().CategoryName;

        // set the category name in header
        Header.GetComponent<Text>().text = categoryName;

        // got to category starting index
        for (int j = 0; j < appInit.category.IndexOf(categoryName); j++)
            startingImgIndex += appInit.subImgCount[j];

        int numImgInSelectedCategory = appInit.subImgCount[appInit.category.IndexOf(categoryName)];

        // load each img in selected category
        for (int i = 0; i < numImgInSelectedCategory; i++) {
            Texture2D image = new Texture2D(1, 1, TextureFormat.PVRTC_RGBA4, false);
            ImagePath currentImagePath = appInit.editedSubImg[startingImgIndex + i];
            string origCurrentImagePath = appInit.origResImg[startingImgIndex + i];

            GameObject imageItem = GameObject.Instantiate(subCategoryItem,
                new Vector3(0, subCategoryListStartPos, 0), Quaternion.identity) as GameObject;

            imageItem.transform.SetParent(subCategoryPanel.transform, false);
            imageItem.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            imageItem.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, subCategoryListStartPos, 0f);


            #if UNITY_ANDROID || UNITY_IOS
            // ImagePathHolder.SaveFileInResources_ForMobileDeviceOnly(string image, List<string> subImgFilePath);
            #endif

            // imageItem.transform.FindChild("Image").GetComponent<Image>().sprite=Sprite.Create(image.LoadImage(
            // ImagePathHolder.Instance.mainCategoryImages[ImagePathHolder.Instance.LoadCategoryFromAsset().IndexOf(s)]));
            //			Debug.Log(ImagePathHolder.imagesInCategory[startingImgIndex+i].imagePath);
            //			imageItem.transform.GetChild(0).GetComponent<Text>().text=g.GetComponent<ImageDetails>().CategoryName+" "+(i+1).ToString();
            //			imageItem.transform.GetChild(1).GetComponent<Image>().sprite=
            //				Resources.Load<Sprite>(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].imagePath);
            //			image.LoadImage(DataManager.Instance.FileReaderBytes(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].imagePath));

            // load image from reasources (in mobile will be a specific dir)
            image.LoadImage(DataManager.Instance.FileReaderBytes(currentImagePath.imagePath));

            image.Apply();
            imageItem.transform.GetChild(1).GetComponent<Image>().sprite =
                Sprite.Create(image, new Rect(0f, 0f, (float)1024, (float)1024), new Vector2(0.5f, 0.5f));

            // if(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].isLocked)
            imageItem.transform.GetChild(2).gameObject.SetActive(currentImagePath.isLocked);

            imageItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                delegate {
                    OnImageHit(imageItem);
                });

            // CLICK su IMAGE for coloring
            imageItem.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>()
                .onClick.AddListener(delegate {
                    OnImageHit(imageItem);
                });

            imageItem.AddComponent<ImageDetails>();
            imageItem.GetComponent<ImageDetails>().FileName = currentImagePath.imagePath;
            imageItem.GetComponent<ImageDetails>().ResName = origCurrentImagePath;

            subCategoryItemList.Add(imageItem);
            subCategoryListStartPos -= 700f;
        }

        CircularLoader.Instance.isLoading = false;
        subCategoryRect.SetActive(true);
        // simo : commented here and moved at the start, to avoid selecting nother category while waiting for the current selected loading 
        //categoryPanel.SetActive (false);
        subCategoryItem.SetActive(false);
    }



    // start a new game play when pressing on a image in category
    public void OnImageHit(GameObject g) {
        Debug.Log("IMAGE HIT");
        Debug.Log(g.GetComponent<ImageDetails>().FileName);
        DataManager.Instance.selectedFileName = g.GetComponent<ImageDetails>().FileName;
        DataManager.Instance.selectedResourceName = g.GetComponent<ImageDetails>().ResName;

        DataManager.Instance.LoadScene("NewGamePlay", 0.25f);
        //		AutoFade.LoadLevel ("NewGamePlay", 0.5f, 0.5f, Color.white);
    }


    public void OnGalleryHit() {
        Debug.Log("Gallery was hit");
        SetHomeScreen();

        foreach (GameObject g in subCategoryItemList)
            Destroy(g);
        Resources.UnloadUnusedAssets();
        subCategoryItemList.Clear();
        subCategoryRect.SetActive(false);
        categoryPanel.SetActive(true);

    }


    public void ReloadLevel() {
        SceneManager.LoadScene("Gallery");
    }


    public void OnMyDrawings() {
        DataManager.Instance.LoadScene("MyDrawings", 0.25f);
        //		AutoFade.LoadLevel ("MyDrawings", 0.5f, 0.5f, Color.white);
        //		Application.LoadLevel("MyDrawings");
    }


    public void OnInspirationHit() {
        SceneManager.LoadScene("Inspiration");
    }


    public void openCredits() {
        SceneManager.LoadScene("Credits");
    }


    public void OnMyGamesHit() {
        Debug.Log("OnMyGamesHit was hit");
        SceneManager.LoadScene("MyGames");

    }
}
