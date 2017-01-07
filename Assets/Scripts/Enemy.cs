using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public int health;
    public enum EnemyType { Skeleton, FlyingSkull, ToxicSkeleton};
    public EnemyType enemyType;
    bool isDead;

    bool hasStartedToAutoDestroySelf;

    s_WanderingAI aiScript;
    NavMeshAgent agent;
    Animator animator;

    public GameObject triggerObject;

    public GameObject toxicGrenade, gold;
    bool hasSpawnedToxGrenade;

    public bool goldSkeleton;
    public bool redSkeleton;

    AudioSource audio;

    public Light enemyLight;

    public bool isNormalSkeltin;
    public GameObject redSkeltinEvolution;
    public float lifeTimer;

    // Use this for initialization
    void Start ()
    {
        aiScript = gameObject.GetComponent<s_WanderingAI>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        //triggerObject = transform.GetChild(10).gameObject;
        audio = GetComponent<AudioSource>();
        lifeTimer = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (enemyLight)
        {
            if(enemyLight.intensity < 2)
            {
                enemyLight.intensity += Time.deltaTime;
            }
        }

        // evolve into red skeltins after a set time
        // IGNORE THIS AND DONT SET ANY ENEMY PREFABS TO THIS BOOL. IT WILL MAKE THEM CHANGE INTO WRAITH SKELTINS AFTER A SET AMOUNT OF TIME. IGNORE IT!
        if (isNormalSkeltin)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= 140)
            {
                Instantiate(redSkeltinEvolution, gameObject.transform.position, gameObject.transform.rotation);
                Destroy(gameObject);
            }
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            KillThisEnemy(true);
        }
    }

    public void KillThisEnemy(bool killedWithBullet)
    {
        if (!isDead)
        {
            Destroy(triggerObject);
            if (enemyType == EnemyType.Skeleton)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                Destroy(aiScript);
                Destroy(agent);
                Destroy(animator);
                if (enemyLight)
                {
                    Destroy(enemyLight);
                }

                // gets all the parts and deparents them
                int children = transform.childCount;
                for (int i = children - 1; i > 0; i--)
                {
                    if (transform.GetChild(i).gameObject.tag != "Enemy")
                    {
                        transform.GetChild(i).gameObject.SendMessage("StartSelfDestruct");
                        transform.GetChild(i).SetParent(null);
                    }
                }

                // only update states if the player is alive, so the end game mass killing doesn't increase stats
                if (GameManager.gameState == GameManager.GameState.Playing)
                {
                    if (goldSkeleton)
                    {
                        Instantiate(gold, gameObject.transform.position, Quaternion.identity);
                        GameManager.stat_GoldSkeltinsKilled++;
                    }
                    else if (redSkeleton)
                    {
                        GameManager.stat_RedSkeltinsKilled++;
                    }
                    else
                    {
                        GameManager.stat_SkeltinsKilled++;
                    }
                }
            }
            else if (enemyType == EnemyType.ToxicSkeleton)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                Destroy(aiScript);
                Destroy(agent);
                Destroy(animator);

                // gets all the parts and deparents them
                int children = transform.childCount;
                for (int i = children - 1; i > 0; i--)
                {
                    if (transform.GetChild(i).gameObject.tag != "Enemy")
                    {
                        transform.GetChild(i).gameObject.SendMessage("StartSelfDestruct");
                        transform.GetChild(i).SetParent(null);
                    }

                }

                if (!hasSpawnedToxGrenade)
                {
                    StartCoroutine(delayedToxicExplosion());
                    hasSpawnedToxGrenade = true;
                }
                if (GameManager.gameState == GameManager.GameState.Playing)
                {
                    GameManager.stat_ToxicSkeltinsKilled++;
                }
            }

            // now we destroy ourselves after a bit
            if (!hasStartedToAutoDestroySelf)
            {
                StartCoroutine(SelfDestruct());
            }

            // put this if statement here because I got an error when killing an enemy post death
            if (GameManager.gameState == GameManager.GameState.Playing)
            {
                if (killedWithBullet)
                {
                    GameObject.FindGameObjectWithTag("Player").SendMessage("AddGrenadeJuice");
                }
            }
            GameManager.enemyCount--;
            if (GameManager.gameState == GameManager.GameState.Playing)
            {
                GameManager.enemiesKilledThisSession++;
            }
            isDead = true;
        }
    }

    public void RandomExplosionDismember()
    {
        int children = transform.childCount;
        for (int i = children - 1; i > 0; i--)
        {
            // random between two options, 50/50 chance to dismember a limb or not
            int rnd = Random.Range(0,2);
            Debug.Log(rnd);
            if (rnd == 0)
            {
                Destroy(transform.GetChild(i).GetComponent<HingeJoint>());
            }
        }
    }

    public IEnumerator SelfDestruct()
    {
        hasStartedToAutoDestroySelf = true;
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    IEnumerator delayedToxicExplosion()
    {
        yield return new WaitForSeconds(.5f);
        Instantiate(toxicGrenade, gameObject.transform.position, Quaternion.identity);
    }

    public void PlayFootStep()
    {
        if (audio)
        {
            audio.PlayOneShot(AudioManager.skel_Footstep, GameManager.SFXVolume / 600);
        }
    }

    public void PlayFootStep2()
    {
        audio.PlayOneShot(AudioManager.skel_Footstep2, GameManager.SFXVolume / 600);
    }
}
