using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(waitThenLoad());
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    IEnumerator waitThenLoad()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(1);
    }
}
