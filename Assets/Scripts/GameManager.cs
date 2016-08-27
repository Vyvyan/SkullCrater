using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead, PreGame};
    static public GameState gameState;
    bool hasKilledAllEnemiesAfterPlayerDeath;

    public GameObject editorLight;

    public GameObject skeletonEnemy, flyingskullEnemy, redSkeleton, redFlyingSkull, toxicSkeleton, toxicFlyingSkull;
    public GameObject[] enemySpawns;
    public GameObject[] flyingenemySpawns;

    public static int enemyCount;
    public int enemyCountMax;
    public static int enemiesKilledThisSession;

    public int chanceToSpawnSpecialSkeleton, chanceToSpawnSpecialFlying;

    public float spawnTimer, flyingSpawnTimer;
    float spawnTimerCurrent, flyingSpawnTimerCurrent;

    public static int heldGold;
    public static int storedGold;
    public static int thisSessionGoldGained;
    public int goldValue, goldBonusAmount, goldBonusLevel;

    public static string enemyThatKilledPlayer;

    public static float gameTimer;
    // we use this variable so we only have to round it to an int once, and not check it a bunch
    int comparisonTimer;

    public static int shotgunWeaponValue = 200, machinegunWeaponValue = 200, rocketWeaponValue = 350;

    public static bool shotgunUnlocked, machinegunUnlocked, rocketUnlocked;
    public Text shotgunEquipButtonText, machinegunEquipButtonText, storedGoldText, rocketEquipButtonText;

	// Use this for initialization
	void Start ()
    {
        // LOAD OUR SAVED STUFF
        LoadPlayerPrefs();

        thisSessionGoldGained = 0;
        enemiesKilledThisSession = 0;
        gameTimer = 0;

        gameState = GameState.PreGame;
        editorLight.SetActive(false);

        // checks to see if our guns are unlocked, then changes the UI buttons accordingly
        ChangeEquipButtonText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            storedGold += 50;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerPrefs.DeleteAll();
        }
        // updates gold on weapon screen
        if (gameState == GameState.PreGame)
        {
            storedGoldText.text = "Balance: " + storedGold.ToString() + " G";
        }

        if (gameState == GameState.Playing)
        {
            // GAME TIMER STUFF
            gameTimer += Time.deltaTime;
            comparisonTimer = Mathf.RoundToInt(gameTimer);

   
            // increase difficulty
            if (comparisonTimer == 60)
            {
                chanceToSpawnSpecialFlying = 3;
                chanceToSpawnSpecialSkeleton = 3;
                spawnTimer = 1f;
                flyingSpawnTimer = 10f;
            }
            else if(comparisonTimer == 140)
            {
                chanceToSpawnSpecialFlying = 10;
                chanceToSpawnSpecialSkeleton = 10;
                spawnTimer = .9f;
                flyingSpawnTimer = 4f;
            }
            else if (comparisonTimer == 200)
            {
                chanceToSpawnSpecialFlying = 25;
                chanceToSpawnSpecialSkeleton = 25;
                spawnTimer = .7f;
                flyingSpawnTimer = 2f;
            }


            if (enemyCount < enemyCountMax)
            {
                // spawning skeletons
                if (spawnTimerCurrent <= spawnTimer)
                {
                    spawnTimerCurrent += Time.deltaTime;
                }
                else
                {
                    // SPAWN A SKELETON
                    foreach(GameObject spawner in enemySpawns)
                    {
                        if (spawner.GetComponent<EnemySpawner>().CanWeSpawnHere())
                        {
                            spawner.tag = "Skeleton_Spawn";
                        }
                        else
                        {
                            spawner.tag = "Untagged";
                        }
                    }

                    GameObject[] activeSkeletonSpawns = GameObject.FindGameObjectsWithTag("Skeleton_Spawn");
                    int rndLocation = Random.Range(0, activeSkeletonSpawns.Length - 1);

                    // SHOULD WE SPAWN A SPECIAL SKELETON
                    int rndToSeeIfWeShouldSpawnSpecialSkele = Random.Range(1, 101);
                    if (rndToSeeIfWeShouldSpawnSpecialSkele < chanceToSpawnSpecialSkeleton)
                    {
                        int rando = Random.Range(1, 4);
                        // if we random a 3, make a toxic, otherwise, make a red. 2/3 for red, 1/3 for toxic
                        if (rando == 3)
                        {
                            Instantiate(toxicSkeleton, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(redSkeleton, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(skeletonEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                    }

                    enemyCount++;

                    spawnTimerCurrent = 0;
                }

                // spawning flying skulls
                if (flyingSpawnTimerCurrent <= flyingSpawnTimer)
                {
                    flyingSpawnTimerCurrent += Time.deltaTime;
                }
                else
                {
                    // SPAWN A SKELETON
                    foreach (GameObject spawner in flyingenemySpawns)
                    {
                        if (spawner.GetComponent<EnemySpawner>().CanWeSpawnHere())
                        {
                            spawner.tag = "Flying_Spawn";
                        }
                        else
                        {
                            spawner.tag = "Untagged";
                        }
                    }

                    GameObject[] activeSkeletonSpawns = GameObject.FindGameObjectsWithTag("Flying_Spawn");
                    int rndLocation = Random.Range(0, activeSkeletonSpawns.Length - 1);

                    // SHOULD WE SPAWN A SPECIAL SKELETON
                    int rndToSeeIfWeShouldSpawnSpecialSkele = Random.Range(1, 101);
                    if (rndToSeeIfWeShouldSpawnSpecialSkele < chanceToSpawnSpecialFlying)
                    {
                        int rando = Random.Range(1, 4);
                        // if we random a 3, make a toxic, otherwise, make a red. 2/3 for red, 1/3 for toxic
                        if (rando == 3)
                        {
                            Instantiate(toxicFlyingSkull, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(redFlyingSkull, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(flyingskullEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                    }

                    enemyCount++;

                    flyingSpawnTimerCurrent = 0;
                }
            }
        }

        if (gameState == GameState.Dead)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            if (!hasKilledAllEnemiesAfterPlayerDeath)
            {
                StartCoroutine(KillAllEnemies());
                hasKilledAllEnemiesAfterPlayerDeath = true;
            }
        }
	}

    public void ChangeEquipButtonText()
    {
        if (shotgunUnlocked)
        {
            shotgunEquipButtonText.text = "Equip";
        }
        else
        {
            shotgunEquipButtonText.text = shotgunWeaponValue.ToString() + " g";
        }

        if (machinegunUnlocked)
        {
            machinegunEquipButtonText.text = "Equip";
        }
        else
        {
            machinegunEquipButtonText.text = machinegunWeaponValue.ToString() + " g";
        }

        if (rocketUnlocked)
        {
            rocketEquipButtonText.text = "Equip";
        }
        else
        {
            rocketEquipButtonText.text = rocketWeaponValue.ToString() + " g";
        }
    }

    public void LoadPlayerPrefs()
    {
        // load our gold
        storedGold = PlayerPrefs.GetInt("storedGold", 0);

        // unlocked shotgun
        if (PlayerPrefs.GetInt("shotgunUnlocked", 0) == 1)
        {
            shotgunUnlocked = true;
        }
        // unlocked MachineGun
        if (PlayerPrefs.GetInt("machinegunUnlocked", 0) == 1)
        {
            machinegunUnlocked = true;
        }
        // unlocked MachineGun
        if (PlayerPrefs.GetInt("rocketUnlocked", 0) == 1)
        {
            rocketUnlocked = true;
        }
    }

    IEnumerator KillAllEnemies()
    {
        yield return new WaitForSeconds(3);
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject skelton in allEnemies)
        {
            skelton.transform.parent.SendMessage("KillThisEnemy");
        }
    }
}
