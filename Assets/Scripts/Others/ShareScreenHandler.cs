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
		Image.GetComponent<Image> ().sprite = Sprite.Create (inComingImg, new Rect (0, 0, (float)1200,(float)1200), new Vector2 (0.5f, 0.5f));
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
	public void OnTwitter()
	{

	}
	public void ImagePosted()
	{
		Debug.Log("Instagram Callback");
	}
	public void OnInstagram()
	{
		#if UNITY_ANDROID
		AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		androidClass = new AndroidJavaObject("com.example.instagram.MainActivity");
		androidClass.Call("callbackToUnityMethod",inComingImg.EncodeToPNG(), transform.name, "ImagePosted",unityClass.GetStatic<AndroidJavaObject>("currentActivity"));
		#endif
		#if UNITY_IOS
		InstagramShare.PostToInstagram("#tinge", inComingImg.EncodeToPNG());
		#endif
	}
    public void OnEmail()
    {
        Debug.Log("In Email");
#if UNITY_ANDROID
		ShareImage (inComingImg.EncodeToPNG(), "Check This Out!", "Image From ColoRelax", "Checkout ColoRelax! #colorelax #coloringforadults #adultcoloringbook #coloringbook #mandala");
#endif
#if UNITY_IOS
		AllShare.MultiShare(DataManager.Instance.selectedFileName,inComingImg.EncodeToPNG());
#endif
    }
	public  void ShareImage(byte[] imageData, string subject, string title, string message)
	{
		#if UNITY_ANDROID
		try
		{
			if (imageData == null || imageData.Length == 0) {
				Debug.LogError("ShareImage: empty image data.");
				return;
			}

			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");

			AndroidJavaClass bitmapFactoryClass = new AndroidJavaClass("android.graphics.BitmapFactory");
			AndroidJavaObject bitmap = bitmapFactoryClass.CallStatic<AndroidJavaObject>(
				"decodeByteArray",
				imageData,
				0,
				imageData.Length
			);

			if (bitmap == null) {
				Debug.LogError("ShareImage: failed to decode bitmap.");
				return;
			}

			AndroidJavaClass mediaStoreImagesMediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
			string insertedImageUriString = mediaStoreImagesMediaClass.CallStatic<string>(
				"insertImage",
				contentResolver,
				bitmap,
				title,
				message
			);

			if (string.IsNullOrEmpty(insertedImageUriString))
			{
				Debug.LogError("ShareImage: MediaStore insertImage failed.");
				return;
			}

			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject contentUri = uriClass.CallStatic<AndroidJavaObject>("parse", insertedImageUriString);

			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject sendIntent = new AndroidJavaObject("android.content.Intent");
			sendIntent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			sendIntent.Call<AndroidJavaObject>("setType", "image/*");
			sendIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
			sendIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), title);
			sendIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), message);
			sendIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), contentUri);
			sendIntent.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION

			AndroidJavaObject chooserIntent = intentClass.CallStatic<AndroidJavaObject>(
				"createChooser",
				sendIntent,
				"Share image"
			);
			currentActivity.Call("startActivity", chooserIntent);
		}
		catch (System.Exception e)
		{
			Debug.LogError("ShareImage failed: " + e.Message);
		}
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
