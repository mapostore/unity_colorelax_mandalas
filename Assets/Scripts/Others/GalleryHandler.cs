using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
TODO : The real problem here is this class and ImagePathHolder : understand the latter before GalleryHandler
The reason why it is so slow, it's that it generates all category's sub images BEFORE clicking in category.
And that's why it decompress all the images at start in ios: because is as it shows it.
Would be different if it load the sub images in category AFTER clicking on category.


Here in GenerateMainCategoryList() seems doing such a thing :

// register button click on category image 
imageItem.GetComponent<UnityEngine.UI.Button>()
	.onClick.AddListener(delegate{
		StartCoroutine(GenerateSubImages(imageItem));
	});

--> then the killing point is here, at loading the category : something go really wrong :
	GenerateMainCategoryList() e quindi :
		List<string> category = ImagePathHolder.LoadCategoryFromAsset ();
		List<string> mainImg  = ImagePathHolder.LoadMainImagePathFromAsset ();
    May be is better nit loading them dynamically but statically?
 
*/
public class GalleryHandler : MonoBehaviour {

	public float categoryListStartPos,subCategoryListStartPos;
	public GameObject mainCategoryItem,categoryPanel,
		subCategoryPanel,
		subCategoryItem,
		subCategoryRect,
		lockCategory,Header,IAPPanel;
    //// public GameObject imageItem; // simo : ERRORE ! must be commented
    public List<GameObject> Footer,FooterText;   // Footer list : CONTAINS BUTTONS !!!!
	public List<GameObject> subCategoryItemList; //,IAPPanelButts;
	public static GalleryHandler myInstance;


	// GalleryHandler Singleton
	public static GalleryHandler Instance
	{
		get{
			if(myInstance==null)
				myInstance=FindObjectOfType(typeof(GalleryHandler)) as GalleryHandler;
			return myInstance;
		}
	}


	// Use this for initialization
	void Start () {

//		bool some = false;
//		string s = some.ToString ();
//		Debug.Log (s);
//		Debug.Log (System.Convert.ToBoolean(s));
//		PlayerPrefs.DeleteAll ();

		if (!PlayerPrefsX.GetBool ("ImagesStoredMobile")) 
		{

			#if UNITY_ANDROID || UNITY_IOS  // on mobile only
			Debug.Log("Creating Files");
			List<string>    filePath = new List<string>();

			List<ImagePath> filePathInAsset = ImagePathHolder.LoadSubImagePathFromUnityAsset();

			foreach(ImagePath i in filePathInAsset)
				filePath.Add(i.imagePath);

			ImagePathHolder.ForMobileDeviceOnly (ImagePathHolder.LoadSubImageResourcePathFromUnityAsset(), filePath);
			#endif
		}
//		UnityEditor.AssetDatabase.Refresh ();
////		PlayerPrefsX.SetBool ("AllColors", false);
		GenerateMainCategoryList ();
		SetHomeScreen ();
		subCategoryListStartPos = categoryListStartPos;
	}



	void SetHomeScreen()
	{
        Debug.Log("DRAW HOME SCREEN");
		List<string> HomeButtNames = new List<string> ();
		List<string> HomeButtImages = new List<string> ();
		List<string> HomeButtColors = new List<string> ();
		HomeButtImages = ImagePathHolder.LoadHomeButtImages ();
		HomeButtNames = ImagePathHolder.LoadHomeButtNames ();
		HomeButtColors = ImagePathHolder.LoadHomeButtColors ();

		Header.GetComponent<Text> ().text = HomeButtNames [0];

		foreach (GameObject g in FooterText)
			g.GetComponent<Text> ().text = HomeButtNames [FooterText.IndexOf (g)];

		foreach (GameObject g in Footer)
			g.GetComponent<Image> ().sprite = Resources.Load<Sprite> (HomeButtImages [Footer.IndexOf (g)]);
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



	Color GetColorFromString(string color)
	{
		Debug.Log (color);
		string[] strings = color.Substring(1,color.Length-2).Split(","[0] );
		Debug.Log (strings.Length);
		Color output=Color.blue;
		for (int i = 0; i < 4; i++) {
			output[i] = System.Single.Parse(strings[i]);
		}
		return output;
	}

	

	// Load categories names with image
	void GenerateMainCategoryList()
	{
		mainCategoryItem.SetActive (true);
		// Texture2D image = new Texture2D(512,512,TextureFormat.PVRTC_RGBA4,false);
		List<string> category = ImagePathHolder.LoadCategoryFromAsset ();
		List<string> mainImg  = ImagePathHolder.LoadMainImagePathFromAsset ();

		foreach (string s in category) 
		{
            GameObject imageItem = GameObject.Instantiate(mainCategoryItem,
				new Vector3(0,categoryListStartPos,0), Quaternion.identity) as GameObject;

			imageItem.transform.SetParent(categoryPanel.transform,false);

			imageItem.GetComponent<RectTransform>().localScale = new Vector3(1f,1f,1f);
			imageItem.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f,categoryListStartPos,0f);

			// imageItem.transform.FindChild("Image").GetComponent<Image>().sprite=Sprite.Create(image.LoadImage(
			// 	ImagePathHolder.Instance.mainCategoryImages[ImagePathHolder.Instance.LoadCategoryFromAsset().IndexOf(s)]));
			// Debug.Log(mainImg[category.IndexOf(s)]);

			// img and text are both children of mainCategoryItem
			imageItem.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(mainImg[category.IndexOf(s)]);
			imageItem.transform.GetChild(0).GetComponent<Text>().text = s;
			imageItem.AddComponent<ImageDetails>();
			imageItem.GetComponent<ImageDetails>().CategoryName = s;
			// imageItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {StartCoroutine(GenerateSubCategoryList(imageItem));});
			// imageItem.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {StartCoroutine(GenerateSubCategoryList(imageItem));});


			// register button click on category image 
			imageItem.GetComponent<UnityEngine.UI.Button>()
				.onClick.AddListener(delegate{
					StartCoroutine(GenerateSubImages(imageItem));
				});


			// TODO : register button click on what !??!
			imageItem.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>()
				.onClick.AddListener(delegate{
					StartCoroutine(GenerateSubImages(imageItem));
				});

			categoryListStartPos -= 700f;
		}
		mainCategoryItem.SetActive (false);
	}








	// Generate images in each category: use WaitForSeconds for allow loading it
	public IEnumerator GenerateSubImages(GameObject g)
	{
		CircularLoader.Instance.Loader ();
		yield return new WaitForSeconds (1.5f);
		GenerateSubCategoryList (g);
	}







	// TODO : Generate images in each category ?!?
	public void  GenerateSubCategoryList(GameObject g)
	{
        Debug.Log("Image category hit, GenerateSubCategoryList");
		subCategoryItem.SetActive (true);

        categoryPanel.SetActive(false); // simo: moved from the bottom of the method; delete and decomment below if necessary
        // simo : enable gallery button in main screen
        Footer[0].SetActive(true);

		List<string> category        = ImagePathHolder.LoadCategoryFromAsset ();
		List<ImagePath> editedSubImg = ImagePathHolder.LoadSubImagePathFromAsset ();
		List<string> origResImg      = ImagePathHolder.LoadSubImageResourcePathFromAsset ();
		List<int> subImgCount        = ImagePathHolder.LoadSubImageCountFromAsset ();

		int startingImgIndex = 0;

		Header.GetComponent<Text> ().text = g.GetComponent<ImageDetails> ().CategoryName;

		for (int j=0; j<category.IndexOf(g.GetComponent<ImageDetails>().CategoryName); j++)
			startingImgIndex += subImgCount [j];

		for (int i=0; i<subImgCount[category.IndexOf(g.GetComponent<ImageDetails>().CategoryName)]; i++) 
		{
			Texture2D image = new Texture2D(1,1,TextureFormat.PVRTC_RGBA4,false);

			GameObject imageItem = GameObject.Instantiate(subCategoryItem,
				new Vector3(0,subCategoryListStartPos,0), Quaternion.identity) as GameObject;
			
			imageItem.transform.SetParent(subCategoryPanel.transform,false);
			imageItem.GetComponent<RectTransform>().localScale=new Vector3(1f,1f,1f);
			imageItem.GetComponent<RectTransform>().anchoredPosition3D=new Vector3(0f,subCategoryListStartPos,0f);

			// imageItem.transform.FindChild("Image").GetComponent<Image>().sprite=Sprite.Create(image.LoadImage(
			// ImagePathHolder.Instance.mainCategoryImages[ImagePathHolder.Instance.LoadCategoryFromAsset().IndexOf(s)]));
//			Debug.Log(ImagePathHolder.imagesInCategory[startingImgIndex+i].imagePath);
//			imageItem.transform.GetChild(0).GetComponent<Text>().text=g.GetComponent<ImageDetails>().CategoryName+" "+(i+1).ToString();
//			imageItem.transform.GetChild(1).GetComponent<Image>().sprite=
//				Resources.Load<Sprite>(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].imagePath);
//			image.LoadImage(DataManager.Instance.FileReaderBytes(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].imagePath));

			image.LoadImage(DataManager.Instance.FileReaderBytes(editedSubImg[startingImgIndex+i].imagePath));
			
			image.Apply();
			imageItem.transform.GetChild(1).GetComponent<Image>().sprite=
				Sprite.Create(image,new Rect(0f,0f,(float)1024,(float)1024),new Vector2(0.5f,0.5f)); 

//			if(ImagePathHolder.Instance.imagesInCategory[startingImgIndex+i].isLocked)
				imageItem.transform.GetChild(2).gameObject.SetActive(editedSubImg[startingImgIndex+i].isLocked);

			imageItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {OnImageHit(imageItem);});

			// CLICK su IMAGE for coloring
			imageItem.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>()
				.onClick.AddListener(delegate {
					OnImageHit(imageItem);
				});
			
			imageItem.AddComponent<ImageDetails>();
			imageItem.GetComponent<ImageDetails>().FileName=editedSubImg[startingImgIndex+i].imagePath;
			imageItem.GetComponent<ImageDetails>().ResName=origResImg[startingImgIndex+i];

			subCategoryItemList.Add(imageItem);
			subCategoryListStartPos-=700f;
		}

		CircularLoader.Instance.isLoading = false;
		subCategoryRect.SetActive (true);
        // simo : commented here and moved at the start, to avoid selecting nother category while waiting for the current selected loading 
		//categoryPanel.SetActive (false);
		subCategoryItem.SetActive (false);
	}



	// start a new game play when pressing on a image in category
	public void OnImageHit(GameObject g)
	{
        Debug.Log("IMAGE HIT");
		Debug.Log(g.GetComponent<ImageDetails> ().FileName);
		DataManager.Instance.selectedFileName = g.GetComponent<ImageDetails> ().FileName;
		DataManager.Instance.selectedResourceName = g.GetComponent<ImageDetails> ().ResName;

		DataManager.Instance.LoadScene ("NewGamePlay", 0.25f);
//		AutoFade.LoadLevel ("NewGamePlay", 0.5f, 0.5f, Color.white);
	}


	public void OnGalleryHit()
	{
		Debug.Log("Gallery was hit");
		SetHomeScreen ();

		foreach (GameObject g in subCategoryItemList)
			Destroy (g);
		Resources.UnloadUnusedAssets ();
		subCategoryItemList.Clear ();
		subCategoryRect.SetActive (false);
		categoryPanel.SetActive (true);

	}


	public void ReloadLevel()
	{
		Application.LoadLevel("Gallery");
	}


	public void OnMyDrawings()
	{
		DataManager.Instance.LoadScene ("MyDrawings", 0.25f);
//		AutoFade.LoadLevel ("MyDrawings", 0.5f, 0.5f, Color.white);
//		Application.LoadLevel("MyDrawings");
	}


	public void OnInspirationHit()
	{
		Application.LoadLevel("Inspiration");
	}

	/* simo
	public void OnHitLocked()
	{
		
		IAPPanel.SetActive (true);
		SetIAPPanel ();
		
	}
	*/


	/* simo
	void SetIAPPanel()
	{
	 // List<string> SKUs = new List<string> ();
	 // SKUs = ImagePathHolder.GetSKUs ();

		List<string> Pricing = new List<string> ();
		Pricing = ImagePathHolder.GetPricing ();
		foreach (GameObject g in IAPPanelButts) 
			g.transform.GetChild (1).GetComponent<UnityEngine.UI.Text> ().text = Pricing [IAPPanelButts.IndexOf (g)];
		IAPPanelButts [0].GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (delegate {InAppManager.Instance.SetUnlockedCategory ();});
		IAPPanelButts [1].GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (delegate {InAppManager.Instance.SetUnlockedColors ();});
		IAPPanelButts [2].GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (delegate {InAppManager.Instance.SetPremium ();});	
	}
	*/

	/*
	public void CloseIAP()
	{
		IAPPanel.SetActive (false);
	}
	*/


    public void openCredits(){
        SceneManager.LoadScene("Credits");
    }


    public void OnMyGamesHit()
    {
        Debug.Log("OnMyGamesHit was hit");
        SceneManager.LoadScene("MyGames");

    }
}
