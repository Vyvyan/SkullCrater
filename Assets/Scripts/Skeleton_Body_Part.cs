﻿using UnityEngine;
using System.Collections;

public class Skeleton_Body_Part : MonoBehaviour {

    HingeJoint joint;
    public bool isVital;
    bool hasStartedToAutoDestroySelf;

	// Use this for initialization
	void Start ()
    {
        joint = gameObject.GetComponent<HingeJoint>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.magnitude > 10)
        {
            if (other.gameObject.tag == "Bullet")
            {
                TakeDamage();
            }
        }
    }

    void TakeDamage()
    {
        if (joint)
        {
            if (isVital)
            {
                if (transform.parent)
                {
                    transform.parent.gameObject.SendMessage("KillThisEnemy");
                }
            }
            Destroy(joint);
        }
        gameObject.transform.SetParent(null);

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

    // we put this as a separate function so we can call an ienumerator using the messenging system inside the enemy class
    public void StartSelfDestruct()
    {
        if (!hasStartedToAutoDestroySelf)
        {
            StartCoroutine(SelfDestruct());
        }
    }
}
