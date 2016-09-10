using UnityEngine;
using System.Collections;

public class SpecialCrate : MonoBehaviour {

    public Rigidbody bloopRB;
    Rigidbody rb;


	// Use this for initialization
	void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        bloopRB.isKinematic = false;
        bloopRB.useGravity = true;
    }
}
