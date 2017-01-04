using UnityEngine;
using System.Collections;
using Steamworks;

public class ToxicFlyingSkull : MonoBehaviour
{

    public GameObject explosionEffect;

    bool hasExploded;

    public float radius;
    public float power;

    FlyingSkull flyingSkullScript;

    GameManager gameManager;
    public GameObject audioSourceObjectToSpawn, audioSourceSLOWMO;

    // this is cause we can use this code for the shotgun pellets, so they have more impact, but no explosion effect
    public bool disableExplosion;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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
                    // if we hit a bunch of things, then slow mo us. each skeleton has 11 colliders
                    if (colliders.Length > 60)
                    {
                        if (GameManager.canGoSlowMo)
                        {
                            Instantiate(audioSourceSLOWMO, transform.position, Quaternion.identity);
                            gameManager.SlowMo();
                        }
                        else
                        {
                            Instantiate(audioSourceObjectToSpawn, transform.position, Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(audioSourceObjectToSpawn, transform.position, Quaternion.identity);
                    }

                    int numberOfEnemiesHit = 0;

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

                            numberOfEnemiesHit++;
                        }
                        // destroy enemies if they are hit and a skull
                        if (hit.gameObject.GetComponent<FlyingSkull>())
                        {
                            hit.gameObject.SendMessage("KillThisEnemy", false);
                            numberOfEnemiesHit++;
                        }

                        if (rb != null)
                            rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                    }

                    if (numberOfEnemiesHit >= 10)
                    {
                        SteamUserStats.SetAchievement("ToxicBigBoom");
                        SteamUserStats.StoreStats();
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
