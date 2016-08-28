using UnityEngine;
using System.Collections;

public class ToxicFlyingSkull : MonoBehaviour
{

    public GameObject explosionEffect;

    bool hasExploded;

    public float radius;
    public float power;

    FlyingSkull flyingSkullScript;

    // this is cause we can use this code for the shotgun pellets, so they have more impact, but no explosion effect
    public bool disableExplosion;

    // Use this for initialization
    void Start()
    {
        flyingSkullScript = gameObject.GetComponent<FlyingSkull>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        if (!flyingSkullScript.isAlive)
        {
            if (other.gameObject.tag != "Bullet")
            {
                if (!hasExploded)
                {
                    Vector3 explosionPos = transform.position;
                    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
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
                            rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
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
    }
}
