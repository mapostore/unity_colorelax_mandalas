using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class MyDrawingController : MonoBehaviour {
    static Sprite backChevronSprite;

    public GoogleMobileAdsScript adsManager; // simo
	public GameObject waterMarkPrefab,Header,WaterMarkPanel;
	public List<GameObject> ButtText,Buttons;
	// Use this for initialization
	void Start () {
		SetMyDrawingsScreen ();

	}
	void SetMyDrawingsScreen()
	{
		List<string> HomeButtNames = new List<string> ();
		List<string> HomeButtImages = new List<string> ();
		List<string> HomeButtColors = new List<string> ();
		HomeButtImages = ImagePathHolder.LoadHomeButtImages ();
		HomeButtNames = ImagePathHolder.LoadHomeButtNames ();
		HomeButtColors = ImagePathHolder.LoadHomeButtColors ();
		Header.GetComponent<Text> ().text = HomeButtNames [2];
		foreach (GameObject g in ButtText)
			g.GetComponent<Text> ().text = HomeButtNames [ButtText.IndexOf (g)];
		foreach (GameObject g in Buttons)
			g.GetComponent<Image> ().sprite = Resources.Load<Sprite> (HomeButtImages [Buttons.IndexOf (g)]);
        ApplyBackButtonSprite();
		GenerateWaterMarks ();
	}

    void ApplyBackButtonSprite()
    {
        if (Buttons == null || Buttons.Count == 0 || Buttons[0] == null)
            return;

        if (backChevronSprite == null)
            backChevronSprite = CreateBackChevronSprite();

        Image buttonImage = Buttons[0].GetComponent<Image>();
        if (buttonImage != null && backChevronSprite != null)
            buttonImage.sprite = backChevronSprite;
    }

    static Sprite CreateBackChevronSprite()
    {
        Texture2D icon = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        icon.wrapMode = TextureWrapMode.Clamp;
        icon.filterMode = FilterMode.Bilinear;

        Color clear = new Color(1f, 1f, 1f, 0f);
        Color stroke = Color.white;
        for (int y = 0; y < icon.height; y++)
            for (int x = 0; x < icon.width; x++)
                icon.SetPixel(x, y, clear);

        for (int y = 0; y < icon.height; y++)
        {
            for (int x = 0; x < icon.width; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float d1 = DistanceToSegment(p, new Vector2(41f, 12f), new Vector2(21f, 32f));
                float d2 = DistanceToSegment(p, new Vector2(21f, 32f), new Vector2(41f, 52f));
                if (Mathf.Min(d1, d2) <= 4.5f)
                    icon.SetPixel(x, y, stroke);
            }
        }

        icon.Apply();
        return Sprite.Create(icon, new Rect(0f, 0f, icon.width, icon.height), new Vector2(0.5f, 0.5f), 100f);
    }

    static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float abSqr = ab.sqrMagnitude;
        if (abSqr <= Mathf.Epsilon)
            return Vector2.Distance(p, a);

        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / abSqr);
        Vector2 projection = a + ab * t;
        return Vector2.Distance(p, projection);
    }
	void GenerateWaterMarks()
	{
		string[] watermarkedImages = PlayerPrefsX.GetStringArray ("WatermarkedImages");
		List<ImagePath> editedsubImages = new List<ImagePath> ();
		editedsubImages = ImagePathHolder.LoadSubImagePathFromAsset ();
		
		for (int i=0; i<watermarkedImages.Length; i++) 
		{
			
			if(!watermarkedImages[i].Contains("NULL"))
			{
				GameObject g=Instantiate(waterMarkPrefab) as GameObject;
				g.SetActive(true);
				g.transform.SetAsFirstSibling();
				g.transform.SetParent(WaterMarkPanel.transform);

				g.GetComponent<DataHolder>().fileName=editedsubImages[i].imagePath;

				g.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>OnSubCategory(g));
				g.GetComponent<SetWaterMark>().SetWater(watermarkedImages[i]);
				g.GetComponent<RectTransform>().localScale=new Vector3(1f,1f,1f);
			}
		}
		if (WaterMarkPanel.transform.childCount <= 2) 
		{
			for(int i=0;i<2;i++)
			{
				GameObject g=Instantiate(waterMarkPrefab) as GameObject;
				g.SetActive(true);
				g.transform.SetParent(WaterMarkPanel.transform);
				g.GetComponent<RectTransform>().localScale=new Vector3(1f,1f,1f);
				g.GetComponent<UnityEngine.UI.Image>().enabled=false;
			}
		}
	}
	public void OnGalleryHit()
	{

		// simo : backing home : show unity ads
		int rnd = Random.Range(1, 101);
		if (rnd <= 50) {
			AdManager.Instance.StartCoroutine(AdManager.Instance.ShowAd());
            // adsManager.showInterstitialAdMob();
		}
		DataManager.Instance.LoadScene ("Gallery", 0.25f);
//		AutoFade.LoadLevel ("Gallery", 0.5f, 0.5f, Color.white);
	} 
	public void OnSubCategory(GameObject g)
	{
		Debug.Log (g.name);
		DataManager.Instance.fromDrawings = true;
		Debug.Log("In MyDrawings");

		DataManager.Instance.selectedFileName = g.GetComponent<DataHolder> ().fileName;
//		Application.LoadLevel("NewGamePlay");
//		AutoFade.LoadLevel ("NewGamePlay", 0.5f, 0.5f, Color.white);
		DataManager.Instance.LoadScene ("NewGamePlay", 0.25f);
	}
	public void OnInspirationHit()
	{
//		Application.LoadLevel("Inspiration");
//		AutoFade.LoadLevel ("Inspiration", 0.5f, 0.5f, Color.white);
		DataManager.Instance.LoadScene ("Inspiration", 0.25f);
	}
}
