using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AethernautSplash : MonoBehaviour {

    public Rigidbody boneballRB;
    public Transform skeltin;
    AudioSource audioS;
    
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(timer());
        StartCoroutine(waitThenPushBall());
        audioS = gameObject.GetComponent<AudioSource>();
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
        yield return new WaitForSeconds(2.2f);
        boneballRB.AddForce((skeltin.transform.position - boneballRB.gameObject.transform.position) * (200 * Time.deltaTime), ForceMode.VelocityChange);
        audioS.Play();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(2);
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(6.5f);
        LoadNextScene();
    }
}
