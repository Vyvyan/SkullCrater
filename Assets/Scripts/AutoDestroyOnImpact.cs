using UnityEngine;
using System.Collections;

public class AutoDestroyOnImpact : MonoBehaviour {

    bool hasStartedTimer;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (!hasStartedTimer)
        {
            StartCoroutine(TimedDestroy());
            hasStartedTimer = true;
        }
    }

    IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(2);
        if (transform.parent)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
