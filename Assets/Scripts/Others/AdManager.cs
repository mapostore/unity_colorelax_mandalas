using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements; // Using the Unity Ads namespace.




public class AdManager : MonoBehaviour{
	


    /* simo
	#if !UNITY_ADS // If the Ads service is not enabled...
	public string gameId; // Set this value from the inspector.
	public bool enableTestMode = true;
	#endif
    */

    #if UNITY_IOS
    private string gameId = "1757958";
    #elif UNITY_ANDROID
    private string gameId = "1757957";
    #endif

	public static AdManager myInstance;
	public static AdManager Instance{
		get{
			if(myInstance  ==  null)
				myInstance = FindObjectOfType(typeof(AdManager)) as AdManager;
			return myInstance;
		}
	

	}



	void Awake(){
		
		if (myInstance  ==  null){
			myInstance = this;
			DontDestroyOnLoad(this.gameObject);
			
		}
	

		
		else{
			DestroyImmediate(this.gameObject);
		}

	}


	public IEnumerator ShowAd (){


		//#if !UNITY_ADS // If the Ads service is not enabled...
		if (Advertisement.isSupported) { // If runtime platform is supported...
			Advertisement.Initialize(gameId, false); // ...initialize.
		}
	

		//#endif
		
		// Wait until Unity Ads is initialized,
		//  and the default ad placement is ready.
        while (!Advertisement.isInitialized || !Advertisement.IsReady()){
	


			yield return new WaitForSeconds(0.5f);
		}
	

		
		// Show the default ad placement.
		Advertisement.Show();

        /*
        // simo 2018 : dummy code for make it works in some way :
        List<string> list = new List<string>();
        list.Add("dummy values1");
        list.Add("dummy values2");
        list.Add("dummy values3");
        return list.GetEnumerator();
        */
	}


}