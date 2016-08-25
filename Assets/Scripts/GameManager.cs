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
    public static int storedGold = 400;

    public static int shotgunWeaponValue = 400, machinegunWeaponValue = 400;

    public static bool shotgunUnlocked, machinegunUnlocked;
    public Text shotgunEquipButtonText, machinegunEquipButtonText;

	// Use this for initialization
	void Start ()
    {
        gameState = GameState.PreGame;
        editorLight.SetActive(false);

        // checks to see if our guns are unlocked, then changes the UI buttons accordingly
        ChangeEquipButtonText();
	}
	
	// Update is called once per frame
	void Update ()
    {
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
}
