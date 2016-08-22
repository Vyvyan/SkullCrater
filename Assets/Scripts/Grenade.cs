using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public GameObject explosionEffect;

    bool hasExploded;

    public float radius;
    public float power;

    // this is cause we can use this code for the shotgun pellets, so they have more impact, but no explosion effect
    public bool disableExplosion;

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
        if (!hasExploded)
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, explosionPos, radius, 3.0F);

                // destroy enemies if they are hit
                if (hit.gameObject.GetComponent<Enemy>())
                {
                    if (!disableExplosion)
                    {
                        hit.gameObject.SendMessage("RandomExplosionDismember");
                    }
                    hit.gameObject.SendMessage("KillThisEnemy");
                }
            }

            if (!disableExplosion)
            {
                Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            }
            hasExploded = true;
            Destroy(gameObject);
        }
    }
}
