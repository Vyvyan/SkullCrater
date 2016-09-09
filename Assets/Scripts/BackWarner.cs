using UnityEngine;
using System.Collections;

public class BackWarner : MonoBehaviour {

    public Light warningLight;
    public float lightTimer;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;
            warningLight.intensity = 6;
        }
        else
        {
            warningLight.intensity = 0;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            lightTimer = 1;
        }
    }
}
