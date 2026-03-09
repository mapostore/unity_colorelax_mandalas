using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class SaveImage {

    static string imagePath = Application.temporaryCachePath + "/temp.png";

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void saveToGallery(string message, string imagePath);
#endif

    public static void SaveToGallery(string imageName, byte[] imageByteArr) {

        System.IO.File.WriteAllBytes(imagePath, imageByteArr);
#if UNITY_IOS && !UNITY_EDITOR
        saveToGallery(imageName, imagePath);
#endif
    }
}
