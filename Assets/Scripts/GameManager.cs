using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Steamworks;

public class GameManager : MonoBehaviour {

    public enum GameState { Playing, Dead, PreGame, EndGame};
    static public GameState gameState;
    public GameState stateForViewInEditor;
    bool hasKilledAllEnemiesAfterPlayerDeath;
    public Player playerScript;
    AudioSource playerAudio;

    public GameObject editorLight;

    public GameObject skeletonEnemy, flyingskullEnemy, redSkeleton, redFlyingSkull, toxicSkeleton, toxicFlyingSkull, boneBallEnemy, goldSkeletonEnemy, goldPrefab;
    public GameObject[] enemySpawns;
    public GameObject[] flyingenemySpawns;
    public GameObject[] ballenemySpawns;

    public GameObject group_MainMenu, group_Pistol, group_Shotgun, group_MachineGun, group_Rocket, group_Grenade;

    public static int enemyCount;
    public int enemyCountMax;
    public static int enemiesKilledThisSession;

    public static float grenadeExplosionRadius = 8, rocketExplosionRadius = 8;

    public int chanceToSpawnSpecialSkeleton, chanceToSpawnSpecialFlying, chanceToSpawnGoldSkeleton;

    public float spawnTimer, flyingSpawnTimer, ballSpawnTimer;
    float spawnTimerCurrent, flyingSpawnTimerCurrent, ballSpawnCurrent;

    public static int heldGold;
    public static int storedGold;
    public static int thisSessionGoldGained;
    public int goldValue, goldBonusAmount, goldBonusLevel;

    public static string enemyThatKilledPlayer;

    public bool disableSkeletonSpawning, disableFlyingSkullSpawning, disableBoneBallSpawning;

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
    public bool invertYbool;
    public Toggle toggle_YInvert;

    public static float MusicVolume, SFXVolume;

    // upgrading variables
    int Pistol_Upgrades_AmmoLevelMax = 10, Pistol_Upgrades_AmmoLevelCurrent = 0, Pistol_Upgrades_ReloadSpeedLevelMax = 3, Pistol_Upgrades_ReloadSpeedLevelCurrent = 0,
        Shotgun_Upgrades_AmmoLevelMax = 8, Shotgun_Upgrades_AmmoLevelCurrent = 0, Shotgun_Upgrades_ReloadSpeedLevelMax = 8, Shotgun_Upgrades_ReloadSpeedLevelCurrent = 0,
        MachineGun_Upgrades_AmmoLevelMax = 8, MachineGun_Upgrades_AmmoLevelCurrent = 0, MachineGun_Upgrades_ReloadSpeedLevelMax = 6, MachineGun_Upgrades_ReloadSpeedLevelCurrent = 0,
        MachineGun_Upgrades_ROFLevelMax = 5, MachineGun_Upgrades_ROFLevelCurrent = 0, Rocket_Upgrades_AmmoLevelMax = 1, Rocket_Upgrades_AmmoLevelCurrent = 0, Rocket_Upgrades_ReloadSpeedLevelMax = 6, 
        Rocket_Upgrades_ReloadSpeedLevelCurrent = 0, Rocket_Upgrades_RadiusMax = 3, Rocket_Upgrades_RadiusCurrent = 0, Grenade_Upgrades_RadiusMax = 3, Grenade_Upgrades_RadiusCurrent = 0, Grenade_Upgrades_RechargeRateMax = 5,
        Grenade_Upgrades_RechargeRateCurrent = 0;

    int Pistol_StartingAmmo = 10, Shotgun_StartingAmmo = 4, MachineGun_StartingAmmo = 20, Rocket_StartingAmmo = 1, Grenade_StartingRechargeRate = 5;
    float Pistol_StartingReloadSpeed = 1.6f, Shotgun_StartingReloadSpeed = 3, MachineGun_StartingReloadSpeed = 3, MachineGun_StartingROF = .2f, Rocket_StartingReloadSpeed = 4, Rocket_StartingRadius = 8, Grenade_StartingRadius = 8;
    public Text Pistol_Upgrades_AmmoText, Pistol_Upgrades_ReloadSpeedText, Shotgun_Upgrades_AmmoText, Shotgun_Upgrades_ReloadSpeedText, MachineGun_Upgrades_AmmoText, MachineGun_Upgrades_ReloadSpeedText,
        MachineGun_Upgrades_ROFText, Rocket_Upgrades_AmmoText, Rocket_Upgrades_ReloadSpeedText, Rocket_Upgrades_RadiusText, Grenade_Upgrades_RadiusText, Grenade_Upgrades_RechargeRateText;

    // text for displaying how much each upgrade costs
    public Text Pistol_Upgrades_CostText, Shotgun_Upgrades_CostText, MachineGun_Upgrades_CostText, Rocket_Upgrades_CostText, Grenade_Upgrades_CostText;

    public Toggle[] Pistol_Upgrades_Ammo_Toggles, Pistol_Upgrades_ReloadSpeed_Toggles, Shotgun_Upgrades_Ammo_Toggles, Shotgun_Upgrades_ReloadSpeed_Toggles, MachineGun_Upgrades_Ammo_Toggles, MachineGun_Upgrades_ReloadSpeed_Toggles,
        MachineGun_Upgrades_ROF_Toggles, Rocket_Upgrades_Ammo_Toggles, Rocket_Upgrades_ReloadSpeed_Toggles, Rocket_Upgrades_Radius_Toggles, Grenade_Upgrades_Radius_Toggles, Grenade_Upgrades_RechargeRate_Toggles;

    public bool slowMotion;
    float slowMoTimer;
    float specialModeTimer;
    float slowMotionCoolDown;
    public float slowMoCD;
    public static bool canGoSlowMo;

    public static int stat_Deaths, stat_SkeltinsKilled, stat_GoldSkeltinsKilled, stat_RedSkeltinsKilled, stat_ToxicSkeltinsKilled, stat_SkellsKilled, stat_RedSkellsKilled, stat_ToxicSkellsKilled, stat_BoneBallsKilled, stat_LargestSingleDeposit,
        stat_MostGoldDropped, stat_MostGoldInARun, stat_SkellLordsKilled, stat_TotalGoldGained;
    public static float stat_LongestTimeSurvived;
    public Text statsText, eventTextObject, specialTimerText, careerStatsHeaderText;

    // special events
    public GameObject crystalSkull;
    public int killsToSpawnCrystalSkull;
    bool hasSpawnedCrystalSkull;
    public enum GameMode {normal, GoldSkeletonMode, BoneBallMode, SpeedySkeletonMode, FlyingSkeletonMode, HordeSkeletonMode, Boss};
    public GameMode gameMode;

    // boss mode
    public float timeToSpawnBoss;
    public static bool isBossDead;
    public Animator bossAnimator;

    // terrain
    public GameObject terrain;

    // to make upgrade code easier
    int pistolTotalUpgradeLevel, shotgunTotalUpgradeLevel, machineGunTotalUpgradeLevel, rocketTotalUpgradeLevel, grenadeTotalUpgradeLevel;

    // shiplights
    public Light[] shipLights;
    public GameObject[] shipAesthetics;

    // reset saved data protector
    public bool resetProtector;
    public Text resetProtectorText;
    float resetTABHoldTimer;

    // radio
    public Animator radioAnim;
    public AudioSource radioAudioSource;
    public ParticleSystem particle_MusicNotes1, particle_MusicNotes2, particle_MusicNotes3;

    // music
    AudioSource musicSource;

    // bloop saving stuff
    public GameObject specialCrate, craterBloop, shipBloop, musicSwapButton;
    public AudioClip blipBloopTheme;

    // wraith skeltins
    public GameObject wraithSkeltin;
    public float wraithSkeltinTimer_Max;
    float wraithSkeltinTimer_Current;

    // Posters
    public GameObject wraithPoster, boneBallPoster;
    public bool hasBeenInvaded;

    // Use this for initialization
    void Start ()
    {
        // game mode
        gameMode = GameMode.normal;
        // turns off our special mode timer at the start
        specialTimerText.gameObject.SetActive(false);

        isBossDead = false;

        slowMotionCoolDown = slowMoCD;
        canGoSlowMo = true;
        
        // make sure our slow mo is off
        Time.timeScale = 1;
        Time.fixedDeltaTime = .02f;

        // resets the static variables (wouldn't update when you reset saved data)
        shotgunUnlocked = false;
        machinegunUnlocked = false;
        rocketUnlocked = false;
        heldGold = 0;

        //starts with all enemies able to be spawned
        disableBoneBallSpawning = disableFlyingSkullSpawning = disableSkeletonSpawning = false;

        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerAudio = playerScript.gameObject.GetComponent<AudioSource>();
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
        toggle_YInvert.onValueChanged.AddListener(delegate { ValueChangeCheckOnYInvertBool(invertYbool); });

        enemyCount = 0;

        // randomizes our kills to spawn an anomolous skull
        killsToSpawnCrystalSkull = UnityEngine.Random.Range(100, 280);

        // change header to steam name on stats panel
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            careerStatsHeaderText.text = name + "'s Career Statistics";
        }

        resetProtector = false;
        resetTABHoldTimer = 0;

        // music
        musicSource = gameObject.GetComponent<AudioSource>();

        hasSpawnedCrystalSkull = false;
        hasBeenInvaded = false;
    }

    // Update is called once per frame
    void Update ()
    {
        /*
        // DEBUG PLEASE REMOVE
        if (Input.GetKeyDown(KeyCode.P))
        {
            goldValue = 500;
            DisplayEventText("Gold value increased to 500, ya' cheater.");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            timeToSpawnBoss = 10;
            DisplayEventText("Skell Lord will spawn at 10 seconds.");
        }
        */

        // Y INVERTED CONTROLS, CHANGE IT EVERY FRAME CAUSE SCREW IT THIS IS THE EASIEST WAY
        if (invertYbool)
        {
            playerController.mouseLook.YSensitivity = slider_MouseSens.value * -1;
        }
        else
        {
            playerController.mouseLook.YSensitivity = slider_MouseSens.value;
        }
        
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (Application.isEditor)
            {
                SteamUserStats.ResetAllStats(true);
                DisplayEventText("Resetting Steam Achievements");
            }
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            if (Application.isEditor)
            {
                PlayerPrefs.SetInt("BloopInShip", 0);
                DisplayEventText("Reset Bloop state");
            }
        }




        // RESETTING DATA
        if (resetProtector)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                resetTABHoldTimer += Time.deltaTime;
            }

            if (resetTABHoldTimer > 5)
            {
                ResetSavedData();
            }
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            resetTABHoldTimer = 0;
        }

        // seeing gamestate in editor
        stateForViewInEditor = gameState;
        // if we are slow motion
        if (slowMotion)
        {
            // if the unscaled time is greater than our timer + the time we want to be slow mo, stop slo mo
            if (Time.unscaledTime > slowMoTimer + 2.2f)
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = .02f;
                slowMotion = false;
            }
        }

        // slo mo cool down
        if (slowMotionCoolDown > 0)
        {
            canGoSlowMo = false;
            // count it down
            slowMotionCoolDown -= Time.deltaTime;
        }
        else
        {
            canGoSlowMo = true;
        }

        // updates gold on weapon screen
        if (gameState == GameState.PreGame)
        {
            storedGoldText.text = "Balance: " + storedGold.ToString() + " G";

            Pistol_Upgrades_AmmoText.text = (Pistol_StartingAmmo + (2 * Pistol_Upgrades_AmmoLevelCurrent)).ToString("f0");
            Pistol_Upgrades_ReloadSpeedText.text = (Pistol_StartingReloadSpeed - (.2f * Pistol_Upgrades_ReloadSpeedLevelCurrent)).ToString("f1") + " Seconds";
            Shotgun_Upgrades_AmmoText.text = (Shotgun_StartingAmmo + (2 * Shotgun_Upgrades_AmmoLevelCurrent)).ToString("f0");
            Shotgun_Upgrades_ReloadSpeedText.text = (Shotgun_StartingReloadSpeed - (.2f * Shotgun_Upgrades_ReloadSpeedLevelCurrent)).ToString("f1") + " Seconds";
            MachineGun_Upgrades_AmmoText.text = (MachineGun_StartingAmmo + (2 * MachineGun_Upgrades_AmmoLevelCurrent)).ToString();
            MachineGun_Upgrades_ReloadSpeedText.text = (MachineGun_StartingReloadSpeed - (.2f * MachineGun_Upgrades_ReloadSpeedLevelCurrent)).ToString() + " Seconds";
            MachineGun_Upgrades_ROFText.text = (MachineGun_StartingROF - (.025f * MachineGun_Upgrades_ROFLevelCurrent)).ToString() + " Seconds";
            Rocket_Upgrades_AmmoText.text = (Rocket_StartingAmmo + (1 * Rocket_Upgrades_AmmoLevelCurrent)).ToString();
            Rocket_Upgrades_ReloadSpeedText.text = (Rocket_StartingReloadSpeed - (.2f * Rocket_Upgrades_ReloadSpeedLevelCurrent)).ToString();
            Rocket_Upgrades_RadiusText.text = (Rocket_StartingRadius + (2 * Rocket_Upgrades_RadiusCurrent)).ToString();
            Grenade_Upgrades_RadiusText.text = (Grenade_StartingRadius + (2 * Grenade_Upgrades_RadiusCurrent)).ToString();
            Grenade_Upgrades_RechargeRateText.text = (Grenade_StartingRechargeRate + (2 * Grenade_Upgrades_RechargeRateCurrent)).ToString();

            // update weapon upgrade total levels
            pistolTotalUpgradeLevel = (Pistol_Upgrades_AmmoLevelCurrent + Pistol_Upgrades_ReloadSpeedLevelCurrent) * 10;
            shotgunTotalUpgradeLevel = (Shotgun_Upgrades_AmmoLevelCurrent + Shotgun_Upgrades_ReloadSpeedLevelCurrent) * 10;
            machineGunTotalUpgradeLevel = (MachineGun_Upgrades_AmmoLevelCurrent + MachineGun_Upgrades_ReloadSpeedLevelCurrent + MachineGun_Upgrades_ROFLevelCurrent) * 10;
            rocketTotalUpgradeLevel = (Rocket_Upgrades_AmmoLevelCurrent + Rocket_Upgrades_RadiusCurrent + Rocket_Upgrades_ReloadSpeedLevelCurrent) * 10;
            grenadeTotalUpgradeLevel = (Grenade_Upgrades_RadiusCurrent + Grenade_Upgrades_RechargeRateCurrent) * 10;

            // updates volume
            musicSource.volume = MusicVolume / 105;
        }

        if (gameState == GameState.Playing)
        {
            // GAME TIMER STUFF
            if (!isBossDead)
            {
                gameTimer += Time.deltaTime;
                comparisonTimer = Mathf.RoundToInt(gameTimer);
            }
            // timer for special modes
            if (gameMode != GameMode.normal)
            {
                specialModeTimer -= Time.deltaTime;
                specialTimerText.text = "Anomaly Ends in: " + FormatSeconds(specialModeTimer);
            }

            // NORMAL GAME MODE DIFFICULTY INCREASE
            if (gameMode == GameMode.normal)
            {
                /*
                // increase difficulty
                if (comparisonTimer > 310)
                {
                    chanceToSpawnSpecialFlying = 18;
                    chanceToSpawnSpecialSkeleton = 18;
                    spawnTimer = .5f;
                    flyingSpawnTimer = 1.5f;
                    ballSpawnTimer = 12;
                }
                else if (comparisonTimer > 230)
                {
                    chanceToSpawnSpecialFlying = 12;
                    chanceToSpawnSpecialSkeleton = 12;
                    spawnTimer = .6f;
                    flyingSpawnTimer = 1.8f;
                    ballSpawnTimer = 14;
                }
                else if (comparisonTimer > 160)
                {
                    chanceToSpawnSpecialFlying = 10;
                    chanceToSpawnSpecialSkeleton = 10;
                    spawnTimer = .7f;
                    flyingSpawnTimer = 2f;
                    ballSpawnTimer = 15;
                }
                else if (comparisonTimer > 100)
                {
                    chanceToSpawnSpecialFlying = 6;
                    chanceToSpawnSpecialSkeleton = 6;
                    spawnTimer = .8f;
                    flyingSpawnTimer = 4f;
                    ballSpawnTimer = 20;
                }
                else if (comparisonTimer > 60)
                {
                    chanceToSpawnSpecialFlying = 3;
                    chanceToSpawnSpecialSkeleton = 3;
                    spawnTimer = 1f;
                    flyingSpawnTimer = 10f;
                    ballSpawnTimer = 30;
                }
                */

                // increase difficulty
                if (comparisonTimer > 310)
                {
                    chanceToSpawnSpecialFlying = 15;
                    chanceToSpawnSpecialSkeleton = 15;
                    spawnTimer = .5f;
                    flyingSpawnTimer = 1.7f;
                    ballSpawnTimer = 12;
                    wraithSkeltinTimer_Max = 5;
                }
                // relax a bit
                else if (comparisonTimer > 285)
                {
                    chanceToSpawnSpecialFlying = 10;
                    chanceToSpawnSpecialSkeleton = 10;
                    spawnTimer = .7f;
                    flyingSpawnTimer = 2.6f;
                    ballSpawnTimer = 17;
                    wraithSkeltinTimer_Max = 5;
                }
                // INCREASE AFTER 30
                else if (comparisonTimer > 240)
                {
                    chanceToSpawnSpecialFlying = 13;
                    chanceToSpawnSpecialSkeleton = 13;
                    spawnTimer = .6f;
                    flyingSpawnTimer = 2f;
                    ballSpawnTimer = 14;
                    wraithSkeltinTimer_Max = 5;
                }
                // relax a bit
                else if (comparisonTimer > 210)
                {
                    chanceToSpawnSpecialFlying = 8;
                    chanceToSpawnSpecialSkeleton = 8;
                    spawnTimer = .8f;
                    flyingSpawnTimer = 3.2f;
                    ballSpawnTimer = 18;
                    wraithSkeltinTimer_Max = 5;
                }
                // INCREASE AFTER 30
                else if (comparisonTimer > 165)
                {
                    chanceToSpawnSpecialFlying = 10;
                    chanceToSpawnSpecialSkeleton = 10;
                    spawnTimer = .7f;
                    flyingSpawnTimer = 2.5f;
                    ballSpawnTimer = 15;
                    wraithSkeltinTimer_Max = 1;
                }
                // relax a bit
                else if (comparisonTimer > 135)
                {
                    chanceToSpawnSpecialFlying = 4;
                    chanceToSpawnSpecialSkeleton = 4;
                    spawnTimer = .9f;
                    flyingSpawnTimer = 6f;
                    ballSpawnTimer = 25;
                    wraithSkeltinTimer_Max = 1;
                }
                // INCREASE AFTER 30
                else if (comparisonTimer > 90)
                {
                    chanceToSpawnSpecialFlying = 6;
                    chanceToSpawnSpecialSkeleton = 6;
                    spawnTimer = .8f;
                    flyingSpawnTimer = 4f;
                    ballSpawnTimer = 20;
                    wraithSkeltinTimer_Max = 1;
                }
                // BASE AFTER 60
                else if (comparisonTimer > 60)
                {
                    chanceToSpawnSpecialFlying = 3;
                    chanceToSpawnSpecialSkeleton = 3;
                    // We heavily decrease the skeltin spawn rate for 30 seconds, since there should be a ton already there from the initial high spawn rate
                    spawnTimer = 2.5f;
                    flyingSpawnTimer = 10f;
                    ballSpawnTimer = 30;
                    wraithSkeltinTimer_Max = 1;
                }


                // spawning crystal skulls
                // if we've killed more than the number to spawn the crystal skull
                if (!hasSpawnedCrystalSkull)
                {
                    if (enemiesKilledThisSession >= killsToSpawnCrystalSkull)
                    {
                        int rndLocation = UnityEngine.Random.Range(0, ballenemySpawns.Length - 1);
                        Instantiate(crystalSkull, ballenemySpawns[rndLocation].transform.position, Quaternion.identity);
                        // announce it to the player
                        DisplayEventText("An Anomalous Skull has appeared?!");
                        // once we spawn the skull, make the next skull spawn require more kills
                        killsToSpawnCrystalSkull += killsToSpawnCrystalSkull * 50000;
                        hasSpawnedCrystalSkull = true;
                    }
                }

                // switching to boss mode after X amount of time
                if (comparisonTimer >= timeToSpawnBoss)
                {
                    gameMode = GameMode.Boss;
                    KillAll();
                    disableBoneBallSpawning = true;
                    disableFlyingSkullSpawning = true;
                    disableSkeletonSpawning = true;
                    bossAnimator.SetTrigger("Spawn");
                    DisplayEventText("!!WARNING: Unknown Super Entity Has Appeared!!");              
                }
            }
            // SPECIAL MODE DIFFICULTY
            else if (gameMode == GameMode.HordeSkeletonMode)
            {
                disableBoneBallSpawning = true;
                disableFlyingSkullSpawning = true;
                spawnTimer = .2f;
                chanceToSpawnGoldSkeleton = 0;
                chanceToSpawnSpecialSkeleton = 0;
                enemyCountMax = 80;
                wraithSkeltinTimer_Max = 10;
            }
            else if (gameMode == GameMode.GoldSkeletonMode)
            {
                disableBoneBallSpawning = true;
                disableFlyingSkullSpawning = true;
                spawnTimer = .7f;
                chanceToSpawnGoldSkeleton = 50;
                enemyCountMax = 45;
            }
            else if (gameMode == GameMode.FlyingSkeletonMode)
            {
                disableBoneBallSpawning = true;
                disableSkeletonSpawning = true;
                flyingSpawnTimer = .8f;
                enemyCountMax = 50;
            }
            else if (gameMode == GameMode.SpeedySkeletonMode)
            {
                disableBoneBallSpawning = true;
                chanceToSpawnSpecialFlying = 100;
                chanceToSpawnSpecialSkeleton = 100;
                flyingSpawnTimer = 4f;
                spawnTimer = 1f;
                enemyCountMax = 35;
            }
            else if (gameMode == GameMode.BoneBallMode)
            {
                disableSkeletonSpawning = true;
                disableFlyingSkullSpawning = true;
                ballSpawnTimer = 2;
                enemyCountMax = 12;
            }

            if (enemyCount < enemyCountMax)
            {
                if (!disableSkeletonSpawning)
                {
                    // spawning skeletons
                    if (spawnTimerCurrent <= spawnTimer)
                    {
                        spawnTimerCurrent += Time.deltaTime;
                    }
                    else
                    {
                        // SPAWN A SKELETON
                        foreach (GameObject spawner in enemySpawns)
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
                        int rndLocation = UnityEngine.Random.Range(0, activeSkeletonSpawns.Length - 1);

                        // SHOULD WE SPAWN A SPECIAL SKELETON
                        int rndToSeeIfWeShouldSpawnSpecialSkele = UnityEngine.Random.Range(1, 101);
                        if (rndToSeeIfWeShouldSpawnSpecialSkele < chanceToSpawnSpecialSkeleton)
                        {
                            int rando = UnityEngine.Random.Range(1, 4);
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
                            // we are spawning a normal skeleton, but random one last time to see if it's a special gold one
                            int randoNumberForGoldSkeleton = UnityEngine.Random.Range(1, 101);
                            // if we are less than or equal to our chance to spawn golden skeleton, do so, otherwise, normal skeleton
                            if (randoNumberForGoldSkeleton <= chanceToSpawnGoldSkeleton)
                            {
                                Instantiate(goldSkeletonEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                            }
                            else
                            {
                                Instantiate(skeletonEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);
                            }
                        }

                        enemyCount++;

                        spawnTimerCurrent = 0;
                    }
                }

                if (!disableFlyingSkullSpawning)
                {
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
                        int rndLocation = UnityEngine.Random.Range(0, activeSkeletonSpawns.Length - 1);

                        // SHOULD WE SPAWN A SPECIAL SKELETON
                        int rndToSeeIfWeShouldSpawnSpecialSkele = UnityEngine.Random.Range(1, 101);
                        if (rndToSeeIfWeShouldSpawnSpecialSkele < chanceToSpawnSpecialFlying)
                        {
                            int rando = UnityEngine.Random.Range(1, 4);
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

                if (!disableBoneBallSpawning)
                {
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
                        int rndLocation = UnityEngine.Random.Range(0, activeSkeletonSpawns.Length - 1);

                        Instantiate(boneBallEnemy, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);

                        enemyCount++;

                        ballSpawnCurrent = 0;
                    }
                }
            }
            // IF WE ARE GREATER OR EQUAL TO OUR MAX ENEMY COUNT SPAWN SOME SPOOKY WRAITH SKELTINS
            /// WRAITH SKELTINS
            else
            {
                // only if we are in the normal mode
                if (gameMode == GameMode.normal || gameMode == GameMode.HordeSkeletonMode)
                {
                    if (wraithSkeltinTimer_Current < wraithSkeltinTimer_Max)
                    {
                        wraithSkeltinTimer_Current += Time.deltaTime;
                    }
                    else
                    {
                        // SPAWN A WRAITH SKELETON
                        foreach (GameObject spawner in enemySpawns)
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
                        int rndLocation = UnityEngine.Random.Range(0, activeSkeletonSpawns.Length - 1);

                        Instantiate(wraithSkeltin, activeSkeletonSpawns[rndLocation].transform.position, Quaternion.identity);

                        DisplayEventText("Invaded by Wraith Skeltin");
                        // this is just here to check if we should display the wraith skeltin poster
                        hasBeenInvaded = true;

                        wraithSkeltinTimer_Current = 0;
                    }
                }
            }
        }

        if (gameState == GameState.Dead)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // load our loading scene
                SceneManager.LoadScene(2);
            }
            if (!hasKilledAllEnemiesAfterPlayerDeath)
            {
                StartCoroutine(KillAllEnemies());
                hasKilledAllEnemiesAfterPlayerDeath = true;
            }

            // stop music on death
            if (musicSource.isPlaying)
            {
                //musicSource.Stop();
                musicSource.volume -= (.2f * Time.deltaTime);
            }
        }

        if (gameState == GameState.EndGame)
        {
            // stop music on endgame
            if (musicSource.isPlaying)
            {
                //musicSource.Stop();
                musicSource.volume -= (.2f * Time.deltaTime);
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
        int invertY_temp = PlayerPrefs.GetInt("InvertY", 0);
        if(invertY_temp == 0)
        {
            invertYbool = false;
            toggle_YInvert.isOn = invertYbool;
        }
        else
        {
            invertYbool = true;
            toggle_YInvert.isOn = invertYbool;
        }

        // volume settings
        MusicVolume = PlayerPrefs.GetFloat("SavedMusicVolume", 75);
        SFXVolume = PlayerPrefs.GetFloat("SavedSFXVolume", 75);
        slider_MusicVolume.value = MusicVolume;
        slider_SFXVolume.value = SFXVolume;

        // load weapon upgrades
        Pistol_Upgrades_AmmoLevelCurrent = PlayerPrefs.GetInt("Pistol_Ammo_Level", 0);
        Pistol_Upgrades_ReloadSpeedLevelCurrent = PlayerPrefs.GetInt("Pistol_ReloadSpeed_Level", 0);
        Shotgun_Upgrades_AmmoLevelCurrent = PlayerPrefs.GetInt("Shotgun_Ammo_Level", 0);
        Shotgun_Upgrades_ReloadSpeedLevelCurrent = PlayerPrefs.GetInt("Shotgun_ReloadSpeed_Level", 0);
        MachineGun_Upgrades_AmmoLevelCurrent = PlayerPrefs.GetInt("MachineGun_Ammo_Level", 0);
        MachineGun_Upgrades_ReloadSpeedLevelCurrent = PlayerPrefs.GetInt("MachineGun_ReloadSpeed_Level", 0);
        MachineGun_Upgrades_ROFLevelCurrent = PlayerPrefs.GetInt("MachineGun_ROF_Level", 0);
        Rocket_Upgrades_AmmoLevelCurrent = PlayerPrefs.GetInt("Rocket_Ammo_Level", 0);
        Rocket_Upgrades_ReloadSpeedLevelCurrent = PlayerPrefs.GetInt("Rocket_ReloadSpeed_Level", 0);
        Rocket_Upgrades_RadiusCurrent = PlayerPrefs.GetInt("Rocket_Radius_Level", 0);
        Grenade_Upgrades_RadiusCurrent = PlayerPrefs.GetInt("Grenade_Radius_Level", 0);
        Grenade_Upgrades_RechargeRateCurrent = PlayerPrefs.GetInt("Grenade_RechargeRate_Level", 0);

        // update the UI elements and the weapon values based on the unlocked levels
        UpdateWeaponValues();
        UpdateUIWeaponUpgradeToggles("all");

        // load stats
        stat_Deaths = PlayerPrefs.GetInt("stats_Deaths", 0);
        stat_BoneBallsKilled = PlayerPrefs.GetInt("stats_BoneBallsKilled", 0);
        stat_GoldSkeltinsKilled = PlayerPrefs.GetInt("stats_GoldSkeltinsKilled", 0);
        stat_LargestSingleDeposit = PlayerPrefs.GetInt("stats_LargestSingleDeposit", 0);
        stat_LongestTimeSurvived = PlayerPrefs.GetFloat("stats_LongestTimeSurvived", 0);
        stat_MostGoldDropped = PlayerPrefs.GetInt("stats_MostGoldDropped", 0);
        stat_MostGoldInARun = PlayerPrefs.GetInt("stats_MostGoldInARun", 0);
        stat_RedSkellsKilled = PlayerPrefs.GetInt("stats_RedSkellsKilled", 0);
        stat_RedSkeltinsKilled = PlayerPrefs.GetInt("stats_RedSkeltinsKilled", 0);
        stat_SkellsKilled = PlayerPrefs.GetInt("stats_SkellsKilled", 0);
        stat_SkeltinsKilled = PlayerPrefs.GetInt("stats_SkeltinsKilled", 0);
        stat_ToxicSkellsKilled = PlayerPrefs.GetInt("stats_ToxicSkellsKilled", 0);
        stat_ToxicSkeltinsKilled = PlayerPrefs.GetInt("stats_ToxicSkeltinsKilled", 0);
        stat_SkellLordsKilled = PlayerPrefs.GetInt("stats_SkellLordsKilled", 0);
        stat_TotalGoldGained = PlayerPrefs.GetInt("stats_TotalGoldGained", 0);

        // updates the Statistics text that displays the stats
        statsText.text = Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "Expeditions: " + stat_Deaths.ToString() + Environment.NewLine +
            //"Longest Time Survived: " + stat_LongestTimeSurvived.ToString("F1") + Environment.NewLine +
            "Longest Time Survived: " + FormatSeconds(stat_LongestTimeSurvived) + Environment.NewLine +
            "Most Gold in a Run: " + stat_MostGoldInARun.ToString() + Environment.NewLine +
            "Total Gold Gained: " + stat_TotalGoldGained.ToString() + Environment.NewLine +
            "Largest Single Gold Deposit: " + stat_LargestSingleDeposit.ToString() + Environment.NewLine +
            "Most Gold Dropped: " + stat_MostGoldDropped.ToString() + Environment.NewLine + Environment.NewLine +
            "Skeltins Killed: " + stat_SkeltinsKilled.ToString() + Environment.NewLine +
            "Gold Skeltins Killed: " + stat_GoldSkeltinsKilled.ToString() + Environment.NewLine +
            "Red Skeltins Killed: " + stat_RedSkeltinsKilled.ToString() + Environment.NewLine +
            "Toxic Skeltins Killed: " + stat_ToxicSkeltinsKilled.ToString() + Environment.NewLine +
            "Skells Killed: " + stat_SkellsKilled.ToString() + Environment.NewLine +
            "Red Skells Killed: " + stat_RedSkellsKilled.ToString() + Environment.NewLine +
            "Toxic Skells Killed: " + stat_ToxicSkellsKilled.ToString() + Environment.NewLine +
            "Bone Balls Killed: " + stat_BoneBallsKilled.ToString() + Environment.NewLine +
            "Skell Lords Killed: " + stat_SkellLordsKilled.ToString() + Environment.NewLine;

        // load radio state
        // if our radio is saved as off, turn it off
        if (PlayerPrefs.GetInt("RadioState", 1) == 0)
        {
            radioAudioSource.enabled = false;
            radioAnim.SetBool("RadioIsOn", false);
            particle_MusicNotes1.Stop();
            particle_MusicNotes2.Stop();
            particle_MusicNotes3.Stop();
        }
        else
        {
            radioAudioSource.enabled = true;
            radioAnim.SetBool("RadioIsOn", true);
            particle_MusicNotes1.Play();
            particle_MusicNotes2.Play();
            particle_MusicNotes3.Play();
        }

        // bloop is NOT in the ship, else he is
        if (PlayerPrefs.GetInt("BloopInShip", 0) == 0)
        {
            shipBloop.SetActive(false);
            musicSwapButton.SetActive(false);
            specialCrate.SetActive(true);
            craterBloop.SetActive(true);
        }
        else
        {
            shipBloop.SetActive(true);
            musicSwapButton.SetActive(true);
            specialCrate.SetActive(false);
            craterBloop.SetActive(false);
        } 

        // POSTERS
        if (stat_BoneBallsKilled > 0)
        {
            boneBallPoster.SetActive(true);
        }

        int wraithPosterInt = PlayerPrefs.GetInt("WraithPoster", 0);
        if (wraithPosterInt > 0)
        {
            wraithPoster.SetActive(true);
        }
    }

    public void SaveStatistics()
    {
        // save stats
        PlayerPrefs.SetInt("stats_Deaths", stat_Deaths);
        PlayerPrefs.SetInt("stats_BoneBallsKilled", stat_BoneBallsKilled);
        PlayerPrefs.SetInt("stats_GoldSkeltinsKilled", stat_GoldSkeltinsKilled);
        PlayerPrefs.SetInt("stats_LargestSingleDeposit", stat_LargestSingleDeposit);
        PlayerPrefs.SetFloat("stats_LongestTimeSurvived", stat_LongestTimeSurvived);
        PlayerPrefs.SetInt("stats_MostGoldDropped", stat_MostGoldDropped);
        PlayerPrefs.SetInt("stats_MostGoldInARun", stat_MostGoldInARun);
        PlayerPrefs.SetInt("stats_RedSkellsKilled", stat_RedSkellsKilled);
        PlayerPrefs.SetInt("stats_RedSkeltinsKilled", stat_RedSkeltinsKilled);
        PlayerPrefs.SetInt("stats_SkellsKilled", stat_SkellsKilled);
        PlayerPrefs.SetInt("stats_SkeltinsKilled", stat_SkeltinsKilled);
        PlayerPrefs.SetInt("stats_ToxicSkellsKilled", stat_ToxicSkellsKilled);
        PlayerPrefs.SetInt("stats_ToxicSkeltinsKilled", stat_ToxicSkeltinsKilled);
        PlayerPrefs.SetInt("stats_SkellLordsKilled", stat_SkellLordsKilled);
        PlayerPrefs.SetInt("stats_TotalGoldGained", stat_TotalGoldGained);

        // save stats to steam
        SteamUserStats.SetStat("number_of_runs", stat_Deaths);
        SteamUserStats.SetStat("longest_time_survived", stat_LongestTimeSurvived);
        SteamUserStats.SetStat("most_gold_in_a_run", stat_MostGoldInARun);
        SteamUserStats.SetStat("largest_gold_deposit", stat_LargestSingleDeposit);
        SteamUserStats.SetStat("most_gold_dropped", stat_MostGoldDropped);
        SteamUserStats.SetStat("skeltins_killed", stat_SkeltinsKilled);
        SteamUserStats.SetStat("gold_skeltins_killed", stat_GoldSkeltinsKilled);
        SteamUserStats.SetStat("red_skeltins_killed", stat_RedSkeltinsKilled);
        SteamUserStats.SetStat("toxic_skeltins_killed", stat_ToxicSkeltinsKilled);
        SteamUserStats.SetStat("skells_killed", stat_SkellsKilled);
        SteamUserStats.SetStat("red_skells_killed", stat_RedSkellsKilled);
        SteamUserStats.SetStat("toxic_skells_killed", stat_ToxicSkellsKilled);
        SteamUserStats.SetStat("bone_balls_killed", stat_BoneBallsKilled);
        SteamUserStats.SetStat("skell_lords_killed", stat_SkellLordsKilled);
        SteamUserStats.SetStat("total_gold", stat_TotalGoldGained);

    }

    IEnumerator KillAllEnemies()
    {
        yield return new WaitForSeconds(3);
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject skelton in allEnemies)
        {
            if (skelton.transform.parent.GetComponent<Enemy>() || skelton.transform.parent.GetComponent<BoneBall>() || skelton.transform.parent.GetComponent<FlyingSkull>())
            {
                skelton.transform.parent.SendMessage("KillThisEnemy", true);
            }
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

    public void ValueChangeCheckOnYInvertBool(bool isYInverted)
    {
        invertYbool = toggle_YInvert.isOn;
        if (invertYbool == true)
        {
            PlayerPrefs.SetInt("InvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertY", 0);
        }
        Debug.Log(isYInverted.ToString());
    }

    // changes the bool value for Y invert, and saves it as well - WE NOW USE THE VALUE CHANGED FUNCTION, BECAUSE IT ACTUALLY WORKS. IT SHOULD BE ABOVE ^^^^
    /*
    public void YInvertToggleChanged()
    {
        invertYbool = !invertYbool;
        if (invertYbool == true)
        {
            PlayerPrefs.SetInt("InvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("InvertY", 0);
        }
    }
    */

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

    public void UpgradeWeapon(string weaponToUpgrade)
    {
        
        playerAudio.PlayOneShot(AudioManager.accept, SFXVolume / 100);
        if (storedGold >= 25 + pistolTotalUpgradeLevel)
        { 
            if (weaponToUpgrade == "Pistol_Ammo")
            {
                if (Pistol_Upgrades_AmmoLevelCurrent < Pistol_Upgrades_AmmoLevelMax)
                {
                    storedGold -= 25 + pistolTotalUpgradeLevel;
                    Pistol_Upgrades_AmmoLevelCurrent++;
                    PlayerPrefs.SetInt("Pistol_Ammo_Level", Pistol_Upgrades_AmmoLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Pistol Ammo Capacity Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }  
            if (weaponToUpgrade == "Pistol_ReloadSpeed")
            {
                if (Pistol_Upgrades_ReloadSpeedLevelCurrent < Pistol_Upgrades_ReloadSpeedLevelMax)
                {
                    storedGold -= 25 + pistolTotalUpgradeLevel;
                    Pistol_Upgrades_ReloadSpeedLevelCurrent++;
                    PlayerPrefs.SetInt("Pistol_ReloadSpeed_Level", Pistol_Upgrades_ReloadSpeedLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Pistol Reload Speed Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
        }

        if (storedGold >= 25 + shotgunTotalUpgradeLevel)
        {
            if (weaponToUpgrade == "Shotgun_Ammo")
            {
                if (Shotgun_Upgrades_AmmoLevelCurrent < Shotgun_Upgrades_AmmoLevelMax)
                {
                    Shotgun_Upgrades_AmmoLevelCurrent++;
                    storedGold -= 25 + shotgunTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Shotgun_Ammo_Level", Shotgun_Upgrades_AmmoLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Shotgun Ammo Capacity Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }

            if (weaponToUpgrade == "Shotgun_ReloadSpeed")
            {
                if (Shotgun_Upgrades_ReloadSpeedLevelCurrent < Shotgun_Upgrades_ReloadSpeedLevelMax)
                {
                    Shotgun_Upgrades_ReloadSpeedLevelCurrent++;
                    storedGold -= 25 + shotgunTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Shotgun_ReloadSpeed_Level", Shotgun_Upgrades_ReloadSpeedLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Shotgun Reload Speed Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
        }

        if (storedGold >= 25 + machineGunTotalUpgradeLevel)
        {
            if (weaponToUpgrade == "MachineGun_Ammo")
            {
                if (MachineGun_Upgrades_AmmoLevelCurrent < MachineGun_Upgrades_AmmoLevelMax)
                {
                    MachineGun_Upgrades_AmmoLevelCurrent++;
                    storedGold -= 25 + machineGunTotalUpgradeLevel;
                    PlayerPrefs.SetInt("MachineGun_Ammo_Level", MachineGun_Upgrades_AmmoLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Machine Gun Ammo Capacity Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
            if (weaponToUpgrade == "MachineGun_ReloadSpeed")
            {
                if (MachineGun_Upgrades_ReloadSpeedLevelCurrent < MachineGun_Upgrades_ReloadSpeedLevelMax)
                {
                    MachineGun_Upgrades_ReloadSpeedLevelCurrent++;
                    storedGold -= 25 + machineGunTotalUpgradeLevel;
                    PlayerPrefs.SetInt("MachineGun_ReloadSpeed_Level", MachineGun_Upgrades_ReloadSpeedLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Machine Gun Reload Speed Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
            if (weaponToUpgrade == "MachineGun_ROF")
            {
                if (MachineGun_Upgrades_ROFLevelCurrent < MachineGun_Upgrades_ROFLevelMax)
                {
                    MachineGun_Upgrades_ROFLevelCurrent++;
                    storedGold -= 25 + machineGunTotalUpgradeLevel;
                    PlayerPrefs.SetInt("MachineGun_ROF_Level", MachineGun_Upgrades_ROFLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Machine Gun Rate of Fire Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
        }

        if (storedGold >= 25 + rocketTotalUpgradeLevel)
        {
            if (weaponToUpgrade == "Rocket_Ammo")
            {
                if (Rocket_Upgrades_AmmoLevelCurrent < Rocket_Upgrades_AmmoLevelMax)
                {
                    Rocket_Upgrades_AmmoLevelCurrent++;
                    storedGold -= 25 + rocketTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Rocket_Ammo_Level", Rocket_Upgrades_AmmoLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Rocket Ammo Capacity Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
            if (weaponToUpgrade == "Rocket_ReloadSpeed")
            {
                if (Rocket_Upgrades_ReloadSpeedLevelCurrent < Rocket_Upgrades_ReloadSpeedLevelMax)
                {
                    Rocket_Upgrades_ReloadSpeedLevelCurrent++;
                    storedGold -= 25 + rocketTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Rocket_ReloadSpeed_Level", Rocket_Upgrades_ReloadSpeedLevelCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Rocket Reload Speed Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
            if (weaponToUpgrade == "Rocket_Radius")
            {
                if (Rocket_Upgrades_RadiusCurrent < Rocket_Upgrades_RadiusMax)
                {
                    Rocket_Upgrades_RadiusCurrent++;
                    storedGold -= 25 + rocketTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Rocket_Radius_Level", Rocket_Upgrades_RadiusCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Rocket Explosion Radius Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
        }

        if (storedGold >= 25 + grenadeTotalUpgradeLevel)
        {
            if (weaponToUpgrade == "Grenade_Radius")
            {
                if (Grenade_Upgrades_RadiusCurrent < Grenade_Upgrades_RadiusMax)
                {
                    Grenade_Upgrades_RadiusCurrent++;
                    storedGold -= 25 + grenadeTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Grenade_Radius_Level", Grenade_Upgrades_RadiusCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Grenade Explosion Radius Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
            if (weaponToUpgrade == "Grenade_RechargeRate")
            {
                if (Grenade_Upgrades_RechargeRateCurrent < Grenade_Upgrades_RechargeRateMax)
                {
                    Grenade_Upgrades_RechargeRateCurrent++;
                    storedGold -= 25 + grenadeTotalUpgradeLevel;
                    PlayerPrefs.SetInt("Grenade_RechargeRate_Level", Grenade_Upgrades_RechargeRateCurrent);
                    UpdateWeaponValues();
                    UpdateUIWeaponUpgradeToggles(weaponToUpgrade);
                    DisplayEventText("Grenade Recharge Rate Upgraded!");
                    PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                }
            }
        }
    }

    public void UpdateWeaponValues()
    {
        // pistol
        playerScript.pistolAmmoMax = Pistol_StartingAmmo + (2 * Pistol_Upgrades_AmmoLevelCurrent);
        playerScript.pistolAmmo = playerScript.pistolAmmoMax;
        playerScript.reloadSpeed_Pistol = (Pistol_StartingReloadSpeed - (.2f * Pistol_Upgrades_ReloadSpeedLevelCurrent));
        // shotgun
        playerScript.shotgunAmmoMax =Shotgun_StartingAmmo + (2 * Shotgun_Upgrades_AmmoLevelCurrent);
        playerScript.shotgunAmmo = playerScript.shotgunAmmoMax;
        playerScript.reloadSpeed_Shotgun = (Shotgun_StartingReloadSpeed - (.2f * Shotgun_Upgrades_ReloadSpeedLevelCurrent));
        // machine gun
        playerScript.machinegunAmmoMax = MachineGun_StartingAmmo + (2 * MachineGun_Upgrades_AmmoLevelCurrent);
        playerScript.machinegunAmmo = playerScript.machinegunAmmoMax;
        playerScript.reloadSpeed_Machinegun = (MachineGun_StartingReloadSpeed - (.2f * MachineGun_Upgrades_ReloadSpeedLevelCurrent));
        playerScript.machinegunFireRate = (MachineGun_StartingROF - (.025f * MachineGun_Upgrades_ROFLevelCurrent));
        // Rocket
        playerScript.rocketAmmoMax = Rocket_StartingAmmo + (1 * Rocket_Upgrades_AmmoLevelCurrent);
        playerScript.rocketAmmo = playerScript.rocketAmmoMax;
        playerScript.reloadSpeed_Rocket = (Rocket_StartingReloadSpeed - (.2f * Rocket_Upgrades_ReloadSpeedLevelCurrent));
        rocketExplosionRadius = (Rocket_StartingRadius + (2 * Rocket_Upgrades_RadiusCurrent));
        // Grenade
        grenadeExplosionRadius = (Grenade_StartingRadius + (2 * Grenade_Upgrades_RadiusCurrent));
        playerScript.grenadeJuicePerKill = (Grenade_StartingRechargeRate + (2 * Grenade_Upgrades_RechargeRateCurrent));
    }

    public void UpdateUIWeaponUpgradeToggles(string weaponToUpgrade)
    {
        // DO THIS HERE AS WELL BECAUSE ITS ALL BORKED
        pistolTotalUpgradeLevel = (Pistol_Upgrades_AmmoLevelCurrent + Pistol_Upgrades_ReloadSpeedLevelCurrent) * 10;
        shotgunTotalUpgradeLevel = (Shotgun_Upgrades_AmmoLevelCurrent + Shotgun_Upgrades_ReloadSpeedLevelCurrent) * 10;
        machineGunTotalUpgradeLevel = (MachineGun_Upgrades_AmmoLevelCurrent + MachineGun_Upgrades_ReloadSpeedLevelCurrent + MachineGun_Upgrades_ROFLevelCurrent) * 10;
        rocketTotalUpgradeLevel = (Rocket_Upgrades_AmmoLevelCurrent + Rocket_Upgrades_RadiusCurrent + Rocket_Upgrades_ReloadSpeedLevelCurrent) * 10;
        grenadeTotalUpgradeLevel = (Grenade_Upgrades_RadiusCurrent + Grenade_Upgrades_RechargeRateCurrent) * 10;

        if (weaponToUpgrade == "Pistol_Ammo")
        {
            int i = Pistol_Upgrades_AmmoLevelCurrent;
            while(i > 0)
            {
                Pistol_Upgrades_Ammo_Toggles[i - 1].isOn = true;
                i--;
            }

            Pistol_Upgrades_CostText.text = "Upgrade Cost: " + (25 + pistolTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Pistol_ReloadSpeed")
        {
            int i = Pistol_Upgrades_ReloadSpeedLevelCurrent;
            while (i > 0)
            {
                Pistol_Upgrades_ReloadSpeed_Toggles[i - 1].isOn = true;
                i--;
            }

            Pistol_Upgrades_CostText.text = "Upgrade Cost: " + (25 + pistolTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Shotgun_Ammo")
        {
            int i = Shotgun_Upgrades_AmmoLevelCurrent;
            while (i > 0)
            {
                Shotgun_Upgrades_Ammo_Toggles[i - 1].isOn = true;
                i--;
            }

            Shotgun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + shotgunTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Shotgun_ReloadSpeed")
        {
            int i = Shotgun_Upgrades_ReloadSpeedLevelCurrent;
            while (i > 0)
            {
                Shotgun_Upgrades_ReloadSpeed_Toggles[i - 1].isOn = true;
                i--;
            }

            Shotgun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + shotgunTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "MachineGun_Ammo")
        {
            int i = MachineGun_Upgrades_AmmoLevelCurrent;
            while (i > 0)
            {
                MachineGun_Upgrades_Ammo_Toggles[i - 1].isOn = true;
                i--;
            }

            MachineGun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + machineGunTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "MachineGun_ReloadSpeed")
        {
            int i = MachineGun_Upgrades_ReloadSpeedLevelCurrent;
            while (i > 0)
            {
                MachineGun_Upgrades_ReloadSpeed_Toggles[i - 1].isOn = true;
                i--;
            }
            MachineGun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + machineGunTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "MachineGun_ROF")
        {
            int i = MachineGun_Upgrades_ROFLevelCurrent;
            while (i > 0)
            {
                MachineGun_Upgrades_ROF_Toggles[i - 1].isOn = true;
                i--;
            }
            MachineGun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + machineGunTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Rocket_Ammo")
        {
            int i = Rocket_Upgrades_AmmoLevelCurrent;
            while (i > 0)
            {
                Rocket_Upgrades_Ammo_Toggles[i - 1].isOn = true;
                i--;
            }

            Rocket_Upgrades_CostText.text = "Upgrade Cost: " + (25 + rocketTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Rocket_ReloadSpeed")
        {
            int i = Rocket_Upgrades_ReloadSpeedLevelCurrent;
            while (i > 0)
            {
                Rocket_Upgrades_ReloadSpeed_Toggles[i - 1].isOn = true;
                i--;
            }

            Rocket_Upgrades_CostText.text = "Upgrade Cost: " + (25 + rocketTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Rocket_Radius")
        {
            int i = Rocket_Upgrades_RadiusCurrent;
            while (i > 0)
            {
                Rocket_Upgrades_Radius_Toggles[i - 1].isOn = true;
                i--;
            }

            Rocket_Upgrades_CostText.text = "Upgrade Cost: " + (25 + rocketTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Grenade_Radius")
        {
            int i = Grenade_Upgrades_RadiusCurrent;
            while (i > 0)
            {
                Grenade_Upgrades_Radius_Toggles[i - 1].isOn = true;
                i--;
            }

            Grenade_Upgrades_CostText.text = "Upgrade Cost: " + (25 + grenadeTotalUpgradeLevel).ToString();
        }
        else if (weaponToUpgrade == "Grenade_RechargeRate")
        {
            int i = Grenade_Upgrades_RechargeRateCurrent;
            while (i > 0)
            {
                Grenade_Upgrades_RechargeRate_Toggles[i - 1].isOn = true;
                i--;
            }

            Grenade_Upgrades_CostText.text = "Upgrade Cost: " + (25 + grenadeTotalUpgradeLevel).ToString();
        }

        if (weaponToUpgrade == "all")
        {
            UpdateUIWeaponUpgradeToggles("Pistol_Ammo");
            UpdateUIWeaponUpgradeToggles("Pistol_ReloadSpeed");
            UpdateUIWeaponUpgradeToggles("Shotgun_Ammo");
            UpdateUIWeaponUpgradeToggles("Shotgun_ReloadSpeed");
            UpdateUIWeaponUpgradeToggles("MachineGun_ReloadSpeed");
            UpdateUIWeaponUpgradeToggles("MachineGun_Ammo");
            UpdateUIWeaponUpgradeToggles("MachineGun_ROF");
            UpdateUIWeaponUpgradeToggles("Rocket_ReloadSpeed");
            UpdateUIWeaponUpgradeToggles("Rocket_Ammo");
            UpdateUIWeaponUpgradeToggles("Rocket_Radius");
            UpdateUIWeaponUpgradeToggles("Grenade_Radius");
            UpdateUIWeaponUpgradeToggles("Grenade_RechargeRate");

            Grenade_Upgrades_CostText.text = "Upgrade Cost: " + (25 + grenadeTotalUpgradeLevel).ToString();
            Rocket_Upgrades_CostText.text = "Upgrade Cost: " + (25 + rocketTotalUpgradeLevel).ToString();
            MachineGun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + machineGunTotalUpgradeLevel).ToString();
            Shotgun_Upgrades_CostText.text = "Upgrade Cost: " + (25 + shotgunTotalUpgradeLevel).ToString();
            Pistol_Upgrades_CostText.text = "Upgrade Cost: " + (25 + pistolTotalUpgradeLevel).ToString();
        }
    }

    public void ChangeMenus(string menuName)
    {
        playerAudio.PlayOneShot(AudioManager.accept, SFXVolume / 100);
        if (menuName == "Main")
        {
            group_MainMenu.SetActive(true);
            group_Pistol.SetActive(false);
            group_Shotgun.SetActive(false);
            group_MachineGun.SetActive(false);
            group_Rocket.SetActive(false);
            group_Grenade.SetActive(false);
        }
        if (menuName == "Pistol")
        {
            group_MainMenu.SetActive(false);
            group_Pistol.SetActive(true);
            group_Shotgun.SetActive(false);
            group_MachineGun.SetActive(false);
            group_Rocket.SetActive(false);
            group_Grenade.SetActive(false);
        }
        if (menuName == "Shotgun")
        {
            if (shotgunUnlocked)
            {
                group_MainMenu.SetActive(false);
                group_Pistol.SetActive(false);
                group_Shotgun.SetActive(true);
                group_MachineGun.SetActive(false);
                group_Rocket.SetActive(false);
                group_Grenade.SetActive(false);
            }
        }
        if (menuName == "MachineGun")
        {
            if (machinegunUnlocked)
            {
                group_MainMenu.SetActive(false);
                group_Pistol.SetActive(false);
                group_Shotgun.SetActive(false);
                group_MachineGun.SetActive(true);
                group_Rocket.SetActive(false);
                group_Grenade.SetActive(false);
            }
        }
        if (menuName == "Rocket")
        {
            if (rocketUnlocked)
            {
                group_MainMenu.SetActive(false);
                group_Pistol.SetActive(false);
                group_Shotgun.SetActive(false);
                group_MachineGun.SetActive(false);
                group_Rocket.SetActive(true);
                group_Grenade.SetActive(false);
            }
        }
        if (menuName == "Grenade")
        {
            group_MainMenu.SetActive(false);
            group_Pistol.SetActive(false);
            group_Shotgun.SetActive(false);
            group_MachineGun.SetActive(false);
            group_Rocket.SetActive(false);
            group_Grenade.SetActive(true);
        }
    }

    public void ResetSavedData()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(2);
    }

    public void ToggleResetSavedDataProtector()
    {
        resetProtector = !resetProtector;
        if (resetProtector)
        {
            resetProtectorText.gameObject.SetActive(true);
        }
        else
        {
            resetProtectorText.gameObject.SetActive(false);
        }
    }

    public void SlowMo()
    {
        if (canGoSlowMo)
        {
            slowMoTimer = Time.unscaledTime;
            Time.timeScale = .25f;
            Time.fixedDeltaTime = .005f;
            slowMotion = true;
            slowMotionCoolDown = slowMoCD;
        }
    }

    public void DisplayEventText(String eventText)
    {
        StartCoroutine(EventTextIEnum(eventText));
    }

    IEnumerator EventTextIEnum(String eventText)
    {
        eventTextObject.text = eventText;
        yield return new WaitForSeconds(5f);
        eventTextObject.text = "";
    }

    public void StartSpecialGameMode(int modeIndex)
    {
        // relay this to the special game mode ienum
        StartCoroutine(SpecialGameMode(modeIndex));
    }

    public IEnumerator SpecialGameMode(int modeIndex)
    {
        float timeLimitOnMode = 0;

        // change the actual game mode
        if (modeIndex == 1)
        {
            gameMode = GameMode.GoldSkeletonMode;
            timeLimitOnMode = 40;
            DisplayEventText("The Skull is Engraved. It reads:" + Environment.NewLine + "Lady Luck shines upon you");
            // achievement
            SteamUserStats.SetAchievement("Anom_Gold");
            SteamUserStats.StoreStats();
        }
        else if (modeIndex >= 2 && modeIndex <= 7)
        {
            gameMode = GameMode.BoneBallMode;
            timeLimitOnMode = 90;
            DisplayEventText("The Skull is Engraved. It reads:" + Environment.NewLine + "Roll The Bones");
        }
        else if (modeIndex >= 8 && modeIndex <= 13)
        {
            gameMode = GameMode.FlyingSkeletonMode;
            timeLimitOnMode = 90;
            DisplayEventText("The Skull is Engraved. It reads:" + Environment.NewLine + "Death From Above");
        }
        else if (modeIndex >= 14 && modeIndex <= 19)
        {
            gameMode = GameMode.HordeSkeletonMode;
            timeLimitOnMode = 90;
            DisplayEventText("The Skull is Engraved. It reads:" + Environment.NewLine + "Overwhelming Darkness");

        }
        else if (modeIndex >= 20 && modeIndex <= 25)
        {
            gameMode = GameMode.SpeedySkeletonMode;
            timeLimitOnMode = 90;
            DisplayEventText("The Skull is Engraved. It reads:" + Environment.NewLine + "Zoom, Boom, Doom and Gloom");
        }
        // show timer
        specialTimerText.gameObject.SetActive(true);
        // set timer
        specialModeTimer = timeLimitOnMode;


        yield return new WaitForSeconds(timeLimitOnMode);

        // resets some variables that won't get reset otherwise when switching back to normal mode
        disableBoneBallSpawning = disableFlyingSkullSpawning = disableSkeletonSpawning = false;
        chanceToSpawnGoldSkeleton = 2;
        enemyCountMax = 60;


        // spawn winning gold
        if (gameMode != GameMode.GoldSkeletonMode)
        {
            foreach(GameObject trans in ballenemySpawns)
            {
                GameObject winGold = Instantiate(goldPrefab, trans.transform.position, Quaternion.identity) as GameObject;
                // throw the gold towards the player
                winGold.GetComponent<Rigidbody>().AddForce((playerScript.gameObject.transform.position - winGold.transform.position).normalized
                    * (Vector3.Distance(winGold.transform.position, playerScript.gameObject.transform.position) / 2f), ForceMode.VelocityChange);
            }
        }
        DisplayEventText("Anomaly Survived!");
        KillAll();

        // achievements
        if (gameState == GameState.Playing)
        {
            if (gameMode == GameMode.BoneBallMode)
            {
                SteamUserStats.SetAchievement("Anom_Ball");
            }
            else if (gameMode == GameMode.SpeedySkeletonMode)
            {
                SteamUserStats.SetAchievement("Anom_Special");
            }
            else if (gameMode == GameMode.FlyingSkeletonMode)
            {
                SteamUserStats.SetAchievement("Anom_Flying");
            }
            else if (gameMode == GameMode.HordeSkeletonMode)
            {
                SteamUserStats.SetAchievement("Anom_Horde");
            }

            SteamUserStats.StoreStats();
        }

        // turns off our timer
        specialTimerText.gameObject.SetActive(false);
        gameMode = GameMode.normal;
    }

    public void KillAll()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject skelton in allEnemies)
        {
            if (skelton.transform.parent.GetComponent<Enemy>() || skelton.transform.parent.GetComponent<BoneBall>() || skelton.transform.parent.GetComponent<FlyingSkull>())
            {
                skelton.transform.parent.SendMessage("KillThisEnemy", true);
            }
        }
    }

    string FormatSeconds(float elapsed)
    {
        int d = (int)(elapsed * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;

        // WE ONLY WANT ONE DIGIT SO WE DIVIDE IT BY TEN, REMOVE THE TEN FOR 2 DIGITS
        int hundredths = (d % 100) / 10;
        if (minutes > 0)
        {
            return String.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, hundredths);
        }
        else
        {
            return String.Format("{0:00}.{1:0}", seconds, hundredths);
        }
    }

    public void startWaitThenSwitchToEndGame()
    {
        StartCoroutine(WaitThenSwitchToEndGame());
    }

    IEnumerator WaitThenSwitchToEndGame()
    {
        yield return new WaitForSeconds(5);
        gameState = GameState.EndGame;
    }

    public void TurnOffShipLights()
    {
        foreach(Light light in shipLights)
        {
            light.range = 6.5f;
            light.intensity = .5f;
        }

        foreach(GameObject thing in shipAesthetics)
        {
            thing.SetActive(false);
        }
    }

    public void CheckSteamAchievements()
    {
        // Grave Digger
        if (stat_SkeltinsKilled >= 1000)
        {
            SteamUserStats.SetAchievement("1000Skeltins");
        }
        // UnderTaker
        if (stat_SkeltinsKilled >= 10000)
        {
            SteamUserStats.SetAchievement("10kSkeltins");
        }
        // Pickpocket
        if (stat_GoldSkeltinsKilled >= 100)
        {
            SteamUserStats.SetAchievement("100GoldSkeltins");
        }
        // Red Dead Redeadtion
        if (stat_RedSkeltinsKilled >= 500)
        {
            SteamUserStats.SetAchievement("500RedSkeltins");
        }
        // Hazmat
        if (stat_ToxicSkeltinsKilled >= 300)
        {
            SteamUserStats.SetAchievement("300ToxicSkeltins");
        }
        // Skeet Shooting
        if (stat_SkellsKilled >= 1000)
        {
            SteamUserStats.SetAchievement("1000Skells");
        }
        // Blood rain
        if (stat_RedSkellsKilled >= 250)
        {
            SteamUserStats.SetAchievement("250RedSkells");
        }
        // acid rain
        if (stat_ToxicSkellsKilled >= 200)
        {
            SteamUserStats.SetAchievement("200ToxicSkells");
        }
        // ballbuster
        if (stat_BoneBallsKilled >= 500)
        {
            SteamUserStats.SetAchievement("500BoneBalls");
        }
        // Skullcracker
        if (stat_SkellLordsKilled >= 1)
        {
            SteamUserStats.SetAchievement("1SkellLord");
        }
        // Lord it Over
        if (stat_SkellLordsKilled >= 5)
        {
            SteamUserStats.SetAchievement("5SkellLords");
        }
        // Survive 1 min
        if (gameTimer >= 60 || stat_LongestTimeSurvived >= 60)
        {
            SteamUserStats.SetAchievement("Survive1Min");
        }
        // Survive 2 min
        if (gameTimer >= 120 || stat_LongestTimeSurvived >= 120)
        {
            SteamUserStats.SetAchievement("Survive2Min");
        }
        // Survive 3 min
        if (gameTimer >= 180 || stat_LongestTimeSurvived >= 180)
        {
            SteamUserStats.SetAchievement("Survive3Min");
        }
        // Survive 4 min
        if (gameTimer >= 240 || stat_LongestTimeSurvived >= 240)
        {
            SteamUserStats.SetAchievement("Survive4Min");
        }
        // Survive 5 min
        if (gameTimer >= 300 || stat_LongestTimeSurvived >= 300)
        {
            SteamUserStats.SetAchievement("Survive5Min");
        }
        // Survive 6 min
        if (gameTimer >= 360 || stat_LongestTimeSurvived >= 360)
        {
            SteamUserStats.SetAchievement("Survive6Min");
        }

        int fullyUpped = NumberOfFullyUpgradedWeapons();
        Debug.Log(fullyUpped.ToString() + " fully upped weapons");
        // Maxing out one weapon
        if (fullyUpped > 0)
        {
            SteamUserStats.SetAchievement("Max1Weapon");
            Debug.Log("maxed 1 weapon fully");
        }
        // Maxing out all weapons
        if (fullyUpped >= 5)
        {
            SteamUserStats.SetAchievement("MaxAllWeapons");
        }
        // 5k total gold, THIS USED TO BE 10K BUT YOU'D MAX ALL YOUR WEAPONS AT THAT POINT AND ITS DUMB
        if (stat_TotalGoldGained >= 5000)
        {
            SteamUserStats.SetAchievement("10kGold");
        }

        SteamUserStats.StoreStats();
    }

    int NumberOfFullyUpgradedWeapons()
    {
        int fullyUppedWeapons = 0;
        if (pistolTotalUpgradeLevel == (Pistol_Upgrades_AmmoLevelMax + Pistol_Upgrades_ReloadSpeedLevelMax) * 10)
        {
            fullyUppedWeapons++;
        }
        if (machineGunTotalUpgradeLevel == (MachineGun_Upgrades_AmmoLevelMax + MachineGun_Upgrades_ReloadSpeedLevelMax + MachineGun_Upgrades_ROFLevelMax) * 10)
        {
            fullyUppedWeapons++;
        }
        if (shotgunTotalUpgradeLevel == (Shotgun_Upgrades_AmmoLevelMax + Shotgun_Upgrades_ReloadSpeedLevelMax) * 10)
        {
            fullyUppedWeapons++;
        }
        if (rocketTotalUpgradeLevel == (Rocket_Upgrades_AmmoLevelMax + Rocket_Upgrades_ReloadSpeedLevelMax + Rocket_Upgrades_RadiusMax) * 10)
        {
            fullyUppedWeapons++;
        }
        if (grenadeTotalUpgradeLevel == (Grenade_Upgrades_RadiusMax + Grenade_Upgrades_RechargeRateMax) * 10)
        {
            fullyUppedWeapons++;
        }

        return fullyUppedWeapons;
    }

    public void RadioToggle()
    {
        // if our animation bool says the radio is on, then shut that thing off
        if (radioAnim.GetBool("RadioIsOn") == true)
        {
            radioAudioSource.enabled = false;
            radioAnim.SetBool("RadioIsOn", false);
            PlayerPrefs.SetInt("RadioState", 0);
            particle_MusicNotes1.Stop();
            particle_MusicNotes2.Stop();
            particle_MusicNotes3.Stop();
        }
        else
        {
            radioAudioSource.enabled = true;
            radioAnim.SetBool("RadioIsOn", true);
            PlayerPrefs.SetInt("RadioState", 1);
            particle_MusicNotes1.Play();
            particle_MusicNotes2.Play();
            particle_MusicNotes3.Play();
        }
    }

    public void StartGameMusic()
    {
        musicSource.Play();
    }

    public void SwapGameplayMusic()
    {
        musicSource.clip = blipBloopTheme;
    }
}
