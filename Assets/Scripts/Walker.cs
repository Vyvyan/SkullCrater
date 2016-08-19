using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

    public Rigidbody rightFoot, leftFoot;
    public float stepTimer, stepTimerCurrent;
    public float upPower, forwardPower;
    bool moveRightFootNext;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (stepTimerCurrent < stepTimer)
        {
            stepTimerCurrent += Time.deltaTime;
        }
        if (stepTimerCurrent >= stepTimer)
        {
            if (moveRightFootNext)
            {
                // move our right foot
                rightFoot.AddForce(transform.forward * forwardPower);
                rightFoot.AddForce(Vector3.up *  upPower);
            }
            else
            {
                // move our left foot
                leftFoot.AddForce(transform.forward * forwardPower);
                leftFoot.AddForce(Vector3.up * upPower);
            }
            // swap our foot
            moveRightFootNext = !moveRightFootNext;
            stepTimerCurrent = 0;
        }
	}
}
