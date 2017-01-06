using UnityEngine;
using System.Collections;
using Steamworks;

public class Bloop : MonoBehaviour {

    public GameObject beamEffect;
    AudioManager audioMan;
    GameManager gameManager;

	// Use this for initialization
	void Start ()
    {
        audioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
            audioMan.Play2DSound(audioMan.bloopStun_Pub);
            gameManager.DisplayEventText("Deposited *New Entity* Bloop");
            SteamUserStats.SetAchievement("Bloop2");
            SteamUserStats.StoreStats();
            Destroy(gameObject);
        }
    }
}
