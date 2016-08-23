using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead};
    static public GameState gameState;

    public GameObject skeletonEnemy, flyingskullEnemy;
    public GameObject[] enemySpawns;
    public GameObject[] flyingenemySpawns;

    public static int enemyCount;
    public int enemyCountMax;

    public float spawnTimer;
    float spawnTimerCurrent;

    public static int heldGold;
    public static int storedGold;

	// Use this for initialization
	void Start ()
    {
        gameState = GameState.Playing;
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
                    int rndLocation = Random.Range(0, enemySpawns.Length - 1);
                    Instantiate(skeletonEnemy, enemySpawns[rndLocation].transform.position, Quaternion.identity);
                    enemyCount++;

                    int rndLocationFlying = Random.Range(0, enemySpawns.Length - 1);
                    Instantiate(flyingskullEnemy, enemySpawns[rndLocationFlying].transform.position, Quaternion.identity);
                    enemyCount++;

                    spawnTimerCurrent = 0;
                }
            }
        }
	}
}
