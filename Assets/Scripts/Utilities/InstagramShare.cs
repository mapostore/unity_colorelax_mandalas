using UnityEngine;
using System.Runtime.InteropServices;

public class InstagramShare
{
	static string imagePath = Application.temporaryCachePath + "/temp.png";
	
	static bool HasHandshook = false;

#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	static extern void _handshake();
#endif
	
	public static void HandShake() 
	{
#if UNITY_IOS && !UNITY_EDITOR
		_handshake();
#endif
		HasHandshook = true;
	}

#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	static extern void _postToInstagram(string message, string imagePath);
#endif
	
	public static void PostToInstagram(string message, byte[] imageByteArr)
	{
		if(!HasHandshook)
			HandShake();
		
		System.IO.File.WriteAllBytes(imagePath, imageByteArr);
#if UNITY_IOS && !UNITY_EDITOR
		_postToInstagram(message, imagePath);
#endif
	}
}
