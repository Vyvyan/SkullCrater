using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public GameObject explosionEffect;

    bool hasExploded;

    public float radius;
    public float power;

    // this is cause we can use this code for the shotgun pellets, so they have more impact, but no explosion effect
    public bool disableExplosion;

    public bool isRocket;

    GameManager gameManager;

	// Use this for initialization
	void Start ()
    {
	    if (isRocket)
        {
            radius = GameManager.rocketExplosionRadius;
        }
        else
        {
            radius = GameManager.grenadeExplosionRadius;
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
            // if we hit a bunch of things, then slow mo us. each skeleton has 11 colliders
            if (colliders.Length > 60)
            {
                gameManager.SlowMo();
            }
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                // destroy enemies if they are hit
                if (hit.gameObject.GetComponent<Enemy>())
                {
                    if (!disableExplosion)
                    {
                        hit.gameObject.SendMessage("RandomExplosionDismember");
                    }
                    hit.gameObject.SendMessage("KillThisEnemy", false);
                }
                // destroy enemies if they are hit and a skull
                if (hit.gameObject.GetComponent<FlyingSkull>())
                {
                    hit.gameObject.SendMessage("KillThisEnemy", false);
                }

                if (rb != null)
                {
                    rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
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
