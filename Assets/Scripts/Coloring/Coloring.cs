using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Coloring : MonoBehaviour {
    const int PalettePreviewTextureWidth = 128;
    const int PalettePreviewTextureHeight = 48;
    const int SavedCustomColorCount = 10;

    public enum GradientFillDirection
    {
        RightToLeft,
        LeftToRight,
        TopToBottom,
        BottomToTop,
        Radial
    }

    public GoogleMobileAdsScript adsManager; // simo
    [Header("UI Migration")]
    public bool useCanvasForDrawingSurface = true;
    [Header("Fill Effect")]
    public bool smoothFillAnimation = true;
    [Range(0.05f, 0.8f)]
    public float smoothFillDuration = 0.22f;
    [Header("Palette Animation")]
    public float swatchLiftPixels = 22f;
    [Range(0.05f, 0.4f)]
    public float swatchLiftDuration = 0.12f;
    [Header("Gradient Fill")]
    public GradientFillDirection gradientFillDirection = GradientFillDirection.LeftToRight;
    [Range(0f, 1f)]
    public float gradientVisibilityBoost = 0.48f;
    public int smallGradientRegionThreshold = 18000;
    [Header("Featured Theme Layout")]
    public float featuredThemeSwatchDiameterPixels = 98f;
    public float featuredThemeRowGapPixels = 60f;
    public float featuredThemeSwatchWidthPixels = 146f;
    public float featuredThemeSwatchHeightPixels = 146f;
    public float featuredThemeSwatchStepXPixels = 200f;
    [Header("Normal And Gradient Swatch Layout")]
    public float colorSwatchWidthPixels = 200f;
    public float colorSwatchHeightPixels = 130f;
    public float colorSwatchStepXPixels = 200f;
    public float colorSwatchTopRowOffsetPixels = 15f;
    public float colorSwatchRowGapPixels = 120f;
    [Header("Tone Palette Layout")]
    public float tonePaletteTileWidthPixels = 180f;
    public float tonePaletteTileHeightPixels = 60f;
    public float tonePaletteHorizontalGapPixels = 50f;
    public float tonePaletteRowGapPixels = 20f;
    public float tonePaletteHorizontalMarginPixels = 34f;
    public float tonePaletteHorizontalOffsetPixels = -60f;
    public float tonePaletteTopInsetPixels = 40f;
    public float tonePaletteGapFromSwatchesPixels = 0f;
    [Header("Backdrop Radius")]
    public float lowerContainerBackdropRadiusPixels = 3.53f;
    public float selectedMainRowBackdropRadiusPixels = 11f;
    [Header("Backdrop Layout")]
    public float mainRowGapFromImagePixels = 20f;
    public float mainRowBackdropVerticalPaddingPixels = 10f;
    public float selectedMainRowBackdropHorizontalPaddingPixels = 1.58f;
    public float selectedMainRowBackdropTopPaddingPixels = 1.27f;
    public float selectedMainRowBackdropBottomPaddingPixels = 15.2f;
    public float expandedPanelVerticalOffsetPixels = 0f;
    public float lowerContainerTopInsetPixels = 20f;
    [Header("Featured Theme Layout")]
    public float featuredThemeViewportInnerPaddingPixels = 3f;
    [Header("Right Column Layout")]
    public float rightColumnPaddingPixels = 22f;
	public static Coloring myInstance;
	public static Coloring Instance
	{
		get{
			if( myInstance == null )
				myInstance = FindObjectOfType(typeof(Coloring)) as Coloring;
			return myInstance;
		}
	}

	public GUISkin customSkin;
	public GUIStyle customStyle;
	private Rect imageRect,palleteRect,selRect,unDoRect,redoRect,saveRect,shareRect,topBannerRect,topWhiteRect,bannerRect,fbRect,instaRect,shareMessageRect,
	shareEmailRect,homeRect,popUpRect,startoverRect,ContinueRect,OrRect,toneRect,lockColorRect,
	saveImageRect,saveImgTextRect,inappPopupRect,premiumRect,IAPColorsRect,origImgRect,closeIAPRect,saveToGallRect,closeShareRect,
    watermarkImageRect,rateRect,whiteSwatchRect,paletteToggleRect,gradientToggleRect,customColorToggleRect,imagePickerToggleRect,gradientDirectionRect,gradientRotateRect,gradientRotateLeftRect,gradientResetRect,gradientBoostSliderRect,gradientBoostLabelRect,customColorOverlayRect,customColorAdvancedButtonRect,customHueSliderRect,customSvSquareRect,customColorPreviewRect,featuredThemeLeftArrowRect,featuredThemeRightArrowRect;
    private Rect imageViewportRect;
    private Rect[] featuredPaletteToggleRects;
    private Rect[] featuredPaletteSwatchRects;
    private Rect featuredThemeViewportRect;
    private Rect shareTextRect, homeTextRect, undoTextRect; /// <summary>
    public Rect supportTextRect, supportImgRect, coverRectAvoidingTouch ; //simo
    ///  button label text
    /// </summary>
	private Rect[] pencilRect,IAPRect; 
    private Color[] palettePreviewColors;
    private Color[] savedCustomColors;
    private Texture2D circularSwatchMask;
    private Texture2D paletteToggleIcon;
    private Texture2D gradientToggleIcon;
    private Texture2D customColorToggleIcon;
    private Texture2D imagePickerToggleIcon;
    private Texture2D backChevronIcon;
    private Texture2D gradientPaletteTexture;
    private Texture2D customHueSliderTexture;
    private Texture2D customSvSquareTexture;
    private Texture2D roundedDirectionButtonTexture;
    private Texture2D roundedToneBandMaskTexture;
    private Texture2D expandedPanelBackdropTexture;
    private Texture2D selectedMainRowBackdropTexture;
    private Texture2D[] featuredPaletteToggleTextures;
    private Texture2D normalPaletteToggleTexture;
    private Texture2D gradientPaletteToggleTexture;
    private Texture2D[] solidToneBandTextures;
    private Texture2D[] gradientToneBandTextures;
    private float[] swatchLiftOffsets;
    private float whiteSwatchLiftOffset;
    private float whiteSwatchScaleOffset;
    private bool whiteSwatchSelected;
    private bool showSwatchPalette;
    private bool showGradientPalette;
    private bool showFeaturedPalette;
    private bool showCustomColorOverlay;
    private bool showAdvancedCustomPicker;
    private bool imagePickerMode;
    private bool draggingCustomHue;
    private bool draggingCustomSv;
    private bool gradientModeActive;
    private bool gradientBandSelected;
    private bool gradientBoostDragging;
    private bool featuredThemeRowDragging;
    private Color32 gradientStartColor;
    private Color32 gradientEndColor;
    private float gradientRotationDegrees;
    private float featuredThemeRowScroll;
    private float featuredThemeContentWidth;
    private float cachedLowerContainerBackdropRadius = -1f;
    private float cachedSelectedMainRowBackdropRadius = -1f;
    private float featuredThemeRowPressStartX;
    private float featuredThemeRowScrollStart;
    private int featuredThemePressedIndex = -1;
    private Color customOverlayActiveColor = Color.white;
    private Color[] customOverlayPaletteColors;
    private Color[] featuredPalettePreviewColors;
    private Color[][] featuredPaletteThemes;
    private string[] featuredPaletteNames;
    private int activeFeaturedPaletteIndex = -1;
    private int selectedFeaturedColorIndex = -1;
    private float customPickerHue;
    private float customPickerSaturation = 1f;
    private float customPickerValue = 1f;
	public Texture2D selectedBorder,white,black,mainImage,testImage,selectedColor,unDo,shareFB,shareEmail,shareInsta,
	shareMessage,FBShare,home,popUpColor,fadedShare,fadedHome,fadedUndo,lockColor,saveImage,fadedsaveImg,transImg,IAPPopUp,Premium,priceBlock,restore,watermark,
	sharePopUp,rate,saveToGall,closeIAP,testwaterImage;
	public Texture2D supportImg;     //simo
    public Texture2D coverRectAvoidingTouchImg; // simo
	public Texture2D backgroundImg;  //simo : background
	private Rect backgroundImgRect;  //simo : background
	public Texture2D[] pencils,pencilTones;
    private Texture2D fillReferenceImage;
    private Texture2D fillRegionMask;
    private int fillRegionSeed = 1;
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
        float fitY = container.y;
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
        redoFillers.Clear();
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
        ReloadFillReferenceImage();
	}


	void LoadActualImage()
	{
		redoFillers.Clear();
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
        ReloadFillReferenceImage();
	}


    void ReloadFillReferenceImage()
    {
        byte[] originalBytes = DataManager.Instance.ReadOriginalImageBytes(DataManager.Instance.selectedFileName, DataManager.Instance.selectedResourceName);
        fillReferenceImage = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (originalBytes != null && originalBytes.Length > 0 && fillReferenceImage.LoadImage(originalBytes, false))
        {
            ResetFillRegionMask();
            return;
        }

        Texture2D fallback = Resources.Load<Texture2D>(DataManager.Instance.selectedResourceName);
        if (fallback != null)
        {
            fillReferenceImage = new Texture2D(fallback.width, fallback.height, TextureFormat.RGBA32, false);
            fillReferenceImage.SetPixels(fallback.GetPixels());
            fillReferenceImage.Apply();
            ResetFillRegionMask();
        }
    }


    void ResetFillRegionMask()
    {
        if (fillReferenceImage == null)
            return;

        fillRegionMask = new Texture2D(fillReferenceImage.width, fillReferenceImage.height, TextureFormat.RGBA32, false);
        fillRegionMask.SetPixels32(fillReferenceImage.GetPixels32());
        fillRegionMask.Apply();
        fillRegionSeed = 1;
    }


    Color32 GenerateNextRegionMarker()
    {
        fillRegionSeed++;
        byte r = (byte)(30 + ((fillRegionSeed * 53) % 200));
        byte g = (byte)(30 + ((fillRegionSeed * 97) % 200));
        byte b = (byte)(30 + ((fillRegionSeed * 151) % 200));
        return new Color32(r, g, b, 255);
    }


    bool IsWhiteLike(Color32 color, int tolerance = 10)
    {
        return color.r >= 255 - tolerance &&
               color.g >= 255 - tolerance &&
               color.b >= 255 - tolerance &&
               color.a > 0;
    }


    bool ColorsApproximatelyEqual(Color32 a, Color32 b, int tolerance = 8)
    {
        return Mathf.Abs(a.r - b.r) <= tolerance &&
               Mathf.Abs(a.g - b.g) <= tolerance &&
               Mathf.Abs(a.b - b.b) <= tolerance &&
               Mathf.Abs(a.a - b.a) <= tolerance;
    }


    bool IsGradientLikeRegion(Color32[] pixels, int width, int height, int hitX, int hitY, Color32 centerColor)
    {
        int mismatches = 0;
        int samples = 0;
        for (int dy = -2; dy <= 2; dy++)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                int sx = hitX + dx;
                int sy = hitY + dy;
                if (sx < 0 || sy < 0 || sx >= width || sy >= height)
                    continue;

                Color32 sample = pixels[sx + sy * width];
                if (sample == Color.black)
                    continue;

                samples++;
                if (!ColorsApproximatelyEqual(sample, centerColor))
                    mismatches++;
            }
        }

        return samples > 0 && mismatches >= 3;
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
        ReloadFillReferenceImage();
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
		startPixelToneX = toneRect.x;
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
		}while(startPixelToneX<(toneRect.x + toneRect.width));
        int bandIndex = Mathf.Clamp(Mathf.FloorToInt((startPixelToneX - toneRect.x) / Mathf.Max(1f, startPixelToneW)), 0, 10);
        SetSelectedToneBandIndex(bandIndex);
		
	}


    void SetSelectedToneBandIndex(int bandIndex)
    {
        selRect = GetToneBandDisplayRect(Mathf.Clamp(bandIndex, 0, 10));
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
		homeRect = new Rect (60 * scale_x, 20 * scale_y, 140 * scale_x, 95 * scale_y);
		homeTextRect = new Rect (100 * scale_x, 165 * scale_y, 270 * scale_x, 225 * scale_y);
		palleteRect = new Rect (0 * scale_x, 1848 * scale_y - (float)(Screen.height * 0.08f), Screen.width, 200 * scale_y);

        float swatchRowLift = 56f * scale_y;
        float mainRowLift = 88f * scale_y;
        const int featuredPaletteCount = 10;
        // simo : voiding touch under pencils+tone rect  rect  
        coverRectAvoidingTouch = new Rect(0, 1948 * scale_y - (float)(Screen.height * 0.2f) - swatchRowLift - mainRowLift, Screen.width, 330 * scale_y);
        // simo : support rect  SupportBanner
        supportImgRect  = new Rect((float)(Screen.width * 0.9f), (float)(Screen.height * 0.94f), 140 * scale_x, 100 * scale_y);
        //supportTextRect = new Rect(supportImgRect.x-Screen.width * 0.5f-20, (float)(Screen.height * 0.95f), Screen.width * 0.5f, 0 * scale_y);
        supportTextRect = new Rect((float)(Screen.width * 0.01f), (float)(Screen.height * 0.95f), Screen.width*0.40f, 0 * scale_y);
		// simo : background rect 
		backgroundImgRect = new Rect(-10, 0, (float)(Screen.width*1.2), (float)(Screen.height*1.2));
        //toneRect = new Rect(0, 1948 * scale_y, Screen.width, 150 * scale_y);
        //simo : TONE RECT MOVED

        // Tone palette position is finalized after swatch rows are laid out so it can keep a fixed gap from them.
        toneRect = new Rect (0, 1948 * scale_y-(float)(Screen.height * 0.12f) + 22f * scale_y - swatchRowLift + (74f * scale_y) - mainRowLift, Screen.width, 220 * scale_y);
        toneRect.height = (tonePaletteTopInsetPixels + tonePaletteTileHeightPixels * 2f + tonePaletteRowGapPixels + 40f) * scale_y;
		bannerRect=new Rect (0 * scale_x, 1848 * scale_y, Screen.width, 200 * scale_y);
		pencilRect = new Rect[pencils.Length-1];
        featuredPaletteToggleRects = new Rect[featuredPaletteCount];
        featuredPaletteSwatchRects = new Rect[11];
        palettePreviewColors = new Color[pencilRect.Length];
        swatchLiftOffsets = new float[pencilRect.Length];

		saveToGallRect=new Rect (875 * scale_x, 1220 * scale_y, 150 * scale_x, 200 * scale_y);
		pencilSelection = new bool[pencilRect.Length];
        // pencils positions
        float swatchSlotWidth = colorSwatchWidthPixels * scale_x;
        float swatchSlotHeight = colorSwatchHeightPixels * scale_y;
        float swatchStepX = colorSwatchStepXPixels * scale_x;
        int topRowCount = Mathf.CeilToInt(pencilRect.Length / 2f);
        int bottomRowCount = pencilRect.Length - topRowCount;
        float gradientControlsTopPadding = 20f + Mathf.Max(2.5f * scale_x, 2.5f);
        if (pencilRect.Length > 0)
        {
            float topControlWidth = 120f * scale_x;
            float topControlHeight = 120f * scale_y;
            float topControlY = 1808 * scale_y - (float)(Screen.height * 0.13f) - topControlHeight - (42f * scale_y) - mainRowLift;
            float topControlStartX = Mathf.Max(10f * scale_x, origImgRect.width > 0f ? origImgRect.x : imageViewportRect.x) + (20f * scale_x) + 10f;
            float toggleGap = 60f * scale_x;
            float featuredToggleGap = 24f * scale_x;
            float rightColumnX = Screen.width - topControlWidth - rightColumnPaddingPixels;
            float featuredArrowGap = 14f * scale_x;
            float featuredArrowWidth = 34f * scale_x;
            float featuredViewportGap = 10f * scale_x;
            whiteSwatchRect = new Rect(rightColumnX, topControlY, topControlWidth, topControlHeight);
            float paletteToggleSide = Mathf.Min(topControlWidth, topControlHeight) * 0.62f;
            float paletteToggleY = whiteSwatchRect.center.y - (paletteToggleSide * 0.5f);
            paletteToggleRect = new Rect(topControlStartX, paletteToggleY, paletteToggleSide, paletteToggleSide);
            gradientToggleRect = new Rect(
                paletteToggleRect.x + paletteToggleRect.width + toggleGap,
                paletteToggleY,
                paletteToggleSide,
                paletteToggleSide);
            float featuredAvailableStart = gradientToggleRect.xMax + featuredArrowGap + featuredArrowWidth + featuredViewportGap;
            float featuredAvailableEnd = rightColumnX - featuredArrowGap - featuredArrowWidth - featuredViewportGap;
            float featuredAvailableWidth = Mathf.Max(120f * scale_x, featuredAvailableEnd - featuredAvailableStart);
            float featuredToggleSize = topControlHeight * 0.76f;
            float featuredViewportInnerPadding = featuredThemeViewportInnerPaddingPixels * scale_x;
            float desiredFeaturedViewportWidth = featuredToggleSize * 6f + featuredToggleGap * 5f;
            float actualFeaturedViewportWidth = Mathf.Min(featuredAvailableWidth, desiredFeaturedViewportWidth);
            float featuredViewportX = featuredAvailableStart + (featuredAvailableWidth - actualFeaturedViewportWidth) * 0.5f;
            featuredThemeViewportRect = new Rect(
                featuredViewportX,
                topControlY,
                actualFeaturedViewportWidth,
                topControlHeight);
            featuredThemeLeftArrowRect = new Rect(
                featuredThemeViewportRect.xMin - featuredViewportGap - featuredArrowWidth,
                topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
                featuredArrowWidth,
                featuredArrowWidth);
            featuredThemeRightArrowRect = new Rect(
                featuredThemeViewportRect.xMax + featuredViewportGap,
                topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
                featuredArrowWidth,
                featuredArrowWidth);
            featuredToggleSize = Mathf.Min(featuredToggleSize, (featuredThemeViewportRect.width - featuredToggleGap * 5f) / 6f);
            float featuredToggleY = (topControlHeight - featuredToggleSize) * 0.5f;
            for (int i = 0; i < featuredPaletteToggleRects.Length; i++)
            {
                float toggleX = featuredViewportInnerPadding + i * (featuredToggleSize + featuredToggleGap);
                featuredPaletteToggleRects[i] = new Rect(toggleX, featuredToggleY, featuredToggleSize, featuredToggleSize);
            }
            featuredThemeContentWidth = featuredPaletteToggleRects.Length > 0 ? featuredPaletteToggleRects[featuredPaletteToggleRects.Length - 1].xMax + featuredViewportInnerPadding : 0f;
            float rightColumnControlSize = topControlWidth;
            customColorToggleRect = new Rect(rightColumnX, topControlY + topControlHeight + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);
            imagePickerToggleRect = new Rect(rightColumnX, customColorToggleRect.y + rightColumnControlSize + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);

            float gradientControlsBottomY = gradientToggleRect.yMax + gradientControlsTopPadding + (54f * scale_y) + (lowerContainerTopInsetPixels * scale_y) + (expandedPanelVerticalOffsetPixels * scale_y);
            float topRowY = gradientControlsBottomY + (colorSwatchTopRowOffsetPixels * scale_y);
            float bottomRowY = topRowY + (colorSwatchRowGapPixels * scale_y);
		    for (int pencilIndex=0; pencilIndex<pencilRect.Length; pencilIndex++) {
                bool isTopRow = pencilIndex < topRowCount;
                int rowCount = isTopRow ? topRowCount : Mathf.Max(1, bottomRowCount);
                int rowIndex = isTopRow ? pencilIndex : pencilIndex - topRowCount;
                float rowWidth = swatchSlotWidth + Mathf.Max(0, rowCount - 1) * swatchStepX;
                float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
                float rowY = isTopRow ? topRowY : bottomRowY;
                pencilRect[pencilIndex] = new Rect(
                    startX + rowIndex * swatchStepX,
                    rowY,
                    swatchSlotWidth,
                    swatchSlotHeight);
			    pencilSelection[pencilIndex]=false;
		    }
            int featuredTopRowCount = Mathf.CeilToInt(featuredPaletteSwatchRects.Length / 2f);
            int featuredBottomRowCount = featuredPaletteSwatchRects.Length - featuredTopRowCount;
            float featuredSlotWidth = featuredThemeSwatchWidthPixels * scale_x;
            float featuredSlotHeight = featuredThemeSwatchHeightPixels * scale_y;
            float featuredStepX = featuredThemeSwatchStepXPixels * scale_x;
            for (int swatchIndex = 0; swatchIndex < featuredPaletteSwatchRects.Length; swatchIndex++)
            {
                bool isTopRow = swatchIndex < featuredTopRowCount;
                int rowCount = isTopRow ? featuredTopRowCount : Mathf.Max(1, featuredBottomRowCount);
                int rowIndex = isTopRow ? swatchIndex : swatchIndex - featuredTopRowCount;
                float rowWidth = featuredSlotWidth + Mathf.Max(0, rowCount - 1) * featuredStepX;
                float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
                float rowY = isTopRow ? topRowY : bottomRowY;
                featuredPaletteSwatchRects[swatchIndex] = new Rect(
                    startX + rowIndex * featuredStepX,
                    rowY,
                    featuredSlotWidth,
                    featuredSlotHeight);
            }

            float swatchRowsBottom = 0f;
            for (int i = 0; i < pencilRect.Length; i++)
                swatchRowsBottom = Mathf.Max(swatchRowsBottom, pencilRect[i].yMax);
            float firstPaletteRowY = swatchRowsBottom + (tonePaletteGapFromSwatchesPixels * scale_y);
            toneRect.y = firstPaletteRowY - (tonePaletteTopInsetPixels * scale_y);
        }
        else
        {
            float topControlWidth = 110f * scale_x;
            float topControlHeight = 110f * scale_y;
            float topControlY = 1808 * scale_y - (float)(Screen.height * 0.13f) - swatchRowLift - mainRowLift;
            float toggleGap = 60f * scale_x;
            float featuredToggleGap = 24f * scale_x;
            float rightColumnX = Screen.width - topControlWidth - rightColumnPaddingPixels;
            float featuredArrowGap = 14f * scale_x;
            float featuredArrowWidth = 34f * scale_x;
            float featuredViewportGap = 10f * scale_x;
            whiteSwatchRect = new Rect(rightColumnX, topControlY, topControlWidth, topControlHeight);
            float paletteToggleSide = Mathf.Min(topControlWidth, topControlHeight) * 0.62f;
            float paletteToggleY = whiteSwatchRect.center.y - (paletteToggleSide * 0.5f);
            paletteToggleRect = new Rect(30f * scale_x, paletteToggleY, paletteToggleSide, paletteToggleSide);
            gradientToggleRect = new Rect(paletteToggleRect.x + paletteToggleRect.width + toggleGap, paletteToggleRect.y, paletteToggleRect.width, paletteToggleRect.height);
            float featuredAvailableStart = gradientToggleRect.xMax + featuredArrowGap + featuredArrowWidth + featuredViewportGap;
            float featuredAvailableEnd = rightColumnX - featuredArrowGap - featuredArrowWidth - featuredViewportGap;
            float featuredAvailableWidth = Mathf.Max(120f * scale_x, featuredAvailableEnd - featuredAvailableStart);
            float featuredToggleSize = topControlHeight * 0.76f;
            float featuredViewportInnerPadding = featuredThemeViewportInnerPaddingPixels * scale_x;
            float desiredFeaturedViewportWidth = featuredToggleSize * 6f + featuredToggleGap * 5f;
            float actualFeaturedViewportWidth = Mathf.Min(featuredAvailableWidth, desiredFeaturedViewportWidth);
            float featuredViewportX = featuredAvailableStart + (featuredAvailableWidth - actualFeaturedViewportWidth) * 0.5f;
            featuredThemeViewportRect = new Rect(
                featuredViewportX,
                topControlY,
                actualFeaturedViewportWidth,
                topControlHeight);
            featuredThemeLeftArrowRect = new Rect(
                featuredThemeViewportRect.xMin - featuredViewportGap - featuredArrowWidth,
                topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
                featuredArrowWidth,
                featuredArrowWidth);
            featuredThemeRightArrowRect = new Rect(
                featuredThemeViewportRect.xMax + featuredViewportGap,
                topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
                featuredArrowWidth,
                featuredArrowWidth);
            featuredToggleSize = Mathf.Min(featuredToggleSize, (featuredThemeViewportRect.width - featuredToggleGap * 5f) / 6f);
            float featuredToggleY = (topControlHeight - featuredToggleSize) * 0.5f;
            for (int i = 0; i < featuredPaletteToggleRects.Length; i++)
            {
                float toggleX = featuredViewportInnerPadding + i * (featuredToggleSize + featuredToggleGap);
                featuredPaletteToggleRects[i] = new Rect(toggleX, featuredToggleY, featuredToggleSize, featuredToggleSize);
            }
            featuredThemeContentWidth = featuredPaletteToggleRects.Length > 0 ? featuredPaletteToggleRects[featuredPaletteToggleRects.Length - 1].xMax + featuredViewportInnerPadding : 0f;
            float rightColumnControlSize = topControlWidth;
            customColorToggleRect = new Rect(rightColumnX, topControlY + topControlHeight + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);
            imagePickerToggleRect = new Rect(rightColumnX, customColorToggleRect.y + rightColumnControlSize + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);

            float gradientControlsBottomY = gradientToggleRect.yMax + gradientControlsTopPadding + (54f * scale_y) + (lowerContainerTopInsetPixels * scale_y);
            float topRowY = gradientControlsBottomY + (colorSwatchTopRowOffsetPixels * scale_y);
            float bottomRowY = topRowY + (colorSwatchRowGapPixels * scale_y);
		    for (int pencilIndex=0; pencilIndex<pencilRect.Length; pencilIndex++) {
                bool isTopRow = pencilIndex < topRowCount;
                int rowCount = isTopRow ? topRowCount : Mathf.Max(1, bottomRowCount);
                int rowIndex = isTopRow ? pencilIndex : pencilIndex - topRowCount;
                float rowWidth = swatchSlotWidth + Mathf.Max(0, rowCount - 1) * swatchStepX;
                float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
                float rowY = isTopRow ? topRowY : bottomRowY;
                pencilRect[pencilIndex] = new Rect(
                    startX + rowIndex * swatchStepX,
                    rowY,
                    swatchSlotWidth,
                    swatchSlotHeight);
			    pencilSelection[pencilIndex]=false;
		    }
            int featuredTopRowCount = Mathf.CeilToInt(featuredPaletteSwatchRects.Length / 2f);
            int featuredBottomRowCount = featuredPaletteSwatchRects.Length - featuredTopRowCount;
            float featuredSlotWidth = featuredThemeSwatchWidthPixels * scale_x;
            float featuredSlotHeight = featuredThemeSwatchHeightPixels * scale_y;
            float featuredStepX = featuredThemeSwatchStepXPixels * scale_x;
            for (int swatchIndex = 0; swatchIndex < featuredPaletteSwatchRects.Length; swatchIndex++)
            {
                bool isTopRow = swatchIndex < featuredTopRowCount;
                int rowCount = isTopRow ? featuredTopRowCount : Mathf.Max(1, featuredBottomRowCount);
                int rowIndex = isTopRow ? swatchIndex : swatchIndex - featuredTopRowCount;
                float rowWidth = featuredSlotWidth + Mathf.Max(0, rowCount - 1) * featuredStepX;
                float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
                float rowY = isTopRow ? topRowY : bottomRowY;
                featuredPaletteSwatchRects[swatchIndex] = new Rect(
                    startX + rowIndex * featuredStepX,
                    rowY,
                    featuredSlotWidth,
                    featuredSlotHeight);
            }

            float swatchRowsBottom = 0f;
            for (int i = 0; i < pencilRect.Length; i++)
                swatchRowsBottom = Mathf.Max(swatchRowsBottom, pencilRect[i].yMax);
            float firstPaletteRowY = swatchRowsBottom + (tonePaletteGapFromSwatchesPixels * scale_y);
            toneRect.y = firstPaletteRowY - (tonePaletteTopInsetPixels * scale_y);
        }
        featuredThemeRowScroll = Mathf.Clamp(featuredThemeRowScroll, 0f, Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width));
        float customPanelLeft = origImgRect.width > 0f ? origImgRect.xMin : imageViewportRect.xMin;
        float customPanelRight = customColorToggleRect.xMin - 10f;
        float customPanelTop = Mathf.Max(featuredThemeViewportRect.yMax, whiteSwatchRect.yMax) + (14f * scale_y) + (lowerContainerTopInsetPixels * scale_y) + (expandedPanelVerticalOffsetPixels * scale_y);
        float customPanelHeight = 330f * scale_y;
        customColorOverlayRect = new Rect(customPanelLeft, customPanelTop, Mathf.Max(220f * scale_x, customPanelRight - customPanelLeft), customPanelHeight);
        float customPanelPadding = 22f * scale_x;
        float customTopPadding = 20f * scale_y;
        float customLeftSectionWidth = Mathf.Max(220f * scale_x, customColorOverlayRect.width * 0.48f);
        float customRightSectionX = customColorOverlayRect.x + customLeftSectionWidth + (22f * scale_x);
        float customRightSectionWidth = customColorOverlayRect.xMax - customPanelPadding - customRightSectionX;
        customSvSquareRect = new Rect(customColorOverlayRect.x + customPanelPadding, customColorOverlayRect.y + customTopPadding, customLeftSectionWidth - customPanelPadding * 2f, customColorOverlayRect.height - customTopPadding * 2f - (56f * scale_y));
        customHueSliderRect = new Rect(customSvSquareRect.x, customSvSquareRect.yMax + (16f * scale_y), customSvSquareRect.width, 30f * scale_y);
        gradientDirectionRect = new Rect(gradientToggleRect.x - (24f * scale_x), gradientToggleRect.yMax + gradientControlsTopPadding + (lowerContainerTopInsetPixels * scale_y), 168f * scale_x, 54f * scale_y);
        float rotateButtonSize = gradientDirectionRect.height + (6f * scale_y);
        gradientRotateLeftRect = new Rect(gradientDirectionRect.x - ((20f * scale_x) + rotateButtonSize), gradientDirectionRect.center.y - rotateButtonSize * 0.5f, rotateButtonSize, rotateButtonSize);
        gradientRotateRect = new Rect(gradientDirectionRect.x + gradientDirectionRect.width + (20f * scale_x), gradientDirectionRect.center.y - rotateButtonSize * 0.5f, rotateButtonSize, rotateButtonSize);
        gradientResetRect = new Rect(gradientRotateRect.x + gradientRotateRect.width + (20f * scale_x), gradientDirectionRect.y, 120f * scale_x, gradientDirectionRect.height);
        float gradientBoostSliderHeight = 34f * scale_y;
        gradientBoostSliderRect = new Rect(
            gradientResetRect.x + gradientResetRect.width + (22f * scale_x),
            gradientDirectionRect.center.y - gradientBoostSliderHeight * 0.5f,
            152f * scale_x,
            gradientBoostSliderHeight);
        gradientBoostLabelRect = new Rect(
            gradientBoostSliderRect.x + gradientBoostSliderRect.width + (10f * scale_x),
            gradientDirectionRect.center.y - (42f * scale_y) * 0.5f,
            110f * scale_x,
            42f * scale_y);
        showSwatchPalette = false;
        showGradientPalette = false;
        showCustomColorOverlay = false;
        showAdvancedCustomPicker = true;
        imagePickerMode = false;
        gradientModeActive = false;
        gradientBandSelected = false;
        gradientRotationDegrees = 0f;
        BuildPalettePreviewColors();
        RefreshPaletteToggleTextures();
        BuildFeaturedPaletteThemes();
        BuildCustomOverlayPaletteColors();
        LoadSavedCustomColors();
        customOverlayActiveColor = palettePreviewColors != null && palettePreviewColors.Length > 0 ? palettePreviewColors[0] : Color.white;
        Color.RGBToHSV(customOverlayActiveColor, out customPickerHue, out customPickerSaturation, out customPickerValue);
        EnsureCircularSwatchMask();
        EnsureRoundedDirectionButtonTexture();
        EnsureCustomHueSliderTexture();
        RefreshCustomSvSquareTexture();
        float viewportSidePadding = 24f * scale_x;
        float viewportTopPadding = 24f * scale_y;
        float viewportTop = topWhiteRect.y + topWhiteRect.height + viewportTopPadding;
        float viewportBottom = toneRect.y - (28f * scale_y);
        float viewportHeight = Mathf.Max(200f * scale_y, viewportBottom - viewportTop);
        imageViewportRect = new Rect(
            viewportSidePadding,
            viewportTop,
            Screen.width - (viewportSidePadding * 2f),
            viewportHeight);

        watermarkImageRect = new Rect(
            Mathf.Max(0f, imageViewportRect.x - (20f * scale_x)),
            Mathf.Max(0f, imageViewportRect.y - (20f * scale_y)),
            imageViewportRect.width + (40f * scale_x),
            imageViewportRect.height + (40f * scale_y));

        origImgRect = GetAspectFitRect(imageViewportRect, mainImage.width, mainImage.height);
		Pricing = ImagePathHolder.GetPricing ();
		imageRect = origImgRect;
        RefreshRuntimePaletteLayout();
		// customSkin.customStyles[0].font=Resources.Load<Font>("font/calibri");
		// customStyle.font=Resources.Load<Font>("font/calibri");
        customSkin.customStyles[0].font = Resources.Load<Font>("font/mandala");  //simo
        customStyle.font = Resources.Load<Font>("font/mandala");  //simo

		selRect = new Rect (Screen.width / 2 - (50 * scale_x), 1600 * scale_y, 100 * scale_x, 100 * scale_y);
		inappPopupRect = new Rect (120 * scale_x, 420 * scale_y, 1300 * scale_x, 1150 * scale_y);
		// unDoRect=new Rect (705*scale_x, 20 * scale_y, 165 * scale_x, 125 * scale_y);
		float undoWidth = 115 * scale_x;
		float undoHeight = 95 * scale_y;
		float undoRedoGap = 35 * scale_x;
		float undoRedoGroupWidth = undoWidth * 2f + undoRedoGap;
		float undoRedoStartX = (Screen.width - undoRedoGroupWidth) * 0.5f;
		unDoRect = new Rect(undoRedoStartX, 20 * scale_y, undoWidth, undoHeight); // simo
        redoRect = new Rect(unDoRect.x + unDoRect.width + undoRedoGap, unDoRect.y, unDoRect.width, unDoRect.height);

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
		selectedColor = new Texture2D (PalettePreviewTextureWidth, PalettePreviewTextureHeight, TextureFormat.RGBA32, false);
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


    void RefreshRuntimePaletteLayout()
    {
        if (pencilRect == null || featuredPaletteToggleRects == null || featuredPaletteSwatchRects == null)
            return;

        float swatchRowLift = 56f * scale_y;
        float mainRowLift = 88f * scale_y;
        float swatchSlotWidth = colorSwatchWidthPixels * scale_x;
        float swatchSlotHeight = colorSwatchHeightPixels * scale_y;
        float swatchStepX = colorSwatchStepXPixels * scale_x;
        int topRowCount = Mathf.CeilToInt(pencilRect.Length / 2f);
        int bottomRowCount = pencilRect.Length - topRowCount;

        toneRect.height = (tonePaletteTopInsetPixels + tonePaletteTileHeightPixels * 2f + tonePaletteRowGapPixels + 40f) * scale_y;
        coverRectAvoidingTouch = new Rect(0, 1948 * scale_y - (float)(Screen.height * 0.2f) - swatchRowLift - mainRowLift, Screen.width, 330 * scale_y);

        float topControlWidth = pencilRect.Length > 0 ? 120f * scale_x : 110f * scale_x;
        float topControlHeight = pencilRect.Length > 0 ? 120f * scale_y : 110f * scale_y;
        float imageBottomY = origImgRect.height > 0f ? origImgRect.yMax : imageViewportRect.yMax;
        float topControlY = imageBottomY + ((mainRowGapFromImagePixels + mainRowBackdropVerticalPaddingPixels) * scale_y);
        float topControlStartX = pencilRect.Length > 0 ? Mathf.Max(10f * scale_x, origImgRect.width > 0f ? origImgRect.x : imageViewportRect.x) + (20f * scale_x) + 10f : 30f * scale_x;
        float toggleGap = 60f * scale_x;
        float featuredToggleGap = 24f * scale_x;
        float rightColumnX = Screen.width - topControlWidth - rightColumnPaddingPixels;
        float featuredArrowGap = 14f * scale_x;
        float featuredArrowWidth = 34f * scale_x;
        float featuredViewportGap = 10f * scale_x;

        whiteSwatchRect = new Rect(rightColumnX, topControlY, topControlWidth, topControlHeight);
        float paletteToggleSide = Mathf.Min(topControlWidth, topControlHeight) * 0.62f;
        float paletteToggleY = whiteSwatchRect.center.y - (paletteToggleSide * 0.5f);

        paletteToggleRect = new Rect(topControlStartX, paletteToggleY, paletteToggleSide, paletteToggleSide);
        gradientToggleRect = new Rect(paletteToggleRect.x + paletteToggleRect.width + toggleGap, paletteToggleY, paletteToggleSide, paletteToggleSide);
        float featuredAvailableStart = gradientToggleRect.xMax + featuredArrowGap + featuredArrowWidth + featuredViewportGap;
        float featuredAvailableEnd = rightColumnX - featuredArrowGap - featuredArrowWidth - featuredViewportGap;
        float featuredAvailableWidth = Mathf.Max(120f * scale_x, featuredAvailableEnd - featuredAvailableStart);
        float featuredToggleSize = topControlHeight * 0.76f;
        float featuredViewportInnerPadding = featuredThemeViewportInnerPaddingPixels * scale_x;
        float desiredFeaturedViewportWidth = featuredToggleSize * 6f + featuredToggleGap * 5f;
        float actualFeaturedViewportWidth = Mathf.Min(featuredAvailableWidth, desiredFeaturedViewportWidth);
        float featuredViewportX = featuredAvailableStart + (featuredAvailableWidth - actualFeaturedViewportWidth) * 0.5f;
        featuredThemeViewportRect = new Rect(
            featuredViewportX,
            topControlY,
            actualFeaturedViewportWidth,
            topControlHeight);
        featuredThemeLeftArrowRect = new Rect(
            featuredThemeViewportRect.xMin - featuredViewportGap - featuredArrowWidth,
            topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
            featuredArrowWidth,
            featuredArrowWidth);
        featuredThemeRightArrowRect = new Rect(
            featuredThemeViewportRect.xMax + featuredViewportGap,
            topControlY + (topControlHeight - featuredArrowWidth) * 0.5f,
            featuredArrowWidth,
            featuredArrowWidth);
        featuredToggleSize = Mathf.Min(featuredToggleSize, (featuredThemeViewportRect.width - featuredToggleGap * 5f) / 6f);
        float featuredToggleY = (topControlHeight - featuredToggleSize) * 0.5f;
        for (int i = 0; i < featuredPaletteToggleRects.Length; i++)
        {
            float toggleX = featuredViewportInnerPadding + i * (featuredToggleSize + featuredToggleGap);
            featuredPaletteToggleRects[i] = new Rect(toggleX, featuredToggleY, featuredToggleSize, featuredToggleSize);
        }
        featuredThemeContentWidth = featuredPaletteToggleRects.Length > 0 ? featuredPaletteToggleRects[featuredPaletteToggleRects.Length - 1].xMax + featuredViewportInnerPadding : 0f;
        featuredThemeRowScroll = Mathf.Clamp(featuredThemeRowScroll, 0f, Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width));

        float rightColumnControlSize = topControlWidth;
        customColorToggleRect = new Rect(rightColumnX, topControlY + topControlHeight + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);
        imagePickerToggleRect = new Rect(rightColumnX, customColorToggleRect.y + rightColumnControlSize + (18f * scale_y), rightColumnControlSize, rightColumnControlSize);

        float gradientControlsTopPadding = 20f + Mathf.Max(2.5f * scale_x, 2.5f);
        float gradientControlsBottomY = gradientToggleRect.yMax + gradientControlsTopPadding + (54f * scale_y) + (lowerContainerTopInsetPixels * scale_y) + (expandedPanelVerticalOffsetPixels * scale_y);
        float topRowY = gradientControlsBottomY + (colorSwatchTopRowOffsetPixels * scale_y);
        float bottomRowY = topRowY + (colorSwatchRowGapPixels * scale_y);

        for (int pencilIndex = 0; pencilIndex < pencilRect.Length; pencilIndex++)
        {
            bool isTopRow = pencilIndex < topRowCount;
            int rowCount = isTopRow ? topRowCount : Mathf.Max(1, bottomRowCount);
            int rowIndex = isTopRow ? pencilIndex : pencilIndex - topRowCount;
            float rowWidth = swatchSlotWidth + Mathf.Max(0, rowCount - 1) * swatchStepX;
            float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
            float rowY = isTopRow ? topRowY : bottomRowY;
            pencilRect[pencilIndex] = new Rect(startX + rowIndex * swatchStepX, rowY, swatchSlotWidth, swatchSlotHeight);
        }

        int featuredTopRowCount = Mathf.CeilToInt(featuredPaletteSwatchRects.Length / 2f);
        int featuredBottomRowCount = featuredPaletteSwatchRects.Length - featuredTopRowCount;
        float featuredSwatchDiameter = featuredThemeSwatchDiameterPixels * Mathf.Min(scale_x, scale_y);
        float featuredVerticalGap = featuredThemeRowGapPixels * scale_y;
        float featuredRowDeltaY = featuredSwatchDiameter + featuredVerticalGap;
        float featuredBottomRowY = topRowY + featuredRowDeltaY;
        float featuredSlotWidth = featuredThemeSwatchWidthPixels * scale_x;
        float featuredSlotHeight = featuredThemeSwatchHeightPixels * scale_y;
        float featuredStepX = featuredThemeSwatchStepXPixels * scale_x;
        for (int swatchIndex = 0; swatchIndex < featuredPaletteSwatchRects.Length; swatchIndex++)
        {
            bool isTopRow = swatchIndex < featuredTopRowCount;
            int rowCount = isTopRow ? featuredTopRowCount : Mathf.Max(1, featuredBottomRowCount);
            int rowIndex = isTopRow ? swatchIndex : swatchIndex - featuredTopRowCount;
            float rowWidth = featuredSlotWidth + Mathf.Max(0, rowCount - 1) * featuredStepX;
            float startX = (Screen.width - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
            float rowY = isTopRow ? topRowY : featuredBottomRowY;
            featuredPaletteSwatchRects[swatchIndex] = new Rect(startX + rowIndex * featuredStepX, rowY, featuredSlotWidth, featuredSlotHeight);
        }

        float swatchRowsBottom = 0f;
        for (int i = 0; i < pencilRect.Length; i++)
            swatchRowsBottom = Mathf.Max(swatchRowsBottom, pencilRect[i].yMax);
        float firstPaletteRowY = swatchRowsBottom + (tonePaletteGapFromSwatchesPixels * scale_y);
        toneRect.y = firstPaletteRowY - (tonePaletteTopInsetPixels * scale_y);

        float customPanelLeft = origImgRect.width > 0f ? origImgRect.xMin : imageViewportRect.xMin;
        float customPanelRight = customColorToggleRect.xMin - 10f;
        float customPanelTop = Mathf.Max(featuredThemeViewportRect.yMax, whiteSwatchRect.yMax) + (14f * scale_y) + (lowerContainerTopInsetPixels * scale_y) + (expandedPanelVerticalOffsetPixels * scale_y);
        float customPanelHeight = 330f * scale_y;
        customColorOverlayRect = new Rect(customPanelLeft, customPanelTop, Mathf.Max(220f * scale_x, customPanelRight - customPanelLeft), customPanelHeight);
        float customPanelPadding = 22f * scale_x;
        float customTopPadding = 20f * scale_y;
        float customLeftSectionWidth = Mathf.Max(220f * scale_x, customColorOverlayRect.width * 0.48f);
        float customRightSectionX = customColorOverlayRect.x + customLeftSectionWidth + (22f * scale_x);
        float customRightSectionWidth = customColorOverlayRect.xMax - customPanelPadding - customRightSectionX;
        customSvSquareRect = new Rect(customColorOverlayRect.x + customPanelPadding, customColorOverlayRect.y + customTopPadding, customLeftSectionWidth - customPanelPadding * 2f, customColorOverlayRect.height - customTopPadding * 2f - (56f * scale_y));
        customHueSliderRect = new Rect(customSvSquareRect.x, customSvSquareRect.yMax + (16f * scale_y), customSvSquareRect.width, 30f * scale_y);
        float gradientControlGap = 20f * scale_x;
        float gradientControlsY = gradientToggleRect.yMax + gradientControlsTopPadding + (lowerContainerTopInsetPixels * scale_y);
        gradientDirectionRect = new Rect(gradientToggleRect.x + (60f * scale_x), gradientControlsY, 168f * scale_x, 54f * scale_y);
        float rotateButtonSize = gradientDirectionRect.height + (6f * scale_y);
        gradientRotateLeftRect = new Rect(gradientDirectionRect.x - gradientControlGap - rotateButtonSize, gradientDirectionRect.center.y - rotateButtonSize * 0.5f, rotateButtonSize, rotateButtonSize);
        gradientRotateRect = new Rect(gradientDirectionRect.x + gradientDirectionRect.width + gradientControlGap, gradientDirectionRect.center.y - rotateButtonSize * 0.5f, rotateButtonSize, rotateButtonSize);
        gradientResetRect = new Rect(gradientRotateRect.x + gradientRotateRect.width + gradientControlGap, gradientControlsY, 120f * scale_x, gradientDirectionRect.height);
        float gradientBoostSliderHeight = 34f * scale_y;
        gradientBoostSliderRect = new Rect(
            gradientResetRect.x + gradientResetRect.width + (22f * scale_x),
            gradientDirectionRect.center.y - gradientBoostSliderHeight * 0.5f,
            152f * scale_x,
            gradientBoostSliderHeight);
        gradientBoostLabelRect = new Rect(
            gradientBoostSliderRect.x + gradientBoostSliderRect.width + (10f * scale_x),
            gradientDirectionRect.center.y - (42f * scale_y) * 0.5f,
            110f * scale_x,
            42f * scale_y);
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


    void BuildCustomOverlayPaletteColors()
    {
        customOverlayPaletteColors = new[]
        {
            new Color(0.94f, 0.29f, 0.27f, 1f),
            new Color(0.98f, 0.54f, 0.21f, 1f),
            new Color(0.99f, 0.83f, 0.25f, 1f),
            new Color(0.67f, 0.83f, 0.27f, 1f),
            new Color(0.28f, 0.74f, 0.42f, 1f),
            new Color(0.22f, 0.76f, 0.72f, 1f),
            new Color(0.28f, 0.62f, 0.92f, 1f),
            new Color(0.26f, 0.38f, 0.92f, 1f),
            new Color(0.47f, 0.34f, 0.88f, 1f),
            new Color(0.73f, 0.35f, 0.92f, 1f),
            new Color(0.95f, 0.38f, 0.74f, 1f),
            new Color(0.73f, 0.45f, 0.28f, 1f),
            new Color(0.93f, 0.78f, 0.68f, 1f),
            new Color(0.72f, 0.72f, 0.72f, 1f),
            new Color(0.42f, 0.42f, 0.42f, 1f),
            new Color(0.08f, 0.08f, 0.08f, 1f)
        };
    }

    string GetSavedCustomColorKey(int index)
    {
        return "custom_saved_color_" + index;
    }

    void LoadSavedCustomColors()
    {
        savedCustomColors = new Color[SavedCustomColorCount];
        for (int i = 0; i < SavedCustomColorCount; i++)
        {
            string key = GetSavedCustomColorKey(i);
            if (!PlayerPrefs.HasKey(key))
            {
                savedCustomColors[i] = new Color(0f, 0f, 0f, 0f);
                continue;
            }

            Color loaded;
            if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString(key), out loaded))
                savedCustomColors[i] = loaded;
            else
                savedCustomColors[i] = new Color(0f, 0f, 0f, 0f);
        }
    }

    void SaveSavedCustomColors()
    {
        if (savedCustomColors == null)
            return;

        for (int i = 0; i < savedCustomColors.Length; i++)
        {
            Color color = savedCustomColors[i];
            if (color.a <= 0f)
                PlayerPrefs.DeleteKey(GetSavedCustomColorKey(i));
            else
                PlayerPrefs.SetString(GetSavedCustomColorKey(i), "#" + ColorUtility.ToHtmlStringRGBA(color));
        }
        PlayerPrefs.Save();
    }

    void StoreSelectedCustomColorInSlot(int slotIndex)
    {
        if (savedCustomColors == null || savedCustomColors.Length != SavedCustomColorCount)
            LoadSavedCustomColors();

        if (slotIndex < 0 || slotIndex >= savedCustomColors.Length)
            return;

        Color selected = customOverlayActiveColor;
        selected.a = 1f;
        savedCustomColors[slotIndex] = selected;
        SaveSavedCustomColors();
    }

    void ClearSavedCustomColors()
    {
        if (savedCustomColors == null || savedCustomColors.Length != SavedCustomColorCount)
            LoadSavedCustomColors();

        for (int i = 0; i < savedCustomColors.Length; i++)
            savedCustomColors[i] = new Color(0f, 0f, 0f, 0f);

        SaveSavedCustomColors();
    }


    Color HexColor(byte r, byte g, byte b)
    {
        return new Color32(r, g, b, 255);
    }


    void BuildFeaturedPaletteThemes()
    {
        featuredPaletteNames = new[]
        {
            "SunsetSorbet",
            "LagoonDream",
            "CitrusBloom",
            "BerryVelvet",
            "EarthMuse",
            "FieryRedSunset",
            "WarmNeutralTones",
            "EmberFusion",
            "PurpleRaindrops",
            "TrendingTideCandy"
        };

        featuredPaletteThemes = new[]
        {
            new[]
            {
                HexColor(255, 214, 165),
                HexColor(255, 189, 117),
                HexColor(255, 160, 122),
                HexColor(255, 127, 80),
                HexColor(244, 96, 54),
                HexColor(232, 78, 99),
                HexColor(214, 61, 121),
                HexColor(188, 59, 140),
                HexColor(149, 54, 138),
                HexColor(108, 52, 131),
                HexColor(78, 52, 120)
            },
            new[]
            {
                HexColor(215, 255, 247),
                HexColor(176, 244, 231),
                HexColor(130, 228, 221),
                HexColor(94, 209, 216),
                HexColor(67, 185, 213),
                HexColor(51, 157, 204),
                HexColor(50, 129, 188),
                HexColor(58, 104, 170),
                HexColor(67, 82, 151),
                HexColor(67, 65, 129),
                HexColor(58, 52, 102)
            },
            new[]
            {
                HexColor(255, 247, 174),
                HexColor(255, 232, 126),
                HexColor(255, 214, 84),
                HexColor(255, 197, 61),
                HexColor(251, 171, 60),
                HexColor(244, 140, 73),
                HexColor(232, 109, 84),
                HexColor(205, 149, 73),
                HexColor(168, 190, 74),
                HexColor(115, 183, 89),
                HexColor(65, 155, 107)
            },
            new[]
            {
                HexColor(255, 210, 232),
                HexColor(248, 172, 215),
                HexColor(235, 130, 199),
                HexColor(211, 97, 182),
                HexColor(176, 73, 162),
                HexColor(136, 58, 142),
                HexColor(100, 49, 125),
                HexColor(81, 45, 115),
                HexColor(62, 43, 104),
                HexColor(49, 37, 88),
                HexColor(34, 27, 67)
            },
            new[]
            {
                HexColor(248, 239, 223),
                HexColor(231, 217, 193),
                HexColor(214, 191, 160),
                HexColor(193, 164, 123),
                HexColor(174, 139, 95),
                HexColor(152, 116, 79),
                HexColor(129, 97, 68),
                HexColor(111, 87, 67),
                HexColor(102, 122, 83),
                HexColor(98, 145, 112),
                HexColor(101, 170, 145)
            },
            new[]
            {
                HexColor(3, 7, 30),
                HexColor(55, 6, 23),
                HexColor(106, 4, 15),
                HexColor(157, 2, 8),
                HexColor(208, 0, 0),
                HexColor(220, 47, 2),
                HexColor(232, 93, 4),
                HexColor(244, 140, 6),
                HexColor(250, 163, 7),
                HexColor(255, 186, 8),
                HexColor(255, 210, 80)
            },
            new[]
            {
                HexColor(88, 47, 14),
                HexColor(127, 79, 36),
                HexColor(147, 102, 57),
                HexColor(166, 138, 100),
                HexColor(182, 173, 144),
                HexColor(194, 197, 170),
                HexColor(180, 189, 148),
                HexColor(164, 172, 134),
                HexColor(101, 109, 74),
                HexColor(65, 72, 51),
                HexColor(51, 61, 41)
            },
            new[]
            {
                HexColor(79, 0, 11),
                HexColor(95, 15, 64),
                HexColor(114, 0, 38),
                HexColor(154, 3, 30),
                HexColor(206, 66, 87),
                HexColor(227, 100, 20),
                HexColor(251, 139, 36),
                HexColor(255, 127, 81),
                HexColor(255, 155, 84),
                HexColor(202, 112, 65),
                HexColor(15, 76, 92)
            },
            new[]
            {
                HexColor(247, 37, 133),
                HexColor(181, 23, 158),
                HexColor(114, 9, 183),
                HexColor(86, 11, 173),
                HexColor(72, 12, 168),
                HexColor(58, 12, 163),
                HexColor(63, 55, 201),
                HexColor(67, 97, 238),
                HexColor(72, 149, 239),
                HexColor(76, 201, 240),
                HexColor(183, 133, 255)
            },
            new[]
            {
                HexColor(255, 209, 102),
                HexColor(247, 127, 0),
                HexColor(214, 40, 40),
                HexColor(168, 24, 48),
                HexColor(0, 48, 73),
                HexColor(102, 155, 188),
                HexColor(111, 255, 233),
                HexColor(255, 159, 28),
                HexColor(255, 92, 122),
                HexColor(46, 196, 182),
                HexColor(1, 22, 39)
            }
        };

        featuredPalettePreviewColors = new Color[featuredPaletteThemes.Length];
        featuredPaletteToggleTextures = new Texture2D[featuredPaletteThemes.Length];
        for (int i = 0; i < featuredPaletteThemes.Length; i++)
        {
            Color[] theme = featuredPaletteThemes[i];
            featuredPalettePreviewColors[i] = theme != null && theme.Length > 0 ? theme[theme.Length / 2] : Color.white;
            featuredPaletteToggleTextures[i] = BuildFeaturedPaletteToggleTexture(theme);
        }
    }


    Texture2D BuildFeaturedPaletteToggleTexture(Color[] theme)
    {
        const int size = 256;
        const float feather = 2.5f;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        Color[] colors = new Color[size * size];
        int bandCount = theme != null && theme.Length > 0 ? theme.Length : 1;
        float bandWidth = size / (float)bandCount;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float alpha = ComputeCircleAlpha(x, y, size, feather);
                if (alpha <= 0f)
                {
                    colors[y * size + x] = new Color(0f, 0f, 0f, 0f);
                    continue;
                }

                int bandIndex = theme != null && theme.Length > 0 ? Mathf.Clamp(Mathf.FloorToInt(x / bandWidth), 0, theme.Length - 1) : 0;
                Color bandColor = theme != null && theme.Length > 0 ? theme[bandIndex] : Color.white;
                bandColor.a = alpha;
                colors[y * size + x] = bandColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }


    void UpdateSwatchLiftAnimation()
    {
        if (swatchLiftOffsets == null || pencilRect == null || swatchLiftOffsets.Length != pencilRect.Length)
            return;

        float targetLift = swatchLiftPixels * scale_y;
        float speed = targetLift / Mathf.Max(0.01f, swatchLiftDuration);
        float whiteTarget = 0f;
        whiteSwatchLiftOffset = Mathf.MoveTowards(whiteSwatchLiftOffset, whiteTarget, speed * Time.deltaTime);
        float targetWhiteScale = whiteSwatchSelected ? 0.16f : 0f;
        float scaleSpeed = 0.16f / Mathf.Max(0.01f, swatchLiftDuration);
        whiteSwatchScaleOffset = Mathf.MoveTowards(whiteSwatchScaleOffset, targetWhiteScale, scaleSpeed * Time.deltaTime);
        for (int i = 0; i < swatchLiftOffsets.Length; i++)
        {
            float target = (i == selectedToneIndex) ? targetLift : 0f;
            swatchLiftOffsets[i] = Mathf.MoveTowards(swatchLiftOffsets[i], target, speed * Time.deltaTime);
        }
    }


    float ComputeCircleAlpha(int x, int y, int size, float feather)
    {
        float center = (size - 1) * 0.5f;
        float radius = center;
        float dx = x - center;
        float dy = y - center;
        float dist = Mathf.Sqrt(dx * dx + dy * dy);
        return 1f - Mathf.Clamp01((dist - (radius - feather)) / Mathf.Max(feather, 0.0001f));
    }


    float ComputeRoundedRectAlpha(int x, int y, int width, int height, float radius, float feather)
    {
        float dx = Mathf.Max(Mathf.Abs(x - (width * 0.5f)) - (width * 0.5f - radius), 0f);
        float dy = Mathf.Max(Mathf.Abs(y - (height * 0.5f)) - (height * 0.5f - radius), 0f);
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        return 1f - Mathf.Clamp01((distance - (radius - feather)) / Mathf.Max(feather, 0.0001f));
    }


    void EnsureCircularSwatchMask()
    {
        if (circularSwatchMask != null)
            return;

        const int size = 256;
        const float feather = 2.5f;
        circularSwatchMask = new Texture2D(size, size, TextureFormat.RGBA32, false);
        circularSwatchMask.wrapMode = TextureWrapMode.Clamp;
        circularSwatchMask.filterMode = FilterMode.Bilinear;

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color solid = new Color(1f, 1f, 1f, 1f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float alpha = ComputeCircleAlpha(x, y, size, feather);
                Color pixel = solid;
                pixel.a = alpha;
                circularSwatchMask.SetPixel(x, y, alpha > 0f ? pixel : clear);
            }
        }
        circularSwatchMask.Apply();
    }


    void EnsureRoundedDirectionButtonTexture()
    {
        if (roundedDirectionButtonTexture != null)
            return;

        const int width = 256;
        const int height = 96;
        const float radius = 18f;
        const float feather = 2.5f;
        roundedDirectionButtonTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        roundedDirectionButtonTexture.wrapMode = TextureWrapMode.Clamp;
        roundedDirectionButtonTexture.filterMode = FilterMode.Bilinear;

        Color fill = Color.white;
        Color clear = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = ComputeRoundedRectAlpha(x, y, width, height, radius * 2f, feather);
                Color pixel = fill;
                pixel.a = alpha;
                roundedDirectionButtonTexture.SetPixel(x, y, alpha > 0f ? pixel : clear);
            }
        }
        roundedDirectionButtonTexture.Apply();
    }


    void EnsureExpandedPanelBackdropTexture()
    {
        float radius = Mathf.Max(0f, lowerContainerBackdropRadiusPixels);
        if (expandedPanelBackdropTexture != null && Mathf.Approximately(cachedLowerContainerBackdropRadius, radius))
            return;

        const int width = 512;
        const int height = 512;
        const float feather = 2.5f;
        expandedPanelBackdropTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        expandedPanelBackdropTexture.wrapMode = TextureWrapMode.Clamp;
        expandedPanelBackdropTexture.filterMode = FilterMode.Bilinear;
        cachedLowerContainerBackdropRadius = radius;

        Color fill = Color.white;
        Color clear = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Max(Mathf.Abs(x - (width * 0.5f)) - (width * 0.5f - radius), 0f);
                float dy = Mathf.Max(Mathf.Abs(y - (height * 0.5f)) - (height * 0.5f - radius), 0f);
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = 1f - Mathf.Clamp01((distance - (radius - feather)) / Mathf.Max(feather, 0.0001f));
                Color pixelColor = fill;
                pixelColor.a = alpha;
                expandedPanelBackdropTexture.SetPixel(x, y, alpha > 0f ? pixelColor : clear);
            }
        }
        expandedPanelBackdropTexture.Apply();
    }

    void EnsureSelectedMainRowBackdropTexture()
    {
        float radius = Mathf.Max(0f, selectedMainRowBackdropRadiusPixels);
        if (selectedMainRowBackdropTexture != null && Mathf.Approximately(cachedSelectedMainRowBackdropRadius, radius))
            return;

        const int width = 512;
        const int height = 256;
        const float feather = 2.5f;
        selectedMainRowBackdropTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        selectedMainRowBackdropTexture.wrapMode = TextureWrapMode.Clamp;
        selectedMainRowBackdropTexture.filterMode = FilterMode.Bilinear;
        cachedSelectedMainRowBackdropRadius = radius;

        Color fill = Color.white;
        Color clear = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = Mathf.Max(Mathf.Abs(x - (width * 0.5f)) - (width * 0.5f - radius), 0f);
                float dy = Mathf.Max(Mathf.Abs(y - (height * 0.5f)) - (height * 0.5f - radius), 0f);
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = 1f - Mathf.Clamp01((distance - (radius - feather)) / Mathf.Max(feather, 0.0001f));
                Color pixelColor = fill;
                pixelColor.a = alpha;
                selectedMainRowBackdropTexture.SetPixel(x, y, alpha > 0f ? pixelColor : clear);
            }
        }
        selectedMainRowBackdropTexture.Apply();
    }

    void EnsureRoundedToneBandMaskTexture()
    {
        if (roundedToneBandMaskTexture != null)
            return;

        const int width = 256;
        const int height = 256;
        const float feather = 2.5f;
        roundedToneBandMaskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        roundedToneBandMaskTexture.wrapMode = TextureWrapMode.Clamp;
        roundedToneBandMaskTexture.filterMode = FilterMode.Bilinear;

        Color fill = Color.white;
        Color clear = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float radius = Mathf.Min(width, height) * 0.22f;
                float alpha = ComputeRoundedRectAlpha(x, y, width, height, radius, feather);
                Color pixel = fill;
                pixel.a = alpha;
                roundedToneBandMaskTexture.SetPixel(x, y, alpha > 0f ? pixel : clear);
            }
        }
        roundedToneBandMaskTexture.Apply();
    }


    void EnsureCustomHueSliderTexture()
    {
        if (customHueSliderTexture != null)
            return;

        customHueSliderTexture = new Texture2D(256, 24, TextureFormat.RGBA32, false);
        customHueSliderTexture.wrapMode = TextureWrapMode.Clamp;
        customHueSliderTexture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < customHueSliderTexture.height; y++)
        {
            for (int x = 0; x < customHueSliderTexture.width; x++)
            {
                float t = x / (float)(customHueSliderTexture.width - 1);
                customHueSliderTexture.SetPixel(x, y, Color.HSVToRGB(t, 1f, 1f));
            }
        }
        customHueSliderTexture.Apply();
    }


    void RefreshCustomSvSquareTexture()
    {
        if (customSvSquareTexture == null)
        {
            customSvSquareTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            customSvSquareTexture.wrapMode = TextureWrapMode.Clamp;
            customSvSquareTexture.filterMode = FilterMode.Bilinear;
        }

        for (int y = 0; y < customSvSquareTexture.height; y++)
        {
            float value = 1f - (y / (float)(customSvSquareTexture.height - 1));
            for (int x = 0; x < customSvSquareTexture.width; x++)
            {
                float saturation = x / (float)(customSvSquareTexture.width - 1);
                customSvSquareTexture.SetPixel(x, y, Color.HSVToRGB(customPickerHue, saturation, value));
            }
        }
        customSvSquareTexture.Apply();
    }


    bool IsInsidePaletteRoundedRect(float x, float y, float width, float height)
    {
        float radius = Mathf.Min(width, height) * 0.2f;
        float dx = Mathf.Max(Mathf.Abs(x - (width * 0.5f)) - (width * 0.5f - radius), 0f);
        float dy = Mathf.Max(Mathf.Abs(y - (height * 0.5f)) - (height * 0.5f - radius), 0f);
        return (dx * dx + dy * dy) <= radius * radius;
    }


    Rect GetCustomColorPanelRect()
    {
        return customColorOverlayRect;
    }


    void ApplyCurrentFillColorPreview()
    {
        int width = selectedColor.width;
        int height = selectedColor.height;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                bool inside = IsInsidePaletteRoundedRect(i, j, width, height);
                selectedColor.SetPixel(i, j, inside ? fillColor : new Color(0f, 0f, 0f, 0f));
            }
        }
        selectedColor.Apply();
    }


    void DisposeToneBandTextures(Texture2D[] textures)
    {
        if (textures == null)
            return;

        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
                Destroy(textures[i]);
            textures[i] = null;
        }
    }


    Texture2D BuildRoundedBandTexture(Color32 startColor, Color32 endColor)
    {
        const int width = 256;
        const int height = 256;
        const float feather = 2.5f;
        Texture2D bandTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        bandTexture.wrapMode = TextureWrapMode.Clamp;
        bandTexture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float radius = Mathf.Min(width, height) * 0.22f;
                float alpha = ComputeRoundedRectAlpha(x, y, width, height, radius, feather);
                if (alpha <= 0f)
                {
                    bandTexture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                    continue;
                }

                float t = width <= 1 ? 0f : x / (float)(width - 1);
                Color pixel = Color.Lerp(startColor, endColor, t);
                pixel.a = alpha;
                bandTexture.SetPixel(x, y, pixel);
            }
        }

        bandTexture.Apply();
        return bandTexture;
    }


    Texture2D BuildPaletteToggleTexture(bool gradient)
    {
        const int width = 256;
        const int height = 128;
        const float feather = 2.5f;
        Texture2D toggleTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        toggleTexture.wrapMode = TextureWrapMode.Clamp;
        toggleTexture.filterMode = FilterMode.Bilinear;

        Color[] colors = (palettePreviewColors != null && palettePreviewColors.Length > 0)
            ? palettePreviewColors
            : new[]
            {
                new Color(0.94f, 0.39f, 0.34f, 1f),
                new Color(0.99f, 0.82f, 0.33f, 1f),
                new Color(0.29f, 0.74f, 0.72f, 1f),
                new Color(0.24f, 0.46f, 0.91f, 1f),
                new Color(0.61f, 0.45f, 0.91f, 1f)
            };

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float radius = Mathf.Min(width, height) * 0.22f;
                float alpha = ComputeRoundedRectAlpha(x, y, width, height, radius, feather);
                if (alpha <= 0f)
                {
                    toggleTexture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                    continue;
                }

                float tx = x / (float)(width - 1);
                if (gradient)
                {
                    float scaled = tx * Mathf.Max(1, colors.Length - 1);
                    int centerIndex = Mathf.Clamp(Mathf.RoundToInt(scaled), 0, colors.Length - 1);
                    int leftIndex = Mathf.Clamp(centerIndex - 1, 0, colors.Length - 1);
                    int rightIndex = Mathf.Clamp(centerIndex + 1, 0, colors.Length - 1);
                    int farLeftIndex = Mathf.Clamp(centerIndex - 2, 0, colors.Length - 1);
                    int farRightIndex = Mathf.Clamp(centerIndex + 2, 0, colors.Length - 1);

                    float farLeftWeight = Mathf.Clamp01(0.75f - Mathf.Abs(scaled - farLeftIndex) * 0.42f);
                    float leftWeight = Mathf.Clamp01(1.15f - Mathf.Abs(scaled - leftIndex) * 0.52f);
                    float centerWeight = Mathf.Clamp01(1.35f - Mathf.Abs(scaled - centerIndex) * 0.4f);
                    float rightWeight = Mathf.Clamp01(1.15f - Mathf.Abs(scaled - rightIndex) * 0.52f);
                    float farRightWeight = Mathf.Clamp01(0.75f - Mathf.Abs(scaled - farRightIndex) * 0.42f);
                    float totalWeight = farLeftWeight + leftWeight + centerWeight + rightWeight + farRightWeight;
                    if (totalWeight <= 0.0001f)
                        totalWeight = 1f;

                    Color blended =
                        colors[farLeftIndex] * (farLeftWeight / totalWeight) +
                        colors[leftIndex] * (leftWeight / totalWeight) +
                        colors[centerIndex] * (centerWeight / totalWeight) +
                        colors[rightIndex] * (rightWeight / totalWeight) +
                        colors[farRightIndex] * (farRightWeight / totalWeight);

                    int segmentLeftIndex = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, colors.Length - 1);
                    int segmentRightIndex = Mathf.Clamp(segmentLeftIndex + 1, 0, colors.Length - 1);
                    float localT = Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, scaled - segmentLeftIndex)));
                    Color baseGradient = Color.Lerp(colors[segmentLeftIndex], colors[segmentRightIndex], localT);

                    Color pixel = Color.Lerp(baseGradient, blended, 0.88f);
                    pixel.a = alpha;
                    toggleTexture.SetPixel(x, y, pixel);
                }
                else
                {
                    int bandIndex = Mathf.Clamp(Mathf.FloorToInt(tx * colors.Length), 0, colors.Length - 1);
                    Color pixel = colors[bandIndex];
                    pixel.a = alpha;
                    toggleTexture.SetPixel(x, y, pixel);
                }
            }
        }

        toggleTexture.Apply();
        return toggleTexture;
    }


    void RefreshPaletteToggleTextures()
    {
        if (normalPaletteToggleTexture != null)
            Destroy(normalPaletteToggleTexture);
        if (gradientPaletteToggleTexture != null)
            Destroy(gradientPaletteToggleTexture);

        normalPaletteToggleTexture = BuildPaletteToggleTexture(false);
        gradientPaletteToggleTexture = BuildPaletteToggleTexture(true);
    }


    Rect GetToneBandDisplayRect(int bandIndex)
    {
        int topRowCount = 6;
        int bottomRowCount = 5;
        bool isTopRow = bandIndex < topRowCount;
        int rowCount = isTopRow ? topRowCount : bottomRowCount;
        int rowIndex = isTopRow ? bandIndex : bandIndex - topRowCount;

        float horizontalMargin = tonePaletteHorizontalMarginPixels * scale_x;
        float tileWidth = tonePaletteTileWidthPixels * scale_x;
        float tileHeight = tonePaletteTileHeightPixels * scale_y;
        float horizontalGap = tonePaletteHorizontalGapPixels * scale_x;
        float rowGap = tonePaletteRowGapPixels * scale_y;

        float rowWidth = rowCount * tileWidth + (rowCount - 1) * horizontalGap;
        float startX = toneRect.x + horizontalMargin + (toneRect.width - horizontalMargin * 2f - rowWidth) * 0.5f + (tonePaletteHorizontalOffsetPixels * scale_x);
        float topRowY = toneRect.y + tonePaletteTopInsetPixels * scale_y;
        float rowY = isTopRow ? topRowY : topRowY + tileHeight + rowGap;

        return new Rect(
            startX + rowIndex * (tileWidth + horizontalGap),
            rowY,
            tileWidth,
            tileHeight);
    }


    void RebuildSolidToneBandTextures()
    {
        if (solidToneBandTextures == null || solidToneBandTextures.Length != 11)
            solidToneBandTextures = new Texture2D[11];

        DisposeToneBandTextures(solidToneBandTextures);
        for (int i = 0; i < solidToneBandTextures.Length; i++)
        {
            Color32 toneColor = SampleToneBandColor(i);
            solidToneBandTextures[i] = BuildRoundedBandTexture(toneColor, toneColor);
        }
    }


    void RebuildGradientToneBandTextures()
    {
        if (gradientToneBandTextures == null || gradientToneBandTextures.Length != 11)
            gradientToneBandTextures = new Texture2D[11];

        DisposeToneBandTextures(gradientToneBandTextures);
        for (int i = 0; i < gradientToneBandTextures.Length; i++)
        {
            GetGradientPairForBand(i, out Color32 bandStartColor, out Color32 bandEndColor);
            GetVisibleGradientEndpoints(bandStartColor, bandEndColor, Mathf.Max(1, smallGradientRegionThreshold / 2), out Color32 previewStartColor, out Color32 previewEndColor);
            gradientToneBandTextures[i] = BuildRoundedBandTexture(previewStartColor, previewEndColor);
        }
    }


    void ApplyGradientPreview(Color32 fromColor, Color32 toColor)
    {
        Color32 previewStartColor;
        Color32 previewEndColor;
        GetVisibleGradientEndpoints(fromColor, toColor, Mathf.Max(1, smallGradientRegionThreshold / 2), out previewStartColor, out previewEndColor);

        int width = selectedColor.width;
        int height = selectedColor.height;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float t = EvaluateGradientT(x, y, 0, width - 1, 0, height - 1);
                Color previewColor = Color.Lerp(previewStartColor, previewEndColor, t);
                bool inside = IsInsidePaletteRoundedRect(x, y, width, height);
                selectedColor.SetPixel(x, y, inside ? previewColor : new Color(0f, 0f, 0f, 0f));
            }
        }
        selectedColor.Apply();
    }


    float EvaluateGradientT(int x, int y, int minX, int maxX, int minY, int maxY)
    {
        float normalizedX = Mathf.InverseLerp(minX, maxX, x);
        float normalizedY = Mathf.InverseLerp(minY, maxY, y);

        switch (gradientFillDirection)
        {
            case GradientFillDirection.RightToLeft:
                return EvaluateRotatedLinearT(normalizedX, normalizedY, Vector2.left);
            case GradientFillDirection.TopToBottom:
                return EvaluateRotatedLinearT(normalizedX, normalizedY, Vector2.up);
            case GradientFillDirection.BottomToTop:
                return EvaluateRotatedLinearT(normalizedX, normalizedY, Vector2.down);
            case GradientFillDirection.Radial:
                float centerX = (minX + maxX) * 0.5f;
                float centerY = (minY + maxY) * 0.5f;
                float dx = x - centerX;
                float dy = y - centerY;
                float maxDx = Mathf.Max(1f, Mathf.Abs(maxX - centerX));
                float maxDy = Mathf.Max(1f, Mathf.Abs(maxY - centerY));
                float maxDistance = Mathf.Sqrt(maxDx * maxDx + maxDy * maxDy);
                return Mathf.Clamp01(Mathf.Sqrt(dx * dx + dy * dy) / maxDistance);
            case GradientFillDirection.LeftToRight:
            default:
                return EvaluateRotatedLinearT(normalizedX, normalizedY, Vector2.right);
        }
    }


    float EvaluateRotatedLinearT(float normalizedX, float normalizedY, Vector2 baseDirection)
    {
        Vector2 centered = new Vector2(normalizedX - 0.5f, normalizedY - 0.5f);
        float angleRadians = -gradientRotationDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRadians);
        float sin = Mathf.Sin(angleRadians);
        Vector2 rotatedDirection = new Vector2(
            baseDirection.x * cos - baseDirection.y * sin,
            baseDirection.x * sin + baseDirection.y * cos).normalized;

        float projection = Vector2.Dot(centered, rotatedDirection);
        float normalizedProjection = Mathf.InverseLerp(-0.5f, 0.5f, projection);
        return Mathf.Clamp01(normalizedProjection);
    }


    Color32 AdjustGradientEndpoint(Color32 source, bool brighten, float boost)
    {
        Color color = source;
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);

        s = Mathf.Clamp01(s + (1f - s) * (0.24f * boost));
        if (brighten)
            v = Mathf.Clamp01(v + (1f - v) * (0.92f * boost));
        else
            v = Mathf.Clamp01(v * (1f - (0.75f * boost)));

        Color adjusted = Color.HSVToRGB(h, s, v);
        adjusted.a = color.a;
        return adjusted;
    }


    void GetVisibleGradientEndpoints(Color32 rawStartColor, Color32 rawEndColor, int regionPixelCount, out Color32 startColor, out Color32 endColor)
    {
        float smallRegionFactor = 1f;
        if (smallGradientRegionThreshold > 0)
            smallRegionFactor = 1f - Mathf.Clamp01((float)regionPixelCount / (float)smallGradientRegionThreshold);

        float baseBoost = Mathf.Lerp(0.42f, 1f, gradientVisibilityBoost);
        float effectiveBoost = Mathf.Clamp01(baseBoost * (1.1f + (0.9f * smallRegionFactor)));
        startColor = AdjustGradientEndpoint(rawStartColor, false, effectiveBoost);
        endColor = AdjustGradientEndpoint(rawEndColor, true, effectiveBoost);
    }


    void SelectDefaultToneColor(int toneIndex)
    {
        selectedToneIndex = toneIndex;
        activeFeaturedPaletteIndex = -1;
        selectedFeaturedColorIndex = -1;
        showFeaturedPalette = false;
        whiteSwatchSelected = false;
        gradientModeActive = false;
        showGradientPalette = false;
        gradientBandSelected = false;

        Color chosen = (palettePreviewColors != null && toneIndex >= 0 && toneIndex < palettePreviewColors.Length)
            ? palettePreviewColors[toneIndex]
            : pencilTones[toneIndex].GetPixelBilinear(0.5f, 0.5f);

        if (chosen == Color.black)
            chosen = new Color(0.1f, 0.1f, 0.1f);

        fillColor = chosen;

        int midToneX = Mathf.RoundToInt(toneRect.x + toneRect.width * 0.5f);
        int midToneY = Mathf.RoundToInt(toneRect.y + toneRect.height * 0.5f);
        FindPixelWithinTone(midToneX, midToneY);
        RebuildSolidToneBandTextures();
        ApplyCurrentFillColorPreview();
    }


    void SelectWhiteSwatch()
    {
        selectedToneIndex = -1;
        selectedFeaturedColorIndex = -1;
        activeFeaturedPaletteIndex = -1;
        whiteSwatchSelected = true;
        gradientModeActive = false;
        showSwatchPalette = false;
        showGradientPalette = false;
        showFeaturedPalette = false;
        showCustomColorOverlay = false;
        imagePickerMode = false;
        gradientBandSelected = false;
        fillColor = Color.white;
        colorSelected = true;
        colorHolds = true;
        customOverlayActiveColor = Color.white;
        ApplyCurrentFillColorPreview();
    }


    void SelectCustomOverlayColor(Color chosenColor)
    {
        Color selected = chosenColor;
        selected.a = 1f;
        fillColor = selected;
        customOverlayActiveColor = selected;
        Color.RGBToHSV(selected, out customPickerHue, out customPickerSaturation, out customPickerValue);
        RefreshCustomSvSquareTexture();
        selectedToneIndex = -1;
        selectedFeaturedColorIndex = -1;
        activeFeaturedPaletteIndex = -1;
        whiteSwatchSelected = false;
        gradientModeActive = false;
        gradientBandSelected = false;
        showSwatchPalette = false;
        showGradientPalette = false;
        showFeaturedPalette = false;
        colorSelected = true;
        colorHolds = true;
        ApplyCurrentFillColorPreview();
    }


    void ToggleFeaturedPalette(int paletteIndex)
    {
        if (activeFeaturedPaletteIndex == paletteIndex && showFeaturedPalette)
        {
            showFeaturedPalette = false;
            activeFeaturedPaletteIndex = -1;
            selectedFeaturedColorIndex = -1;
            return;
        }

        whiteSwatchSelected = false;
        showSwatchPalette = false;
        showGradientPalette = false;
        showCustomColorOverlay = false;
        imagePickerMode = false;
        gradientModeActive = false;
        gradientBandSelected = false;
        selectedToneIndex = -1;
        activeFeaturedPaletteIndex = paletteIndex;
        selectedFeaturedColorIndex = -1;
        showFeaturedPalette = true;
        colorSelected = false;
        colorHolds = false;
        EnsureFeaturedToggleVisibleWithPadding(paletteIndex);
    }


    void SelectFeaturedPaletteColor(int colorIndex)
    {
        if (featuredPaletteThemes == null || activeFeaturedPaletteIndex < 0 || activeFeaturedPaletteIndex >= featuredPaletteThemes.Length)
            return;

        Color[] activeTheme = featuredPaletteThemes[activeFeaturedPaletteIndex];
        if (activeTheme == null || colorIndex < 0 || colorIndex >= activeTheme.Length)
            return;

        Color selected = activeTheme[colorIndex];
        selected.a = 1f;
        selectedFeaturedColorIndex = colorIndex;
        fillColor = selected;
        customOverlayActiveColor = selected;
        Color.RGBToHSV(selected, out customPickerHue, out customPickerSaturation, out customPickerValue);
        RefreshCustomSvSquareTexture();
        whiteSwatchSelected = false;
        gradientModeActive = false;
        gradientBandSelected = false;
        selectedToneIndex = -1;
        colorSelected = true;
        colorHolds = true;
        ApplyCurrentFillColorPreview();
    }


    int GetCurrentToneBandIndex()
    {
        if (toneRect.width <= 0f || selRect.width <= 0f)
            return 0;

        float selectedCenterX = selRect.center.x;
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < 11; i++)
        {
            Rect bandRect = GetToneBandDisplayRect(i);
            float distance = Mathf.Abs(bandRect.center.x - selectedCenterX);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }


    Color32 SampleToneBandColor(int bandIndex)
    {
        if (selectedToneIndex < 0 || pencilTones == null || selectedToneIndex >= pencilTones.Length || pencilTones[selectedToneIndex] == null)
            return (Color32)fillColor;

        Texture2D toneTexture = pencilTones[selectedToneIndex];
        float u = (bandIndex + 0.5f) / 11f;
        Color sampled = toneTexture.GetPixelBilinear(u, 0.5f);
        if (sampled == Color.black)
            sampled = new Color(0.1f, 0.1f, 0.1f, 1f);
        sampled.a = 1f;
        return (Color32)sampled;
    }


    void GetGradientPairForBand(int bandIndex, out Color32 startColor, out Color32 endColor)
    {
        int clampedBand = Mathf.Clamp(bandIndex, 0, 10);
        int neighborBand = clampedBand > 0 ? clampedBand - 1 : Mathf.Min(10, clampedBand + 1);

        startColor = SampleToneBandColor(neighborBand);
        endColor = SampleToneBandColor(clampedBand);
    }


    void SelectGradientBand(int bandIndex)
    {
        SetSelectedToneBandIndex(bandIndex);
        GetGradientPairForBand(bandIndex, out gradientStartColor, out gradientEndColor);
        fillColor = gradientEndColor;
        gradientModeActive = true;
        gradientBandSelected = true;
        colorSelected = true;
        colorHolds = true;
        ApplyGradientPreview(gradientStartColor, gradientEndColor);
    }


    void RefreshGradientPaletteTexture()
    {
        if (gradientPaletteTexture == null)
        {
            gradientPaletteTexture = new Texture2D(352, 32, TextureFormat.RGBA32, false);
            gradientPaletteTexture.wrapMode = TextureWrapMode.Clamp;
            gradientPaletteTexture.filterMode = FilterMode.Bilinear;
        }

        if (selectedToneIndex >= 0)
        {
            int currentBand = GetCurrentToneBandIndex();
            GetGradientPairForBand(currentBand, out gradientStartColor, out gradientEndColor);
        }
        else
        {
            gradientEndColor = (Color32)fillColor;
            gradientStartColor = (Color32)Color.Lerp(Color.white, gradientEndColor, 0.35f);
        }

        for (int y = 0; y < gradientPaletteTexture.height; y++)
        {
            for (int x = 0; x < gradientPaletteTexture.width; x++)
            {
                if (selectedToneIndex >= 0)
                {
                    float normalizedX = gradientPaletteTexture.width <= 1 ? 0f : x / (float)gradientPaletteTexture.width;
                    int bandIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedX * 11f), 0, 10);
                    float bandStart = bandIndex / 11f;
                    float localT = Mathf.InverseLerp(bandStart, bandStart + (1f / 11f), normalizedX);
                    GetGradientPairForBand(bandIndex, out Color32 bandStartColor, out Color32 bandEndColor);
                    GetVisibleGradientEndpoints(bandStartColor, bandEndColor, Mathf.Max(1, smallGradientRegionThreshold / 2), out Color32 previewStartColor, out Color32 previewEndColor);
                    gradientPaletteTexture.SetPixel(x, y, Color.Lerp(previewStartColor, previewEndColor, localT));
                }
                else
                {
                    float t = gradientPaletteTexture.width <= 1 ? 1f : x / (float)(gradientPaletteTexture.width - 1);
                    GetVisibleGradientEndpoints(gradientStartColor, gradientEndColor, Mathf.Max(1, smallGradientRegionThreshold / 2), out Color32 previewStartColor, out Color32 previewEndColor);
                    gradientPaletteTexture.SetPixel(x, y, Color.Lerp(previewStartColor, previewEndColor, t));
                }
            }
        }
        gradientPaletteTexture.Apply();
        RebuildGradientToneBandTextures();
    }


    void OpenGradientPaletteChooser()
    {
        if (whiteSwatchSelected)
            whiteSwatchSelected = false;

        activeFeaturedPaletteIndex = -1;
        selectedFeaturedColorIndex = -1;
        showFeaturedPalette = false;
        showSwatchPalette = false;
        showGradientPalette = true;
        gradientModeActive = false;
        gradientBandSelected = false;
        selectedToneIndex = -1;
        colorSelected = false;
        colorHolds = false;
    }


    void SelectGradientMode()
    {
        activeFeaturedPaletteIndex = -1;
        selectedFeaturedColorIndex = -1;
        showFeaturedPalette = false;
        showSwatchPalette = false;
        showGradientPalette = true;
        gradientModeActive = true;
        colorSelected = true;
        colorHolds = true;
        RefreshGradientPaletteTexture();
        ApplyGradientPreview(gradientStartColor, gradientEndColor);
    }


    string GetGradientDirectionLabel()
    {
        switch (gradientFillDirection)
        {
            case GradientFillDirection.RightToLeft:
                return "R->L";
            case GradientFillDirection.TopToBottom:
                return "T->B";
            case GradientFillDirection.BottomToTop:
                return "B->T";
            case GradientFillDirection.Radial:
                return "RAD";
            case GradientFillDirection.LeftToRight:
            default:
                return "L->R";
        }
    }


    string GetGradientDirectionGlyph()
    {
        switch (gradientFillDirection)
        {
            case GradientFillDirection.RightToLeft:
                return "<-";
            case GradientFillDirection.TopToBottom:
                return "v";
            case GradientFillDirection.BottomToTop:
                return "^";
            case GradientFillDirection.Radial:
                return "o";
            case GradientFillDirection.LeftToRight:
            default:
                return "->";
        }
    }


    void CycleGradientDirection()
    {
        gradientFillDirection = (GradientFillDirection)(((int)gradientFillDirection + 1) % System.Enum.GetValues(typeof(GradientFillDirection)).Length);
        if (showGradientPalette && selectedToneIndex >= 0)
        {
            RefreshGradientPaletteTexture();
            if (gradientBandSelected)
                ApplyGradientPreview(gradientStartColor, gradientEndColor);
        }
    }


    void RotateGradientClockwise()
    {
        if (gradientFillDirection == GradientFillDirection.Radial)
            return;

        gradientRotationDegrees = Mathf.Repeat(gradientRotationDegrees + 10f, 360f);
        if (showGradientPalette && selectedToneIndex >= 0)
        {
            RefreshGradientPaletteTexture();
            if (gradientBandSelected)
                ApplyGradientPreview(gradientStartColor, gradientEndColor);
        }
    }


    void RotateGradientCounterClockwise()
    {
        if (gradientFillDirection == GradientFillDirection.Radial)
            return;

        gradientRotationDegrees = Mathf.Repeat(gradientRotationDegrees - 10f, 360f);
        if (showGradientPalette && selectedToneIndex >= 0)
        {
            RefreshGradientPaletteTexture();
            if (gradientBandSelected)
                ApplyGradientPreview(gradientStartColor, gradientEndColor);
        }
    }


    void ResetGradientOrientation()
    {
        gradientFillDirection = GradientFillDirection.LeftToRight;
        gradientRotationDegrees = 0f;

        if (showGradientPalette && selectedToneIndex >= 0)
        {
            RefreshGradientPaletteTexture();
            if (gradientBandSelected)
                ApplyGradientPreview(gradientStartColor, gradientEndColor);
        }
    }


    void RefreshGradientPreviewState()
    {
        if (!showGradientPalette)
            return;

        RefreshGradientPaletteTexture();
        if (gradientBandSelected)
            ApplyGradientPreview(gradientStartColor, gradientEndColor);
    }


    void UpdateGradientBoostFromPointer(float pointerX)
    {
        float t = Mathf.InverseLerp(gradientBoostSliderRect.xMin, gradientBoostSliderRect.xMax, pointerX);
        float newBoost = Mathf.Clamp01(t);
        if (Mathf.Abs(newBoost - gradientVisibilityBoost) < 0.001f)
            return;

        gradientVisibilityBoost = newBoost;
        RefreshGradientPreviewState();
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
        paletteToggleIcon = Resources.Load<Texture2D>("Graphics/UIIcons/palette_toggle_icon");
        if (paletteToggleIcon == null)
            paletteToggleIcon = CreatePaletteTogglePlaceholderIcon();
        gradientToggleIcon = Resources.Load<Texture2D>("Graphics/UIIcons/gradient_toggle_icon");
        if (gradientToggleIcon == null)
            gradientToggleIcon = CreateGradientTogglePlaceholderIcon();
        customColorToggleIcon = Resources.Load<Texture2D>("Graphics/UIIcons/custom_color_toggle_icon");
        if (customColorToggleIcon == null)
            customColorToggleIcon = CreateCustomColorTogglePlaceholderIcon();
        imagePickerToggleIcon = Resources.Load<Texture2D>("Graphics/UIIcons/color_picker_ic");
        if (imagePickerToggleIcon == null)
            imagePickerToggleIcon = Resources.Load<Texture2D>("Graphics/UIIcons/image_picker_toggle_icon");
        if (imagePickerToggleIcon == null)
            imagePickerToggleIcon = CreateImagePickerTogglePlaceholderIcon();
        backChevronIcon = Resources.Load<Texture2D>("Graphics/UIIcons/back_chevron");
	}


    Texture2D CreatePaletteTogglePlaceholderIcon()
    {
        Texture2D placeholder = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        placeholder.wrapMode = TextureWrapMode.Clamp;
        placeholder.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(1f, 1f, 1f, 0f);
        Color frame = new Color(1f, 1f, 1f, 0.95f);
        Color red = new Color(0.94f, 0.39f, 0.34f, 1f);
        Color yellow = new Color(0.99f, 0.82f, 0.33f, 1f);
        Color teal = new Color(0.29f, 0.74f, 0.72f, 1f);
        Color violet = new Color(0.61f, 0.45f, 0.91f, 1f);

        for (int y = 0; y < placeholder.height; y++)
        {
            for (int x = 0; x < placeholder.width; x++)
            {
                placeholder.SetPixel(x, y, transparent);
            }
        }

        for (int y = 6; y < 58; y++)
        {
            for (int x = 6; x < 58; x++)
            {
                bool border = x < 10 || x > 53 || y < 10 || y > 53;
                if (border)
                {
                    placeholder.SetPixel(x, y, frame);
                    continue;
                }

                Color band = x < 22 ? red : x < 34 ? yellow : x < 46 ? teal : violet;
                placeholder.SetPixel(x, y, band);
            }
        }

        placeholder.Apply();
        return placeholder;
    }


    Texture2D CreateGradientTogglePlaceholderIcon()
    {
        Texture2D placeholder = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        placeholder.wrapMode = TextureWrapMode.Clamp;
        placeholder.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < placeholder.height; y++)
        {
            for (int x = 0; x < placeholder.width; x++)
            {
                float tx = x / 63f;
                float ty = y / 63f;
                Color baseColor = Color.Lerp(new Color(0.95f, 0.42f, 0.34f, 1f), new Color(0.32f, 0.74f, 0.86f, 1f), tx);
                Color finalColor = Color.Lerp(baseColor, new Color(0.98f, 0.9f, 0.55f, 1f), ty * 0.35f);
                if (x < 6 || x > 57 || y < 6 || y > 57)
                {
                    placeholder.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                    continue;
                }

                bool frame = x < 10 || x > 53 || y < 10 || y > 53;
                placeholder.SetPixel(x, y, frame ? Color.white : finalColor);
            }
        }

        placeholder.Apply();
        return placeholder;
    }


    Texture2D CreateCustomColorTogglePlaceholderIcon()
    {
        Texture2D placeholder = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        placeholder.wrapMode = TextureWrapMode.Clamp;
        placeholder.filterMode = FilterMode.Bilinear;

        Color clear = new Color(1f, 1f, 1f, 0f);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
                placeholder.SetPixel(x, y, clear);
        }

        Color[] blocks =
        {
            new Color(0.95f, 0.39f, 0.33f, 1f),
            new Color(0.98f, 0.84f, 0.29f, 1f),
            new Color(0.26f, 0.74f, 0.71f, 1f),
            new Color(0.38f, 0.46f, 0.91f, 1f)
        };

        for (int i = 0; i < 4; i++)
        {
            int startX = (i % 2 == 0) ? 11 : 33;
            int startY = (i < 2) ? 11 : 33;
            for (int y = startY; y < startY + 18; y++)
            {
                for (int x = startX; x < startX + 18; x++)
                {
                    placeholder.SetPixel(x, y, blocks[i]);
                }
            }
        }

        placeholder.Apply();
        return placeholder;
    }


    Texture2D CreateImagePickerTogglePlaceholderIcon()
    {
        Texture2D placeholder = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        placeholder.wrapMode = TextureWrapMode.Clamp;
        placeholder.filterMode = FilterMode.Bilinear;

        Color clear = new Color(1f, 1f, 1f, 0f);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
                placeholder.SetPixel(x, y, clear);
        }

        Color solid = Color.white;
        for (int i = 10; i < 54; i++)
        {
            placeholder.SetPixel(32, i, solid);
            placeholder.SetPixel(i, 32, solid);
        }

        for (int i = 22; i < 43; i++)
        {
            placeholder.SetPixel(22, i, solid);
            placeholder.SetPixel(42, i, solid);
            placeholder.SetPixel(i, 22, solid);
            placeholder.SetPixel(i, 42, solid);
        }

        placeholder.Apply();
        return placeholder;
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

		if (moveToShare || DataManager.Instance.fromDrawings) 
		{
			DataManager.Instance.waterMarkedImage = testwaterImage.EncodeToPNG ();
			if (oldFillers.Count > 0)
				StoreImageChange ();
			else if (DataManager.Instance.fromDrawings)
				DataManager.Instance.fromDrawings = false;

			moveToShare = false;
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
		UpdateSwatchLiftAnimation();

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

        if (imagePickerMode && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !fillAnimationRunning && !eagerShare && ButtonHit(imageRect) && previousEvent == TouchEvent.None && !showSharePopUp)
        {
            Color32 pickedColor = SampleColorFromDrawingAtPointer(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            SelectCustomOverlayColor(pickedColor);
            imagePickerMode = false;
            showCustomColorOverlay = false;
            return;
        }

		if (!showInapp && !showSavedPopUp && colorHolds && !isZooming && !startPanning && !fillAnimationRunning && !eagerShare && !ButtonFade(unDoRect,0) && 
            !ButtonFade(shareRect,2) && !ButtonFade(homeRect,1) && ButtonHit(imageRect) && previousEvent==TouchEvent.None &&! showSharePopUp) {
            // COLOR IMAGE : color fill area based on selected color and store hit position and original color in stack 
			if(colorHolds && colorSelected && (Screen.height - Input.mousePosition.y)>topBannerRect.y && 
              //(Screen.height - Input.mousePosition.y)<(1808 * scale_y))
               (Screen.height - Input.mousePosition.y) < 1948 * scale_y - (float)(Screen.height * 0.2f))  //simo DO NOT PAINT below the pencil stripe included
			{ 
                int hitX = Mathf.CeilToInt((Input.mousePosition.x-imageRect.x)*mainImage.width/imageRect.width);
                int hitY = Mathf.CeilToInt(mainImage.height-( Screen.height-Input.mousePosition.y-imageRect.y)*(mainImage.height/(imageRect.height)));
                if (fillRegionMask == null || fillRegionMask.width != mainImage.width || fillRegionMask.height != mainImage.height)
                    ResetFillRegionMask();

                Color32[] pixelsBeforeFill = mainImage.GetPixels32();
                if (fillRegionMask != null)
                    FloodFiller.RevisedQueueFloodFill(fillRegionMask, hitX, hitY, GenerateNextRegionMarker(), false);

                List<int> filledIndices = new List<int>(FloodFiller.lastFillIndices);
                Color32 sourceColor = filledIndices.Count > 0 ? pixelsBeforeFill[filledIndices[0]] : (Color32)fillColor;
                Color32[] beforeColors = new Color32[filledIndices.Count];
                for (int i = 0; i < filledIndices.Count; i++)
                {
                    int idx = filledIndices[i];
                    beforeColors[i] = (idx >= 0 && idx < pixelsBeforeFill.Length) ? pixelsBeforeFill[idx] : sourceColor;
                }
                Color32[] afterColors = gradientModeActive
                    ? BuildGradientFillColors(filledIndices)
                    : BuildUniformColorArray(filledIndices.Count, (Color32)fillColor);

                ApplyFillColors(filledIndices, afterColors);
                redoFillers.Clear();
				oldFillers.Push (new FillInfo (
                    sourceColor,
                    (Color32)fillColor,
                    hitX,
                    hitY,
                    filledIndices.ToArray(),
                    beforeColors,
                    afterColors));
                if (smoothFillAnimation && filledIndices.Count > 0) {
                    StartCoroutine(AnimateFillRegion(
                        filledIndices,
                        beforeColors,
                        afterColors));
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

        if (showCustomColorOverlay && customOverlayPaletteColors != null)
        {
            for (int i = 0; i < customOverlayPaletteColors.Length; i++)
            {
                Rect swatchRect = GetCustomOverlaySwatchRect(i);
                if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(swatchRect))
                {
                    SelectCustomOverlayColor(customOverlayPaletteColors[i]);
                    return;
                }
            }

            if (savedCustomColors != null)
            {
                for (int i = 0; i < savedCustomColors.Length; i++)
                {
                    Rect savedRect = GetSavedCustomColorRect(i);
                    if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(savedRect))
                    {
                        StoreSelectedCustomColorInSlot(i);
                        return;
                    }
                }
            }

            if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(GetClearSavedCustomColorsButtonRect()))
            {
                ClearSavedCustomColors();
                return;
            }

            Vector2 customGuiMousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            if (showAdvancedCustomPicker)
            {
                bool pointerInHue = customHueSliderRect.Contains(customGuiMousePosition);
                bool pointerInSv = customSvSquareRect.Contains(customGuiMousePosition);

                if (Input.GetMouseButtonDown(0) && pointerInHue)
                {
                    draggingCustomHue = true;
                    UpdateCustomPickerHueFromPointer(customGuiMousePosition.x);
                    return;
                }

                if (Input.GetMouseButtonDown(0) && pointerInSv)
                {
                    draggingCustomSv = true;
                    UpdateCustomPickerSvFromPointer(customGuiMousePosition);
                    return;
                }

                if (draggingCustomHue)
                {
                    if (Input.GetMouseButton(0))
                    {
                        UpdateCustomPickerHueFromPointer(Mathf.Clamp(customGuiMousePosition.x, customHueSliderRect.xMin, customHueSliderRect.xMax));
                        return;
                    }
                    draggingCustomHue = false;
                }

                if (draggingCustomSv)
                {
                    if (Input.GetMouseButton(0))
                    {
                        UpdateCustomPickerSvFromPointer(new Vector2(
                            Mathf.Clamp(customGuiMousePosition.x, customSvSquareRect.xMin, customSvSquareRect.xMax),
                            Mathf.Clamp(customGuiMousePosition.y, customSvSquareRect.yMin, customSvSquareRect.yMax)));
                        return;
                    }
                    draggingCustomSv = false;
                }
            }
            else
            {
                draggingCustomHue = false;
                draggingCustomSv = false;
            }
        }

        Vector2 guiMousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		
        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(paletteToggleRect)) {
            if (whiteSwatchSelected) {
                whiteSwatchSelected = false;
                colorHolds = false;
                colorSelected = selectedToneIndex >= 0;
            }
            showFeaturedPalette = false;
            activeFeaturedPaletteIndex = -1;
            selectedFeaturedColorIndex = -1;
            gradientModeActive = false;
            showCustomColorOverlay = false;
            imagePickerMode = false;
            showGradientPalette = false;
            showSwatchPalette = !showSwatchPalette;
            return;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(gradientToggleRect)) {
            if (whiteSwatchSelected) {
                whiteSwatchSelected = false;
                colorHolds = false;
                colorSelected = selectedToneIndex >= 0;
            }

            showCustomColorOverlay = false;
            imagePickerMode = false;
            showFeaturedPalette = false;
            activeFeaturedPaletteIndex = -1;
            selectedFeaturedColorIndex = -1;
            if (showGradientPalette) {
                showGradientPalette = false;
                gradientModeActive = false;
                gradientBandSelected = false;
                selectedToneIndex = -1;
                colorSelected = false;
                colorHolds = false;
            } else {
                OpenGradientPaletteChooser();
            }
            return;
        }

        if (featuredThemeViewportRect.width > 0f && featuredPaletteToggleRects != null)
        {
            bool pointerInFeaturedRow = featuredThemeViewportRect.Contains(guiMousePosition);
            if (Input.GetMouseButtonDown(0) && pointerInFeaturedRow)
            {
                featuredThemeRowDragging = false;
                featuredThemeRowPressStartX = guiMousePosition.x;
                featuredThemeRowScrollStart = featuredThemeRowScroll;
                featuredThemePressedIndex = -1;
                for (int i = 0; i < featuredPaletteToggleRects.Length; i++)
                {
                    if (GetFeaturedToggleDisplayRect(i).Contains(guiMousePosition))
                    {
                        featuredThemePressedIndex = i;
                        break;
                    }
                }
            }

            if (Input.GetMouseButton(0) && featuredThemePressedIndex >= -1 && pointerInFeaturedRow)
            {
                float dragDelta = guiMousePosition.x - featuredThemeRowPressStartX;
                if (Mathf.Abs(dragDelta) > 10f * scale_x)
                {
                    featuredThemeRowDragging = true;
                    float maxScroll = Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width);
                    featuredThemeRowScroll = Mathf.Clamp(featuredThemeRowScrollStart - dragDelta, 0f, maxScroll);
                }
            }

            if (Input.GetMouseButtonUp(0) && featuredThemePressedIndex != -1)
            {
                int pressedIndex = featuredThemePressedIndex;
                bool wasDragging = featuredThemeRowDragging;
                featuredThemePressedIndex = -1;
                featuredThemeRowDragging = false;
                if (!wasDragging && pointerInFeaturedRow && GetFeaturedToggleDisplayRect(pressedIndex).Contains(guiMousePosition) &&
                    !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp)
                {
                    ToggleFeaturedPalette(pressedIndex);
                    return;
                }
            }

            if (!Input.GetMouseButton(0) && featuredThemePressedIndex == -1)
                featuredThemeRowDragging = false;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(featuredThemeLeftArrowRect))
        {
            float itemStep = (featuredPaletteToggleRects != null && featuredPaletteToggleRects.Length > 1)
                ? featuredPaletteToggleRects[1].x - featuredPaletteToggleRects[0].x
                : featuredThemeViewportRect.width * 0.25f;
            float maxScroll = Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width);
            featuredThemeRowScroll = Mathf.Clamp(featuredThemeRowScroll - itemStep, 0f, maxScroll);
            return;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(featuredThemeRightArrowRect))
        {
            float itemStep = (featuredPaletteToggleRects != null && featuredPaletteToggleRects.Length > 1)
                ? featuredPaletteToggleRects[1].x - featuredPaletteToggleRects[0].x
                : featuredThemeViewportRect.width * 0.25f;
            float maxScroll = Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width);
            featuredThemeRowScroll = Mathf.Clamp(featuredThemeRowScroll + itemStep, 0f, maxScroll);
            return;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(customColorToggleRect))
        {
            whiteSwatchSelected = false;
            showSwatchPalette = false;
            showGradientPalette = false;
            showFeaturedPalette = false;
            activeFeaturedPaletteIndex = -1;
            selectedFeaturedColorIndex = -1;
            gradientModeActive = false;
            gradientBandSelected = false;
            imagePickerMode = false;
            showCustomColorOverlay = !showCustomColorOverlay;
            if (showCustomColorOverlay)
                showAdvancedCustomPicker = true;
            return;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(imagePickerToggleRect))
        {
            whiteSwatchSelected = false;
            showSwatchPalette = false;
            showGradientPalette = false;
            showFeaturedPalette = false;
            activeFeaturedPaletteIndex = -1;
            selectedFeaturedColorIndex = -1;
            showCustomColorOverlay = false;
            gradientModeActive = false;
            gradientBandSelected = false;
            imagePickerMode = !imagePickerMode;
            return;
        }

        if (showGradientPalette && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(gradientDirectionRect)) {
            CycleGradientDirection();
            return;
        }

        if (showGradientPalette && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(gradientRotateLeftRect)) {
            RotateGradientCounterClockwise();
            return;
        }

        if (showGradientPalette && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(gradientRotateRect)) {
            RotateGradientClockwise();
            return;
        }

        if (showGradientPalette && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(gradientResetRect)) {
            ResetGradientOrientation();
            return;
        }

        if (showGradientPalette && !showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp)
        {
            bool pointerInsideBoost = gradientBoostSliderRect.Contains(guiMousePosition);
            if (Input.GetMouseButtonDown(0) && pointerInsideBoost)
            {
                gradientBoostDragging = true;
                UpdateGradientBoostFromPointer(guiMousePosition.x);
                return;
            }

            if (gradientBoostDragging)
            {
                if (Input.GetMouseButton(0))
                {
                    UpdateGradientBoostFromPointer(Mathf.Clamp(guiMousePosition.x, gradientBoostSliderRect.xMin, gradientBoostSliderRect.xMax));
                    return;
                }

                gradientBoostDragging = false;
            }
        }
        else
        {
            gradientBoostDragging = false;
        }

        if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(whiteSwatchRect) && !ButtonHit(toneRect)) {
            SelectWhiteSwatch();
            return;
        }

        if (showFeaturedPalette && featuredPaletteSwatchRects != null)
        {
            for (int i = 0; i < featuredPaletteSwatchRects.Length; i++)
            {
                if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && !eagerShare && !showSharePopUp && ButtonHit(featuredPaletteSwatchRects[i]))
                {
                    SelectFeaturedPaletteColor(i);
                    return;
                }
            }
        }

		for (int i=0; (showSwatchPalette || showGradientPalette) && i< pencilRect.Length; i++) {			
			if (!showInapp && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (pencilRect [i]) && selectedToneIndex<0 &&
                !eagerShare && !showInapp && !showSharePopUp) {
                selectedToneIndex=i;
				pencilSelection [i] = !pencilSelection[i];
                Debug.Log(((float)Screen.width / (float)Screen.height).ToString() + " Simo Pencil Selected");
                if (showGradientPalette)
                    SelectGradientMode();
                else
                    SelectDefaultToneColor(selectedToneIndex);

				break;
			}	
			else if(!showInapp && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (pencilRect [i]) && !ButtonHit(toneRect) && 
                    !eagerShare && !showInapp && !showSharePopUp) {	
                selectedToneIndex=i;
				pencilSelection [i] = !pencilSelection[i];
                Debug.Log(((float)Screen.width / (float)Screen.height).ToString() + " Simo Pencil Selected");
                if (showGradientPalette)
                    SelectGradientMode();
                else
                    SelectDefaultToneColor(selectedToneIndex);

				break;
			}	
		}
		
		if ((!showInapp && !showSavedPopUp && !isZooming && !startPanning && !fillAnimationRunning && ButtonFade(unDoRect,0))
            || (ButtonHit(unDoRect) && !showInapp && !showSharePopUp)) {
			Debug.Log("In Undo");
			colorHolds=false;
			//Debug.Log(oldFillers.Count);
			if(oldFillers.Count>0)			
				UndoFill();
		}

        if ((!showInapp && !showSavedPopUp && !isZooming && !startPanning && !fillAnimationRunning && ButtonFade(redoRect,3))
            || (ButtonHit(redoRect) && !showInapp && !showSharePopUp))
        {
            colorHolds = false;
            if (redoFillers.Count > 0)
                RedoFill();
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

		if (showSwatchPalette && !showInapp && !eagerShare && !showSavedPopUp && !isZooming && !startPanning && ButtonHit (toneRect) && selectedToneIndex>=0 &&
            !showInapp && !showSharePopUp) {
            Debug.Log("Simo : a tone is selected");
            Vector2 tonePointer = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            int clickedBandIndex = GetCurrentToneBandIndex();
            for (int i = 0; i < 11; i++)
            {
                if (GetToneBandDisplayRect(i).Contains(tonePointer))
                {
                    clickedBandIndex = i;
                    break;
                }
            }
			//Selecting tone index based on tone selected for particular pencil
            //// simo: tone located at (Input.mousePosition.x) < (990 * scale_x), so the not locked ones
			//if((!ImagePathHolder.GetLockedColors() && (Mathf.CeilToInt(Input.mousePosition.x)<(990*scale_x))) || (ImagePathHolder.GetLockedColors()))
			//{							
				fillColor=pencilTones[selectedToneIndex].GetPixel(Mathf.CeilToInt((Input.mousePosition.x-toneRect.x)*pencilTones[selectedToneIndex].width/Screen.width),Mathf.CeilToInt(pencilTones[selectedToneIndex].height-(Screen.height-Input.mousePosition.y-toneRect.y)*toneRect.height/(pencilTones[selectedToneIndex].height)));
				
				SetSelectedToneBandIndex(clickedBandIndex);
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

        if (showGradientPalette && !showInapp && !eagerShare && !showSavedPopUp && !isZooming && !startPanning && ButtonHit(toneRect) &&
            !showSharePopUp)
        {
            RefreshGradientPaletteTexture();
            Vector2 tonePointer = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            int gradientBandIndex = GetCurrentToneBandIndex();
            for (int i = 0; i < 11; i++)
            {
                if (GetToneBandDisplayRect(i).Contains(tonePointer))
                {
                    gradientBandIndex = i;
                    break;
                }
            }
            SelectGradientBand(gradientBandIndex);
        }
		if (showSavedPopUp && !savedChoiceTransitionRunning && ButtonHit(startoverRect)) {
			StartCoroutine(ApplySavedChoiceWithFade(true));
		}
		if (showSavedPopUp && !savedChoiceTransitionRunning && ButtonHit(ContinueRect)) {
			StartCoroutine(ApplySavedChoiceWithFade(false));
		}

        // simo : check if Avoid touch was touched  
        if ((showSwatchPalette || showGradientPalette || showFeaturedPalette) && ButtonHit(coverRectAvoidingTouch))
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
        if (!showSwatchPalette && !showGradientPalette && !showFeaturedPalette)
            return;
        // Intentionally left blank: keep touch-blocking rect logic without visual overlay.
    }


	// simo : drawbackground
	void DrawBackground()
	{		
		//backgroundImg = new Texture2D(testImage.width, testImage.height);
		backgroundImg = Resources.Load<Texture2D>("Graphics/UIIcons/background_grey_mandala");
		GUI.DrawTexture(backgroundImgRect, backgroundImg);
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
        if (!topSelection[3])
            DrawMirroredTexture(redoRect, unDo);
        else
        {
            DrawMirroredTexture(redoRect, unDo);
            GUI.DrawTexture(redoRect, transImg);
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


    void DrawMirroredTexture(Rect rect, Texture texture)
    {
        if (texture == null)
            return;

        Matrix4x4 oldMatrix = GUI.matrix;
        GUIUtility.ScaleAroundPivot(new Vector2(-1f, 1f), new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f));
        GUI.DrawTexture(rect, texture);
        GUI.matrix = oldMatrix;
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
        int currentToneBandIndex = GetCurrentToneBandIndex();
        DrawMainRowBackdrop();
        DrawSelectedMainRowItemBackdrop();
        DrawExpandedPanelBackdrop();
        DrawPaletteToggleRect(paletteToggleRect, normalPaletteToggleTexture, showSwatchPalette, IsPressingRect(paletteToggleRect));
        DrawPaletteToggleRect(gradientToggleRect, gradientPaletteToggleTexture, showGradientPalette, IsPressingRect(gradientToggleRect));
        if (featuredPaletteToggleRects != null && featuredPaletteToggleTextures != null)
        {
            DrawFeaturedThemeScrollArrow(featuredThemeLeftArrowRect, true, IsPressingRect(featuredThemeLeftArrowRect));
            GUI.BeginGroup(featuredThemeViewportRect);
            for (int i = 0; i < featuredPaletteToggleRects.Length && i < featuredPaletteToggleTextures.Length; i++)
            {
                bool isActiveTheme = showFeaturedPalette && activeFeaturedPaletteIndex == i;
                Rect localRect = featuredPaletteToggleRects[i];
                Rect drawRect = new Rect(localRect.x - featuredThemeRowScroll, localRect.y, localRect.width, localRect.height);
                if (drawRect.xMax < 0f || drawRect.xMin > featuredThemeViewportRect.width)
                    continue;
                DrawFeaturedPaletteToggleSwatch(drawRect, featuredPaletteToggleTextures[i], isActiveTheme);
            }
            GUI.EndGroup();
            DrawFeaturedThemeScrollArrow(featuredThemeRightArrowRect, false, IsPressingRect(featuredThemeRightArrowRect));
        }
        DrawIconControl(customColorToggleRect, customColorToggleIcon, showCustomColorOverlay, IsPressingRect(customColorToggleRect));
        DrawImagePickerControl(imagePickerToggleRect, imagePickerToggleIcon, imagePickerMode, IsPressingRect(imagePickerToggleRect));

        Rect customPreviewRect = GetScaledRect(customColorToggleRect, showCustomColorOverlay ? 1.08f : 1f);
        float customPreviewInset = 18f * scale_x;
        float customPreviewSide = Mathf.Min(customPreviewRect.width, customPreviewRect.height) - customPreviewInset * 2f;
        Rect customPreviewInner = new Rect(
            customPreviewRect.center.x - customPreviewSide * 0.5f,
            customPreviewRect.center.y - customPreviewSide * 0.5f,
            customPreviewSide,
            customPreviewSide);
        EnsureRoundedToneBandMaskTexture();
        GUI.color = customOverlayActiveColor;
        GUI.DrawTexture(customPreviewInner, roundedToneBandMaskTexture != null ? roundedToneBandMaskTexture : Texture2D.whiteTexture, ScaleMode.StretchToFill, true);
        GUI.color = Color.white;

        if (whiteSwatchRect.width > 0f)
        {
            DrawPaletteSwatch(whiteSwatchRect, Color.white, whiteSwatchSelected, 0f, true, whiteSwatchSelected ? whiteSwatchScaleOffset : 0f);
        }

        if (showCustomColorOverlay)
            DrawCustomColorOverlay();

        if (!showSwatchPalette && !showGradientPalette && !showFeaturedPalette)
            return;

        if (showSwatchPalette || showGradientPalette)
        {
		    for (int pencilIndex=0; pencilIndex<pencilRect.Length; pencilIndex++) {
                Color swatchColor = palettePreviewColors != null && pencilIndex < palettePreviewColors.Length ? palettePreviewColors[pencilIndex] : Color.white;
                DrawPaletteSwatch(pencilRect[pencilIndex], swatchColor, pencilIndex == selectedToneIndex, 0f, false, 0f);
		    }
        }
        else if (showFeaturedPalette && featuredPaletteSwatchRects != null && activeFeaturedPaletteIndex >= 0 && featuredPaletteThemes != null && activeFeaturedPaletteIndex < featuredPaletteThemes.Length)
        {
            Color[] activeTheme = featuredPaletteThemes[activeFeaturedPaletteIndex];
            for (int i = 0; i < featuredPaletteSwatchRects.Length && i < activeTheme.Length; i++)
            {
                bool isSelected = i == selectedFeaturedColorIndex;
                DrawPaletteSwatch(featuredPaletteSwatchRects[i], activeTheme[i], isSelected, 0f, false, isSelected ? 0.18f : 0.12f);
            }
        }

        if (showGradientPalette)
        {
            DrawCircularControlButton(gradientRotateLeftRect);
            DrawRoundedControlButton(gradientDirectionRect);
            DrawCircularControlButton(gradientRotateRect);
            DrawRoundedControlButton(gradientResetRect);
            DrawGradientBoostSlider();
            if (IsPressingRect(gradientRotateLeftRect) || IsPressingRect(gradientDirectionRect) || IsPressingRect(gradientRotateRect) || IsPressingRect(gradientResetRect))
            {
                Color oldColor = GUI.color;
                if (IsPressingRect(gradientRotateLeftRect))
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.14f);
                    DrawCircularOverlay(gradientRotateLeftRect);
                }
                if (IsPressingRect(gradientDirectionRect))
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.14f);
                    GUI.DrawTexture(gradientDirectionRect, Texture2D.whiteTexture);
                }
                if (IsPressingRect(gradientRotateRect))
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.14f);
                    DrawCircularOverlay(gradientRotateRect);
                }
                if (IsPressingRect(gradientResetRect))
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.14f);
                    GUI.DrawTexture(gradientResetRect, Texture2D.whiteTexture);
                }
                GUI.color = oldColor;
            }
            GUIStyle oldStyle = new GUIStyle(customStyle);
            oldStyle.alignment = TextAnchor.MiddleCenter;
            oldStyle.fontSize = Mathf.CeilToInt(26 * scale_y);
            oldStyle.normal.textColor = Color.white;
            GUI.Label(gradientRotateLeftRect, "<", oldStyle);
            GUI.Label(gradientDirectionRect, GetGradientDirectionLabel(), oldStyle);
            GUI.Label(gradientRotateRect, ">", oldStyle);
            GUI.Label(gradientResetRect, "RESET", oldStyle);
        }

		if(selectedToneIndex>=0 && showSwatchPalette)
		{
            if (solidToneBandTextures == null || solidToneBandTextures.Length != 11 || solidToneBandTextures[0] == null)
                RebuildSolidToneBandTextures();
            for (int i = 0; i < 11; i++)
            {
                Rect bandRect = GetToneBandDisplayRect(i);
                DrawToneBandSelection(bandRect, i == currentToneBandIndex && colorSelected && !whiteSwatchSelected, 8f);
                if (solidToneBandTextures != null && i < solidToneBandTextures.Length && solidToneBandTextures[i] != null)
                    GUI.DrawTexture(bandRect, solidToneBandTextures[i], ScaleMode.StretchToFill, true);
            }
            // simo : comment/decomment to not see/see the lock
            /*                
			if(!ImagePathHolder.GetLockedColors())
				for(int i=0;i<4;i++)                   
                    GUI.DrawTexture(new Rect((1020 + i * 135) * scale_x, 1958 * scale_y, 80 * scale_x, 80 * scale_y), lockColor);
                    // simo : draw the lock IN ANOTHER POSITION
                    //GUI.DrawTexture(new Rect((1020+i*135)*scale_x,1958*scale_y -(float)(Screen.height * 0.12f),80*scale_x,80*scale_y),lockColor);
            */        
		}
        else if (showGradientPalette && selectedToneIndex >= 0 && gradientPaletteTexture != null)
        {
            if (gradientToneBandTextures == null || gradientToneBandTextures.Length != 11 || gradientToneBandTextures[0] == null)
                RebuildGradientToneBandTextures();
            for (int i = 0; i < 11; i++)
            {
                Rect bandRect = GetToneBandDisplayRect(i);
                DrawToneBandSelection(bandRect, gradientBandSelected && i == currentToneBandIndex && colorSelected && !whiteSwatchSelected, 8f);
                if (gradientToneBandTextures != null && i < gradientToneBandTextures.Length && gradientToneBandTextures[i] != null)
                    GUI.DrawTexture(bandRect, gradientToneBandTextures[i], ScaleMode.StretchToFill, true);
            }
        }
		
	}


    void ExpandRect(ref Rect bounds, ref bool hasBounds, Rect candidate)
    {
        if (candidate.width <= 0f || candidate.height <= 0f)
            return;

        if (!hasBounds)
        {
            bounds = candidate;
            hasBounds = true;
            return;
        }

        float xMin = Mathf.Min(bounds.xMin, candidate.xMin);
        float yMin = Mathf.Min(bounds.yMin, candidate.yMin);
        float xMax = Mathf.Max(bounds.xMax, candidate.xMax);
        float yMax = Mathf.Max(bounds.yMax, candidate.yMax);
        bounds = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }


    Rect GetLargestExpandedPanelReferenceRect()
    {
        Rect bounds = new Rect();
        bool hasBounds = false;

        if (pencilRect != null)
        {
            for (int i = 0; i < pencilRect.Length; i++)
                ExpandRect(ref bounds, ref hasBounds, pencilRect[i]);
        }

        ExpandRect(ref bounds, ref hasBounds, toneRect);
        ExpandRect(ref bounds, ref hasBounds, gradientRotateLeftRect);
        ExpandRect(ref bounds, ref hasBounds, gradientDirectionRect);
        ExpandRect(ref bounds, ref hasBounds, gradientRotateRect);
        ExpandRect(ref bounds, ref hasBounds, gradientResetRect);
        ExpandRect(ref bounds, ref hasBounds, gradientBoostSliderRect);
        ExpandRect(ref bounds, ref hasBounds, gradientBoostLabelRect);

        return hasBounds ? bounds : new Rect();
    }


    Rect GetCurrentExpandedPanelRect()
    {
        Rect bounds = new Rect();
        bool hasBounds = false;

        if (showSwatchPalette || showGradientPalette)
        {
            if (pencilRect != null)
            {
                for (int i = 0; i < pencilRect.Length; i++)
                    ExpandRect(ref bounds, ref hasBounds, pencilRect[i]);
            }
            ExpandRect(ref bounds, ref hasBounds, toneRect);
        }

        if (showGradientPalette)
        {
            ExpandRect(ref bounds, ref hasBounds, gradientRotateLeftRect);
            ExpandRect(ref bounds, ref hasBounds, gradientDirectionRect);
            ExpandRect(ref bounds, ref hasBounds, gradientRotateRect);
            ExpandRect(ref bounds, ref hasBounds, gradientResetRect);
            ExpandRect(ref bounds, ref hasBounds, gradientBoostSliderRect);
            ExpandRect(ref bounds, ref hasBounds, gradientBoostLabelRect);
        }

        if (showFeaturedPalette && featuredPaletteSwatchRects != null)
        {
            for (int i = 0; i < featuredPaletteSwatchRects.Length; i++)
                ExpandRect(ref bounds, ref hasBounds, featuredPaletteSwatchRects[i]);
        }

        if (showCustomColorOverlay)
        {
            ExpandRect(ref bounds, ref hasBounds, GetCustomColorPanelRect());
            ExpandRect(ref bounds, ref hasBounds, customHueSliderRect);
            if (showAdvancedCustomPicker)
                ExpandRect(ref bounds, ref hasBounds, customSvSquareRect);
        }

        return hasBounds ? bounds : new Rect();
    }


    float GetExpandedPanelContentRightEdge()
    {
        float rightEdge = 0f;

        if (showSwatchPalette || showGradientPalette)
        {
            if (pencilRect != null)
            {
                for (int i = 0; i < pencilRect.Length; i++)
                    rightEdge = Mathf.Max(rightEdge, pencilRect[i].xMax);
            }

            for (int i = 0; i < 11; i++)
                rightEdge = Mathf.Max(rightEdge, GetToneBandDisplayRect(i).xMax);
        }

        if (showGradientPalette)
        {
            rightEdge = Mathf.Max(rightEdge, gradientRotateLeftRect.xMax);
            rightEdge = Mathf.Max(rightEdge, gradientDirectionRect.xMax);
            rightEdge = Mathf.Max(rightEdge, gradientRotateRect.xMax);
            rightEdge = Mathf.Max(rightEdge, gradientResetRect.xMax);
            rightEdge = Mathf.Max(rightEdge, gradientBoostSliderRect.xMax);
            rightEdge = Mathf.Max(rightEdge, gradientBoostLabelRect.xMax);
        }

        if (showFeaturedPalette && featuredPaletteSwatchRects != null)
        {
            for (int i = 0; i < featuredPaletteSwatchRects.Length; i++)
                rightEdge = Mathf.Max(rightEdge, featuredPaletteSwatchRects[i].xMax);
        }

        return rightEdge;
    }


    void DrawExpandedPanelBackdrop()
    {
        if (!showSwatchPalette && !showGradientPalette && !showFeaturedPalette && !showCustomColorOverlay)
            return;

        Rect referenceRect = GetLargestExpandedPanelReferenceRect();
        Rect activeRect = GetCurrentExpandedPanelRect();
        Rect backdropRect = activeRect;
        bool hasBackdrop = activeRect.width > 0f && activeRect.height > 0f;

        if (referenceRect.width > 0f && referenceRect.height > 0f)
        {
            if (!hasBackdrop)
            {
                backdropRect = referenceRect;
                hasBackdrop = true;
            }
            else
            {
                bool merge = true;
                ExpandRect(ref backdropRect, ref merge, referenceRect);
            }
        }

        if (!hasBackdrop)
            return;

        float horizontalPadding = 18f * scale_x;
        float topPadding = 18f * scale_y;
        float bottomPadding = 6f * scale_y;
        backdropRect = new Rect(
            backdropRect.x - horizontalPadding,
            backdropRect.y - topPadding,
            backdropRect.width + horizontalPadding * 2f,
            backdropRect.height + topPadding + bottomPadding);

        float leftEdge = origImgRect.width > 0f ? origImgRect.xMin : imageViewportRect.xMin;
        float rightEdge = origImgRect.width > 0f ? origImgRect.xMax : imageViewportRect.xMax;
        float horizontalExpand = 2f;
        if (rightEdge > leftEdge)
            backdropRect = Rect.MinMaxRect(leftEdge - horizontalExpand, backdropRect.yMin, rightEdge + horizontalExpand, backdropRect.yMax);

        EnsureExpandedPanelBackdropTexture();
        Texture buttonTexture = expandedPanelBackdropTexture != null ? expandedPanelBackdropTexture : Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = new Color32(92, 96, 104, 255);
        GUI.DrawTexture(backdropRect, buttonTexture, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }


    Rect GetMainRowBackdropRect()
    {
        float leftEdge = origImgRect.width > 0f ? origImgRect.xMin : imageViewportRect.xMin;
        float rightEdge = origImgRect.width > 0f ? origImgRect.xMax : imageViewportRect.xMax;
        if (rightEdge <= leftEdge)
            return new Rect();

        Rect contentRect = paletteToggleRect;
        bool hasContent = paletteToggleRect.width > 0f;

        ExpandRect(ref contentRect, ref hasContent, gradientToggleRect);
        ExpandRect(ref contentRect, ref hasContent, featuredThemeLeftArrowRect);
        ExpandRect(ref contentRect, ref hasContent, featuredThemeViewportRect);
        ExpandRect(ref contentRect, ref hasContent, featuredThemeRightArrowRect);
        ExpandRect(ref contentRect, ref hasContent, whiteSwatchRect);

        if (!hasContent)
            return new Rect();

        float topPadding = mainRowBackdropVerticalPaddingPixels * scale_y;
        float bottomPadding = mainRowBackdropVerticalPaddingPixels * scale_y;
        return Rect.MinMaxRect(
            leftEdge,
            contentRect.yMin - topPadding,
            rightEdge,
            contentRect.yMax + bottomPadding);
    }


    void DrawMainRowBackdrop()
    {
        Rect backdropRect = GetMainRowBackdropRect();
        if (backdropRect.width <= 0f || backdropRect.height <= 0f)
            return;

        EnsureExpandedPanelBackdropTexture();
        Texture buttonTexture = expandedPanelBackdropTexture != null ? expandedPanelBackdropTexture : Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = new Color32(39, 39, 43, 255);
        GUI.DrawTexture(backdropRect, buttonTexture, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }


    Rect GetSelectedMainRowItemRect()
    {
        if (showSwatchPalette)
            return GetNormalizedMainRowSelectionRect(paletteToggleRect);

        if (showGradientPalette)
            return GetNormalizedMainRowSelectionRect(gradientToggleRect);

        if (showFeaturedPalette && activeFeaturedPaletteIndex >= 0)
            return GetFeaturedToggleDisplayRect(activeFeaturedPaletteIndex);

        return new Rect();
    }


    Rect GetNormalizedMainRowSelectionRect(Rect sourceRect)
    {
        if (featuredPaletteToggleRects == null || featuredPaletteToggleRects.Length == 0)
            return sourceRect;

        Rect referenceRect = GetFeaturedToggleDisplayRect(0);
        if (referenceRect.width <= 0f || referenceRect.height <= 0f)
            return sourceRect;

        return new Rect(
            sourceRect.center.x - referenceRect.width * 0.5f,
            sourceRect.center.y - referenceRect.height * 0.5f,
            referenceRect.width,
            referenceRect.height);
    }


    void DrawSelectedMainRowItemBackdrop()
    {
        Rect selectedRect = GetSelectedMainRowItemRect();
        if (selectedRect.width <= 0f || selectedRect.height <= 0f)
            return;

        float horizontalPadding = selectedMainRowBackdropHorizontalPaddingPixels * scale_x;
        float topPadding = selectedMainRowBackdropTopPaddingPixels * scale_y;
        float bottomPadding = selectedMainRowBackdropBottomPaddingPixels * scale_y;
        Rect backdropRect = new Rect(
            selectedRect.xMin - horizontalPadding,
            selectedRect.yMin - topPadding,
            selectedRect.width + horizontalPadding * 2f,
            selectedRect.height + topPadding + bottomPadding);

        Rect mainRowBackdropRect = GetMainRowBackdropRect();
        if (mainRowBackdropRect.width > 0f)
        {
            if (backdropRect.xMin < mainRowBackdropRect.xMin)
                backdropRect.x = mainRowBackdropRect.xMin;
            if (backdropRect.xMax > mainRowBackdropRect.xMax)
                backdropRect.x = mainRowBackdropRect.xMax - backdropRect.width;
        }

        EnsureSelectedMainRowBackdropTexture();
        Texture buttonTexture = selectedMainRowBackdropTexture != null ? selectedMainRowBackdropTexture : Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = new Color32(92, 96, 104, 255);

        if (showFeaturedPalette && featuredThemeViewportRect.width > 0f)
        {
            Rect horizontalClipRect = new Rect(featuredThemeViewportRect.x, 0f, featuredThemeViewportRect.width, Screen.height);
            GUI.BeginGroup(horizontalClipRect);
            Rect localBackdropRect = new Rect(
                backdropRect.x - horizontalClipRect.x,
                backdropRect.y - horizontalClipRect.y,
                backdropRect.width,
                backdropRect.height);
            GUI.DrawTexture(localBackdropRect, buttonTexture, ScaleMode.StretchToFill, true);
            GUI.EndGroup();
        }
        else
        {
            GUI.DrawTexture(backdropRect, buttonTexture, ScaleMode.StretchToFill, true);
        }

        GUI.color = oldColor;
    }


    void EnsureFeaturedToggleVisibleWithPadding(int index)
    {
        if (featuredPaletteToggleRects == null || index < 0 || index >= featuredPaletteToggleRects.Length || featuredThemeViewportRect.width <= 0f)
            return;

        Rect localRect = featuredPaletteToggleRects[index];
        float edgePadding = Mathf.Max(18f * scale_x, localRect.width * 0.35f);
        float maxScroll = Mathf.Max(0f, featuredThemeContentWidth - featuredThemeViewportRect.width);

        float visibleMin = localRect.x - featuredThemeRowScroll;
        float visibleMax = localRect.xMax - featuredThemeRowScroll;

        if (visibleMin < edgePadding)
            featuredThemeRowScroll = Mathf.Clamp(localRect.x - edgePadding, 0f, maxScroll);
        else if (visibleMax > featuredThemeViewportRect.width - edgePadding)
            featuredThemeRowScroll = Mathf.Clamp(localRect.xMax - (featuredThemeViewportRect.width - edgePadding), 0f, maxScroll);
    }


    void DrawPaletteSwatch(Rect slot, Color fill, bool isSelected, float liftOffset, bool isWhiteSwatch, float extraScale)
    {
        float diameter = Mathf.Min(slot.width, slot.height) * (0.62f + extraScale);
        float centerX = slot.x + slot.width * 0.5f;
        float centerY = slot.y + slot.height * (isWhiteSwatch ? 0.5f : 0.42f) - liftOffset;
        float border = isSelected ? 7f * scale_x : 4f * scale_x;

        Rect outerCircle = new Rect(centerX - diameter * 0.5f - border, centerY - diameter * 0.5f - border, diameter + border * 2f, diameter + border * 2f);
        Rect innerCircle = new Rect(centerX - diameter * 0.5f, centerY - diameter * 0.5f, diameter, diameter);

        Texture mask = circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture;
        if (isSelected || isWhiteSwatch)
        {
            Color outerColor = Color.white;
            if (isWhiteSwatch && !isSelected)
                outerColor = new Color(0.85f, 0.85f, 0.85f, 1f);

            GUI.color = outerColor;
            GUI.DrawTexture(outerCircle, mask, ScaleMode.StretchToFill, true);
        }
        GUI.color = fill;
        GUI.DrawTexture(innerCircle, mask, ScaleMode.StretchToFill, true);
        GUI.color = Color.white;
    }


    void DrawFeaturedPaletteToggleSwatch(Rect slot, Texture paletteTexture, bool isSelected)
    {
        float extraScale = isSelected ? 0.08f : 0f;
        float diameter = Mathf.Min(slot.width, slot.height) * (0.62f + extraScale);
        float centerX = slot.x + slot.width * 0.5f;
        float centerY = slot.y + slot.height * 0.5f;
        float border = isSelected ? 7f * scale_x : 4f * scale_x;

        Rect outerCircle = new Rect(centerX - diameter * 0.5f - border, centerY - diameter * 0.5f - border, diameter + border * 2f, diameter + border * 2f);
        Rect innerCircle = new Rect(centerX - diameter * 0.5f, centerY - diameter * 0.5f, diameter, diameter);
        Texture mask = circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture;

        if (isSelected)
        {
            GUI.color = Color.white;
            GUI.DrawTexture(outerCircle, mask, ScaleMode.StretchToFill, true);
        }
        GUI.DrawTexture(innerCircle, paletteTexture != null ? paletteTexture : mask, ScaleMode.StretchToFill, true);
        GUI.color = Color.white;
    }


    void DrawFeaturedThemeScrollArrow(Rect rect, bool pointLeft, bool isPressed)
    {
        if (rect.width <= 0f || rect.height <= 0f)
            return;

        if (isPressed)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.18f);
            DrawCircularOverlay(rect);
            GUI.color = oldColor;
        }

        if (backChevronIcon != null)
        {
            Rect drawRect = GetScaledRect(rect, 0.92f);
            if (pointLeft)
            {
                GUI.DrawTexture(drawRect, backChevronIcon, ScaleMode.ScaleToFit, true);
            }
            else
            {
                Matrix4x4 oldMatrix = GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(-1f, 1f), new Vector2(drawRect.x + drawRect.width * 0.5f, drawRect.y + drawRect.height * 0.5f));
                GUI.DrawTexture(drawRect, backChevronIcon, ScaleMode.ScaleToFit, true);
                GUI.matrix = oldMatrix;
            }
            return;
        }

        GUIStyle arrowStyle = new GUIStyle(customStyle);
        arrowStyle.alignment = TextAnchor.MiddleCenter;
        arrowStyle.fontSize = Mathf.CeilToInt(40f * scale_y);
        arrowStyle.normal.textColor = Color.white;
        GUI.Label(rect, pointLeft ? "<" : ">", arrowStyle);
    }


    Rect GetScaledRect(Rect rect, float scale)
    {
        float width = rect.width * scale;
        float height = rect.height * scale;
        return new Rect(
            rect.x + (rect.width - width) * 0.5f,
            rect.y + (rect.height - height) * 0.5f,
            width,
            height);
    }


    Rect GetFeaturedToggleDisplayRect(int index)
    {
        if (featuredPaletteToggleRects == null || index < 0 || index >= featuredPaletteToggleRects.Length)
            return new Rect();

        Rect localRect = featuredPaletteToggleRects[index];
        return new Rect(
            featuredThemeViewportRect.x + localRect.x - featuredThemeRowScroll,
            featuredThemeViewportRect.y + localRect.y,
            localRect.width,
            localRect.height);
    }


    Rect GetCustomOverlaySwatchRect(int index)
    {
        int columns = 4;
        float paddingX = 18f * scale_x;
        float paddingY = 20f * scale_y;
        float cellSpacingX = 12f * scale_x;
        float cellSpacingY = 12f * scale_y;
        float leftSectionWidth = Mathf.Max(220f * scale_x, customColorOverlayRect.width * 0.48f);
        float rightSectionX = customColorOverlayRect.x + leftSectionWidth + (22f * scale_x);
        float rightSectionWidth = customColorOverlayRect.xMax - (22f * scale_x) - rightSectionX;
        float cellWidth = (rightSectionWidth - (paddingX * 2f) - (cellSpacingX * (columns - 1))) / columns;
        float cellHeight = Mathf.Min(58f * scale_y, (customColorOverlayRect.height - paddingY - (28f * scale_y) - (cellSpacingY * 3f)) / 4f);
        int row = index / columns;
        int column = index % columns;

        return new Rect(
            rightSectionX + paddingX + column * (cellWidth + cellSpacingX),
            customColorOverlayRect.y + paddingY + row * (cellHeight + cellSpacingY),
            cellWidth,
            cellHeight);
    }

    Rect GetSavedCustomColorRect(int index)
    {
        Rect lastGridRect = GetCustomOverlaySwatchRect(3);
        float availableLeft = customHueSliderRect.xMin;
        float buttonSize = GetCustomOverlaySwatchRect(0).height;
        float buttonGap = 12f * scale_x;
        float availableRight = customColorOverlayRect.xMax - buttonSize - buttonGap;
        float spacingX = 12f * scale_x;
        int slotCount = savedCustomColors != null && savedCustomColors.Length > 0 ? savedCustomColors.Length : SavedCustomColorCount;
        float cellWidth = (availableRight - availableLeft - spacingX * (slotCount - 1)) / Mathf.Max(1, slotCount);
        float cellHeight = GetCustomOverlaySwatchRect(0).height;
        float y = Mathf.Max(customHueSliderRect.yMax, lastGridRect.yMax) + 24f;
        return new Rect(
            availableLeft + index * (cellWidth + spacingX),
            y,
            cellWidth,
            cellHeight);
    }

    Rect GetClearSavedCustomColorsButtonRect()
    {
        Rect firstSavedRect = GetSavedCustomColorRect(0);
        Rect lastSavedRect = GetSavedCustomColorRect((savedCustomColors != null && savedCustomColors.Length > 0 ? savedCustomColors.Length : SavedCustomColorCount) - 1);
        float size = firstSavedRect.height;
        return new Rect(
            lastSavedRect.xMax + (12f * scale_x),
            firstSavedRect.y,
            size,
            size);
    }


    void UpdateCustomPickerHueFromPointer(float pointerX)
    {
        float t = Mathf.InverseLerp(customHueSliderRect.xMin, customHueSliderRect.xMax, pointerX);
        customPickerHue = Mathf.Clamp01(t);
        RefreshCustomSvSquareTexture();
        SelectCustomOverlayColor(Color.HSVToRGB(customPickerHue, customPickerSaturation, customPickerValue));
    }


    void UpdateCustomPickerSvFromPointer(Vector2 pointer)
    {
        customPickerSaturation = Mathf.Clamp01(Mathf.InverseLerp(customSvSquareRect.xMin, customSvSquareRect.xMax, pointer.x));
        customPickerValue = Mathf.Clamp01(Mathf.InverseLerp(customSvSquareRect.yMin, customSvSquareRect.yMax, pointer.y));
        SelectCustomOverlayColor(Color.HSVToRGB(customPickerHue, customPickerSaturation, customPickerValue));
    }


    Color32 SampleColorFromDrawingAtPointer(Vector2 pointerPosition)
    {
        if (mainImage == null || imageRect.width <= 0f || imageRect.height <= 0f)
            return (Color32)fillColor;

        float localX = Mathf.Clamp01((pointerPosition.x - imageRect.x) / imageRect.width);
        float localY = Mathf.Clamp01((pointerPosition.y - imageRect.y) / imageRect.height);
        float textureY = 1f - localY;
        Color sampled = mainImage.GetPixelBilinear(localX, textureY);
        sampled.a = 1f;
        return (Color32)sampled;
    }


    bool IsPressingRect(Rect rect)
    {
        return rect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) && Input.GetMouseButton(0);
    }


    void DrawIconControl(Rect rect, Texture icon, bool isActive, bool isPressed)
    {
        Rect drawRect = rect;
        if (icon != null)
            GUI.DrawTexture(drawRect, icon, ScaleMode.ScaleToFit, true);

        if (isPressed)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.18f);
            GUI.DrawTexture(drawRect, Texture2D.whiteTexture);
            GUI.color = oldColor;
        }
    }

    void DrawImagePickerControl(Rect rect, Texture icon, bool isActive, bool isPressed)
    {
        EnsureRoundedToneBandMaskTexture();
        Texture mask = roundedToneBandMaskTexture != null ? roundedToneBandMaskTexture : Texture2D.whiteTexture;

        float backgroundInset = 10f * Mathf.Min(scale_x, scale_y);
        float iconInset = 24f * Mathf.Min(scale_x, scale_y);
        float squareSide = Mathf.Min(rect.width, rect.height);
        Rect squareRect = new Rect(
            rect.center.x - squareSide * 0.5f,
            rect.center.y - squareSide * 0.5f,
            squareSide,
            squareSide);
        Rect backgroundRect = new Rect(
            squareRect.x + backgroundInset,
            squareRect.y + backgroundInset,
            squareRect.width - backgroundInset * 2f,
            squareRect.height - backgroundInset * 2f);
        Rect iconRect = new Rect(
            squareRect.x + iconInset,
            squareRect.y + iconInset,
            squareRect.width - iconInset * 2f,
            squareRect.height - iconInset * 2f);

        Color oldColor = GUI.color;

        if (isActive)
        {
            GUI.color = new Color32(92, 96, 104, 255);
            GUI.DrawTexture(backgroundRect, mask, ScaleMode.StretchToFill, true);
        }

        if (icon != null)
        {
            GUI.color = isActive ? Color.black : Color.white;
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
        }

        if (isPressed)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.14f);
            GUI.DrawTexture(backgroundRect, mask, ScaleMode.StretchToFill, true);
        }

        GUI.color = oldColor;
    }


    void DrawPaletteToggleRect(Rect rect, Texture texture, bool isActive, bool isPressed)
    {
        EnsureRoundedToneBandMaskTexture();
        Texture mask = roundedToneBandMaskTexture != null ? roundedToneBandMaskTexture : Texture2D.whiteTexture;
        if (isActive)
        {
            float stroke = Mathf.Max(4f * Mathf.Min(scale_x, scale_y), 4f);
            Rect outerRect = new Rect(rect.x - stroke, rect.y - stroke, rect.width + stroke * 2f, rect.height + stroke * 2f);
            GUI.color = Color.white;
            GUI.DrawTexture(outerRect, mask, ScaleMode.StretchToFill, true);
        }
        if (texture != null)
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true);
        else
        {
            GUI.color = Color.white;
            GUI.DrawTexture(rect, mask, ScaleMode.StretchToFill, true);
            GUI.color = Color.white;
        }

        if (isPressed)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.18f);
            GUI.DrawTexture(rect, mask, ScaleMode.StretchToFill, true);
            GUI.color = oldColor;
        }
    }


    void DrawRoundedControlButton(Rect rect)
    {
        Texture buttonTexture = roundedDirectionButtonTexture != null ? roundedDirectionButtonTexture : Texture2D.whiteTexture;
        float stroke = Mathf.Max(2.5f * scale_x, 2.5f);
        Rect outerRect = new Rect(rect.x - stroke, rect.y - stroke, rect.width + stroke * 2f, rect.height + stroke * 2f);

        Color oldColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(outerRect, buttonTexture, ScaleMode.StretchToFill, true);
        GUI.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        GUI.DrawTexture(rect, buttonTexture, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }


    void DrawCircularControlButton(Rect rect)
    {
        float stroke = Mathf.Max(2.5f * scale_x, 2.5f);
        float diameter = Mathf.Min(rect.width, rect.height);
        Vector2 center = rect.center;
        Rect innerCircle = new Rect(center.x - diameter * 0.5f, center.y - diameter * 0.5f, diameter, diameter);
        Rect outerCircle = new Rect(innerCircle.x - stroke, innerCircle.y - stroke, innerCircle.width + stroke * 2f, innerCircle.height + stroke * 2f);

        Texture mask = circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(outerCircle, mask, ScaleMode.StretchToFill, true);
        GUI.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        GUI.DrawTexture(innerCircle, mask, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }

    void DrawCircularControlButton(Rect rect, float stroke, Color fillColor)
    {
        float diameter = Mathf.Min(rect.width, rect.height);
        Vector2 center = rect.center;
        Rect innerCircle = new Rect(center.x - diameter * 0.5f, center.y - diameter * 0.5f, diameter, diameter);
        Rect outerCircle = new Rect(innerCircle.x - stroke, innerCircle.y - stroke, innerCircle.width + stroke * 2f, innerCircle.height + stroke * 2f);

        Texture mask = circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture;
        Color oldColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(outerCircle, mask, ScaleMode.StretchToFill, true);
        GUI.color = fillColor;
        GUI.DrawTexture(innerCircle, mask, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }


    void DrawCircularOverlay(Rect rect)
    {
        float diameter = Mathf.Min(rect.width, rect.height);
        Vector2 center = rect.center;
        Rect circle = new Rect(center.x - diameter * 0.5f, center.y - diameter * 0.5f, diameter, diameter);
        Texture mask = circularSwatchMask != null ? circularSwatchMask : Texture2D.whiteTexture;
        GUI.DrawTexture(circle, mask, ScaleMode.StretchToFill, true);
    }


    void DrawToneBandSelection(Rect rect, bool isSelected)
    {
        DrawToneBandSelection(rect, isSelected, 2f);
    }


    void DrawToneBandSelection(Rect rect, bool isSelected, float strokePixels)
    {
        if (!isSelected)
            return;

        EnsureRoundedToneBandMaskTexture();

        Texture mask = roundedToneBandMaskTexture != null ? roundedToneBandMaskTexture : Texture2D.whiteTexture;
        float stroke = Mathf.Max(strokePixels * Mathf.Min(scale_x, scale_y), strokePixels);
        Rect outerRect = new Rect(rect.x - stroke, rect.y - stroke, rect.width + stroke * 2f, rect.height + stroke * 2f);

        Color oldColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(outerRect, mask, ScaleMode.StretchToFill, true);
        GUI.color = Color.white;
        GUI.DrawTexture(rect, mask, ScaleMode.StretchToFill, true);
        GUI.color = oldColor;
    }


    void DrawCustomColorOverlay()
    {
        if (customOverlayPaletteColors == null)
            return;

        EnsureRoundedToneBandMaskTexture();
        Texture mask = roundedToneBandMaskTexture != null ? roundedToneBandMaskTexture : Texture2D.whiteTexture;

        for (int i = 0; i < customOverlayPaletteColors.Length; i++)
        {
            Rect swatchRect = GetCustomOverlaySwatchRect(i);
            bool isSelected = customOverlayActiveColor == customOverlayPaletteColors[i] && !whiteSwatchSelected && !gradientModeActive && selectedToneIndex < 0;
            DrawToneBandSelection(swatchRect, isSelected);
            GUI.color = customOverlayPaletteColors[i];
            GUI.DrawTexture(swatchRect, mask, ScaleMode.StretchToFill, true);
            GUI.color = Color.white;
        }

        if (savedCustomColors != null)
        {
            Rect clearRect = GetClearSavedCustomColorsButtonRect();
            DrawCircularControlButton(clearRect, 1f, new Color(0.08f, 0.08f, 0.08f, 1f));
            GUIStyle clearStyle = new GUIStyle(customStyle);
            clearStyle.alignment = TextAnchor.MiddleCenter;
            clearStyle.fontSize = Mathf.CeilToInt(22f * scale_y);
            clearStyle.normal.textColor = Color.white;
            GUI.Label(clearRect, "X", clearStyle);

            for (int i = 0; i < savedCustomColors.Length; i++)
            {
                Rect savedRect = GetSavedCustomColorRect(i);
                float stroke = Mathf.Max(2f * Mathf.Min(scale_x, scale_y), 2f);
                Rect outerRect = new Rect(savedRect.x - stroke, savedRect.y - stroke, savedRect.width + stroke * 2f, savedRect.height + stroke * 2f);
                GUI.color = Color.white;
                GUI.DrawTexture(outerRect, mask, ScaleMode.StretchToFill, true);
                if (savedCustomColors[i].a > 0f)
                {
                    GUI.color = savedCustomColors[i];
                    GUI.DrawTexture(savedRect, mask, ScaleMode.StretchToFill, true);
                }
                else
                {
                    GUI.color = new Color32(92, 96, 104, 255);
                    GUI.DrawTexture(savedRect, mask, ScaleMode.StretchToFill, true);
                }
                GUI.color = Color.white;
            }
        }

        if (!showAdvancedCustomPicker)
            return;

        EnsureCustomHueSliderTexture();
        if (customSvSquareTexture == null)
            RefreshCustomSvSquareTexture();

        GUI.DrawTexture(customHueSliderRect, customHueSliderTexture, ScaleMode.StretchToFill, true);
        float hueMarkerX = Mathf.Lerp(customHueSliderRect.xMin, customHueSliderRect.xMax, customPickerHue);
        Rect hueMarker = new Rect(hueMarkerX - (6f * scale_x), customHueSliderRect.y - (4f * scale_y), 12f * scale_x, customHueSliderRect.height + (8f * scale_y));
        GUI.color = Color.white;
        GUI.DrawTexture(hueMarker, mask, ScaleMode.StretchToFill, true);

        GUI.DrawTexture(customSvSquareRect, customSvSquareTexture, ScaleMode.StretchToFill, true);
        float svMarkerX = Mathf.Lerp(customSvSquareRect.xMin, customSvSquareRect.xMax, customPickerSaturation);
        float svMarkerY = Mathf.Lerp(customSvSquareRect.yMin, customSvSquareRect.yMax, customPickerValue);
        Rect svMarker = new Rect(svMarkerX - (10f * scale_x), svMarkerY - (10f * scale_y), 20f * scale_x, 20f * scale_y);
        DrawCircularControlButton(svMarker);
        if (draggingCustomHue)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.12f);
            GUI.DrawTexture(customHueSliderRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        if (draggingCustomSv)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.08f);
            GUI.DrawTexture(customSvSquareRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }


    void DrawGradientBoostSlider()
    {
        float trackThickness = Mathf.Max(2f * scale_y, 2f);
        Rect trackRect = new Rect(
            gradientBoostSliderRect.x,
            gradientBoostSliderRect.center.y - trackThickness * 0.5f,
            gradientBoostSliderRect.width,
            trackThickness);
        Color oldTrackColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(trackRect, Texture2D.whiteTexture);
        GUI.color = oldTrackColor;

        float knobWidth = Mathf.Max(trackThickness * 2f, 4f * scale_x);
        float knobHeight = Mathf.Max(gradientBoostSliderRect.height * 0.8f, trackThickness * 4f);
        float knobCenterX = Mathf.Lerp(gradientBoostSliderRect.xMin, gradientBoostSliderRect.xMax, gradientVisibilityBoost);
        Rect knobRect = new Rect(
            knobCenterX - knobWidth * 0.5f,
            gradientBoostSliderRect.center.y - knobHeight * 0.5f,
            knobWidth,
            knobHeight);

        Color oldKnobColor = GUI.color;
        GUI.color = Color.white;
        GUI.DrawTexture(knobRect, Texture2D.whiteTexture);
        GUI.color = oldKnobColor;
        if (gradientBoostDragging)
        {
            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.14f);
            GUI.DrawTexture(new Rect(
                knobRect.x - trackThickness,
                knobRect.y,
                knobRect.width + trackThickness * 2f,
                knobRect.height), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }

        GUIStyle labelStyle = new GUIStyle(customStyle);
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = Mathf.CeilToInt(20 * scale_y);
        labelStyle.normal.textColor = Color.white;
        GUI.Label(gradientBoostLabelRect, "BOOST", labelStyle);
    }


    float GetGradientDirectionAngle()
    {
        switch (gradientFillDirection)
        {
            case GradientFillDirection.RightToLeft:
                return 180f + gradientRotationDegrees;
            case GradientFillDirection.TopToBottom:
                return 90f + gradientRotationDegrees;
            case GradientFillDirection.BottomToTop:
                return -90f + gradientRotationDegrees;
            case GradientFillDirection.LeftToRight:
            default:
                return gradientRotationDegrees;
        }
    }


    void DrawGradientDirectionIndicator(Rect rect)
    {
        if (gradientFillDirection == GradientFillDirection.Radial)
        {
            GUIStyle radialStyle = new GUIStyle(customStyle);
            radialStyle.alignment = TextAnchor.MiddleCenter;
            radialStyle.fontSize = Mathf.CeilToInt(34 * scale_y);
            radialStyle.normal.textColor = new Color(1f, 1f, 1f, 0.95f);
            GUI.Label(rect, "◎", radialStyle);
            return;
        }

        GUIStyle arrowStyle = new GUIStyle(customStyle);
        arrowStyle.alignment = TextAnchor.MiddleCenter;
        arrowStyle.fontSize = Mathf.CeilToInt(34 * scale_y);
        arrowStyle.normal.textColor = new Color(1f, 1f, 1f, 0.95f);

        Matrix4x4 oldMatrix = GUI.matrix;
        Vector2 pivot = new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);
        GUIUtility.RotateAroundPivot(GetGradientDirectionAngle(), pivot);
        GUI.Label(rect, "→", arrowStyle);
        GUI.matrix = oldMatrix;
    }


	void OnGUI()
	{
        RefreshRuntimePaletteLayout();
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
		if ((showSwatchPalette || showGradientPalette) && colorSelected && !whiteSwatchSelected && (!showGradientPalette || gradientBandSelected)) {
            if (gradientModeActive && gradientBandSelected) {
			    GUI.DrawTexture (selRect, selectedColor, ScaleMode.StretchToFill, true);
                DrawGradientDirectionIndicator(selRect);
            }
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

    Color32[] BuildUniformColorArray(int count, Color32 color)
    {
        Color32[] colors = new Color32[count];
        for (int i = 0; i < count; i++)
            colors[i] = color;
        return colors;
    }


    Color32[] BuildGradientFillColors(List<int> pixelIndices)
    {
        Color32[] colors = new Color32[pixelIndices.Count];
        if (pixelIndices.Count == 0)
            return colors;

        int width = mainImage.width;
        int minX = width;
        int maxX = 0;
        int height = mainImage.height;
        int minY = height;
        int maxY = 0;
        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            int x = idx % width;
            int y = idx / width;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        Color32 startColor;
        Color32 endColor;
        GetVisibleGradientEndpoints(gradientStartColor, gradientEndColor, pixelIndices.Count, out startColor, out endColor);

        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            int x = idx % width;
            int y = idx / width;
            float t = EvaluateGradientT(x, y, minX, maxX, minY, maxY);
            colors[i] = Color.Lerp(startColor, endColor, t);
        }

        return colors;
    }


    void ApplyFillColors(List<int> pixelIndices, Color32[] colors)
    {
        if (pixelIndices == null || colors == null || pixelIndices.Count != colors.Length)
            return;

        Color32[] pixels = mainImage.GetPixels32();
        int pixelsLength = pixels.Length;
        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            if (idx >= 0 && idx < pixelsLength)
                pixels[idx] = colors[i];
        }
        mainImage.SetPixels32(pixels);
    }


    IEnumerator AnimateFillRegion(List<int> pixelIndices, Color32[] fromColors, Color32[] toColors)
    {
        if (pixelIndices == null || fromColors == null || toColors == null || pixelIndices.Count == 0 || pixelIndices.Count != fromColors.Length || pixelIndices.Count != toColors.Length)
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
                pixels[idx] = fromColors[i];
        }
        mainImage.SetPixels32(pixels);
        mainImage.Apply();

        float duration = Mathf.Max(0.01f, smoothFillDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < pixelIndices.Count; i++)
            {
                int idx = pixelIndices[i];
                if (idx >= 0 && idx < pixelsLength)
                    pixels[idx] = Color32.Lerp(fromColors[i], toColors[i], t);
            }
            mainImage.SetPixels32(pixels);
            mainImage.Apply();
            yield return null;
        }

        for (int i = 0; i < pixelIndices.Count; i++)
        {
            int idx = pixelIndices[i];
            if (idx >= 0 && idx < pixelsLength)
                pixels[idx] = toColors[i];
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


    Stack<FillInfo> redoFillers = new Stack<FillInfo> ();
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
        public Color newColor;
        public byte[] newColorRGBs;
        public int[] pixelIndices;
        public Color32[] beforeColors;
        public Color32[] afterColors;
		public int x, y;
		public FillInfo(Color32 olderCol, Color32 newerCol, int startPosX,int startPosY, int[] changedIndices, Color32[] oldPixels, Color32[] newPixels){oldColor=olderCol;oldColorRGBs=new byte[4];oldColorRGBs[0]=olderCol.r;oldColorRGBs[1]=olderCol.g;oldColorRGBs[2]=olderCol.b;oldColorRGBs[3]=olderCol.a;newColor=newerCol;newColorRGBs=new byte[4];newColorRGBs[0]=newerCol.r;newColorRGBs[1]=newerCol.g;newColorRGBs[2]=newerCol.b;newColorRGBs[3]=newerCol.a;x=startPosX;y=startPosY;pixelIndices=changedIndices;beforeColors=oldPixels;afterColors=newPixels;}
	}

	void UndoFill()
	{//pop last operation from stack and apply operation.
		FillInfo lastFill = oldFillers.Pop ();
        redoFillers.Push(lastFill);
        if (lastFill.pixelIndices != null && lastFill.beforeColors != null && lastFill.pixelIndices.Length == lastFill.beforeColors.Length)
            ApplyFillColors(new List<int>(lastFill.pixelIndices), lastFill.beforeColors);
		mainImage.Apply ();
	}


    void RedoFill()
    {
        FillInfo redoFill = redoFillers.Pop();
        oldFillers.Push(redoFill);
        if (redoFill.pixelIndices != null && redoFill.afterColors != null && redoFill.pixelIndices.Length == redoFill.afterColors.Length)
            ApplyFillColors(new List<int>(redoFill.pixelIndices), redoFill.afterColors);
        mainImage.Apply();
    }

}
