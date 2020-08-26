using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


/*
Deactivate after a bit the splash screen, call Start scene for gdpr checking & c.
*/
public class DeactivateAfterDelay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Deactivate",2.0f);
	}
	

	void Deactivate(){
        //SceneManager.LoadScene ("Gallery");
        SceneManager.LoadScene("Start");
	}
}
