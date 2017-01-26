using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackSkull : MonoBehaviour {

    GameObject player;
    public GameObject meteor, ball;
    public float firingRate, ballFiringRate;
    float firingRateTimer, ballFiringRateTimer;
    public Transform eye1, eye2, ballSpawn1, ballSpawn2;
    bool eyeToShootFrom;
    public Transform[] skullTargets;

    public enum BossState { spawning, idle, spin_attack, dead};
    public BossState bossState;

    public float spinAttack_TimeInterval;
    float spinAttack_TimerCurrent;
    int skullTargetToAttack = 0;
    int skullTargetsBeforeSwitchingStates = 0;

    public float idleToSpinAttackTimer;
    float idleToSpinAttackTimerCurrent;

    public float rotationSpeed;

    public static float health = 27;

    GameManager gameManager;

    bool isShaking;

    AudioSource audioS;
    AudioManager audioManager;

    //Animator anim;

    public GameObject deadBoss;
    public Light bossLight;
    public bool lightOn;

    public Image leftEye, rightEye;

	// Use this for initialization
	void Start ()
    {
        audioS = GetComponent<AudioSource>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        firingRateTimer = firingRate;
        ballFiringRateTimer = ballFiringRate;
        spinAttack_TimerCurrent = spinAttack_TimeInterval;
        idleToSpinAttackTimerCurrent = idleToSpinAttackTimer;
        bossState = BossState.spawning;
        //anim = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        // put the health here, so it resets each round
        health = 27;
        // start our light off
        bossLight.intensity = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        leftEye.fillAmount = rightEye.fillAmount = (health / 28);
        
        if (bossState == BossState.idle)
        {
            // slow look at
            //calculate the rotation needed 
            Quaternion quat = Quaternion.LookRotation(player.transform.position - transform.position);

            //use spherical interpollation over time 
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime * rotationSpeed);

            // fire off balls
            if (ballFiringRateTimer > 0)
            {
                ballFiringRateTimer -= Time.deltaTime;
            }
            else
            {
                ballFiringRateTimer = ballFiringRate;
                // shoot out two balls
                GameObject tempBall1 = Instantiate(ball, ballSpawn1.position, Quaternion.identity) as GameObject;
                GameObject tempBall2 = Instantiate(ball, ballSpawn2.position, Quaternion.identity) as GameObject;
                audioS.PlayOneShot(AudioManager.rocketFire, GameManager.SFXVolume / 100);
                audioS.PlayOneShot(AudioManager.rocketFire, GameManager.SFXVolume / 100);
                tempBall1.GetComponent<Rigidbody>().AddForce(ballSpawn1.transform.forward * 20, ForceMode.VelocityChange);
                tempBall2.GetComponent<Rigidbody>().AddForce(ballSpawn2.transform.forward * 20, ForceMode.VelocityChange);
            }
            
            // timer to change boss states
            if (idleToSpinAttackTimerCurrent > 0)
            {
                idleToSpinAttackTimerCurrent -= Time.deltaTime;
            }
            else
            {
                bossState = BossState.spin_attack;
                skullTargetToAttack = 0;
                skullTargetsBeforeSwitchingStates = 0;
                rotationSpeed = 10;
                idleToSpinAttackTimerCurrent = idleToSpinAttackTimer;
            }
        }
        else if (bossState == BossState.spin_attack)
        {
            spinAttack_TimerCurrent -= Time.deltaTime;

            //transform.LookAt(skullTargets[skullTargetToAttack].transform.position);

            Quaternion quat = Quaternion.LookRotation(skullTargets[skullTargetToAttack].transform.position - transform.position);

            //use spherical interpollation over time 
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime * rotationSpeed);

            if (spinAttack_TimerCurrent < 0)
            {
                spinAttack_TimerCurrent = spinAttack_TimeInterval;
                //figure out which eye to shoot from
                if (eyeToShootFrom)
                {
                    // spawn meteor, make it look at the target we want, then shoot it forward towards it
                    GameObject temp = Instantiate(meteor, eye1.position, Quaternion.identity) as GameObject;
                    temp.transform.LookAt(skullTargets[skullTargetToAttack]);
                    temp.GetComponent<Rigidbody>().AddForce(temp.transform.forward * 80, ForceMode.VelocityChange);
                    // increase our target number for the next shot
                    if (skullTargetToAttack < 27)
                    {
                        skullTargetToAttack++;
                    }
                    skullTargetsBeforeSwitchingStates++;
                    eyeToShootFrom = !eyeToShootFrom;
                }
                else
                {
                    // spawn meteor, make it look at the target we want, then shoot it forward towards it
                    GameObject temp = Instantiate(meteor, eye2.position, Quaternion.identity) as GameObject;
                    temp.transform.LookAt(skullTargets[skullTargetToAttack]);
                    temp.GetComponent<Rigidbody>().AddForce(temp.transform.forward * 80, ForceMode.VelocityChange);
                    // increase our target number for the next shot
                    if (skullTargetToAttack < 27)
                    {
                        skullTargetToAttack++;
                    }
                    skullTargetsBeforeSwitchingStates++;
                    eyeToShootFrom = !eyeToShootFrom;
                }
            }

            // after we'ev shot at all of the skull targets, switch back to idle
            if (skullTargetsBeforeSwitchingStates > 28)
            {
                spinAttack_TimerCurrent = spinAttack_TimeInterval;
                rotationSpeed = 1;
                ballFiringRateTimer = ballFiringRate;
                bossState = BossState.idle;
            }
        }
        else if (bossState == BossState.dead)
        {
            if (isShaking)
            {
                Vector3 newPosition = Random.insideUnitSphere * .2f;
                newPosition.x += transform.position.x;
                newPosition.y += transform.position.y;
                newPosition.z += transform.position.z;
                gameObject.transform.position = newPosition;
                bossLight.intensity -= .6f * Time.deltaTime;
            }
        }

        // killing the boss
        if (bossState != BossState.dead)
        {
            if (health < 0)
            {
                StartCoroutine(MakeSureAllEnemiesDie());
                StartCoroutine(ShakeSkullAfterDeath());
                //Destroy(anim);
                bossState = BossState.dead;
            }
        }
    }

    IEnumerator MakeSureAllEnemiesDie()
    {
        gameManager.KillAll();
        yield return new WaitForSeconds(2);
        gameManager.KillAll();
    }

    IEnumerator ShakeSkullAfterDeath()
    {
        isShaking = true;
        yield return new WaitForSeconds(6);
        isShaking = false;
        // only end the game if the player is alive
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            gameManager.startWaitThenSwitchToEndGame();
        }
        GameManager.isBossDead = true;
        GameManager.stat_SkellLordsKilled++;
        audioManager.Play2DSound(AudioManager.boss_Explode);
        deadBoss.SetActive(true);
        gameObject.SetActive(false);
        deadBoss.transform.SetParent(null);
    }
}
