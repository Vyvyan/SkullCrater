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

    public int chanceToSpawnSpecialSkeleton, chanceToSpawnSpecialFlying;

    public float spawnTimer, flyingSpawnTimer;
    float spawnTimerCurrent, flyingSpawnTimerCurrent;

    public static int heldGold;
    public static int storedGold;
    public static int thisSessionGoldGained;
    public int goldValue, goldBonusAmount, goldBonusLevel;

    float gameTimer;

    public static int shotgunWeaponValue = 400, machinegunWeaponValue = 400;

    public static bool shotgunUnlocked, machinegunUnlocked;
    public Text shotgunEquipButtonText, machinegunEquipButtonText, storedGoldText;

	// Use this for initialization
	void Start ()
    {
        // LOAD OUR SAVED STUFF
        LoadPlayerPrefs();

        thisSessionGoldGained = 0;
        gameTimer = 0;

        gameState = GameState.PreGame;
        editorLight.SetActive(false);

        // checks to see if our guns are unlocked, then changes the UI buttons accordingly
        ChangeEquipButtonText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // updates gold on weapon screen
        if (gameState == GameState.PreGame)
        {
            storedGoldText.text = "Balance: " + storedGold.ToString() + " G";
        }

        if (gameState == GameState.Playing)
        {
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
            if (!hasKilledAllEnemiesAfterPlayerDeath)
            {
                StartCoroutine(KillAllEnemies());
                hasKilledAllEnemiesAfterPlayerDeath = true;
            }
        }

        // TEST STUFF
        if (Input.GetKeyDown(KeyCode.P))
        {
            storedGold += 1;
            PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
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
