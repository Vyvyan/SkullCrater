using UnityEngine;
using System.Collections;

public class BlackSkull : MonoBehaviour {

    GameObject player;
    public GameObject meteor, ball;
    public float firingRate, ballFiringRate;
    float firingRateTimer, ballFiringRateTimer;
    public Transform eye1, eye2, ballSpawn1, ballSpawn2;
    bool eyeToShootFrom;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        firingRateTimer = firingRate;
        ballFiringRateTimer = ballFiringRate;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(player.transform.position);

        if (firingRateTimer > 0)
        {
            firingRateTimer -= Time.deltaTime;
        }
        else
        {
            firingRateTimer = firingRate;

            // shoot a skull
            // figure out which eye to shoot from
            if (eyeToShootFrom)
            {
                // shoot a thingy
                Instantiate(meteor, eye1.position, Quaternion.identity);
                // switch eye
                eyeToShootFrom = !eyeToShootFrom;
            }
            else
            {
                // shoot a thingy
                Instantiate(meteor, eye2.position, Quaternion.identity);
                // switch eye
                eyeToShootFrom = !eyeToShootFrom;
            }
        }

        if (ballFiringRateTimer > 0)
        {
            ballFiringRateTimer -= Time.deltaTime;
        }
        else
        {
            ballFiringRateTimer = ballFiringRate;
            // shoot out a ball
            GameObject tempBall1 = Instantiate(ball, ballSpawn1.position, Quaternion.identity) as GameObject;
            GameObject tempBall2 = Instantiate(ball, ballSpawn2.position, Quaternion.identity) as GameObject;
            tempBall1.GetComponent<Rigidbody>().AddForce(ballSpawn1.transform.forward * 20, ForceMode.VelocityChange);
            tempBall2.GetComponent<Rigidbody>().AddForce(ballSpawn2.transform.forward * 20, ForceMode.VelocityChange);
        }
    }
}
