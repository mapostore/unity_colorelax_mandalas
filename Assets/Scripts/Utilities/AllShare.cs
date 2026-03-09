using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AllShare {
	
	static string imagePath = Application.temporaryCachePath+"/temp.png";
	
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	static extern void openShare(string imagePath);
#endif
	
	public static void MultiShare(string imageName, byte[] imageByteArr)
	{
		
		System.IO.File.WriteAllBytes(imagePath, imageByteArr);
#if UNITY_IOS && !UNITY_EDITOR
		openShare(imagePath);
#endif
	}
}
