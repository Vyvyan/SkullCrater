using UnityEngine;
using System.Collections;

public class RocketProjectile : MonoBehaviour {

    public float rocketSpeed;
    Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // move it forward
        rb.AddForce(transform.forward * (rocketSpeed * Time.deltaTime), ForceMode.Force);
	}
}
