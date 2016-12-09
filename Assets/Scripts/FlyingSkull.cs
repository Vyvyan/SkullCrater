using UnityEngine;
using System.Collections;

public class FlyingSkull : MonoBehaviour {

    GameObject player;
    Rigidbody rb;
    public float speed;
    public bool isAlive;
    GameObject triggerObject;
    public bool isToxic, isRed;
    AudioSource audioS;

	// Use this for initialization
	void Start ()
    {
        isAlive = true;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody>();
        triggerObject = gameObject.transform.GetChild(0).gameObject;
        audioS = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    void FixedUpdate()
    {
        if (isAlive)
        {
            transform.LookAt(player.transform.position);
            gameObject.transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            if (isAlive)
            {
                KillThisEnemy(true);
                rb.AddExplosionForce(2000, other.contacts[0].point, 10);
                rb.AddTorque(new Vector3(Random.Range(15, 45), Random.Range(15, 45), Random.Range(15, 45)));
            }
        }
    }

    void KillThisEnemy(bool killedWithBullet)
    {
        isAlive = false;
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        Destroy(triggerObject);

        AutoDestroy tempAutoDestroy = gameObject.AddComponent<AutoDestroy>();
        tempAutoDestroy.timeUntilDestroy = 10;

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

            if (isToxic)
            {
                GameManager.stat_ToxicSkellsKilled++;
            }
            else if (isRed)
            {
                GameManager.stat_RedSkellsKilled++;
            }
            else
            {
                GameManager.stat_SkellsKilled++;
            }
        }

        if (audioS)
        {
            audioS.Stop();
        }
    }
}
