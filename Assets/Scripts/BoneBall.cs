using UnityEngine;
using System.Collections;

public class BoneBall : MonoBehaviour {

    public GameObject[] boneBits;
    bool isDead;
    Collider col;
    Rigidbody rigBod;
    Vector3 velocityLastFrame;
    GameObject player;
    public float attackPower;
    public GameObject triggerObject;

    // we limit the ball so they cant attack for a few seconds on spawn, since they are in the air
    bool canAttack;

    public float attackTimer, attackTimerCurrent;

	// Use this for initialization
	void Start ()
    {
        col = GetComponent<Collider>();
        rigBod = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        attackTimerCurrent = 0;
        canAttack = false;
        StartCoroutine(DelayFirstAttack());
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isDead)
        {
            if (canAttack)
            {
                attackTimerCurrent += Time.deltaTime;
                if (attackTimerCurrent >= attackTimer)
                {
                    Attack();
                    attackTimerCurrent = 0;
                }
            }
        }

        // getting errors with the kill all function when the player dies, so lets not kill bone balls
        if (!player)
        {
            gameObject.tag = "Untagged";
        }
	}

    void LateUpdate()
    {
        velocityLastFrame = rigBod.velocity;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (other.contacts[0].thisCollider == col)
            {
                if (!isDead)
                {
                    KillThisEnemy(true);
                }
            }
        }
    }

    void KillThisEnemy(bool killedByBullet)
    {
        if (!isDead)
        {
            foreach (GameObject bit in boneBits)
            {
                Rigidbody rb = bit.AddComponent<Rigidbody>();
                rb.velocity = velocityLastFrame;
                AutoDestroy ad = bit.AddComponent<AutoDestroy>();
                ad.timeUntilDestroy = 10;
                bit.transform.SetParent(null);
            }

            if (GameManager.gameState == GameManager.GameState.Playing)
            {
                GameObject.FindGameObjectWithTag("Player").SendMessage("AddGrenadeJuice");
            }

            AutoDestroy tempAD = gameObject.AddComponent<AutoDestroy>();
            tempAD.timeUntilDestroy = 10;

            Destroy(triggerObject);
            GameManager.enemiesKilledThisSession++;
            GameManager.enemyCount--;
            GameManager.stat_BoneBallsKilled++;
            isDead = true;
        }    
    }

    void Attack()
    {
        rigBod.AddForce((player.transform.position - gameObject.transform.position).normalized * attackPower, ForceMode.Impulse);
    }

    IEnumerator DelayFirstAttack()
    {
        yield return new WaitForSeconds(3);
        canAttack = true;
    }
}
