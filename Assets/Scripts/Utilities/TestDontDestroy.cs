using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestDontDestroy : MonoBehaviour {

	private static TestDontDestroy instanceRef;
	// Use this for initialization
	void Start () {

	}
	
	void Awake()
	{

			 if (instanceRef==null)
		{
			instanceRef=this;
			DontDestroyOnLoad(this.gameObject);

		}
		
		else
		{
			DestroyImmediate(this.gameObject);
		}
	}
	void Update()
	{
		if(Input.GetKey(KeyCode.A))
		   {
			Debug.Log(SceneManager.GetActiveScene().name);
			if(SceneManager.GetActiveScene().buildIndex>0)
				SceneManager.LoadScene("Test1");
			else
				SceneManager.LoadScene("Test2");
		}
		  
	}
}
