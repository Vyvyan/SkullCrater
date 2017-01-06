using UnityEngine;
using System.Collections;
using Steamworks;

public class Bloop : MonoBehaviour {

    public GameObject beamEffect;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Beam")
        {
            PlayerPrefs.SetInt("BloopInShip", 1);
            Instantiate(beamEffect, gameObject.transform.position, Quaternion.identity);
            SteamUserStats.SetAchievement("Bloop2");
            SteamUserStats.StoreStats();
            Destroy(gameObject);
        }
    }
}
