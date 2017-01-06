using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip pistolFire_Pub, shotgunFire_Pub, machinegunFire_Pub, rocketFire_Pub, playerDeath_Pub, accept_Pub, decline_Pub,  gold_Pickup_Pub, gold_DropOff_Pub, anomalousSkull_Pub, reload_Pistol_Pub, reload_MG_Pub, reload_Shotgun_Pub,
        reload_Rocket_Pub, skel_Footstep_Pub, skel_Footstep_Pub2, boneball_Hit_Pub, boneball_Death_Pub, bullet_Bone1_Pub, bullet_Bone2_Pub, bullet_Bone3_Pub, bullet_Wood_Pub, bullet_Metal_Pub, bullet_Crater_Pub, boss_MeteorCollide_Pub,
        boss_WeakPointImpact_Pub, boss_Explode_Pub, outOfAmmo_Pub, bloop_Pub, fallingMusicIntro_Pub, bloopStun_Pub;

    public static AudioClip pistolFire, shotgunFire, machinegunFire, rocketFire, playerDeath, accept, decline, gold_Pickup, gold_DropOff, anomalousSkull, reload_Pistol, reload_MG, reload_Shotgun, reload_Rocket, skel_Footstep, skel_Footstep2,
        boneball_Hit, boneball_Death, bullet_Bone1, bullet_Bone2, bullet_Bone3, bullet_Wood, bullet_Metal, bullet_Crater, boss_MeteorCollide, boss_WeakPointImpact, boss_Explode, outOfAmmo, bloop, fallingMusicIntro, bloopStun;

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
        anomalousSkull = anomalousSkull_Pub;
        reload_Pistol = reload_Pistol_Pub;
        reload_MG = reload_MG_Pub;
        reload_Shotgun = reload_Shotgun_Pub;
        reload_Rocket = reload_Rocket_Pub;
        skel_Footstep = skel_Footstep_Pub;
        boneball_Death = boneball_Death_Pub;
        boneball_Hit = boneball_Hit_Pub;
        bullet_Bone1 = bullet_Bone1_Pub;
        bullet_Bone2 = bullet_Bone2_Pub;
        bullet_Bone3 = bullet_Bone3_Pub;
        bullet_Crater = bullet_Crater_Pub;
        bullet_Metal = bullet_Metal_Pub;
        bullet_Wood = bullet_Wood_Pub;
        boss_MeteorCollide = boss_MeteorCollide_Pub;
        boss_WeakPointImpact = boss_WeakPointImpact_Pub;
        boss_Explode = boss_Explode_Pub;
        outOfAmmo = outOfAmmo_Pub;
        bloop = bloop_Pub;
        fallingMusicIntro = fallingMusicIntro_Pub;
        bloopStun = bloopStun_Pub;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void Play2DSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, GameManager.SFXVolume / 100);
    }

    public void Play2DSoundLoud(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, GameManager.SFXVolume / 30);
    }

    public void Play2DSoundMusicClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, GameManager.MusicVolume / 60);
    }
}
