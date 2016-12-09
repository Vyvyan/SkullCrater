using UnityEngine;
using System.Collections;
using Steamworks;

public class SpecialCrate : MonoBehaviour {

    public Rigidbody bloopRB;
    Rigidbody rb;
    GameManager gameManager;
    Light bloopLight;
    bool hasSpawnedBloop;

	// Use this for initialization
	void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        bloopLight = bloopRB.gameObject.GetComponentInChildren<Light>();
        bloopLight.intensity = 0;
        hasSpawnedBloop = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (!hasSpawnedBloop)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            bloopRB.isKinematic = false;
            bloopRB.useGravity = true;
            gameManager.DisplayEventText("A Big Blue Bully has appeared?");
            bloopLight.intensity = 2;
            SteamUserStats.SetAchievement("Bloop");
            SteamUserStats.StoreStats();
            hasSpawnedBloop = true;
        }
        
    }
}
