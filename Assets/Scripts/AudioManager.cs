using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip pistolFire_Pub, shotgunFire_Pub, machinegunFire_Pub, rocketFire_Pub, playerDeath_Pub, accept_Pub, decline_Pub,  gold_Pickup_Pub, gold_DropOff_Pub;
    public static AudioClip pistolFire, shotgunFire, machinegunFire, rocketFire, playerDeath, accept, decline, gold_Pickup, gold_DropOff;

    AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        // set the static clips from the public ones
        pistolFire = pistolFire_Pub;
        shotgunFire = shotgunFire_Pub;
        machinegunFire = machinegunFire_Pub;
        rocketFire = rocketFire_Pub;
        playerDeath = playerDeath_Pub;
        accept = accept_Pub;
        decline = decline_Pub;
        gold_Pickup = gold_Pickup_Pub;
        gold_DropOff = gold_DropOff_Pub;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void Play2DSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, GameManager.SFXVolume / 100);
    }
}
