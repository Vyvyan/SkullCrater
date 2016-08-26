using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead, PreGame};
    static public GameState gameState;

    public GameObject editorLight;

    public GameObject skeletonEnemy, flyingskullEnemy;
    public GameObject[] enemySpawns;
    public GameObject[] flyingenemySpawns;

    public static int enemyCount;
    public int enemyCountMax;

    public float spawnTimer;
    float spawnTimerCurrent;

    public static int heldGold;
    public static int storedGold;

    public static int shotgunWeaponValue = 400, machinegunWeaponValue = 400;

    public static bool shotgunUnlocked, machinegunUnlocked;
    public Text shotgunEquipButtonText, machinegunEquipButtonText, storedGoldText;

	// Use this for initialization
	void Start ()
    {
        // LOAD OUR SAVED STUFF
        LoadPlayerPrefs();

        gameState = GameState.PreGame;
        editorLight.SetActive(false);

        // checks to see if our guns are unlocked, then changes the UI buttons accordingly
        ChangeEquipButtonText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // updates gold on weapon screen
        storedGoldText.text = "Balance: " + storedGold.ToString() + " G";

        if (gameState == GameState.Playing)
        {
            if (enemyCount < enemyCountMax)
            {
                if (spawnTimerCurrent <= spawnTimer)
                {
                    spawnTimerCurrent += Time.deltaTime;
                }
                else
                {
                    /*
                    int rndLocation = Random.Range(0, enemySpawns.Length - 1);
                    Instantiate(skeletonEnemy, enemySpawns[rndLocation].transform.position, Quaternion.identity);
                    enemyCount++;
                    */

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
                    Instantiate(skeletonEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                    enemyCount++;

                    // SPAWN A SKULL
                    /*
                    int rndLocationFlying = Random.Range(0, enemySpawns.Length - 1);
                    Instantiate(flyingskullEnemy, flyingenemySpawns[rndLocationFlying].transform.position, Quaternion.identity);
                    enemyCount++;
                    */

                    spawnTimerCurrent = 0;
                }
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
}
