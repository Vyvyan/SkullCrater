using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    Transform player;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public bool CanWeSpawnHere()
    {
        // checks to see the distance from this spawn to the player, if it's too short, we can't spawn here
        if (Vector3.Distance(transform.position, player.position) < 65)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
