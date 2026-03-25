using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class ShareScreenHandler : MonoBehaviour {

    public GoogleMobileAdsScript adsManager; // simo
	public List<GameObject> Footer;
	public GameObject Image;
	public Texture2D inComingImg;
	#if UNITY_ANDROID
	AndroidJavaObject androidClass;
	#endif
	// Use this for initialization
	void Start () {
        int rnd = Random.Range(1, 101);
        SetShareScreen();
        if ((ImagePathHolder.GetShowAds()) && (rnd <= 50))
        { // simo : show ads             
           AdManager.Instance.StartCoroutine (AdManager.Instance.ShowAd ());
           //adsManager.showInterstitialAdMob();
        }
		
	}
	public void SetShareScreen()
	{
		Debug.Log (Image.GetComponent<Image> ().sprite.texture.width);
		inComingImg=new Texture2D(1,1,TextureFormat.RGBA32,false);
		inComingImg.LoadImage (DataManager.Instance.waterMarkedImage);
		inComingImg.Apply ();
		DataManager.Instance.waterMarkedImage = null;
		Debug.Log (inComingImg.width);
//		Resources.UnloadUnusedAssets ();
		Debug.Log(Image.GetComponent<Image> ().sprite.textureRect);
		RoundedTextureUtility.ApplyRoundedCorners(inComingImg, AppInit.GetImagePreviewCornerRadiusDp());
		Image previewImage = Image.GetComponent<Image>();
		previewImage.sprite = Sprite.Create (
            inComingImg,
            new Rect (0, 0, (float)inComingImg.width, (float)inComingImg.height),
            new Vector2 (0.5f, 0.5f));
		List<string> ShareIcons = new List<string> (); 
		ShareIcons=ImagePathHolder.LoadSocialIcons ();
		foreach (GameObject g in Footer) 
		{
			Debug.Log(Footer.IndexOf(g));
			g.GetComponent<Image> ().sprite = Resources.Load<Sprite> (ShareIcons [Footer.IndexOf(g)]);
		}
			
	}
	public void OnFacebook()
	{

	}
	public void ImagePosted()
	{
		Debug.Log("Instagram Callback");
	}
	public void OnInstagram()
	{
		#if UNITY_ANDROID
        if (!EnsureLegacyStoragePermission())
        {
            Debug.Log("OnInstagram: waiting for storage permission.");
            return;
        }
        androidClass = new AndroidJavaObject("com.example.imagesave.SaveImageUnityBridgeCompat");
        androidClass.CallStatic(
            "ShareImage",
            inComingImg.EncodeToPNG(),
            "Check This Out!",
            "Image From ColoRelax",
            "Checkout ColoRelax! #colorelax #coloringforadults #adultcoloringbook #coloringbook #mandala"
        );
		#endif
		#if UNITY_IOS
		InstagramShare.PostToInstagram("#tinge", inComingImg.EncodeToPNG());
		#endif
	}

    bool EnsureLegacyStoragePermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = versionClass.GetStatic<int>("SDK_INT");
        if (sdkInt < 29)
        {
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
            {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
                return false;
            }
        }
#endif
        return true;
    }

    public void ShareImage(byte[] imageData, string subject, string title, string message)
    {
#if UNITY_ANDROID
        if (!EnsureLegacyStoragePermission())
        {
            Debug.Log("ShareImage: waiting for storage permission.");
            return;
        }
        androidClass = new AndroidJavaObject("com.example.imagesave.SaveImageUnityBridgeCompat");
        androidClass.CallStatic("ShareImage", imageData, subject, title, message);
#endif
    }

	public void OnSaveToGallery()
	{
		Debug.Log (DataManager.Instance.selectedFileName);
//		DataManager.Instance.StoreWatermark (inComingImg.EncodeToPNG(), DataManager.Instance.selectedFileName);
		#if UNITY_IPHONE&&!UNITY_EDITOR
		SaveImage.SaveToGallery(DataManager.Instance.selectedFileName,inComingImg.EncodeToPNG());
		#endif
		#if UNITY_ANDROID
        if (!EnsureLegacyStoragePermission())
        {
            Debug.Log("OnSaveToGallery: waiting for storage permission.");
            return;
        }
		androidClass = new AndroidJavaObject("com.example.imagesave.SaveImageUnityBridgeCompat");
		androidClass.CallStatic("CallSaveImage",inComingImg.EncodeToPNG());
		#endif
	}


	public void OnBack()
	{
		DataManager.Instance.fromDrawings = true;
		Application.LoadLevel ("NewGamePlay");
	}
}
