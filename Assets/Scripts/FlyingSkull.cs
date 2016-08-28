using UnityEngine;
using System.Collections;

public class FlyingSkull : MonoBehaviour {

    GameObject player;
    Rigidbody rb;
    public float speed;
    public bool isAlive;
    GameObject triggerObject;
    public bool isToxic;

	// Use this for initialization
	void Start ()
    {
        isAlive = true;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody>();
        triggerObject = gameObject.transform.GetChild(0).gameObject;
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
        rb.isKinematic = false;
        rb.useGravity = true;
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
        GameManager.enemiesKilledThisSession++;
    }
}
