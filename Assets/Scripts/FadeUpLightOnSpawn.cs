using UnityEngine;
using System.Collections;

public class FadeUpLightOnSpawn : MonoBehaviour {

    public Light light;
    public float maxIntensity;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(light.intensity < maxIntensity)
        {
            light.intensity += Time.deltaTime;
        }
        else
        {
            Destroy(this);
        }
	}
}
