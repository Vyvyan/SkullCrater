using UnityEngine;
using System.Collections;

public class BossSkullMeteor : MonoBehaviour {

    public GameObject skeltin, flyingSkeltin, redSkeltin, toxicSkeltin, flyingRedSkeltin, flyingToxicSkeltin;
    public GameObject spawnParticles;
    public GameObject noise;

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
        if (other.gameObject.tag == "Ground")
        {
            int rnd = Random.Range(1, 7);
            if (rnd == 1)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(skeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }        
            }
            else if (rnd == 2)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(flyingSkeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }
            }
            else if (rnd == 3)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(redSkeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }
            }
            else if (rnd == 4)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(toxicSkeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }
            }
            else if (rnd == 5)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(flyingRedSkeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }
            }
            else if (rnd == 6)
            {
                if (GameManager.enemyCount < 40)
                {
                    if (GameManager.gameState == GameManager.GameState.Playing)
                    {
                        Instantiate(flyingToxicSkeltin, gameObject.transform.position, Quaternion.identity);
                        Instantiate(spawnParticles, gameObject.transform.position, Quaternion.identity);
                        GameManager.enemyCount++;
                    }
                }
            }
            Instantiate(noise, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
