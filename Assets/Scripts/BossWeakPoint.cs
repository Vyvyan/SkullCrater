using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour {

    public GameObject hitParticles;
    public BlackSkull blackSkull;
    AudioSource audioS;

	// Use this for initialization
	void Start ()
    {
        audioS = GetComponentInParent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (blackSkull.bossState != BlackSkull.BossState.spawning)
        {
            if (other.gameObject.name == "RocketProjectile(Clone)")
            {
                BlackSkull.health -= 10f;
                Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
                audioS.PlayOneShot(AudioManager.boss_WeakPointImpact, GameManager.SFXVolume / 200);
            }
            else if (other.gameObject.name == "Pellet")
            {
                BlackSkull.health -= .5f;
                Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
                audioS.PlayOneShot(AudioManager.boss_WeakPointImpact, GameManager.SFXVolume / 200);
            }
            else if (other.gameObject.name == "Grenade(Clone)")
            {
                BlackSkull.health -= 12f;
                Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
                audioS.PlayOneShot(AudioManager.boss_WeakPointImpact, GameManager.SFXVolume / 200);
            }
            else if (other.gameObject.tag == "Bullet")
            {
                if (other.gameObject.name == "mgBullet")
                {
                    BlackSkull.health -= .6f;
                    Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
                    audioS.PlayOneShot(AudioManager.boss_WeakPointImpact, GameManager.SFXVolume / 200);
                }
                else if (other.gameObject.name == "pistolBullet")
                {
                    BlackSkull.health -= 1.2f;
                    Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
                    audioS.PlayOneShot(AudioManager.boss_WeakPointImpact, GameManager.SFXVolume / 200);
                }
            }
        }

        //Debug.Log(BlackSkull.health.ToString());
    }
}
