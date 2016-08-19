using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public int health;
    public enum EnemyType { Skeleton, FlyingSkull };
    public EnemyType enemyType;

    bool hasStartedToAutoDestroySelf;

    s_WanderingAI aiScript;
    NavMeshAgent agent;
    Animator animator;

    // Use this for initialization
    void Start ()
    {
        aiScript = gameObject.GetComponent<s_WanderingAI>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
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
                transform.GetChild(i).gameObject.SendMessage("StartSelfDestruct");
                transform.GetChild(i).SetParent(null);
            }
        }

        // now we destroy ourselves after a bit
        if (!hasStartedToAutoDestroySelf)
        {
            StartCoroutine(SelfDestruct());
        }
    }

    public IEnumerator SelfDestruct()
    {
        hasStartedToAutoDestroySelf = true;
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
