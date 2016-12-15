using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AethernautSplash : MonoBehaviour {

    public Rigidbody boneballRB;
    public Transform skeltin;
    
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(timer());
        StartCoroutine(waitThenPushBall());
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            LoadNextScene();
        }
	}

    IEnumerator waitThenPushBall()
    {
        yield return new WaitForSeconds(2.5f);
        boneballRB.AddForce((skeltin.transform.position - boneballRB.gameObject.transform.position) * (200 * Time.deltaTime), ForceMode.VelocityChange);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(2);
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(6);
        LoadNextScene();
    }
}
