using UnityEngine;
using System.Collections;

public class KeepLightAboveObject : MonoBehaviour {

    Light light;
    public float verticleDistance;

	// Use this for initialization
	void Start ()
    {
        light = gameObject.GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        light.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + verticleDistance, gameObject.transform.position.z);
	}
}
