using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead, PreGame};
    static public GameState gameState;
    bool hasKilledAllEnemiesAfterPlayerDeath;

    public GameObject editorLight;

    public GameObject skeletonEnemy, flyingskullEnemy, redSkeleton, redFlyingSkull, toxicSkeleton, toxicFlyingSkull, boneBallEnemy;
    public GameObject[] enemySpawns;
    public GameObject[] flyingenemySpawns;
    public GameObject[] ballenemySpawns;

    public static int enemyCount;
    public int enemyCountMax;
    public static int enemiesKilledThisSession;

    public int chanceToSpawnSpecialSkeleton, chanceToSpawnSpecialFlying;

    public float spawnTimer, flyingSpawnTimer, ballSpawnTimer;
    float spawnTimerCurrent, flyingSpawnTimerCurrent, ballSpawnCurrent;

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

    // settings elements
    public UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController playerController;
    public Slider slider_MouseSens;
    public Text text_MouseSens;
    public Slider slider_MusicVolume;
    public Slider slider_SFXVolume;

    public static float MusicVolume, SFXVolume;

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

        // adds a listener to the slider
        slider_MouseSens.onValueChanged.AddListener(delegate { ValueChangeCheckOnMouseSensitivity(); });
        slider_MusicVolume.onValueChanged.AddListener(delegate { ValueChangeCheckOnMusicVolume(); });
        slider_SFXVolume.onValueChanged.AddListener(delegate { ValueChangeCheckOnSFXVolume(); });
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

                // spawning bone balls
                if (ballSpawnCurrent <= ballSpawnTimer)
                {
                    ballSpawnCurrent += Time.deltaTime;
                }
                else
                {
                    // SPAWN A SKELETON
                    foreach (GameObject spawner in ballenemySpawns)
                    {
                        if (spawner.GetComponent<EnemySpawner>().CanWeSpawnHere())
                        {
                            spawner.tag = "Ball_Spawn";
                        }
                        else
                        {
                            spawner.tag = "Untagged";
                        }
                    }

                    GameObject[] activeSkeletonSpawns = GameObject.FindGameObjectsWithTag("Ball_Spawn");
                    int rndLocation = Random.Range(0, activeSkeletonSpawns.Length - 1);

                    Instantiate(boneBallEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);

                    enemyCount++;

                    ballSpawnCurrent = 0;
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

        // saved sensitivity settings
        slider_MouseSens.value = PlayerPrefs.GetFloat("SavedSensitivity", 2);
        playerController.mouseLook.XSensitivity = slider_MouseSens.value;
        playerController.mouseLook.YSensitivity = slider_MouseSens.value;
        text_MouseSens.text = "Look Sensitivity: " + playerController.mouseLook.XSensitivity.ToString("F1");


        MusicVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 100);
        SFXVolume = PlayerPrefs.GetFloat("SavedSFXVolume", 100);
        slider_MusicVolume.value = MusicVolume;
        slider_SFXVolume.value = SFXVolume;
    }

    IEnumerator KillAllEnemies()
    {
        yield return new WaitForSeconds(3);
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject skelton in allEnemies)
        {
            skelton.transform.parent.SendMessage("KillThisEnemy", true);
        }
    }

    public void ValueChangeCheckOnMouseSensitivity()
    {
        playerController.mouseLook.XSensitivity = slider_MouseSens.value;
        playerController.mouseLook.YSensitivity = slider_MouseSens.value;
        text_MouseSens.text = "Look Sensitivity: " + slider_MouseSens.value.ToString("F1");
        PlayerPrefs.SetFloat("SavedSensitivity", slider_MouseSens.value);
    }

    public void ValueChangeCheckOnMusicVolume()
    {
        PlayerPrefs.SetFloat("SavedMusicVolume", slider_MusicVolume.value);
        MusicVolume = slider_MusicVolume.value;
    }

    public void ValueChangeCheckOnSFXVolume()
    {
        PlayerPrefs.SetFloat("SavedSFXVolume", slider_SFXVolume.value);
        SFXVolume = slider_SFXVolume.value;
    }

    public void IncreaseSensitivity()
    {
        slider_MouseSens.value += 0.1f;
        playerController.mouseLook.XSensitivity = slider_MouseSens.value;
        playerController.mouseLook.YSensitivity = slider_MouseSens.value;
    }

    public void DecreaseSensitivity()
    {
        slider_MouseSens.value -= 0.1f;
        playerController.mouseLook.XSensitivity = slider_MouseSens.value;
        playerController.mouseLook.YSensitivity = slider_MouseSens.value;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("we quit the game");
    }
}
