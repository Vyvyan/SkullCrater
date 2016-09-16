using UnityEngine;
using System.Collections;

public class BulletAudio : MonoBehaviour {

    AudioSource audioS;
    bool hasMadeASound = false;

	// Use this for initialization
	void Start ()
    {
        audioS = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (!hasMadeASound)
        {
            if (other.gameObject.tag == "Wood")
            {
                audioS.PlayOneShot(AudioManager.bullet_Wood, GameManager.SFXVolume / 200);
            }
            else if (other.gameObject.tag == "Metal")
            {
                audioS.PlayOneShot(AudioManager.bullet_Metal, GameManager.SFXVolume / 150);
            }
            else if (other.gameObject.tag == "Ground")
            {
                audioS.PlayOneShot(AudioManager.bullet_Crater, GameManager.SFXVolume / 200);
            }
            else
            {
                // if not, we hit bone, so randomize the bone
                int rnd = Random.Range(1, 4);
                if (rnd == 1)
                {
                    audioS.PlayOneShot(AudioManager.bullet_Bone1, GameManager.SFXVolume / 400);
                }
                else if (rnd == 2)
                {
                    audioS.PlayOneShot(AudioManager.bullet_Bone2, GameManager.SFXVolume / 400);
                }
                else if (rnd == 3)
                {
                    audioS.PlayOneShot(AudioManager.bullet_Bone3, GameManager.SFXVolume / 400);
                }
            }

            hasMadeASound = true;
        }
    }
}
