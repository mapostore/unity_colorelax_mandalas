using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataManager : MonoBehaviour {
    const int GalleryThumbSize = 256;
    const float DefaultSceneFadeDuration = 0.25f;
    bool isObjectFound = false;
    public bool fromDrawings;
    public int watermarkStatus;
    public byte[] waterMarkedImage;
    public string selectedFileName, selectedThumb, selectedResourceName;
    public string[] storedWaterMarks;
    TextWriter tW;
    Canvas sceneFadeCanvas;
    Image sceneFadeImage;
    bool isSceneTransitioning;

    public static DataManager myInstance;
    public static DataManager Instance {
        get {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(DataManager)) as DataManager;
            return myInstance;
        }
    }


    void Awake() {

        if (myInstance == null) {
            myInstance = this;
            DontDestroyOnLoad(this.gameObject);

        } else {
            DestroyImmediate(this.gameObject);
        }
    }


    void Start() {
        Debug.Log("====================== DataManager Start ======================");

        Debug.Log(ImagePathHolder.LoadSubImageResourcePathFromAsset().Count);
        //		Debug.Log(PlayerPrefs.HasKey("SavedWaterMark"));
        if (PlayerPrefs.HasKey("SavedWaterMark"))
            storedWaterMarks = PlayerPrefsX.GetStringArray("WatermarkedImages");
        else {
            PlayerPrefs.SetInt("SavedWaterMark", 1);
            string[] waterMarkArr = new string[ImagePathHolder.LoadSubImageResourcePathFromAsset().Count];
            for (int i = 0; i < waterMarkArr.Length; i++)
                waterMarkArr[i] = "NULL";
            PlayerPrefsX.SetStringArray("WatermarkedImages", waterMarkArr);
            storedWaterMarks = PlayerPrefsX.GetStringArray("WatermarkedImages");
        }
        //		storedWaterMarks = PlayerPrefsX.GetStringArray ("WatermarkedImages", "NULL", ImagePathHolder.LoadSubImageResourcePathFromAsset().Count);
        //		if (PlayerPrefs.HasKey ("SavedWaterMark"))
        //			watermarkStatus = PlayerPrefs.GetInt ("SavedWaterMark");
        //		else
        //		{
        //			PlayerPrefs.SetInt("SavedWaterMark",-1);
        //			watermarkStatus = PlayerPrefs.GetInt ("SavedWaterMark");
        //		}
    }


    void StoreSavedFile(string latestFile) {
        Debug.Log(GetIndexFromImagePath());
        storedWaterMarks[GetIndexFromImagePath()] = latestFile;
        PlayerPrefsX.SetStringArray("WatermarkedImages", storedWaterMarks);
    }


    int GetIndexFromImagePath() {
        List<ImagePath> allEditedImgPaths = new List<ImagePath>();
        allEditedImgPaths = ImagePathHolder.LoadSubImagePathFromAsset();
        foreach (ImagePath i in allEditedImgPaths)
            if (i.imagePath == DataManager.Instance.selectedFileName)
                return allEditedImgPaths.IndexOf(i);
        return 0;
    }


    public void StoreWatermark(byte[] fileData, string fileName) {
        fileName += "Watermark";
        string filePath;
        filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        FileStream fs;
        fs = File.Create(filePath);
        fs.Write(fileData, 0, fileData.Length);
        fs.Close();
        StoreSavedFile(fileName);
    }


    public void FileCreatorBytes(byte[] fileData, string fileName) {
        //Create a file of specificed file-name and save byte array to it.
        string filePath;
        #if UNITY_ANDROID
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        #elif UNITY_IPHONE && !UNITY_EDITOR
        		//		string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        		//		filePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #elif UNITY_EDITOR
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #endif
        //		BinaryWriter bw;
        //		bw=File.WriteAllBytes (filePath, fileData);
        //		bw.Close ();
        FileStream fs;
        fs = File.Create(filePath);
        fs.Write(fileData, 0, fileData.Length);
        fs.Close();
    }


    public string OriginalStateFileKey(string baseFileName) {
        return baseFileName + "__orig";
    }


    public string WorkingStateFileKey(string baseFileName) {
        return baseFileName + "__work";
    }


    public string ThumbnailStateFileKey(string baseFileName) {
        return baseFileName + "__thumb";
    }


    public string ProgressStateKey(string baseFileName) {
        return "progress__" + baseFileName;
    }


    public string ThumbnailVersionKey(string baseFileName) {
        return "thumbver__" + baseFileName;
    }


    public void FileCopier(string fileName, string copiedFileName) {
        string filePath, copiedFilePath;
        #if UNITY_ANDROID
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
                copiedFilePath = Application.persistentDataPath + "/" + copiedFileName + ".txt";
        #elif UNITY_IPHONE && !UNITY_EDITOR
        		copiedFilePath=Application.persistentDataPath + "/"+copiedFileName + ".txt";
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #elif UNITY_EDITOR
        		copiedFilePath=Application.persistentDataPath + "/"+copiedFileName + ".txt";
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #endif
        System.IO.File.Copy(filePath, copiedFilePath, true);
    }



    public byte[] FileReaderBytes(string fileName) {
        //read byte array of colors from specified file-path and return.
        byte[] imageColors = null;
        string filePath;
        #if UNITY_ANDROID
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        
        #elif UNITY_IPHONE && !UNITY_EDITOR
        		//	string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        		//		filePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #elif UNITY_EDITOR
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #endif
        if (System.IO.File.Exists(filePath))
            imageColors = System.IO.File.ReadAllBytes(filePath);
        return imageColors;

    }


    bool IsValidImageBytes(byte[] bytes) {
        if (bytes == null || bytes.Length == 0)
            return false;
        Texture2D probe = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool ok = probe.LoadImage(bytes, false);
        Destroy(probe);
        return ok;
    }


    bool BytesEqual(byte[] a, byte[] b) {
        if (a == null || b == null || a.Length != b.Length)
            return false;
        for (int i = 0; i < a.Length; i++) {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }


    byte[] BuildThumbFromBytes(byte[] sourceBytes) {
        if (sourceBytes == null || sourceBytes.Length == 0)
            return null;
        Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!source.LoadImage(sourceBytes, false)) {
            Destroy(source);
            return null;
        }
        TextureScale.Bilinear(source, GalleryThumbSize, GalleryThumbSize);
        byte[] thumbBytes = source.EncodeToPNG();
        Destroy(source);
        return thumbBytes;
    }


    byte[] LoadResourceImageBytes(string resourcePath) {
        if (string.IsNullOrEmpty(resourcePath))
            return null;
        Texture2D resourceTexture = Resources.Load<Texture2D>(resourcePath);
        if (resourceTexture == null)
            return null;
        return resourceTexture.EncodeToPNG();
    }


    public bool EnsureImageStateFiles(string baseFileName, string resourcePath) {
        if (string.IsNullOrEmpty(baseFileName))
            return false;

        string originalKey = OriginalStateFileKey(baseFileName);
        string workingKey = WorkingStateFileKey(baseFileName);
        string thumbKey = ThumbnailStateFileKey(baseFileName);

        byte[] originalBytes = FileReaderBytes(originalKey);
        byte[] workingBytes = FileReaderBytes(workingKey);
        byte[] thumbBytes = FileReaderBytes(thumbKey);
        byte[] legacyBytes = FileReaderBytes(baseFileName);

        bool originalValid = IsValidImageBytes(originalBytes);
        bool workingValid = IsValidImageBytes(workingBytes);
        bool thumbValid = IsValidImageBytes(thumbBytes);
        bool legacyValid = IsValidImageBytes(legacyBytes);

        if (!originalValid) {
            originalBytes = LoadResourceImageBytes(resourcePath);
            originalValid = IsValidImageBytes(originalBytes);

            // Fallback only when resource cannot be resolved.
            if (!originalValid && legacyValid) {
                originalBytes = legacyBytes;
                originalValid = true;
            }

            if (originalValid)
                FileCreatorBytes(originalBytes, originalKey);
        }

        if (!workingValid) {
            if (legacyValid) {
                workingBytes = legacyBytes;
                workingValid = true;
            } else if (originalValid) {
                workingBytes = originalBytes;
                workingValid = true;
            }

            if (workingValid)
                FileCreatorBytes(workingBytes, workingKey);
        }

        if (!thumbValid) {
            byte[] thumbSource = workingValid ? workingBytes : originalBytes;
            byte[] rebuiltThumb = BuildThumbFromBytes(thumbSource);
            if (rebuiltThumb != null && rebuiltThumb.Length > 0) {
                thumbBytes = rebuiltThumb;
                thumbValid = true;
                FileCreatorBytes(thumbBytes, thumbKey);
                IncrementThumbnailVersion(baseFileName);
            }
        }

        if (workingValid && !legacyValid) {
            FileCreatorBytes(workingBytes, baseFileName);
        }

        if (workingValid && legacyValid && !BytesEqual(workingBytes, legacyBytes)) {
            FileCreatorBytes(workingBytes, baseFileName);
        }

        return originalValid && workingValid;
    }


    public byte[] ReadOriginalImageBytes(string baseFileName, string resourcePath) {
        if (!EnsureImageStateFiles(baseFileName, resourcePath))
            return null;
        return FileReaderBytes(OriginalStateFileKey(baseFileName));
    }


    public byte[] ReadWorkingImageBytes(string baseFileName, string resourcePath) {
        if (!EnsureImageStateFiles(baseFileName, resourcePath))
            return null;
        return FileReaderBytes(WorkingStateFileKey(baseFileName));
    }


    public byte[] ReadThumbnailImageBytes(string baseFileName, string resourcePath) {
        if (!EnsureImageStateFiles(baseFileName, resourcePath))
            return null;
        return FileReaderBytes(ThumbnailStateFileKey(baseFileName));
    }


    public void WriteWorkingImageBytes(string baseFileName, byte[] pngBytes, string resourcePath) {
        if (string.IsNullOrEmpty(baseFileName) || pngBytes == null || pngBytes.Length == 0)
            return;

        EnsureImageStateFiles(baseFileName, resourcePath);

        FileCreatorBytes(pngBytes, WorkingStateFileKey(baseFileName));
        FileCreatorBytes(pngBytes, baseFileName);

        byte[] thumbBytes = BuildThumbFromBytes(pngBytes);
        if (thumbBytes != null && thumbBytes.Length > 0) {
            FileCreatorBytes(thumbBytes, ThumbnailStateFileKey(baseFileName));
            IncrementThumbnailVersion(baseFileName);
        }
    }


    public void ResetWorkingToOriginal(string baseFileName, string resourcePath) {
        byte[] originalBytes = ReadOriginalImageBytes(baseFileName, resourcePath);
        if (!IsValidImageBytes(originalBytes))
            return;

        FileCreatorBytes(originalBytes, WorkingStateFileKey(baseFileName));
        FileCreatorBytes(originalBytes, baseFileName);

        byte[] thumbBytes = BuildThumbFromBytes(originalBytes);
        if (thumbBytes != null && thumbBytes.Length > 0) {
            FileCreatorBytes(thumbBytes, ThumbnailStateFileKey(baseFileName));
            IncrementThumbnailVersion(baseFileName);
        }
    }


    public void MarkProgress(string baseFileName, bool started) {
        if (string.IsNullOrEmpty(baseFileName))
            return;
        PlayerPrefs.SetInt(ProgressStateKey(baseFileName), started ? 1 : 0);
    }


    public bool HasStartedProgress(string baseFileName) {
        if (string.IsNullOrEmpty(baseFileName))
            return false;
        string key = ProgressStateKey(baseFileName);
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetInt(key) > 0;
        return PlayerPrefs.GetInt(baseFileName, 0) > 0;
    }


    public int GetThumbnailVersion(string baseFileName) {
        if (string.IsNullOrEmpty(baseFileName))
            return 0;
        return PlayerPrefs.GetInt(ThumbnailVersionKey(baseFileName), 0);
    }


    void IncrementThumbnailVersion(string baseFileName) {
        if (string.IsNullOrEmpty(baseFileName))
            return;
        int nextValue = GetThumbnailVersion(baseFileName) + 1;
        PlayerPrefs.SetInt(ThumbnailVersionKey(baseFileName), nextValue);
    }



    public void LoadScene(string level) {
        StartSceneTransition(level, DefaultSceneFadeDuration);
    }


    public void LoadScene(string level, float duration) {
        StartSceneTransition(level, duration);
    }


    void StartSceneTransition(string level, float duration) {
        if (isSceneTransitioning || string.IsNullOrEmpty(level))
            return;
        StartCoroutine(LoadSceneWithFade(level, Mathf.Max(0.01f, duration)));
    }


    IEnumerator LoadSceneWithFade(string level, float duration) {
        isSceneTransitioning = true;
        EnsureSceneFadeOverlay();

        yield return FadeOverlay(0f, 1f, duration);
        SceneManager.LoadScene(level);
        yield return null;
        yield return FadeOverlay(1f, 0f, duration);

        isSceneTransitioning = false;
    }


    void EnsureSceneFadeOverlay() {
        if (sceneFadeCanvas != null && sceneFadeImage != null)
            return;

        GameObject canvasGo = new GameObject("GlobalSceneFader");
        canvasGo.transform.SetParent(transform, false);
        sceneFadeCanvas = canvasGo.AddComponent<Canvas>();
        sceneFadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        sceneFadeCanvas.sortingOrder = 5000;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject imageGo = new GameObject("FadeImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        sceneFadeImage = imageGo.AddComponent<Image>();
        sceneFadeImage.color = new Color(0f, 0f, 0f, 0f);
        sceneFadeImage.raycastTarget = false;

        RectTransform rect = sceneFadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }


    IEnumerator FadeOverlay(float from, float to, float duration) {
        if (sceneFadeImage == null)
            yield break;

        float elapsed = 0f;
        Color color = sceneFadeImage.color;
        color.a = from;
        sceneFadeImage.color = color;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(from, to, t);
            sceneFadeImage.color = color;
            yield return null;
        }

        color.a = to;
        sceneFadeImage.color = color;
    }

}
