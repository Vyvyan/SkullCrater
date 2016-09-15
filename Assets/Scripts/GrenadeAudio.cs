using UnityEngine;
using System.Collections;

public class GrenadeAudio : MonoBehaviour {

    AudioSource audio;

	// Use this for initialization
	void Start ()
    {
        audio = GetComponent<AudioSource>();
        audio.volume = GameManager.SFXVolume * 10;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
