using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public int health;
    public enum EnemyType { Skeleton, FlyingSkull, ToxicSkeleton };
    public EnemyType enemyType;

    bool hasStartedToAutoDestroySelf;

    s_WanderingAI aiScript;
    NavMeshAgent agent;
    Animator animator;

    GameObject triggerObject;

    public GameObject toxicGrenade;
    bool hasSpawnedToxGrenade;

    // Use this for initialization
    void Start ()
    {
        aiScript = gameObject.GetComponent<s_WanderingAI>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
        triggerObject = transform.GetChild(10).gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        
    }

    public void KillThisEnemy()
    {
        Destroy(triggerObject);
        if (enemyType == EnemyType.Skeleton)
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
        }

        // now we destroy ourselves after a bit
        if (!hasStartedToAutoDestroySelf)
        {
            StartCoroutine(SelfDestruct());
        }

        // put this if statement here because I got an error when killing an enemy post death
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            GameObject.FindGameObjectWithTag("Player").SendMessage("AddGrenadeJuice");
        }
        GameManager.enemyCount--;
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
}
