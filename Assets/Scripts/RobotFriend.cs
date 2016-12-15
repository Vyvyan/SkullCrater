using UnityEngine;
using System.Collections;
using Steamworks;

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
    bool isInIenum;

    Animator anim;
    // the audio source is only for the digging sound, the rocket is on a different object, and it's always playing so ez pz
    AudioSource audioS;
    GameManager gameManager;

    // bool for turning frand back on after anamalous skulls
    bool wereWeInANonNormalModeLastFrame;

    public int numberOfTimesShot;
    bool hasAwardedAchievement;

	// Use this for initialization
	void Start ()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        audioS = GetComponent<AudioSource>();
        robotState = RobotState.gettingNewTarget;
        dustParticles.loop = false;
        audioS.volume = GameManager.SFXVolume / 600;
        audioS.Play();
        audioS.Pause();
        isInIenum = false;
        numberOfTimesShot = 0;
        hasAwardedAchievement = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        audioS.volume = GameManager.SFXVolume / 600;
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            if (gameManager.gameMode == GameManager.GameMode.normal)
            {
                // reset robot frand so he moves
                if (wereWeInANonNormalModeLastFrame)
                {
                    agent.Resume();
                    wereWeInANonNormalModeLastFrame = false;
                }


                if (robotState == RobotState.gettingNewTarget)
                {
                    int rnd = Random.Range(0, goldTargets.Length - 1);
                    target = goldTargets[rnd];
                    agent.SetDestination(target.position);
                    anim.SetBool("isDigging", false);
                    robotState = RobotState.moving;
                    miningTimerCurrent = miningTimer;
                }
                else if (robotState == RobotState.moving)
                {
                    float dist = agent.remainingDistance;
                    if (!isInIenum)
                    {
                        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                        {
                            miningTimerCurrent = 0;
                            dustParticles.loop = true;
                            dustParticles.Play();
                            anim.SetBool("isDigging", true);
                            audioS.UnPause();
                            robotState = RobotState.mining;
                        }
                    }
                }
                else if (robotState == RobotState.mining)
                {
                    if (miningTimerCurrent <= miningTimer)
                    {
                        agent.Stop();
                        miningTimerCurrent += Time.deltaTime;
                    }
                    else
                    {
                        // we set this here, since frand likes to dig while moving
                        miningTimerCurrent = miningTimer;
                        audioS.Pause();
                        audioS.Pause();
                        dustParticles.loop = false;
                        anim.SetBool("isDigging", false);
                        if (!isInIenum)
                        {
                            StartCoroutine(waitThenFindNewTarget());
                        }
                    }
                }
            }
            else
            {
                agent.Stop();
                anim.SetBool("isDigging", false);
                dustParticles.Stop();
                audioS.Pause();
                wereWeInANonNormalModeLastFrame = true;
            }
        }

        if (!hasAwardedAchievement)
        {
            if (numberOfTimesShot >= 50)
            {
                SteamUserStats.SetAchievement("ShootFrand");
                SteamUserStats.StoreStats();
                hasAwardedAchievement = true;
            }
        }
    }

    void SpawnGold()
    {
        GameObject temp = Instantiate(gold, goldSpawnLocation.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Rigidbody>().AddForce(Vector3.up * 6, ForceMode.Impulse);
    }

    IEnumerator waitThenFindNewTarget()
    {
        if (!isInIenum)
        {
            SpawnGold();
            audioS.Pause();
            dustParticles.loop = false;
            anim.SetBool("isDigging", false);
            isInIenum = true;
        }
        yield return new WaitForSeconds(.4f);
        isInIenum = false;
        //miningTimerCurrent = 0;
        robotState = RobotState.gettingNewTarget;
        agent.Resume();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            numberOfTimesShot++;
        }
    }
}
