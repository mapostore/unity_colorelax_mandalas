using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircularLoader : MonoBehaviour {

    public float speed;
    public bool isLoading = false;
    Canvas loaderCanvas;
    RectTransform loaderRect;
    Image loaderImage;
    SpriteRenderer spriteRenderer;
    // Use this for initialization
    public static CircularLoader myInstance;
    public static CircularLoader Instance {
        get {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(CircularLoader)) as CircularLoader;
            return myInstance;
        }
    }


    void Awake() {
        Debug.Log("====================== CircularLoader Awake ======================");

        if (myInstance == null) {
            myInstance = this;
            DontDestroyOnLoad(this.gameObject);
            EnsureUiLoader();

        } else {
            DestroyImmediate(this.gameObject);
        }
    }


    public void Loader() {
        isLoading = true;
        EnsureUiLoader();
        PositionAndShowLoader();
    }


    void EnsureUiLoader() {
        if (loaderCanvas != null && loaderImage != null && loaderRect != null)
            return;

        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject canvasObject = new GameObject("CircularLoaderCanvas");
        canvasObject.transform.SetParent(transform, false);
        loaderCanvas = canvasObject.AddComponent<Canvas>();
        loaderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        loaderCanvas.sortingOrder = 6000;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("CircularLoaderImage");
        imageObject.transform.SetParent(canvasObject.transform, false);
        loaderImage = imageObject.AddComponent<Image>();
        loaderImage.raycastTarget = false;
        loaderRect = loaderImage.rectTransform;
        loaderRect.anchorMin = new Vector2(0.5f, 0.5f);
        loaderRect.anchorMax = new Vector2(0.5f, 0.5f);
        loaderRect.pivot = new Vector2(0.5f, 0.5f);
        loaderRect.anchoredPosition = Vector2.zero;

        if (spriteRenderer != null && spriteRenderer.sprite != null) {
            loaderImage.sprite = spriteRenderer.sprite;
            float size = Mathf.Max(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height);
            loaderRect.sizeDelta = new Vector2(size, size);
            spriteRenderer.enabled = false;
        } else {
            loaderRect.sizeDelta = new Vector2(128f, 128f);
        }

        loaderImage.enabled = false;
    }


    void PositionAndShowLoader() {
        if (loaderImage == null || loaderRect == null)
            return;

        loaderRect.anchoredPosition = Vector2.zero;
        loaderImage.enabled = true;
    }


    void Update() {
        if (isLoading) {
            if (loaderRect != null)
                loaderRect.Rotate(0f, 0f, -speed * Time.deltaTime);
        } else {
            if (loaderImage != null && loaderImage.enabled)
                loaderImage.enabled = false;
        }
    }
}
