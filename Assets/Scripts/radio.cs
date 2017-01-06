using UnityEngine;
using System.Collections;

public class radio : MonoBehaviour {

    AudioSource audioS;

	// Use this for initialization
	void Start ()
    {
        audioS = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (GameManager.gameState == GameManager.GameState.PreGame)
        {
            audioS.volume = GameManager.MusicVolume / 225;
        }
	}
}
