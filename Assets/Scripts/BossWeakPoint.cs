using UnityEngine;
using System.Collections;

public class BossWeakPoint : MonoBehaviour {

    public GameObject hitParticles;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "RocketProjectile(Clone)")
        {
            BlackSkull.health -= 5f;
            Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
        }
        else if (other.gameObject.name == "Pellet")
        {
            BlackSkull.health -= .2f;
            Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
        }
        else if (other.gameObject.name == "Grenade(Clone)")
        {
            BlackSkull.health -= 5f;
            Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
        }
        else if (other.gameObject.tag == "Bullet")
        {
            BlackSkull.health -= 1f;
            Instantiate(hitParticles, other.contacts[0].point, Quaternion.identity);
        }

        Debug.Log(BlackSkull.health.ToString());
    }
}
