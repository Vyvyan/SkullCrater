using UnityEngine;
using System.Collections;

public class GrenadeAudio : MonoBehaviour {

    AudioSource audio;
    public float quietMultiplier;

	// Use this for initialization
	void Start ()
    {
        if (quietMultiplier < 1)
        {
            quietMultiplier = 1;
        }
        audio = GetComponent<AudioSource>();
        audio.volume = GameManager.SFXVolume / (100 * quietMultiplier);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
