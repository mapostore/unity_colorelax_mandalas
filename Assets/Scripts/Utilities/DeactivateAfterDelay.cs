using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// TODO : may be not used
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
