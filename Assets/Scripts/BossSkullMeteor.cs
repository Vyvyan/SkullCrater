using UnityEngine;
using System.Collections;

public class BossSkullMeteor : MonoBehaviour {

    GameObject player;
    Rigidbody rb;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        if (player)
        {
            transform.LookAt(player.transform.position);
        }
        rb.AddForce(transform.forward * 100, ForceMode.VelocityChange);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
