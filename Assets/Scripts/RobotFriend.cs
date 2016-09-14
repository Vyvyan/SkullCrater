﻿using UnityEngine;
using System.Collections;

public class RobotFriend : MonoBehaviour {

    NavMeshAgent agent;
    public Transform[] goldTargets;
    Transform target;

    public enum RobotState { mining, moving, gettingNewTarget };
    public RobotState robotState;

    public float miningTimer, miningTimerCurrent;

    public GameObject gold;
    public Transform goldSpawnLocation;
    public ParticleSystem dustParticles;

    Animator anim;

    GameManager gameManager;

	// Use this for initialization
	void Start ()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        robotState = RobotState.gettingNewTarget;
        dustParticles.loop = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            if (gameManager.gameMode != GameManager.GameMode.Boss)
            {
                if (robotState == RobotState.gettingNewTarget)
                {
                    int rnd = Random.Range(0, goldTargets.Length - 1);
                    target = goldTargets[rnd];
                    agent.SetDestination(target.position);
                    robotState = RobotState.moving;
                }
                else if (robotState == RobotState.moving)
                {
                    float dist = agent.remainingDistance;
                    if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                    {
                        miningTimerCurrent = 0;
                        robotState = RobotState.mining;
                        dustParticles.loop = true;
                        dustParticles.Play();
                        anim.SetBool("isDigging", true);
                    }
                }
                else if (robotState == RobotState.mining)
                {
                    if (miningTimerCurrent <= miningTimer)
                    {
                        miningTimerCurrent += Time.deltaTime;
                    }
                    else
                    {
                        SpawnGold();
                        robotState = RobotState.gettingNewTarget;
                        dustParticles.loop = false;
                        anim.SetBool("isDigging", false);
                    }
                }
            }
        }
    }

    void SpawnGold()
    {
        GameObject temp = Instantiate(gold, goldSpawnLocation.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Rigidbody>().AddForce(Vector3.up * 6, ForceMode.Impulse);
    }

}
