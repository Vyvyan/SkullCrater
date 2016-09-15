using UnityEngine;
using System.Collections;

public class RocketProjectile : MonoBehaviour {

    public float rocketSpeed;
    Rigidbody rb;
    AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        // we jsut make the rocket noise a louder than most sounds to balance it
        audioSource.volume = GameManager.SFXVolume / 50;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // move it forward
        rb.AddForce(transform.forward * (rocketSpeed * Time.deltaTime), ForceMode.Force);
	}
}
