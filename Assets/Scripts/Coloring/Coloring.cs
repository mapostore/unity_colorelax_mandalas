using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Coloring : MonoBehaviour {

    public GoogleMobileAdsScript adsManager; // simo
    [Header("UI Migration")]
    public bool useCanvasForDrawingSurface = true;
    [Header("Fill Effect")]
    public bool smoothFillAnimation = true;
    [Range(0.05f, 0.8f)]
    public float smoothFillDuration = 0.22f;
	public static Coloring myInstance;
	public static Coloring Instance
	{
		get{
			if(myInstance==null)
				myInstance=FindObjectOfType(typeof(Coloring)) as Coloring;
			return myInstance;
		}
	}

	public GUISkin customSkin;
	public GUIStyle customStyle;
	private Rect imageRect,palleteRect,selRect,unDoRect,saveRect,shareRect,topBannerRect,topWhiteRect,bannerRect,fbRect,instaRect,shareMessageRect,
	shareEmailRect,homeRect,popUpRect,startoverRect,ContinueRect,OrRect,toneRect,lockColorRect,
	saveImageRect,saveImgTextRect,inappPopupRect,premiumRect,IAPColorsRect,origImgRect,closeIAPRect,saveToGallRect,closeShareRect,
    watermarkImageRect,rateRect;
    private Rect imageViewportRect;
    private Rect shareTextRect, homeTextRect, undoTextRect; /// <summary>
    public Rect supportTextRect, supportImgRect, coverRectAvoidingTouch ; //simo
    ///  button label text
    /// </summary>
	private Rect[] pencilRect,IAPRect; 
    private Color[] palettePreviewColors;
    private Texture2D circularSwatchMask;
	public Texture2D selectedBorder,white,black,mainImage,testImage,selectedColor,unDo,shareFB,shareEmail,shareInsta,
	shareMessage,FBShare,home,popUpColor,fadedShare,fadedHome,fadedUndo,lockColor,saveImage,fadedsaveImg,transImg,IAPPopUp,Premium,priceBlock,restore,watermark,
	sharePopUp,rate,saveToGall,closeIAP,testwaterImage;
    public Texture2D supportImg;     //simo
    public Texture2D coverRectAvoidingTouchImg; // simo
	public Texture2D backgroundImg;  //simo : background
	private Rect backgroundImgRect;  //simo : background
	public Texture2D[] pencils,pencilTones;
	private float scale_x,scale_y,baseRes_X,baseRes_Y;
	int TouchCount;
	Touch firstTouch,secondTouch;
	bool[] pencilSelection;
	bool[] topSelection;
	bool colorSelected,showInapp,sendMail,sendSMS,moveToShare;
	GUIStyle popUpStyle;
	Color32 fillColor;
	List<string> Pricing = new List<string> ();

	public enum TouchEvent{Panning,Zooming,None};
	TouchEvent currentEvent,previousEvent;
	string[] categories ={"MANDALAS","ANIMALS","FLOWERS","WORLDCULTURES","ANTISTRESS"};
	string uploadedImgPath;
	string domain="http://www.koulumb.com/colorjoy/";
	string sharingDomain="http://apple.co/1XmQW30";
	#if UNITY_ANDROID
	AndroidJavaObject androidClass;
	#endif
	public Vector2 selRectPos,selRectDim;

	IEnumerator UploadImage()
	{
		WWWForm imgUploadForm = new WWWForm ();
		imgUploadForm.AddBinaryData("img",testwaterImage.EncodeToPNG(),"abcd");
		WWW imageUploadURL=new WWW(domain+"imageupload.php",imgUploadForm);
		yield return imageUploadURL;
		if (imageUploadURL.isDone) 
		{
			if(imageUploadURL.error==null)
			{
				Debug.Log(imageUploadURL.text.ToString());
				uploadedImgPath=imageUploadURL.text.ToString().Trim();
				uploadComplete=true;
			}
		}
	}
	bool uploadComplete=false;
	bool showWaterMark=false;
	bool waterMarkComeplete=false;
	Texture2D waterImage;
    private Canvas gameplayCanvas;
    private RectTransform backgroundRectTransformCanvas;
    private RawImage backgroundRawImageCanvas;
    private RectTransform drawingRectTransform;
    private RawImage drawingRawImage;
    private Texture2D lastAssignedDrawingTexture;
//	void MoveToShare()
//	{
//		moveToShare = false;
//	}


	void  DrawWatermark()
	{
		if(watermark==null)
			watermark=Resources.Load<Texture2D>(ImagePathHolder.LoadWaterMark());
//		testwaterImage = new Texture2D (watermark.width, watermark.height,TextureFormat.RGB24,false); 
//		testwaterImage.SetPixels (watermark.GetPixels ());
//		testwaterImage.Apply ();
		int idx = 0;
		Color[] wmm = watermark.GetPixels ();
		Color[] bgm = mainImage.GetPixels ();
		//			if(idx<0 || idx+wmm.Length-1>bgm.Length)return;
		for(int i=0;i<wmm.Length;i++){
			wmm[i+idx]=(wmm[i]*wmm[i].a+bgm[i+idx]*bgm[i+idx].a*(1-wmm[i].a))/(wmm[i].a+bgm[idx+i].a*(1-wmm[i].a));
		}
		watermark.SetPixels(bgm);
		watermark.Apply();
//		if(saveGalleryImage)
//		{
//			saveGalleryImage=false;
//			SaveToGallery();
//		}
//
//		int idx = (testwaterImage.width - mainImage.width) / 2;
//		int idy = (testwaterImage.height - mainImage.height) / 2;
//		for (int i=0; i<mainImage.width; i++) 
//		{
//			idx++;
//			idy=(testwaterImage.height - mainImage.height) / 2;
//			for (int j=0; j<mainImage.height; j++)
//			{
//				testwaterImage.SetPixel (idx, idy,mainImage.GetPixel(i,j));
//				idy++;
//			}	
//		}
//		testwaterImage.Apply ();	
//		showWaterMark = false;
//		waterMarkComeplete = true;
//		DataManager.Instance.waterMarkedImage = new Texture2D (watermark.width, watermark.height);
//		DataManager.Instance.waterMarkedImage.SetPixels (watermark.GetPixels ());
//		DataManager.Instance.waterMarkedImage.Apply ();
//		Application.LoadLevel("Share");
//		return mainImage;
	}
	
	
	//		waterImage.SetPixels32(pix1);
	////		tex1.Apply();
	//		waterImage.Apply();
	//		showWaterMark = false;
	//		waterMarkComeplete = true;
	//		return waterImage;
	////		imageRect = new Rect (170 * scale_x, 350 * scale_y, 1200 * scale_x, 1200 * scale_y);
	//
	//	}
	void DrawShare()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), transImg);
		if (((float)Screen.width / (float)Screen.height) > 0.7f) {
			GUI.DrawTexture (new Rect (200 * scale_x, 500 * scale_y, 1100 * scale_x, 1000 * scale_y), sharePopUp);
			GUI.DrawTexture (new Rect (1170 * scale_x, 520 * scale_y, 75 * scale_x, 60 * scale_y), closeIAP);
			GUI.DrawTexture (new Rect (475 * scale_x, 670 * scale_y, 150 * scale_x, 150 * scale_y), FBShare);
			GUI.DrawTexture (new Rect (875 * scale_x, 670 * scale_y, 150 * scale_x, 150 * scale_y), shareInsta);
			GUI.DrawTexture (new Rect (475 * scale_x, 920 * scale_y, 150 * scale_x, 150 * scale_y), shareEmail);
			GUI.DrawTexture (new Rect (875 * scale_x, 920 * scale_y, 150 * scale_x, 150 * scale_y), shareMessage);
			GUI.DrawTexture (new Rect (475 * scale_x, 1220 * scale_y, 150 * scale_x, 150 * scale_y), rate);
			GUI.DrawTexture (new Rect (875 * scale_x, 1220 * scale_y, 150 * scale_x, 150 * scale_y), saveToGall);
			customStyle.normal.textColor = Color.black;
			customStyle.fontSize = Mathf.CeilToInt (35 * scale_y);
			GUI.Label (new Rect (455 * scale_x, 830 * scale_y, 200 * scale_x, 100 * scale_y), "FACEBOOK", customStyle);
			GUI.Label (new Rect (855 * scale_x, 830 * scale_y, 200 * scale_x, 100 * scale_y), "INSTAGRAM", customStyle);
			GUI.Label (new Rect (485 * scale_x, 1080 * scale_y, 200 * scale_x, 100 * scale_y), "EMAIL", customStyle);
			GUI.Label (new Rect (875 * scale_x, 1080 * scale_y, 200 * scale_x, 100 * scale_y), "MESSAGE", customStyle);
			GUI.Label (new Rect (495 * scale_x, 1380 * scale_y, 200 * scale_x, 100 * scale_y), "RATE US", customStyle);
			GUI.Label (new Rect (905 * scale_x, 1380 * scale_y, 200 * scale_x, 100 * scale_y), "SAVE", customStyle);
		}
		else
		{
			
			GUI.DrawTexture (new Rect (200 * scale_x, 500 * scale_y, 1150 * scale_x, 1000 * scale_y), sharePopUp);
			GUI.DrawTexture (new Rect (1240 * scale_x, 520 * scale_y, 75 * scale_x, 60 * scale_y), closeIAP);
			GUI.DrawTexture (new Rect (475 * scale_x, 690 * scale_y, 150 * scale_x, 120 * scale_y), FBShare);
			GUI.DrawTexture (new Rect (875 * scale_x, 690 * scale_y, 150 * scale_x, 120 * scale_y), shareInsta);
			GUI.DrawTexture (new Rect (475 * scale_x, 940 * scale_y, 150 * scale_x, 120 * scale_y), shareEmail);
			GUI.DrawTexture (new Rect (875 * scale_x, 940 * scale_y, 150 * scale_x, 120 * scale_y), shareMessage);
			GUI.DrawTexture (new Rect (475 * scale_x, 1240 * scale_y, 150 * scale_x, 120 * scale_y), rate);
			GUI.DrawTexture (new Rect (875 * scale_x, 1240 * scale_y, 150 * scale_x, 120 * scale_y), saveToGall);
			customStyle.normal.textColor = Color.black;
			customStyle.fontSize = Mathf.CeilToInt (35 * scale_y);
			GUI.Label (new Rect (445 * scale_x, 830 * scale_y, 200 * scale_x, 100 * scale_y), "FACEBOOK", customStyle);
			GUI.Label (new Rect (845 * scale_x, 830 * scale_y, 200 * scale_x, 100 * scale_y), "INSTAGRAM", customStyle);
			GUI.Label (new Rect (485 * scale_x, 1080 * scale_y, 200 * scale_x, 100 * scale_y), "EMAIL", customStyle);
			GUI.Label (new Rect (855 * scale_x, 1080 * scale_y, 200 * scale_x, 100 * scale_y), "MESSAGE", customStyle);
			GUI.Label (new Rect (475 * scale_x, 1380 * scale_y, 200 * scale_x, 100 * scale_y), "RATE US", customStyle);
			GUI.Label (new Rect (885 * scale_x, 1380 * scale_y, 200 * scale_x, 100 * scale_y), "SAVE", customStyle);
			
		}
	}


	void DrawIAP()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), IAPPopUp);


		customSkin.customStyles [0].alignment = TextAnchor.MiddleCenter;
		customSkin.customStyles [0].normal.textColor = Color.black;
		customSkin.customStyles [0].fontSize = Mathf.CeilToInt (105 * scale_y);
		GUI.Label(new Rect (630 * scale_x, 85 * scale_y, 250 * scale_x, 180 * scale_y),"STORE",customSkin.customStyles[0]);
		closeIAPRect = new Rect (1280 * scale_x, 105 * scale_y, 95 * scale_x, 100 * scale_y);
		GUI.DrawTexture (new Rect (1280 * scale_x, 105 * scale_y, 95 * scale_x, 100 * scale_y), closeIAP);
		customStyle.fontSize = Mathf.CeilToInt (66 * scale_y);
		customStyle.normal.textColor = Color.black;
		Debug.Log (customSkin.customStyles [0].alignment);
		customSkin.customStyles [0].normal.textColor = Color.white;
		customSkin.customStyles [0].fontSize = Mathf.CeilToInt (55 * scale_y);
		customSkin.customStyles [0].alignment = TextAnchor.MiddleLeft;
		for (int i=0; i<IAPRect.Length; i++) 
		{
			GUI.DrawTexture(IAPRect[i],priceBlock);
			switch(i)
			{
			case 0:GUI.Label(new Rect(IAPRect[i].x+50*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),"Unlock All Categories",customSkin.customStyles[0]);
				GUI.Label(new Rect(IAPRect[i].x+850*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),Pricing[0],customSkin.customStyles[0]);
					break;
			case 1:GUI.Label(new Rect(IAPRect[i].x+50*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),"Unlock All Colors",customSkin.customStyles[0]);
				GUI.Label(new Rect(IAPRect[i].x+850*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),Pricing[1],customSkin.customStyles[0]);
				    break;
			case 2:GUI.Label(new Rect(IAPRect[i].x+50*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),"Unlock Everything",customSkin.customStyles[0]);
				GUI.Label(new Rect(IAPRect[i].x+850*scale_x,IAPRect[i].y+20*scale_y,IAPRect[i].width,IAPRect[i].height),Pricing[2],customSkin.customStyles[0]);
					break;
			}
		}
//		GUI.Label (new Rect (210*scale_x,765*scale_y,300*scale_x,200*scale_y), "ALL COLORS",customStyle);
//		GUI.Label (new Rect (210*scale_x,885*scale_y,300*scale_x,200*scale_y), "MANDALAS",customStyle);
//		GUI.Label (new Rect (210*scale_x,1005*scale_y,300*scale_x,200*scale_y), "ANIMALS",customStyle);
//		//		GUI.Label (new Rect (210*scale_x,1125*scale_y,300*scale_x,200*scale_y), "ARTS",customStyle);
//		GUI.Label (new Rect (210*scale_x,1125*scale_y,300*scale_x,200*scale_y), "FLOWERS",customStyle);
//		GUI.Label (new Rect (210*scale_x,1245*scale_y,300*scale_x,200*scale_y), "WORLD CULTURES",customStyle);
//		GUI.Label (new Rect (210*scale_x,1365*scale_y,300*scale_x,200*scale_y), "ANTI-STRESS THERAPY",customStyle);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 750 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 870 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 990 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		//		GUI.DrawTexture (new Rect (1080 * scale_x, 1110 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 1110 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 1230 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		GUI.DrawTexture (new Rect (1080 * scale_x, 1350 * scale_y, 220 * scale_x, 100 * scale_y), priceBlock);
//		customStyle.fontSize = Mathf.CeilToInt (65 * scale_y);
//		customStyle.normal.textColor = Color.white;
//		GUI.Label (new Rect (1100 * scale_x, 765 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
//		GUI.Label (new Rect (1100 * scale_x, 885 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
//		GUI.Label (new Rect (1100 * scale_x, 1005 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
//		GUI.Label (new Rect (1100 * scale_x, 1125 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
//		GUI.Label (new Rect (1100 * scale_x, 1245 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
//		GUI.Label (new Rect (1100 * scale_x, 1365 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
		//		GUI.Label (new Rect (1100 * scale_x, 1485 * scale_y, 200 * scale_x, 150 * scale_y), "$0.99", customStyle); 
		//		GUI.DrawTexture (new Rect (630 * scale_x, 1600 * scale_y, 250 * scale_x, 60 * scale_y), restore);
	} 



	Color GetColorFromString(string color)
	{

		string[] strings = color.Substring(1,color.Length-2).Split(","[0] );

		Color output=Color.blue;
		for (int i = 0; i < 4; i++) {
			output[i] = System.Single.Parse(strings[i]);
		}
		return output;
	}



	void DrawSavedImage()
	{

		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), transImg);
		popUpStyle = new GUIStyle (GUI.skin.label);
		popUpStyle.normal.textColor = Color.black;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height),popUpColor);
		popUpStyle.fontSize = 20;
		//customSkin.customStyles[0].font=Resources.Load<Font>("font/calibri");
        customSkin.customStyles[0].font = Resources.Load<Font>("font/mandala"); // simo
		customSkin.customStyles [0].fontSize = Mathf.CeilToInt(110 * scale_y);
		customSkin.customStyles [0].normal.textColor = GetColorFromString(ImagePathHolder.LoadPopUpButtonColors()[0]);
//		GUI.Label (startoverRect, "START OVER", customSkin.customStyles[0]);
		GUI.Label (startoverRect,ImagePathHolder.LoadPopUpButtonNames()[0], customSkin.customStyles[0]);
		//		customSkin.customStyles [0].normal.textColor = Color.red;
		//		GUI.Label (OrRect, "OR", customSkin.customStyles[0]);
		customSkin.customStyles [0].normal.textColor = GetColorFromString(ImagePathHolder.LoadPopUpButtonColors()[1]);
//		GUI.Label (ContinueRect, "CONTINUE",customSkin.customStyles[0]);
		GUI.Label (ContinueRect,ImagePathHolder.LoadPopUpButtonNames()[1],customSkin.customStyles[0]);
	}


	void MessagePosted()
	{
		//		eagerShare=false;
		//		showShares = false;
		palleteRect.y=2048*scale_y;
		bannerRect.y=1848*scale_y;
		//		shareMessageRect.y =
		shareEmailRect.y = fbRect.y = instaRect.y = 1898 * scale_y;
		StartCoroutine(HidePallete(palleteRect,!hideBanner,(2048*scale_y),(1848*scale_y)));
	}



	void ShowShareBanner()
	{
		colorSelected = false;
		palleteRect.y=1848*scale_y;
		//		shareMessageRect.y = 
		shareEmailRect.y = fbRect.y = instaRect.y =bannerRect.y=2048*scale_y;
		StartCoroutine(HidePallete(palleteRect,hideBanner,(1848*scale_y),(2048*scale_y)));
	}


	bool showSavedPopUp=false;
    bool savedChoiceTransitionRunning=false;
    float savedChoiceFadeAlpha=0f;
	// Use this for initialization
	
    private Rect GetAspectFitRect(Rect container, int texWidth, int texHeight)
    {
        if (texWidth <= 0 || texHeight <= 0)
            return container;

        float texAspect = (float)texWidth / (float)texHeight;
        float containerAspect = container.width / container.height;

        float fitWidth;
        float fitHeight;
        if (texAspect > containerAspect) {
            fitWidth = container.width;
            fitHeight = fitWidth / texAspect;
        } else {
            fitHeight = container.height;
            fitWidth = fitHeight * texAspect;
        }

        float fitX = container.x + (container.width - fitWidth) * 0.5f;
        float fitY = container.y + (container.height - fitHeight) * 0.5f;
        return new Rect(fitX, fitY, fitWidth, fitHeight);
    }

    private void ClampImageRectToViewport()
    {
        if (origImgRect.width <= 0f || origImgRect.height <= 0f)
            return;

        const float maxZoomFactor = 10f;
        Vector2 center = imageRect.center;

        // Keep zoom in a valid range while preserving aspect ratio.
        float minScale = Mathf.Max(origImgRect.width / imageRect.width, origImgRect.height / imageRect.height);
        if (minScale > 1f) {
            imageRect.width *= minScale;
            imageRect.height *= minScale;
            imageRect.center = center;
            center = imageRect.center;
        }

        float maxScale = Mathf.Min((origImgRect.width * maxZoomFactor) / imageRect.width,
                                   (origImgRect.height * maxZoomFactor) / imageRect.height);
        if (maxScale < 1f) {
            imageRect.width *= maxScale;
            imageRect.height *= maxScale;
            imageRect.center = center;
        }

        if (imageRect.width <= origImgRect.width) {
            imageRect.x = origImgRect.x;
        } else {
            float minX = origImgRect.x + origImgRect.width - imageRect.width;
            imageRect.x = Mathf.Clamp(imageRect.x, minX, origImgRect.x);
        }

        if (imageRect.height <= origImgRect.height) {
            imageRect.y = origImgRect.y;
        } else {
            float minY = origImgRect.y + origImgRect.height - imageRect.height;
            imageRect.y = Mathf.Clamp(imageRect.y, minY, origImgRect.y);
        }
    }


    void LoadSavedImage()
	{
		showSavedPopUp=false;
		string testFile = DataManager.Instance.selectedFileName;//get file-name of selected image in categories
		mainImage=new Texture2D(testImage.width, testImage.height);

		Debug.Log (testFile);
		byte[] savedBytes = DataManager.Instance.ReadWorkingImageBytes(testFile, DataManager.Instance.selectedResourceName);
		if (savedBytes != null && savedBytes.Length > 0) {
			mainImage.LoadImage(savedBytes);// load saved colors into image for coloring
		} else {
			mainImage.SetPixels(testImage.GetPixels());
		}
		
		mainImage.Apply ();
	}


	void LoadActualImage()
	{
		DataManager.Instance.ResetWorkingToOriginal(DataManager.Instance.selectedFileName, DataManager.Instance.selectedResourceName);
		DataManager.Instance.MarkProgress(DataManager.Instance.selectedFileName, false);
		PlayerPrefs.SetInt (DataManager.Instance.selectedFileName, 0);
		showSavedPopUp=false;
		byte[] originalBytes = DataManager.Instance.ReadOriginalImageBytes(DataManager.Instance.selectedFileName, DataManager.Instance.selectedResourceName);
		mainImage = new Texture2D(2, 2, TextureFormat.RGBA32, false);
		if (originalBytes != null && originalBytes.Length > 0 && mainImage.LoadImage(originalBytes, false)) {
			testImage = new Texture2D(mainImage.width, mainImage.height, TextureFormat.RGBA32, false);
			testImage.SetPixels(mainImage.GetPixels());
		} else {
			testImage = Resources.Load <Texture2D>(DataManager.Instance.selectedResourceName);
			mainImage = new Texture2D (testImage.width, testImage.height);
			mainImage.SetPixels (testImage.GetPixels ());
		}
		mainImage.Apply ();
	}

	void OnFBInitiated()
	{
		Debug.Log("Fb initialized");
	}
	
	void Start () {
		//		testImage = Resources.Load<Texture2D> (PathStorage.GetFilePathInResource (DataManager.Instance.selectedFileName));
		//		FB.Init (OnFBInitiated, null, null);
		if(DataManager.Instance!=null)
			Debug.Log (DataManager.Instance.selectedFileName);

		DataManager.Instance.EnsureImageStateFiles(DataManager.Instance.selectedFileName, DataManager.Instance.selectedResourceName);

		testImage = new Texture2D (1024, 1024, TextureFormat.RGBA32, false);
		byte[] selectedImageBytes = DataManager.Instance.ReadWorkingImageBytes(DataManager.Instance.selectedFileName, DataManager.Instance.selectedResourceName);
		if (selectedImageBytes != null && selectedImageBytes.Length > 0) {
			testImage.LoadImage (selectedImageBytes);
		} else {
			Texture2D defaultImage = Resources.Load<Texture2D> (DataManager.Instance.selectedResourceName);
			if (defaultImage != null) {
				testImage = new Texture2D(defaultImage.width, defaultImage.height, TextureFormat.RGBA32, false);
				testImage.SetPixels(defaultImage.GetPixels());
			}
		}
		testImage.Apply ();
//		testImage = Resources.Load<Texture2D> (DataManager.Instance.selectedResourceName);
		if (!DataManager.Instance.fromDrawings
			&& selectedImageBytes != null
			&& selectedImageBytes.Length > 0
			&& DataManager.Instance.HasStartedProgress(DataManager.Instance.selectedFileName)) {
			showSavedPopUp=true;
		}
//		
		Init ();
		
	}


	void PrepareAttachment()
	{
		System.IO.File.WriteAllBytes (Application.persistentDataPath+"/tempImage.png",mainImage.EncodeToPNG ());
		uploadedImgPath = Application.persistentDataPath + "/tempImage.png";
		if (sendMail) {
			sendMail=!sendMail;
			SendMail();
		}
	}


	void SendMail()
	{
		//		Application.OpenURL("mailto:"+"?subject=Check%20out%20this%20image&body="+uploadedImgPath); 
		
		uploadComplete = false;
		showSharePopUp = false;
		sendMail = false;
		SendMailWithAttachment.SendMailAttach("Check this coloring app, it is awesome! "+sharingDomain,testwaterImage.EncodeToPNG());
		
	}


	void SendSMS()
	{
		//		if (Application.platform == RuntimePlatform.IPhonePlayer) 
		//		{
		//			Application.OpenURL("sms:"+"&body="+uploadedImgPath+"%20Check%20out%20this%20image%20made%20through%20this%20new%20application%20called%20Color%20Joy!");
		//		}
		
		uploadComplete = false;
		showSharePopUp = false;
		sendSMS = false;
		SendMailWithAttachment.SendMessageWithImage("Check this coloring app, it is awesome! "+sharingDomain,testwaterImage.EncodeToPNG());
		
	}


	bool hitOnthis = false;


	bool FadeButton(Rect rt)
	{
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonDown (0)) {

			hitOnthis=true;
		}
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButton (0)) {
			//			hitOnthis=true;
		}
		if (hitOnthis && rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonUp (0)) {
			hitOnthis = false;
			for (int i=0; i<topSelection.Length; i++)
				topSelection [i] = false;
			//Debug.Log(!hitOnthis);
			return (!hitOnthis);
		}
		else if (hitOnthis && Input.GetMouseButtonUp (0))
		{
			for (int i=0; i<topSelection.Length; i++)
				topSelection [i] = false;
			return false;
		}
		return false;
	}



	bool ButtonFade(Rect rt,int index)
	{
		
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonDown (0)) {
			if(index>=0)
				topSelection[index]=true;
			hitOnthis=true;
		}
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButton (0)) {
			//			hitOnthis=true;
		}
		if (hitOnthis && rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonUp (0)) {
			hitOnthis = false;
			for (int i=0; i<topSelection.Length; i++)
				topSelection [i] = false;
			//Debug.Log(!hitOnthis);
			return (!hitOnthis);
		}
		else if (hitOnthis && Input.GetMouseButtonUp (0))
		{
			for (int i=0; i<topSelection.Length; i++)
				topSelection [i] = false;
			return false;
		}
		return false;
	}



	void FindPixelWithinTone(int hitPixelX,int hitPixelY )
	{
		colorSelected=true;
		colorHolds=true;
		float startPixelToneX, startPixelToneY,startPixelToneW,startPixelToneH;
		startPixelToneX = 0;
		startPixelToneY = toneRect.y;
		startPixelToneW = toneRect.width / 11f;
		startPixelToneH = toneRect.height;
		do
		{
			if(hitPixelX>startPixelToneX&&hitPixelX<(startPixelToneX+startPixelToneW))	
			{
				break;
			}
			else
			{
				startPixelToneX+=startPixelToneW;
			}
		}while(startPixelToneX<Screen.width);
		selRect.x = startPixelToneX;
		selRect.y = startPixelToneY;
		selRect.width = toneRect.width / 11f;
		selRect.height = startPixelToneH;
		
	}



	void UpdateTouches()
	{
		TouchCount = Input.touchCount;
		if (TouchCount ==2) {
			firstTouch = Input.GetTouch (0);
			secondTouch = Input.GetTouch (1);
		}
		else if (Input.touchCount==1){
			firstTouch = Input.GetTouch (0);
		}
	}



	int GetTouchStatus()
	{
		TouchCount = Input.touchCount;
		if (TouchCount == 1) 
		{
			
			if(firstTouch.phase==TouchPhase.Moved)//Panning
				return 1;
			else if(firstTouch.phase==TouchPhase.Ended)//coloring
				return 2;
			
			
		}
		if (TouchCount == 2) 
		{
			firstTouch = Input.GetTouch (0);
			secondTouch = Input.GetTouch (1);
			if(firstTouch.phase==TouchPhase.Moved||secondTouch.phase==TouchPhase.Moved)//zooming
				return 3;
		}
		return 0;
	}



	void SaveToGallery()
	{
		showSharePopUp=false;
		#if UNITY_IPHONE&&!UNITY_EDITOR
		SaveImage.SaveToGallery(DataManager.Instance.selectedFileName,testwaterImage.EncodeToPNG());
		#endif
		#if UNITY_ANDROID
		AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION");
		int sdkInt = versionClass.GetStatic<int>("SDK_INT");
		if (sdkInt < 29 &&
			!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
		{
			UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
			return;
		}
		androidClass = new AndroidJavaObject("com.example.imagesave.SaveImageUnityBridgeCompat");
		androidClass.CallStatic("CallSaveImage",mainImage.EncodeToPNG());
		#endif
//		TextureScale.Bilinear (testwaterImage, 512, 512);
//		PlayerPrefs.SetInt ("SavedWaterMark", 1);
//		DataManager.Instance.StoreWatermark (testwaterImage.EncodeToPNG (), DataManager.Instance.selectedFileName);
//		StoreImageChange ();
	}


    // DRAW THE GUI
	void Init()
	{
		//		Chartboost.showInterstitial (CBLocation.Default);
		sendMail = sendSMS = false;
		TouchCount = 0;

		topSelection = new bool[4];
		for (int i=0; i<4; i++)
			topSelection [i] = false;
		mainImage = new Texture2D (testImage.width, testImage.height);
		mainImage.SetPixels (testImage.GetPixels ());
		mainImage.Apply ();
		//		testwaterImage = new Texture2D (watermark.width, watermark.height);
		//		testwaterImage.SetPixels (watermark.GetPixels ());
		//		testwaterImage.Apply ();
		baseRes_X = 1536f;
		baseRes_Y = 2048f;
		scale_x = Screen.width/baseRes_X;
		scale_y = Screen.height/baseRes_Y;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (scale_x, scale_y, 1));
		colorSelected = false;
		showInapp = false;
		// topWhiteRect = new Rect (0, 0, Screen.width, 160 * scale_y);
		topWhiteRect = new Rect (0, 0, Screen.width, 130 * scale_y); // simo
        //topBannerRect = new Rect(0, 220 * scale_y, Screen.width, 10 * scale_y); 
        // simo
        topBannerRect = new Rect(0, 220 * scale_y, Screen.width, 0 * scale_y);

		fbRect = new Rect (475 * scale_x, 670 * scale_y, 150 * scale_x, 200 * scale_y);
		//		shareEmailRect=new Rect (675 * scale_x, 1888 * scale_y, 150 * scale_x, 140 * scale_y);
		shareEmailRect = new Rect (475 * scale_x, 920 * scale_y, 150 * scale_x, 200 * scale_y);
		instaRect = new Rect (875 * scale_x, 670 * scale_y, 150 * scale_x, 200 * scale_y);
		//		instaRect=new Rect (875 * scale_x, 1888 * scale_y, 150 * scale_x, 140 * scale_y);
		//		shareMessageRect=new Rect (975 * scale_x, 1888 * scale_y, 150 * scale_x, 120 * scale_y);
		shareMessageRect = new Rect (875 * scale_x, 920 * scale_y, 150 * scale_x, 200 * scale_y);
		saveRect = new Rect (Screen.width - 250 * scale_x, 1600 * scale_y, 200 * scale_x, 200 * scale_y);
		// homeRect = new Rect (90 * scale_x, 20 * scale_y, 170 * scale_x, 125 * scale_y);
		homeRect = new Rect (90 * scale_x, 20 * scale_y, 140 * scale_x, 95 * scale_y);
		homeTextRect = new Rect (100 * scale_x, 165 * scale_y, 270 * scale_x, 225 * scale_y);
		palleteRect = new Rect (0 * scale_x, 1848 * scale_y - (float)(Screen.height * 0.08f), Screen.width, 200 * scale_y);

        // simo : voiding touch under pencils+tone rect  rect  
        coverRectAvoidingTouch = new Rect(0, 1948 * scale_y - (float)(Screen.height * 0.2f), Screen.width, 330 * scale_y);
        // simo : support rect  SupportBanner
        supportImgRect  = new Rect((float)(Screen.width * 0.9f), (float)(Screen.height * 0.94f), 140 * scale_x, 100 * scale_y);
        //supportTextRect = new Rect(supportImgRect.x-Screen.width * 0.5f-20, (float)(Screen.height * 0.95f), Screen.width * 0.5f, 0 * scale_y);
        supportTextRect = new Rect((float)(Screen.width * 0.01f), (float)(Screen.height * 0.95f), Screen.width*0.40f, 0 * scale_y);
		// simo : background rect 
		backgroundImgRect = new Rect(-10, 0, (float)(Screen.width*1.2), (float)(Screen.height*1.2));
        //toneRect = new Rect(0, 1948 * scale_y, Screen.width, 150 * scale_y);
        //simo : TONE RECT MOVED

        // Keep a small visual gap between swatch row and linear palette.
        toneRect = new Rect (0, 1948 * scale_y-(float)(Screen.height * 0.12f) + 22f * scale_y, Screen.width, 130 * scale_y);
		bannerRect=new Rect (0 * scale_x, 1848 * scale_y, Screen.width, 200 * scale_y);
		pencilRect = new Rect[pencils.Length-1];
        palettePreviewColors = new Color[pencilRect.Length];

		saveToGallRect=new Rect (875 * scale_x, 1220 * scale_y, 150 * scale_x, 200 * scale_y);
		pencilSelection = new bool[pencilRect.Length];
        // pencils positions
		for (int pencilIndex=1; pencilIndex<pencils.Length; pencilIndex++) {
            //pencilRect [pencilIndex-1] = new Rect (0 + (140* (pencilIndex - 1)) * scale_x, 1808 * scale_y , 142 * scale_x, 255 * scale_y);
            // simo : pencils RECT MOVED
            pencilRect [pencilIndex-1] = new Rect (0 + (140* (pencilIndex - 1)) * scale_x, 1808 * scale_y - (float)(Screen.height*0.13f), 142 * scale_x, 255 * scale_y);
			pencilSelection[pencilIndex-1]=false;
		}
        BuildPalettePreviewColors();
        EnsureCircularSwatchMask();
		if(scale_x==scale_y)
		{
            imageViewportRect = new Rect (170 * scale_x, 350 * scale_y, 1200 * scale_x, 1200 * scale_y);
			watermarkImageRect = new Rect (120 * scale_x, 320 * scale_y, 1300 * scale_x, 1300 * scale_y);
		}
		else
		{
            imageViewportRect = new Rect (90 * scale_x, 350 * scale_y, 1350 * scale_x, 1000 * scale_y);
			watermarkImageRect = new Rect (70 * scale_x, 320 * scale_y, 1450 * scale_x, 1040 * scale_y);
		}

        origImgRect = GetAspectFitRect(imageViewportRect, mainImage.width, mainImage.height);
		Pricing = ImagePathHolder.GetPricing ();
		imageRect = origImgRect;
		// customSkin.customStyles[0].font=Resources.Load<Font>("font/calibri");
		// customStyle.font=Resources.Load<Font>("font/calibri");
        customSkin.customStyles[0].font = Resources.Load<Font>("font/mandala");  //simo
        customStyle.font = Resources.Load<Font>("font/mandala");  //simo

		selRect = new Rect (Screen.width / 2 - (50 * scale_x), 1600 * scale_y, 100 * scale_x, 100 * scale_y);
		inappPopupRect = new Rect (120 * scale_x, 420 * scale_y, 1300 * scale_x, 1150 * scale_y);
		// unDoRect=new Rect (705*scale_x, 20 * scale_y, 165 * scale_x, 125 * scale_y);
		unDoRect=new Rect (705*scale_x, 20 * scale_y, 115 * scale_x, 95 * scale_y); // simo

		undoTextRect = new Rect (700*scale_x, 165 * scale_y, 250 * scale_x, 225 * scale_y);
		saveImageRect = new Rect (910 * scale_x, 20 * scale_y, 155 * scale_x, 125 * scale_y);
		saveImgTextRect=new Rect (915 * scale_x, 165 * scale_y, 255 * scale_x, 225 * scale_y);
		//shareRect= new Rect (1310 * scale_x, 20 * scale_y, 165 * scale_x, 125 * scale_y);
		shareRect= new Rect (1310 * scale_x, 20 * scale_y, 110 * scale_x, 90 * scale_y); // simo

		rateRect=new Rect (475 * scale_x, 1220 * scale_y, 250 * scale_x, 250 * scale_y);
		shareTextRect=new Rect (1315 * scale_x, 165 * scale_y, 155 * scale_x, 125 * scale_y);
		popUpRect = new Rect (165 * scale_x, 400 * scale_y, 1200 * scale_x, 950 * scale_y);
		startoverRect = new Rect (550 * scale_x, 650 * scale_y, 400 * scale_x, 300 * scale_y);
		ContinueRect = new Rect (560 * scale_x, 1050 * scale_y, 400 * scale_x, 300 * scale_y);
		closeIAPRect = new Rect (1220 * scale_x, 415 * scale_y, 175 * scale_x, 120 * scale_y);
		OrRect=new Rect(startoverRect.x+(20*scale_x),startoverRect.y + (150 * scale_y), startoverRect.width, startoverRect.height);
		selectedColor = new Texture2D (100, 100);
		usedColors=new Stack<Color32>(); 
		IAPColorsRect = new Rect (1020 * scale_x, 750 * scale_y, 220 * scale_x, 100 * scale_y);
		IAPRect = new Rect[3];
		for (int IAPindex=0; IAPindex<IAPRect.Length; IAPindex++)
			IAPRect [IAPindex] = new Rect (220 * scale_x, (700+(240*IAPindex)) * scale_y, 1080 * scale_x, 150 * scale_y);
		premiumRect = new Rect (180 * scale_x, 540 * scale_y, 1150 * scale_x, 180 * scale_y);
		if(((float)(Screen.width)/(float)(Screen.height))<0.7f)
			closeShareRect = new Rect (1210 * scale_x, 500 * scale_y, 175 * scale_x, 120 * scale_y);
		else
			closeShareRect = new Rect (1170 * scale_x, 520 * scale_y, 175 * scale_x, 120 * scale_y);
		if (DataManager.Instance.fromDrawings) 
		{
			LoadSavedImage();

		}
        EnsureGameplayCanvas();
		GetUIImages ();
		Resources.UnloadUnusedAssets();
	}


    void BuildPalettePreviewColors()
    {
        if (palettePreviewColors == null || pencilTones == null)
            return;

        for (int i = 0; i < palettePreviewColors.Length; i++)
        {
            int toneIndex = Mathf.Clamp(i, 0, pencilTones.Length - 1);
            Texture2D toneTexture = pencilTones[toneIndex];
            if (toneTexture == null)
            {
                palettePreviewColors[i] = Color.white;
                continue;
            }

            Color sampled = toneTexture.GetPixelBilinear(0.5f, 0.5f);
            sampled.a = 1f;
            palettePreviewColors[i] = sampled;
        }
    }


    void EnsureCircularSwatchMask()
    {
        if (circularSwatchMask != null)
            return;

        const int size = 128;
        circularSwatchMask = new Texture2D(size, size, TextureFormat.RGBA32, false);
        circularSwatchMask.wrapMode = TextureWrapMode.Clamp;
        circularSwatchMask.filterMode = FilterMode.Bilinear;

        float center = (size - 1) * 0.5f;
        float radius = center;
        Color clear = new Color(1f, 1f, 1f, 0f);
        Color solid = new Color(1f, 1f, 1f, 1f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                circularSwatchMask.SetPixel(x, y, dist <= radius ? solid : clear);
            }
        }
        circularSwatchMask.Apply();
    }


    void ApplyCurrentFillColorPreview()
    {
        for (int i = 0; i <= 100; i++)
        {
            for (int j = 0; j <= 100; j++)
                selectedColor.SetPixel(i, j, fillColor);
        }
        selectedColor.Apply();
    }


    void SelectDefaultToneColor(int toneIndex)
    {
        selectedToneIndex = toneIndex;

        Color chosen = (palettePreviewColors != null && toneIndex >= 0 && toneIndex < palettePreviewColors.Length)
            ? palettePreviewColors[toneIndex]
            : pencilTones[toneIndex].GetPixelBilinear(0.5f, 0.5f);

        if (chosen == Color.black)
            chosen = new Color(0.1f, 0.1f, 0.1f);

        fillColor = chosen;

        int midToneX = Mathf.RoundToInt(toneRect.x + toneRect.width * 0.5f);
        int midToneY = Mathf.RoundToInt(toneRect.y + toneRect.height * 0.5f);
        FindPixelWithinTone(midToneX, midToneY);
        ApplyCurrentFillColorPreview();
    }

    private void EnsureGameplayCanvas()
    {
        if (!useCanvasForDrawingSurface)
            return;

        if (gameplayCanvas == null)
        {
            GameObject canvasGO = new GameObject("GameplayCanvas");
            gameplayCanvas = canvasGO.AddComponent<Canvas>();
            gameplayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameplayCanvas.overrideSorting = true;
            gameplayCanvas.sortingOrder = 100;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject backgroundGO = new GameObject("DrawingBackground");
            backgroundGO.transform.SetParent(canvasGO.transform, false);
            backgroundRectTransformCanvas = backgroundGO.AddComponent<RectTransform>();
            backgroundRawImageCanvas = backgroundGO.AddComponent<RawImage>();
            backgroundRawImageCanvas.raycastTarget = false;

            GameObject drawingGO = new GameObject("DrawingSurface");
            drawingGO.transform.SetParent(canvasGO.transform, false);
            drawingRectTransform = drawingGO.AddComponent<RectTransform>();
            drawingRawImage = drawingGO.AddComponent<RawImage>();
            drawingRawImage.raycastTarget = false;
        }

        if (backgroundRectTransformCanvas != null)
        {
            backgroundRectTransformCanvas.anchorMin = Vector2.zero;
            backgroundRectTransformCanvas.anchorMax = Vector2.one;
            backgroundRectTransformCanvas.pivot = new Vector2(0.5f, 0.5f);
            backgroundRectTransformCanvas.anchoredPosition = Vector2.zero;
            backgroundRectTransformCanvas.sizeDelta = Vector2.zero;
        }

        if (backgroundRawImageCanvas != null && backgroundRawImageCanvas.texture == null)
        {
            Texture2D bg = Resources.Load<Texture2D>("Graphics/UIIcons/background_grey_mandala");
            if (bg != null)
                backgroundRawImageCanvas.texture = bg;
        }

        AssignDrawingTexture();
        SyncCanvasDrawingSurface();
    }

    private void AssignDrawingTexture()
    {
        if (!useCanvasForDrawingSurface || drawingRawImage == null || mainImage == null)
            return;

        if (lastAssignedDrawingTexture != mainImage)
        {
            drawingRawImage.texture = mainImage;
            lastAssignedDrawingTexture = mainImage;
        }
    }

    private void SyncCanvasDrawingSurface()
    {
        if (!useCanvasForDrawingSurface || drawingRectTransform == null)
            return;

        // imageRect is already in the same coordinate space used by gameplay math.
        // Only convert Y from top-left (IMGUI style) to bottom-left (UGUI).
        float drawX = imageRect.x;
        float drawYTop = imageRect.y;
        float drawW = imageRect.width;
        float drawH = imageRect.height;
        float drawYBottom = Screen.height - (drawYTop + drawH);

        drawingRectTransform.anchorMin = Vector2.zero;
        drawingRectTransform.anchorMax = Vector2.zero;
        drawingRectTransform.pivot = new Vector2(0f, 0f);
        drawingRectTransform.anchoredPosition = new Vector2(drawX, drawYBottom);
        drawingRectTransform.sizeDelta = new Vector2(drawW, drawH);
    }


	void GetUIImages()
	{
		List<string> UIIconPaths = new List<string> ();
		UIIconPaths = ImagePathHolder.LoadUIIcons ();
		foreach (string s in UIIconPaths) 
		{
			switch(UIIconPaths.IndexOf(s))
			{
			case 0:unDo=Resources.Load<Texture2D>(s);
				break;
			case 1: shareFB=Resources.Load<Texture2D>(s);
				break;
			case 2:home=Resources.Load<Texture2D>(s);
				break;
			case 3:closeIAP=Resources.Load<Texture2D>(s);
				break;
			}
		}
	}



	void TwitterShare()
	{
		eagerShare = false;
		//			TwitterSNS.Instance().PostImage(mainImage.EncodeToPNG(),"Shared via Color&Share! #colorandshare #colorapp");
	}


	void CreateWatermark()
	{
		if(watermark==null)
			watermark=Resources.Load<Texture2D>(ImagePathHolder.LoadWaterMark());
		testwaterImage = new Texture2D (watermark.width, watermark.height,TextureFormat.RGB24,false); 
		testwaterImage.SetPixels (watermark.GetPixels ());
		testwaterImage.Apply ();
		int idx = (testwaterImage.width - mainImage.width) / 2;
		int idy = (testwaterImage.height - mainImage.height) / 2;
		for (int i=0; i<mainImage.width; i++) 
		{
			idx++;
			idy=(testwaterImage.height - mainImage.height) / 2;
			for (int j=0; j<mainImage.height; j++)
			{
				testwaterImage.SetPixel (idx, idy,mainImage.GetPixel(i,j));
				idy++;
			}	
		}
		testwaterImage.Apply ();	
		showWaterMark = true;
		waterMarkComeplete = true;
		Debug.Log (testwaterImage.width);
		Debug.Log (testwaterImage.height);
//		

		if ((moveToShare&&oldFillers.Count>0)||DataManager.Instance.fromDrawings) 
		{
			DataManager.Instance.waterMarkedImage = testwaterImage.EncodeToPNG ();
			if(oldFillers.Count>0)
			StoreImageChange ();
			else if(DataManager.Instance.fromDrawings)
				DataManager.Instance.fromDrawings=false;
			DataManager.Instance.LoadScene("Share",0.25f);
//			AutoFade.LoadLevel ("Share", 0.25f, 0.25f, Color.white);
		}

//		if(saveGalleryImage)
//		{
//			saveGalleryImage=false;
//			SaveToGallery();
//		}
//		if (!showWaterMark&&fbshare) {
//			StartCoroutine(UploadImage());
//		}
//		if(!showWaterMark&&sendSMS)
//			SendSMS();
//		if(!showWaterMark&&sendMail)
//			SendMail();
//		if(!showWaterMark&&instaShare)
//			ShareToInstagram();
	}


	void ShareToInstagram()
	{
		instaShare = false;
		eagerShare = false;
		
		uploadComplete = false;
		showSharePopUp = false;
		Debug.Log (transform.name);
		#if UNITY_ANDROID
        androidClass = new AndroidJavaObject("com.example.imagesave.SaveImageUnityBridgeCompat");
        androidClass.CallStatic(
            "ShareImage",
            mainImage.EncodeToPNG(),
            "Check This Out!",
            "Image From ColoRelax",
            "Checkout ColoRelax! #colorelax #coloringforadults #adultcoloringbook #coloringbook #mandala"
        );
		#endif
		#if UNITY_IOS
		InstagramShare.PostToInstagram("#tinge", testwaterImage.EncodeToPNG());
		#endif
		
		
	}


	void ImagePosted(string resp)
	{
		if(resp=="READY")
			MessagePosted ();
	}
	
	IEnumerator HidePallete(Rect hideRect,bool show,float startVal,float endVal)
	{
		float frameTime = 40.0f;
		float speed = Mathf.Abs(startVal-endVal) / frameTime;
		float speedforButts = Mathf.Abs (2048 * scale_y - 1898 * scale_y) / frameTime;
		hideRect.y = startVal;
		if (show) {
			while (palleteRect.y>endVal) {
				palleteRect.y -= speed;
				bannerRect.y+=speed;
				//								shareMessageRect.y+=speedforButts;
				shareEmailRect.y+=speedforButts;
				instaRect.y+=speedforButts;
				fbRect.y+=speedforButts;
				yield return new WaitForEndOfFrame ();
			}
		} 
		else
		{	
			while (palleteRect.y<endVal) {
				
				palleteRect.y += speed;
				bannerRect.y-=speed;
				//				shareMessageRect.y-=speedforButts;
				shareEmailRect.y-=speedforButts;
				instaRect.y-=speedforButts;
				fbRect.y-=speedforButts;
				yield return new WaitForEndOfFrame ();
			}
		}
		
		if (shareEmailRect.y >= 2048 * scale_y&&colorHolds)
			colorSelected = true;
	}


	void OnDisable()
	{
		
	}


	void HandleIAP()
	{
		
		if (ButtonHit (IAPRect[2])) 
		{
			/* simo
			if(InAppManager.Instance!=null)
				InAppManager.Instance.SetPremium();
			showInapp=false;
			*/
		}
		if (ButtonHit (IAPRect[1])) 
		{
            /* simo
			if(InAppManager.Instance!=null)
				InAppManager.Instance.SetUnlockedColors();
			showInapp=false;
			*/
		}
		if (ButtonHit (IAPRect[0])) 
		{
            /* simo
			if(InAppManager.Instance!=null)
				InAppManager.Instance.SetUnlockedCategory();
			showInapp=false;
			*/
		}
	}


	Stack<FillInfo> oldFillers = new Stack<FillInfo> ();
	bool startPanning=false;
	bool isZooming=false;
	bool colorHolds=false;
	bool eagerShare=false;
	bool fbshare=false;
	bool twitterShare=false;
	bool instaShare=false;
	
	void OnLoggedInFB()
	{
		eagerShare = false;
		//		if(isLoggedIn)
		
		uploadComplete = false;
		showSharePopUp = false;
		fbshare=false;
		
//		FacebookSNS.Instance ().PostStoryWithFeedDialog ("", "Image from Color Joy", "Check this Out!", "", uploadedImgPath, uploadedImgPath, uploadedImgPath);
		
		//		FacebookSNS.Instance ().PostImage (mainImage.EncodeToPNG (), "Shared via Color and Share! #colorandshare #colorapp");
		//		GFaceBook.Instance.PostPic("Shared via Tinge! #tinge #colorapp",mainImage.EncodeToPNG());
		
	}



	void FBShareCode()
	{
		//			eagerShare=true;
		//		if(GFaceBook.Instance.facebookLoginDone)
		//		if(isLoggedIn) 
		//		{
		
		fbshare=false;
		uploadComplete = false;

		
		showSharePopUp = false;
//		FacebookSNS.Instance ().PostStoryWithFeedDialog ("", "Image from Color Joy", "Check this Out!", "", uploadedImgPath, uploadedImgPath, uploadedImgPath);
		
		
		//		}
		//		else
		//		{
		//			FacebookSNS.Instance().Login();
		////			GFaceBook.Instance.Login();
		//		}
	}


	void TwitterShareCode()
	{
		//		if(TwitterSNS.Instance().IsUserLoggedIn())
		//		{
		//			eagerShare=false;
		//			twitterShare=false;
		//			TwitterSNS.Instance().PostImage(mainImage.EncodeToPNG(),"Shared via Tinge!");
		//			MessagePosted();
		//		}
		//		else
		//			TwitterSNS.Instance().Login();
	}



	bool showSharePopUp=false;
	int selectedToneIndex=-1;
	bool hideBanner=false;
	void Pan()
	{
		UpdateTouches ();	if (Input.touchCount == 1) {
			if(imageRect.width>origImgRect.width&&firstTouch.deltaPosition.magnitude>5f
               && imageRect.Contains (new Vector2 (firstTouch.position.x, (Screen.height - firstTouch.position.y)))&&firstTouch.phase==TouchPhase.Moved)
			{
				previousEvent=currentEvent;
				colorHolds=false;
				imageRect.x+=(firstTouch.deltaPosition.x);
				imageRect.y-=(firstTouch.deltaPosition.y);
				currentEvent=TouchEvent.Panning;
			}
			else if(firstTouch.phase==TouchPhase.Ended)
			{
				if(colorSelected)
					colorHolds=true;
				previousEvent=currentEvent;
				currentEvent=TouchEvent.None;
			}
			
		}
	}


	Vector2 NormalizeDeltaTouch(Touch t)
	{
		float dt=Time.deltaTime/t.deltaTime;
		if(float.IsNaN(dt)||float.IsInfinity(dt))
			dt=1f;
		return t.deltaPosition*dt;
	}


    /* simo : see my version below
	void Pinch()
	{
		UpdateTouches ();
		if (Input.touchCount == 2 && imageRect.Contains (new Vector2 (firstTouch.position.x, (Screen.height - firstTouch.position.y))) && imageRect.Contains (new Vector2 (secondTouch.position.x, (Screen.height - secondTouch.position.y)))) {
			colorHolds = false;
			currentEvent = TouchEvent.Zooming;
			Vector2 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
			Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;
			float oldDistance = Vector2.Distance (firstTouchPrevPos, secondTouchPrevPos);
			float newDistance = Vector2.Distance (firstTouch.position, secondTouch.position);
			float scaleFactor = (newDistance / oldDistance);
			if (scaleFactor > 0f) {// scale image based on finger positions with a max zoomed position.
				if (imageRect.width >= origImgRect.width && imageRect.width < (3f * origImgRect.width))
                    imageRect.width *= (scaleFactor );  
				if (imageRect.height >= origImgRect.height && imageRect.height < (3f * origImgRect.height))
                    imageRect.height *= (scaleFactor );  
                Vector2 newPosZero = new Vector2 (firstTouch.position.x - (firstTouchPrevPos.x - imageRect.x) * scaleFactor, firstTouch.position.y - (firstTouchPrevPos.y - imageRect.y) * scaleFactor); 
                Vector2 newPosOne = new Vector2 (secondTouch.position.x - (secondTouchPrevPos.x - imageRect.x) * scaleFactor, secondTouch.position.y - (secondTouchPrevPos.y - imageRect.y) * scaleFactor);  
				if (imageRect.height < (3f * origImgRect.height) && imageRect.height >= origImgRect.height && imageRect.width < (3f * origImgRect.width) && imageRect.width >= origImgRect.width) {
					imageRect.x = (newPosOne.x + newPosZero.x) / 2;
					imageRect.y = (newPosOne.y + newPosZero.y) / 2;
				}
				
			}
			scaleFactor = 0f;
		} else {
			previousEvent=currentEvent;
			currentEvent = TouchEvent.None; 
		}
	}
    */

    // simo version 22/03/2018 ; ZOOMING
    void Pinch()
    {
        UpdateTouches();
        if (Input.touchCount != 2)
        {
            previousEvent = currentEvent;
            currentEvent = TouchEvent.None;
            return;
        }

        Vector2 firstGui = new Vector2(firstTouch.position.x, Screen.height - firstTouch.position.y);
        Vector2 secondGui = new Vector2(secondTouch.position.x, Screen.height - secondTouch.position.y);
        if (!imageRect.Contains(firstGui) || !imageRect.Contains(secondGui))
        {
            previousEvent = currentEvent;
            currentEvent = TouchEvent.None;
            return;
        }

        colorHolds = false;
        currentEvent = TouchEvent.Zooming;

        Vector2 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
        Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

        float oldDistance = Vector2.Distance(firstTouchPrevPos, secondTouchPrevPos);
        float newDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
        if (oldDistance <= 0.001f)
            return;

        float scaleFactor = newDistance / oldDistance;
        if (scaleFactor <= 0f)
            return;

        Vector2 prevMidScreen = (firstTouchPrevPos + secondTouchPrevPos) * 0.5f;
        Vector2 currMidScreen = (firstTouch.position + secondTouch.position) * 0.5f;
        Vector2 prevMidGui = new Vector2(prevMidScreen.x, Screen.height - prevMidScreen.y);
        Vector2 currMidGui = new Vector2(currMidScreen.x, Screen.height - currMidScreen.y);

        float anchorU = Mathf.Clamp01((prevMidGui.x - imageRect.x) / imageRect.width);
        float anchorV = Mathf.Clamp01((prevMidGui.y - imageRect.y) / imageRect.height);

        float newWidth = Mathf.Clamp(imageRect.width * scaleFactor, origImgRect.width, 18f * origImgRect.width);
        float newHeight = Mathf.Clamp(imageRect.height * scaleFactor, origImgRect.height, 18f * origImgRect.height);

        imageRect.x = currMidGui.x - anchorU * newWidth;
        imageRect.y = currMidGui.y - anchorV * newHeight;
        imageRect.width = newWidth;
        imageRect.height = newHeight;

        ClampImageRectToViewport();
    }


	bool saveGalleryImage=false;
	int currentTouchStatus;
	public bool isLoggedIn=false;
    bool fillAnimationRunning=false;




	void Update () {		
		if (Input.touchCount == 0)
			previousEvent = currentEvent = TouchEvent.None;
		if (Input.touchCount>1) {
			colorHolds=false;
		}
		else if(colorSelected)
			colorHolds=true;
		//		if (sendSMS && uploadComplete && showSharePopUp)
		//			SendSMS ();
		//		if (sendMail && uploadComplete&&showSharePopUp)
		//			SendMail ();
		if (fbshare && uploadComplete && showSharePopUp)
			FBShareCode ();
        
		if (!showSavedPopUp &&!showInapp&& Input.touchCount == 2&&!showInapp&&!showSharePopUp) {
			Pinch ();

		} else  if (!showSavedPopUp&&Input.touchCount == 1 && !showInapp && !showSharePopUp) Pan ();

		if (!showSavedPopUp && !isZooming && !startPanning && !showInapp && ButtonFade(homeRect,1) && !showSharePopUp) {			
			goHome=true;
			StoreImageChange();
		}
	
		if(showSharePopUp && ButtonHit(saveToGallRect) && !showInapp)
		{
			showWaterMark=true;
			saveGalleryImage=true;
		}
		
		//		if (GFaceBook.Instance.facebookLoginDone&&eagerShare&&fbshare) {
		if(eagerShare&&fbshare)
		{
//			FacebookSNS.Instance().Login();
			Debug.Log("Facebook Logged in");
			OnLoggedInFB();
			eagerShare=false;
			fbshare=false;
		}
		//		if (TwitterSNS.Instance ().IsUserLoggedIn ()&&eagerShare&&twitterShare) {
		//			TwitterShare();
		//			eagerShare=false;
		//			twitterShare=false;
		//				}
	
//		if ( ButtonFade (saveImageRect, 3)&&!showInapp&&!showSharePopUp) {
//			showWaterMark=true;
//			saveGalleryImage=true;
//						SaveToGallery();
//		}

        // SHARE watermark creation
		if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && ButtonFade(shareRect,2)) {
			moveToShare=true;
			CreateWatermark();
//			DrawWatermark();
			//			showShares=true;
			//			eagerShare=!eagerShare;
			//			Debug.Log(showShares.ToString()+"ShowShares");
			//			Debug.Log(eagerShare.ToString()+"eagerShares");
			//			if(eagerShare)
			//			{
			//				Chartboost.showInterstitial(CBLocation.Default);
			//				ShowShareBanner();
			//			}
			//			else
			//				MessagePosted();
		}

		if (!showInapp && showSharePopUp && ButtonHit (closeShareRect)) {
			showSharePopUp = false;
			colorHolds=false;
		}

		if (!showInapp && !showSavedPopUp && colorHolds && !isZooming && !startPanning && !fillAnimationRunning && !eagerShare && !ButtonFade(unDoRect,0) && 
            !ButtonFade(shareRect,2) && !ButtonFade(homeRect,1) && ButtonHit(imageRect) && previousEvent==TouchEvent.None &&! showSharePopUp) {
            // COLOR IMAGE : color fill area based on selected color and store hit position and original color in stack 
			if(colorHolds && colorSelected && (Screen.height - Input.mousePosition.y)>topBannerRect.y && 
              //(Screen.height - Input.mousePosition.y)<(1808 * scale_y))
               (Screen.height - Input.mousePosition.y) < 1948 * scale_y - (float)(Screen.height * 0.2f))  //simo DO NOT PAINT below the pencil stripe included
			{ 
				FloodFiller.RevisedQueueFloodFill(mainImage,Mathf.CeilToInt((Input.mousePosition.x-imageRect.x)*mainImage.width/imageRect.width),Mathf.CeilToInt(mainImage.height-( Screen.height-Input.mousePosition.y-imageRect.y)*(mainImage.height/(imageRect.height))),fillColor,false);
				usedColors.Push(FloodFiller.tarGetCol);
				oldFillers.Push (new FillInfo (FloodFiller.tarGetCol,Mathf.CeilToInt((Input.mousePosition.x-imageRect.x)*mainImage.width/imageRect.width),Mathf.CeilToInt(mainImage.height-( Screen.height-Input.mousePosition.y-imageRect.y)*(mainImage.height/(imageRect.height)))));
                if (smoothFillAnimation && FloodFiller.lastFillIndices.Count > 0) {
                    StartCoroutine(AnimateFillRegion(
                        new List<int>(FloodFiller.lastFillIndices),
                        (Color32)FloodFiller.tarGetCol,
                        (Color32)fillColor));
                } else {
				    mainImage.Apply();
                }
			}
			
			
		}	

        // Debug
		if (Input.GetKeyDown (KeyCode.D)) {
			showWaterMark=true;
			//			StartCoroutine(UploadImage());
			//			MessagePosted();
			//			showShares=true;
			//			palleteRect.y=2048*scale_y;
			//			bannerRect.y=1848*scale_y;
			//			StartCoroutine(HidePallete(palleteRect,!hideBanner,(2048*scale_y),(1848*scale_y)));
		}

        // Debug
		if (Input.GetKeyDown (KeyCode.E)) {
			if(colorSelected)
				colorHolds=true;
			//			palleteRect.y=1848*scale_y;
			//			bannerRect.y=2048*scale_y;
			//			StartCoroutine(HidePallete(palleteRect,hideBanner,(1848*scale_y),(2048*scale_y)));
		}
		
		for (int i=0; i< pencilRect.Length; i++) {			
			if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (pencilRect [i]) && selectedToneIndex<0 &&
                !eagerShare && !showInapp && !showSharePopUp) {
				selectedToneIndex=i;
				pencilSelection [i] = !pencilSelection[i];
                Debug.Log(((float)Screen.width / (float)Screen.height).ToString() + " Simo Pencil Selected");
                SelectDefaultToneColor(selectedToneIndex);

				break;
			}	
			else if(!showInapp && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (pencilRect [i]) && !ButtonHit(toneRect) && 
                    !eagerShare && !showInapp && !showSharePopUp) {	
				selectedToneIndex=i;
				pencilSelection [i] = !pencilSelection[i];
                Debug.Log(((float)Screen.width / (float)Screen.height).ToString() + " Simo Pencil Selected");
                SelectDefaultToneColor(selectedToneIndex);

				break;
			}	
		}
		
		if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !fillAnimationRunning && ButtonFade(unDoRect,0) || ButtonHit(unDoRect) && !showInapp &&
            !showSharePopUp) {
			Debug.Log("In Undo");
			colorHolds=false;
			//Debug.Log(oldFillers.Count);
			if(oldFillers.Count>0)			
				UndoFill();
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			imageRect.width*=1.1f;
			imageRect.height*=1.1f;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			imageRect.x+=10*scale_x;
		}

		//		if (!eagerShare && !showSavedPopUp && !isZooming && !startPanning &&!ButtonHit(inappPopupRect)&&!ButtonHit(toneRect)&&showInapp)
		//			showInapp=false;
		
        /*
        if (showInapp && ButtonHit (closeIAPRect))
			showInapp = false;
		
        if (showInapp)
			HandleIAP ();		
			*/

        // simo: uncomment/comment to on/off iap : this disable the showing of iap window,but color still locked
        /*
        // Action in case the four locked iap buttons are being selected, located at (Input.mousePosition.x) > (990 * scale_x)
		if (!eagerShare && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (toneRect) && selectedToneIndex >= 0 && 
            (Mathf.CeilToInt (Input.mousePosition.x) > (990 * scale_x)) && !ImagePathHolder.GetLockedColors ())
		    showInapp = true;
            //Debug.Log("Simo : showInapp = true");
		*/

		if (!showInapp && !eagerShare && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (toneRect) && selectedToneIndex>=0 &&
            !showInapp && !showSharePopUp) {
            Debug.Log("Simo : a tone is selected");
			//Selecting tone index based on tone selected for particular pencil
            //// simo: tone located at (Input.mousePosition.x) < (990 * scale_x), so the not locked ones
			//if((!ImagePathHolder.GetLockedColors() && (Mathf.CeilToInt(Input.mousePosition.x)<(990*scale_x))) || (ImagePathHolder.GetLockedColors()))
			//{							
				fillColor=pencilTones[selectedToneIndex].GetPixel(Mathf.CeilToInt((Input.mousePosition.x-toneRect.x)*pencilTones[selectedToneIndex].width/Screen.width),Mathf.CeilToInt(pencilTones[selectedToneIndex].height-(Screen.height-Input.mousePosition.y-toneRect.y)*toneRect.height/(pencilTones[selectedToneIndex].height)));
				
				FindPixelWithinTone(Mathf.CeilToInt(Input.mousePosition.x),Mathf.CeilToInt(pencilTones[selectedToneIndex].height-(Screen.height-Input.mousePosition.y-toneRect.y)*(pencilTones[selectedToneIndex].height/toneRect.height)));
				if(fillColor==Color.black)
					fillColor=new Color(0.1f,0.1f,0.1f);				
				for(int i=0;i<=100;i++)
				{
					for(int j=0;j<=100;j++)
						selectedColor.SetPixel(i,j,fillColor);
				}
				selectedColor.Apply();
			//}
			
			
			
		}
		if (showSavedPopUp && !savedChoiceTransitionRunning && ButtonHit(startoverRect)) {
			StartCoroutine(ApplySavedChoiceWithFade(true));
		}
		if (showSavedPopUp && !savedChoiceTransitionRunning && ButtonHit(ContinueRect)) {
			StartCoroutine(ApplySavedChoiceWithFade(false));
		}

        // simo : check if supportImg was touched and activate video rw 
        if ( ButtonHit(supportImgRect) || ButtonHit(supportTextRect) )
        {
            AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
            //adsManager.showInterstitialAdMob();
            Debug.Log("Simo : clapperBoard icon touched");
        }

        // simo : check if Avoid touch was touched  
        if (ButtonHit(coverRectAvoidingTouch))
        {            
            Debug.Log("Simo : coverRectAvoidingTouch has been touched");
        }

        if (useCanvasForDrawingSurface)
        {
            AssignDrawingTexture();
            SyncCanvasDrawingSurface();
        }
	}



	bool goHome=false;
	bool showShares=false;

	bool ButtonDown(Rect rt)
	{
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonDown (0))
			return true;
		else
			return false;
	}


	bool ButtonHit(Rect rt)
	{
		if (rt.Contains (new Vector2 (Input.mousePosition.x, (Screen.height - Input.mousePosition.y))) && Input.GetMouseButtonUp (0))
			return true;
		else
			return false;
	}
	void DrawShareBanner()
	{
		if (showShares) {
			GUI.DrawTexture(bannerRect,white);
			GUI.DrawTexture(fbRect,FBShare);
			GUI.DrawTexture(shareEmailRect,shareEmail);
			GUI.DrawTexture(instaRect,shareInsta);
			//			GUI.DrawTexture(shareMessageRect,shareMessage);
		}
	}


    // simo : draw cover rect to avoid touch
    void DrawCoverRectAvoidingTouch()
    {
        // Intentionally left blank: keep touch-blocking rect logic without visual overlay.
    }


	// simo : drawbackground
	void DrawBackground()
	{		
		//backgroundImg = new Texture2D(testImage.width, testImage.height);
		backgroundImg = Resources.Load<Texture2D>("Graphics/UIIcons/background_grey_mandala");
		GUI.DrawTexture(backgroundImgRect, backgroundImg);
	}

	// simo : draw support bottom banner
	void DrawSupportBanner()
	{
		customStyle.normal.textColor = Color.white;
		customStyle.fontSize = Mathf.CeilToInt(45 * scale_y);
		GUI.Label(supportTextRect, "Like ColoRelax? Support it watching a FREE video! ", customStyle);
		supportImg = Resources.Load<Texture2D>("Graphics/UIIcons/clapperBoard");
		GUI.DrawTexture(supportImgRect, supportImg);
	}


    IEnumerator FadeSavedChoiceOverlay(float from, float to, float duration)
    {
        float elapsed = 0f;
        savedChoiceFadeAlpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            savedChoiceFadeAlpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        savedChoiceFadeAlpha = to;
    }


    IEnumerator ApplySavedChoiceWithFade(bool startOver)
    {
        if (savedChoiceTransitionRunning)
            yield break;

        savedChoiceTransitionRunning = true;
        yield return FadeSavedChoiceOverlay(0f, 1f, 0.2f);

        if (startOver)
            LoadActualImage();
        else
            LoadSavedImage();

        yield return null;
        yield return FadeSavedChoiceOverlay(1f, 0f, 0.2f);
        savedChoiceTransitionRunning = false;
    }


	void DrawTopBanner()
	{
		GUI.DrawTexture (topBannerRect, black);
		GUI.DrawTexture (topWhiteRect, white);
		if (!topSelection [2])
			GUI.DrawTexture (shareRect, shareFB);
		else 
		{

			GUI.DrawTexture (shareRect, shareFB);
			GUI.DrawTexture(shareRect,transImg);
		}
//			GUI.DrawTexture (shareRect, fadedShare); 
		if(!topSelection[0])
			GUI.DrawTexture (unDoRect, unDo);
		else
		{

			GUI.DrawTexture (unDoRect, unDo);
			GUI.DrawTexture(unDoRect,transImg);
		}
		if(!topSelection[1])
			GUI.DrawTexture (homeRect, home);
		else
		{

			GUI.DrawTexture (homeRect, home);
			GUI.DrawTexture(homeRect,transImg);
		}
//		if(!topSelection[3])
//			GUI.DrawTexture (saveImageRect, saveImage);
//		else
//			GUI.DrawTexture (saveImageRect, fadedsaveImg);
//		customStyle.normal.textColor = new Color ((57f / 255f), (122f / 255f), (159f / 255f));

		customStyle.normal.textColor = Color.black;
		customStyle.fontSize=Mathf.CeilToInt(55*scale_y);
        // simo : deleted btn label in top banner
		GUI.Label (homeTextRect, "", customStyle);
		GUI.Label (shareTextRect, "", customStyle);
		GUI.Label (undoTextRect, "", customStyle); 
//		GUI.Label (saveImgTextRect, "SAVE", customStyle);
	}



	void StoreImageChange()
	{
		DataManager.Instance.WriteWorkingImageBytes(
			DataManager.Instance.selectedFileName,
			mainImage.EncodeToPNG(),
			DataManager.Instance.selectedResourceName);
		if ((oldFillers.Count > 0 && !moveToShare)||DataManager.Instance.fromDrawings) 
		{
			DataManager.Instance.fromDrawings=false;
			PlayerPrefs.SetInt (DataManager.Instance.selectedFileName, 1);
			DataManager.Instance.MarkProgress(DataManager.Instance.selectedFileName, true);
			CreateWatermark ();
			TextureScale.Bilinear(testwaterImage,512,512);
			DataManager.Instance.StoreWatermark (testwaterImage.EncodeToPNG(), DataManager.Instance.selectedFileName);
		}

		if (goHome)            
			LoadCategories ();
	}



	void LoadCategories()
	{
		//		colorSelected = false;
		
		//		Texture2D someImg= new Texture2D(mainImage.width,mainImage.height);
		//		someImg.SetPixels(mainImage.GetPixels ());
		//		someImg.Apply ();
		//		TextureScale.Bilinear (someImg, 256, 256);
		//		DataManager.Instance.FileCreatorBytes (someImg.EncodeToPNG (), DataManager.Instance.selectedThumb);// store thumbnail into file
		//		PlayerPrefs.SetString (DataManager.Instance.selectedThumb, DataManager.Instance.selectedThumb);
		//		PlayerPrefs.Save();
		//		for(int i=0;i<=100;i++)
		//		{
		//			for(int j=0;j<=100;j++)
		//				selectedColor.SetPixel(i,j,Color.black);
		//		}
		//		selectedColor.Apply();

		if (goHome)
			goHome = false;
        // simo : backing home : show unity ads
        int rnd = Random.Range(1, 101);
        if (rnd <= 50) {
            AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
            // adsManager.showInterstitialAdMob();
        }
		SceneManager.LoadScene("Gallery");
//		if(SceneManager.Instance!=null)
//			SceneManager.Instance.LoadScene("CategoryIntermediate");
	}


	void DrawPencilAndTones()
	{
		for (int pencilIndex=0; pencilIndex<pencilRect.Length; pencilIndex++) {
            Rect slot = pencilRect[pencilIndex];
            float diameter = Mathf.Min(slot.width, slot.height) * 0.62f;
            float centerX = slot.x + slot.width * 0.5f;
            float centerY = slot.y + slot.height * 0.42f;
            float border = pencilIndex == selectedToneIndex ? 7f * scale_x : 4f * scale_x;

            Rect outerCircle = new Rect(centerX - diameter * 0.5f - border, centerY - diameter * 0.5f - border, diameter + border * 2f, diameter + border * 2f);
            Rect innerCircle = new Rect(centerX - diameter * 0.5f, centerY - diameter * 0.5f, diameter, diameter);

            GUI.color = Color.white;
            GUI.DrawTexture(outerCircle, circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture);
            GUI.color = palettePreviewColors != null && pencilIndex < palettePreviewColors.Length ? palettePreviewColors[pencilIndex] : Color.white;
            GUI.DrawTexture(innerCircle, circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture);
            GUI.color = Color.white;
		}
		if(selectedToneIndex>=0)
		{
			//			GUI.DrawTexture(new Rect(pencilRect[selectedToneIndex].x-4*scale_x,pencilRect[selectedToneIndex].y-26*scale_y,pencilRect[selectedToneIndex].width+16*scale_x,pencilRect[selectedToneIndex].height+24*scale_y),pencils[0]);
			GUI.DrawTexture(toneRect,pencilTones[selectedToneIndex]);
            // simo : comment/decomment to not see/see the lock
            /*                
			if(!ImagePathHolder.GetLockedColors())
				for(int i=0;i<4;i++)                   
                    GUI.DrawTexture(new Rect((1020 + i * 135) * scale_x, 1958 * scale_y, 80 * scale_x, 80 * scale_y), lockColor);
                    // simo : draw the lock IN ANOTHER POSITION
                    //GUI.DrawTexture(new Rect((1020+i*135)*scale_x,1958*scale_y -(float)(Screen.height * 0.12f),80*scale_x,80*scale_y),lockColor);
            */        
		}
		
	}


	void OnGUI()
	{
        if (!useCanvasForDrawingSurface)
		    DrawBackground(); //simo
		//		if (!waterMarkComeplete)
        if (!useCanvasForDrawingSurface)
		    GUI.DrawTexture (imageRect, mainImage);
		//		else
		//		{
		//			GUI.DrawTexture(watermarkImageRect,testwaterImage);
		//			//			GUI.DrawTexture(watermarkImageRect,watermark);
		//			//			GUI.DrawTexture(imageRect,mainImage);
		//		}
		
//		if (showWaterMark) {
//			showWaterMark=false;
//			CreateWatermark ();
//		}
//		if (showWaterMark)
//			GUI.DrawTexture (watermarkImageRect, testwaterImage);

		DrawTopBanner ();
        DrawCoverRectAvoidingTouch(); // simo
		DrawPencilAndTones ();
		DrawShareBanner ();
        DrawSupportBanner(); // simo
		if(!isZooming)
			EventHandle ();
		if(showSavedPopUp)
			DrawSavedImage ();	
        if (savedChoiceFadeAlpha > 0f) {
            Color oldColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, savedChoiceFadeAlpha);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }
		if (colorSelected) {
			GUI.DrawTexture(new Rect(selRect.x-(5*scale_x),selRect.y-(4*scale_y),selRect.width+(10*scale_x),selRect.height+(8*scale_y)),selectedBorder);
			GUI.DrawTexture (selRect, selectedColor);
		}
        // simo : comment/decomment this and the lock images to allow iap in tone bar
        /*
        if(showInapp) {  
			DrawIAP (); 
        }
        */



		//		if (showWaterMark)
		//		{
		//			waterImage=DrawWatermark ();
		//			showWaterMark=false;
		//		}		
//		if(showSharePopUp)
//			DrawShare ();
	}


    IEnumerator AnimateFillRegion(List<int> pixelIndices, Color32 fromColor, Color32 toColor)
    {
        if (pixelIndices == null || pixelIndices.Count == 0)
        {
            mainImage.Apply();
            yield break;
        }

        fillAnimationRunning = true;
        Color32[] pixels = mainImage.GetPixels32();
        int pixelsLength = pixels.Length;

        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            if (idx >= 0 && idx < pixelsLength)
                pixels[idx] = fromColor;
        }
        mainImage.SetPixels32(pixels);
        mainImage.Apply();

        float duration = Mathf.Max(0.01f, smoothFillDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color32 frameColor = Color32.Lerp(fromColor, toColor, t);
            for (int i = 0; i < pixelIndices.Count; i++)
            {
                int idx = pixelIndices[i];
                if (idx >= 0 && idx < pixelsLength)
                    pixels[idx] = frameColor;
            }
            mainImage.SetPixels32(pixels);
            mainImage.Apply();
            yield return null;
        }

        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            if (idx >= 0 && idx < pixelsLength)
                pixels[idx] = toColor;
        }
        mainImage.SetPixels32(pixels);
        mainImage.Apply();

        fillAnimationRunning = false;
    }


	void EventHandle()
	{
        if (savedChoiceTransitionRunning)
            return;

		if (Event.current.type==EventType.MouseUp) {
			// Keep image bounded to viewport and preserve aspect ratio.
            ClampImageRectToViewport();
		}
	}


	Stack<Color32> usedColors;
	public struct Point
	{
		public short x;
		public short y;
		public Point(short aX, short aY) { x = aX; y = aY; }
		public Point(int aX, int aY) : this((short)aX, (short)aY) { }
	}
	
	public struct FillInfo
	{
		public Color oldColor;
		public byte[] oldColorRGBs ;
		public int x, y;
		public FillInfo(Color32 olderCol,int startPosX,int startPosY){oldColor=olderCol;oldColorRGBs=new byte[4];oldColorRGBs[0]=olderCol.r;oldColorRGBs[1]=olderCol.g;oldColorRGBs[2]=olderCol.b;oldColorRGBs[3]=olderCol.a;x=startPosX;y=startPosY;}
	}

	void UndoFill()
	{//pop last operation from stack and apply operation.
		FillInfo lastFill = oldFillers.Pop ();
		Color32 lastCol = new Color32 (lastFill.oldColorRGBs [0], lastFill.oldColorRGBs [1], lastFill.oldColorRGBs [2], lastFill.oldColorRGBs [3]);
		Color32 lastColUsed=new Color32();
		if(usedColors.Count>0)
			lastColUsed = usedColors.Pop ();
		if(lastColUsed.r==lastCol.r&&lastColUsed.g==lastCol.g&&lastColUsed.b==lastCol.b&&lastColUsed.a==lastCol.a)
			FloodFiller.RevisedQueueFloodFill (mainImage, lastFill.x, lastFill.y, lastCol,false);
		mainImage.Apply ();
	}

}
