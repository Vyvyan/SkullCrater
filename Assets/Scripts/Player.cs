using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Steamworks;

public class Player : MonoBehaviour {

    public Transform bulletSpawnPoint;
    public GameObject bullet, shotgunPellet, rocketProjectile;
    public GameObject grenadeObject;
    public float gunPower;
    Camera mainCamera;
    public GameObject deadCam;
    GameManager gameManager;

    public Light playerLight;
    Animator playerLightAnim;
    AudioSource audioSourcePlayer;
    AudioManager audioManager;

    public int pistolAmmoMax, shotgunAmmoMax, machinegunAmmoMax, rocketAmmoMax;
    public int pistolAmmo, shotgunAmmo, machinegunAmmo, rocketAmmo;
    public bool isReloading;
    public float reloadSpeed_Pistol, reloadSpeed_Shotgun, reloadSpeed_Machinegun, reloadSpeed_Rocket;

    public float health;

    public float grenadeJuiceMax, GrenadeJuiceCurrent, grenadeJuicePerKill;
    public bool hasGrenadeReady;

    public Animator pistolAnim, shotgunAnim, machinegunAnim, rocketAnim;
    public WeaponEffects pistolEffects, shotgunEffects, machinegunEffects;
    public Light pistolMuzzleLight, shotgunMuzzleLight, machinegunMuzzleLight;

    public GameObject pistolModel, shotgunModel, machinegunModel, rocketModel;

    public enum WeaponType {pistol, shotgun, machinegun, rocket, none};
    public WeaponType weapon1, weapon2;

    public float machinegunFireRate, mgFireRateCurrent;
    // bool for machine gun shooting
    bool mgCanShoot;
    bool canPlaymgOOASound;

    // music starting
    bool hasStartedMusic;

    // gold effect
    public GameObject goldPickupParticles;
    public Transform particleSpawnLocation;

	// Use this for initialization
	void Start ()
    {
        // start with just a pistol
        weapon1 = WeaponType.pistol;
        weapon2 = WeaponType.none;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        audioSourcePlayer = GetComponent<AudioSource>();

        playerLightAnim = playerLight.GetComponent<Animator>();

        // start with a grenade at the ready
        hasGrenadeReady = true;
        GrenadeJuiceCurrent = grenadeJuiceMax;

        mainCamera = Camera.main;
        //start with full clip
        pistolAmmo = pistolAmmoMax;
        shotgunAmmo = shotgunAmmoMax;
        machinegunAmmo = machinegunAmmoMax;
        rocketAmmo = rocketAmmoMax;

        // start our health at 1, cause idk how much we need
        health = 1;

        // get our pistol effects script from our anim, which is on the same object
        pistolEffects = pistolAnim.gameObject.GetComponent<WeaponEffects>();
        shotgunEffects = shotgunAnim.gameObject.GetComponent<WeaponEffects>();
        machinegunEffects = machinegunAnim.gameObject.GetComponent<WeaponEffects>();

        // load our weapons from last session
        loadOurWeaponLoadout();

        // swap our weapon model out for whatever we are holding
        ChangeWeaponModel();

        canPlaymgOOASound = true;
        hasStartedMusic = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            // player light for bonus modes
            if (gameManager.gameMode == GameManager.GameMode.HordeSkeletonMode)
            {
                playerLight.range = 11;
            }
            else
            {
                playerLight.range = 22;
            }

            // fire a shot
            // IF WE DO NOT HAVE THE MACHINE GUN OUT
            if (weapon1 != WeaponType.machinegun)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (!isReloading)
                    {
                        FireWeapon();
                    }
                }
            }
            else
            {
                if (mgCanShoot)
                {
                    // machine gun stuff
                    if (Input.GetButton("Fire1"))
                    {
                        if (!isReloading)
                        {
                            FireWeapon();
                        }
                    }
                }
            }

            // grenade
            if (Input.GetButtonDown("Fire2"))
            {
                if (hasGrenadeReady)
                {
                    GameObject temp = Instantiate(grenadeObject, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                    temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                    GrenadeJuiceCurrent = 0;
                    hasGrenadeReady = false;
                }
            }

            // reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isReloading)
                {
                    ReloadWeapon();
                }
            }
        }

        if (GameManager.gameState != GameManager.GameState.Dead)
        {
            // change weapons
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchWeapons();
            }
        }

            // if we die
            if (health <= 0)
        {
            GameManager.gameState = GameManager.GameState.Dead;
            KillPlayer();
        }

        // machine gun stuff
        if (!mgCanShoot)
        {
            if(mgFireRateCurrent >= 0)
            {
                mgFireRateCurrent -= Time.deltaTime;
            }
            else
            {
                mgCanShoot = true;
            }
        }

        // gaining grenade stuff
        if (GrenadeJuiceCurrent >= grenadeJuiceMax)
        {
            hasGrenadeReady = true;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            TakeDamage(other.gameObject.name);
        }

        if (other.gameObject.tag == "Gold")
        {
            GameManager.heldGold += gameManager.goldValue + (gameManager.goldBonusAmount * gameManager.goldBonusLevel);
            gameManager.goldBonusLevel++;
            GameObject tempPart = Instantiate(goldPickupParticles, particleSpawnLocation.position, Quaternion.identity) as GameObject;
            tempPart.transform.SetParent(particleSpawnLocation);
            Destroy(other.gameObject);
            audioSourcePlayer.PlayOneShot(AudioManager.gold_Pickup, GameManager.SFXVolume / 200);
        }

        // switch game modes when we hit the planet
        if (GameManager.gameState == GameManager.GameState.PreGame)
        {
            // cue music intro
            if (other.gameObject.tag == "MusicCue")
            {
                audioManager.Play2DSoundMusicClip(AudioManager.fallingMusicIntro);
            }

            // start game trigger
            if (other.gameObject.tag == "StartGameTrigger")
            {
                GameManager.gameState = GameManager.GameState.Playing;
                // turn on player light
                playerLightAnim.SetTrigger("LightFadeUp");
                gameManager.TurnOffShipLights();
            }
        }

        // stores gold when touching the beam
        if (other.gameObject.tag == "Beam")
        {
            if (GameManager.heldGold > 0)
            {
                // achievement
                if (GameManager.heldGold >= 200)
                {
                    SteamUserStats.SetAchievement("Deposit200");
                    SteamUserStats.StoreStats();
                }
                GameManager.thisSessionGoldGained += GameManager.heldGold;
                GameManager.storedGold += GameManager.heldGold;
                PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                gameManager.DisplayEventText("Deposited " + GameManager.heldGold + "g");
                // update stats
                if (GameManager.heldGold > GameManager.stat_LargestSingleDeposit)
                {
                    GameManager.stat_LargestSingleDeposit = GameManager.heldGold;
                }
                GameManager.heldGold = 0;
                gameManager.goldBonusLevel = 0;
                // if you enter the beam when the game is ended, it starts the noise, then gets cut off. it's annoying
                if (GameManager.gameState != GameManager.GameState.EndGame)
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.gold_DropOff, GameManager.SFXVolume / 125);
                }
            }

            if (GameManager.gameState == GameManager.GameState.EndGame)
            {
                // if we won the game, and touch the beam
                // update stats
                // total gold gained
                GameManager.stat_TotalGoldGained += GameManager.thisSessionGoldGained;
                // session gold best
                if (GameManager.thisSessionGoldGained > GameManager.stat_MostGoldInARun)
                {
                    GameManager.stat_MostGoldInARun = GameManager.thisSessionGoldGained;
                }
                // best time survived
                if (GameManager.gameTimer > GameManager.stat_LongestTimeSurvived)
                {
                    GameManager.stat_LongestTimeSurvived = GameManager.gameTimer;
                }
                // we add a slight random rotation to the camera to give a good effect
                gameManager.SaveStatistics();
                gameManager.CheckSteamAchievements();
                SteamUserStats.SetAchievement("SurviveARound");
                SteamUserStats.StoreStats();
                SceneManager.LoadScene(2);
            }
        }

        // picking up a crystal skull
        if (other.gameObject.tag == "Crystal Skull")
        {
            // only pick up crystal skulls if we are in the normal game mode
            if (gameManager.gameMode == GameManager.GameMode.normal)
            {
                // KILL ALL ENEMIES FIRST
                gameManager.KillAll();
                // there are 5 modes, all modes except jackpot mode have 6/25 ~(24%) chance to spawn, jackpot has 1/25 ~(4%)
                int blahblahrandom = Random.Range(1, 26);
                Debug.Log(blahblahrandom.ToString());
                gameManager.StartSpecialGameMode(blahblahrandom);
                audioSourcePlayer.PlayOneShot(AudioManager.anomalousSkull, GameManager.SFXVolume / 100);
                // achievement
                SteamUserStats.SetAchievement("FindAnAnomSkull");
                SteamUserStats.StoreStats();
                // destroy the skull
                Destroy(other.transform.parent.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // if we touch something with a rigid body, push it away so we don't get stuck
        if (other.gameObject.GetComponent<Rigidbody>())
        {
            Rigidbody rigbidje = other.gameObject.GetComponent<Rigidbody>();
            Vector3 forceDirection = (rigbidje.transform.position - gameObject.transform.position).normalized * 50;
            rigbidje.AddForce(new Vector3(forceDirection.x, 0, forceDirection.z), ForceMode.Impulse);
        }

        // start playing music when we hit the crater
        if (other.gameObject.tag == "Ground")
        {
            // if we haven't started playing music
            if (!hasStartedMusic)
            {
                // play music
                gameManager.StartGameMusic();
                hasStartedMusic = true;
            }
        }
    }

    IEnumerator Reload()
    {
        if (weapon1 == WeaponType.pistol)
        {
            isReloading = true;
            pistolAnim.SetBool("ReloadGun", true);
            audioSourcePlayer.pitch = AudioManager.reload_Pistol.length / reloadSpeed_Pistol;
            audioSourcePlayer.PlayOneShot(AudioManager.reload_Pistol, GameManager.SFXVolume / 400);
            yield return new WaitForSeconds(reloadSpeed_Pistol);
            audioSourcePlayer.pitch = 1;
            pistolAnim.SetBool("ReloadGun", false);
            pistolAmmo = pistolAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.shotgun)
        {
            isReloading = true;
            shotgunAnim.SetBool("ReloadGun", true);
            audioSourcePlayer.pitch = AudioManager.reload_Shotgun.length / reloadSpeed_Shotgun;
            audioSourcePlayer.PlayOneShot(AudioManager.reload_Shotgun, GameManager.SFXVolume / 400);
            yield return new WaitForSeconds(reloadSpeed_Shotgun);
            audioSourcePlayer.pitch = 1;
            shotgunAnim.SetBool("ReloadGun", false);
            shotgunAmmo = shotgunAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.machinegun)
        {
            isReloading = true;
            machinegunAnim.SetBool("ReloadGun", true);
            audioSourcePlayer.pitch = AudioManager.reload_MG.length / reloadSpeed_Machinegun;
            audioSourcePlayer.PlayOneShot(AudioManager.reload_MG, GameManager.SFXVolume / 400);
            yield return new WaitForSeconds(reloadSpeed_Machinegun);
            audioSourcePlayer.pitch = 1;
            machinegunAnim.SetBool("ReloadGun", false);
            machinegunAmmo = machinegunAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.rocket)
        {
            isReloading = true;
            rocketAnim.SetBool("ReloadGun", true);
            audioSourcePlayer.pitch = AudioManager.reload_Rocket.length / reloadSpeed_Rocket;
            audioSourcePlayer.PlayOneShot(AudioManager.reload_Rocket, GameManager.SFXVolume / 400);
            yield return new WaitForSeconds(reloadSpeed_Rocket);
            audioSourcePlayer.pitch = 1;
            rocketAnim.SetBool("ReloadGun", false);
            rocketAmmo = rocketAmmoMax;
            isReloading = false;
        }
    }

    public void TakeDamage(string thingThatHitUs)
    {
        health--;
        // send the name of the enemy that hit us to the game manager for easy access
        GameManager.enemyThatKilledPlayer = thingThatHitUs;
        Debug.Log(thingThatHitUs);
        saveOurWeaponLoadout();

        if (gameManager.hasBeenInvaded == true)
        {
            PlayerPrefs.SetInt("WraithPoster", 1);
        }
    }

    public void KillPlayer()
    {
        audioManager.Play2DSound(AudioManager.playerDeath);
        // achievements
        if (GameManager.heldGold >= 200)
        {
            SteamUserStats.SetAchievement("Drop200");
            SteamUserStats.StoreStats();
        }
        // update stats
        // total gold gained
        GameManager.stat_TotalGoldGained += GameManager.thisSessionGoldGained;
        // dropping gold
        if (GameManager.heldGold > GameManager.stat_MostGoldDropped)
        {
            GameManager.stat_MostGoldDropped = GameManager.heldGold;
        }
        // session gold best
        if (GameManager.thisSessionGoldGained > GameManager.stat_MostGoldInARun)
        {
            GameManager.stat_MostGoldInARun = GameManager.thisSessionGoldGained;
        }
        // best time survived
        if (GameManager.gameTimer > GameManager.stat_LongestTimeSurvived)
        {
            GameManager.stat_LongestTimeSurvived = GameManager.gameTimer;
        }
        // we spawn a new camera that flops to the ground when we die
        GameObject tempCam = Instantiate(deadCam, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;
        // we add a slight random rotation to the camera to give a good effect
        tempCam.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10,30), Random.Range(10,30), Random.Range(10,30)));
        gameObject.SetActive(false);
        GameManager.stat_Deaths++;
        gameManager.SaveStatistics();
        gameManager.CheckSteamAchievements();
        GameManager.gameState = GameManager.GameState.Dead;
    }

    IEnumerator MuzzleFlash(Light light)
    {
        light.intensity = 4;
        yield return new WaitForSeconds(.1f);
        light.intensity = 0;
    }

    void FireWeapon()
    {
        // pistol
        if (weapon1 == WeaponType.pistol)
        {
            if (pistolAmmo > 0)
            {
                pistolAnim.SetTrigger("FireGun");
                //muzzle flash
                pistolEffects.CreateMuzzleFlash();
                audioSourcePlayer.PlayOneShot(AudioManager.pistolFire, GameManager.SFXVolume / 180);
                StartCoroutine(MuzzleFlash(pistolMuzzleLight));
                GameObject temp = Instantiate(bullet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                temp.name = "pistolBullet";
                pistolAmmo--;
            }
            else
            {
                audioSourcePlayer.PlayOneShot(AudioManager.outOfAmmo, GameManager.SFXVolume / 180);
            }
        }
        // shotgun
        if (weapon1 == WeaponType.shotgun)
        {
            if (shotgunAmmo > 0)
            {
                // if we aren't current in the shooting animation
                if (!shotgunAnim.GetCurrentAnimatorStateInfo(0).IsName("Shotgun_Fire"))
                {
                    shotgunAnim.SetTrigger("FireGun");
                    //muzzle flash
                    shotgunEffects.CreateMuzzleFlash();
                    audioSourcePlayer.PlayOneShot(AudioManager.shotgunFire, GameManager.SFXVolume / 100);

                    StartCoroutine(MuzzleFlash(shotgunMuzzleLight));
                    GameObject temp = Instantiate(shotgunPellet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;

                    // gets all the rigid bodies of the pellets, then fires them
                    Rigidbody[] pellets = temp.GetComponentsInChildren<Rigidbody>();
                    foreach (Rigidbody rb in pellets)
                    {
                        rb.AddForce(rb.transform.forward * (gunPower * 1), ForceMode.Impulse);
                    }

                    shotgunAmmo--;
                }
            }
            else
            {
                audioSourcePlayer.PlayOneShot(AudioManager.outOfAmmo, GameManager.SFXVolume / 180);
            }
        }
        // MACHINE GUN
        if (weapon1 == WeaponType.machinegun)
        {
            if (machinegunAmmo > 0)
            {
                if (mgCanShoot)
                {
                    machinegunAnim.SetTrigger("FireGun");
                    //muzzle flash
                    machinegunEffects.CreateMuzzleFlash();
                    audioSourcePlayer.PlayOneShot(AudioManager.machinegunFire, GameManager.SFXVolume / 100);

                    StartCoroutine(MuzzleFlash(machinegunMuzzleLight));
                    GameObject temp = Instantiate(bullet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                    temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                    temp.name = "mgBullet";

                    machinegunAmmo--;
                    mgCanShoot = false;
                    // reset our timer
                    mgFireRateCurrent = machinegunFireRate;
                }
                
            }
            else
            {
                if (mgCanShoot)
                {
                    if (canPlaymgOOASound)
                    {
                        audioSourcePlayer.PlayOneShot(AudioManager.outOfAmmo, GameManager.SFXVolume / 180);
                        StartCoroutine(resetMGLowAmmoNoise());
                    }
                }
            }
        }
        // ROCKET
        if (weapon1 == WeaponType.rocket)
        {
            // if we aren't current in the shooting animation
            if (!rocketAnim.GetCurrentAnimatorStateInfo(0).IsName("Rocket_Fire"))
            {
                if (rocketAmmo > 0)
                {
                    rocketAnim.SetTrigger("FireGun");
                    audioSourcePlayer.PlayOneShot(AudioManager.rocketFire, GameManager.SFXVolume / 180);
                    GameObject temp = Instantiate(rocketProjectile, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                    // no need to add force to the rocket, since we'll have a script that moves it on the rocket itself
                    //temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                    rocketAmmo--;
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.outOfAmmo, GameManager.SFXVolume / 180);
                }
            }
        }
    }

    void SwitchWeapons()
    {
        // if we aren't reloading
        if (!isReloading)
        {
            // if we have a weapon in our second slot
            if (weapon2 != WeaponType.none)
            {
                // swap the weapons
                WeaponType tempHolder = weapon2;
                weapon2 = weapon1;
                weapon1 = tempHolder;

                // change the model too
                ChangeWeaponModel();
            }
        }
    }

    void ReloadWeapon()
    {
        if (weapon1 == WeaponType.pistol)
        {
            if (pistolAmmo < pistolAmmoMax)
            {
                StartCoroutine(Reload());
            }
        }
        if (weapon1 == WeaponType.shotgun)
        {
            if (shotgunAmmo < shotgunAmmoMax)
            {
                StartCoroutine(Reload());
            }
        }
        if (weapon1 == WeaponType.machinegun)
        {
            if (machinegunAmmo < machinegunAmmoMax)
            {
                StartCoroutine(Reload());
            }
        }
        if (weapon1 == WeaponType.rocket)
        {
            if (rocketAmmo < rocketAmmoMax)
            {
                StartCoroutine(Reload());
            }
        }

    }

    void ChangeWeaponModel()
    {
        if (weapon1 == WeaponType.pistol)
        {
            pistolModel.SetActive(true);
            shotgunModel.SetActive(false);
            machinegunModel.SetActive(false);
            rocketModel.SetActive(false);
        }
        if (weapon1 == WeaponType.shotgun)
        {
            pistolModel.SetActive(false);
            shotgunModel.SetActive(true);
            machinegunModel.SetActive(false);
            rocketModel.SetActive(false);
        }
        if (weapon1 == WeaponType.machinegun)
        {
            pistolModel.SetActive(false);
            shotgunModel.SetActive(false);
            machinegunModel.SetActive(true);
            rocketModel.SetActive(false);
        }
        if (weapon1 == WeaponType.rocket)
        {
            pistolModel.SetActive(false);
            shotgunModel.SetActive(false);
            machinegunModel.SetActive(false);
            rocketModel.SetActive(true);
        }
    }

    public void AddGrenadeJuice()
    {
        if (GrenadeJuiceCurrent < grenadeJuiceMax)
        {
            GrenadeJuiceCurrent += grenadeJuicePerKill;
        }
    }

    public void EquipWeapon(string weaponName)
    {
        // if we don't have a weapon 2
        if (weapon2 == WeaponType.none)
        {
            // if we're trying to equip a pistol
            if (weaponName == "Pistol")
            {
                // if we don't already have a pistol
                if (weapon1 != WeaponType.pistol)
                {
                    // move our weapon 1 to weapon 2 and equip a pistol as weapon 1
                    weapon2 = weapon1;
                    weapon1 = WeaponType.pistol;

                    ChangeWeaponModel();
                    audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                }
            }
            // if we're trying to equip a Machine Gun
            if (weaponName == "MachineGun")
            {
                // if the machine gun is unlocked
                if (GameManager.machinegunUnlocked)
                {
                    // if we don't already have a machinegun
                    if (weapon1 != WeaponType.machinegun)
                    {
                        // move our weapon 1 to weapon 2 and equip a machine gun as weapon 1
                        weapon2 = weapon1;
                        weapon1 = WeaponType.machinegun;
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        ChangeWeaponModel();
                    }
                }
                // else, buy the machine gun
                else
                {
                    // do we have enough gold to buy the machine gun?
                    if (GameManager.storedGold >= GameManager.machinegunWeaponValue)
                    {
                        // buy it
                        GameManager.storedGold -= GameManager.machinegunWeaponValue;
                        GameManager.machinegunUnlocked = true;
                        // save it that we unlocked the machinegun
                        PlayerPrefs.SetInt("machinegunUnlocked", 1);
                        PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                        gameManager.ChangeEquipButtonText();
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                    }
                    else
                    {
                        audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                    }
                }
                
            }
            // if we're trying to equip a pistol
            if (weaponName == "Shotgun")
            {
                // if the machine gun is unlocked
                if (GameManager.shotgunUnlocked)
                {
                    // if we don't already have a shotgun
                    if (weapon1 != WeaponType.shotgun)
                    {
                        // move our weapon 1 to weapon 2 and equip a machine gun as weapon 1
                        weapon2 = weapon1;
                        weapon1 = WeaponType.shotgun;
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        ChangeWeaponModel();
                    }
                }
                // else, buy the shotgun gun
                else
                {
                    // do we have enough gold to buy the machine gun?
                    if (GameManager.storedGold >= GameManager.shotgunWeaponValue)
                    {
                        // buy it
                        GameManager.storedGold -= GameManager.shotgunWeaponValue;
                        GameManager.shotgunUnlocked = true;
                        // save it that we unlocked the shotgun
                        PlayerPrefs.SetInt("shotgunUnlocked", 1);
                        PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        gameManager.ChangeEquipButtonText();
                    }
                    else
                    {
                        audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                    }
                }
            }
            // if we're trying to equip a rocket
            if (weaponName == "Rocket")
            {
                // if the machine gun is unlocked
                if (GameManager.rocketUnlocked)
                {
                    // if we don't already have a machinegun
                    if (weapon1 != WeaponType.rocket)
                    {
                        // move our weapon 1 to weapon 2 and equip a machine gun as weapon 1
                        weapon2 = weapon1;
                        weapon1 = WeaponType.rocket;
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        ChangeWeaponModel();
                    }
                }
                // else, buy the rocket
                else
                {
                    // do we have enough gold to buy the machine gun?
                    if (GameManager.storedGold >= GameManager.rocketWeaponValue)
                    {
                        // buy it
                        GameManager.storedGold -= GameManager.rocketWeaponValue;
                        GameManager.rocketUnlocked = true;
                        // save it that we unlocked the machinegun
                        PlayerPrefs.SetInt("rocketUnlocked", 1);
                        PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        gameManager.ChangeEquipButtonText();
                    }
                    else
                    {
                        audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                    }
                }

            }
        }
        // WE DO HAVE A SECOND WEAPON
        else
        {
            if (weaponName == "Pistol")
            {
                // if neither of our guns is a pistol
                if (weapon1 != WeaponType.pistol && weapon2 != WeaponType.pistol)
                {
                    // equip the pistol
                    weapon1 = WeaponType.pistol;
                    audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                    ChangeWeaponModel();
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                }
            }
            if (weaponName == "MachineGun")
            {
                // if neither of our guns is a machinegun
                if (weapon1 != WeaponType.machinegun && weapon2 != WeaponType.machinegun)
                {
                    // if the machine gun is unlocked
                    if (GameManager.machinegunUnlocked)
                    {
                        // equip the machine gun
                        weapon1 = WeaponType.machinegun;
                        ChangeWeaponModel();
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                    }
                    // else, buy the machine gun
                    else
                    {
                        // do we have enough gold to buy the machine gun?
                        if (GameManager.storedGold >= GameManager.machinegunWeaponValue)
                        {
                            // buy it
                            GameManager.storedGold -= GameManager.machinegunWeaponValue;
                            GameManager.machinegunUnlocked = true;
                            // save it that we unlocked the machine gun
                            PlayerPrefs.SetInt("machinegunUnlocked", 1);
                            PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                            audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                            gameManager.ChangeEquipButtonText();
                        }
                        else
                        {
                            audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                        }
                    }
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                }
            }
            if (weaponName == "Shotgun")
            {
                // if neither of our guns is a shotgun
                if (weapon1 != WeaponType.shotgun && weapon2 != WeaponType.shotgun)
                {
                    // if the machine gun is unlocked
                    if (GameManager.shotgunUnlocked)
                    {
                        // equip the machine gun
                        weapon1 = WeaponType.shotgun;
                        ChangeWeaponModel();
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                    }
                    // else, buy the machine gun
                    else
                    {
                        // do we have enough gold to buy the machine gun?
                        if (GameManager.storedGold >= GameManager.shotgunWeaponValue)
                        {
                            // buy it
                            GameManager.storedGold -= GameManager.shotgunWeaponValue;
                            GameManager.shotgunUnlocked = true;
                            // save it that we unlocked the shotgun
                            PlayerPrefs.SetInt("shotgunUnlocked", 1);
                            PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                            audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                            gameManager.ChangeEquipButtonText();
                        }
                        else
                        {
                            audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                        }
                    }
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                }
            }
            // rocket
            if (weaponName == "Rocket")
            {
                // if neither of our guns is a machinegun
                if (weapon1 != WeaponType.rocket && weapon2 != WeaponType.rocket)
                {
                    // if the rocket is unlocked
                    if (GameManager.rocketUnlocked)
                    {
                        // equip the rocket gun
                        weapon1 = WeaponType.rocket;
                        audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                        ChangeWeaponModel();
                    }
                    // else, buy the rocket
                    else
                    {
                        // do we have enough gold to buy the machine gun?
                        if (GameManager.storedGold >= GameManager.rocketWeaponValue)
                        {
                            // buy it
                            GameManager.storedGold -= GameManager.rocketWeaponValue;
                            GameManager.rocketUnlocked = true;
                            // save it that we unlocked the rocket
                            PlayerPrefs.SetInt("rocketUnlocked", 1);
                            PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                            audioSourcePlayer.PlayOneShot(AudioManager.accept, GameManager.SFXVolume / 100);
                            gameManager.ChangeEquipButtonText();
                        }
                        else
                        {
                            audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                        }
                    }
                }
                else
                {
                    audioSourcePlayer.PlayOneShot(AudioManager.decline, GameManager.SFXVolume / 100);
                }
            }
        }
    }

    void saveOurWeaponLoadout()
    {
        if (weapon1 == WeaponType.pistol)
        {
            PlayerPrefs.SetString("Weapon1", "Pistol");
        }
        else if (weapon1 == WeaponType.machinegun)
        {
            PlayerPrefs.SetString("Weapon1", "MachineGun");
        }
        else if (weapon1 == WeaponType.shotgun)
        {
            PlayerPrefs.SetString("Weapon1", "Shotgun");
        }
        else if (weapon1 == WeaponType.rocket)
        {
            PlayerPrefs.SetString("Weapon1", "Rocket");
        }

        if (weapon2 == WeaponType.pistol)
        {
            PlayerPrefs.SetString("Weapon2", "Pistol");
        }
        else if (weapon2 == WeaponType.machinegun)
        {
            PlayerPrefs.SetString("Weapon2", "MachineGun");
        }
        else if (weapon2 == WeaponType.shotgun)
        {
            PlayerPrefs.SetString("Weapon2", "Shotgun");
        }
        else if (weapon2 == WeaponType.rocket)
        {
            PlayerPrefs.SetString("Weapon2", "Rocket");
        }
        else if (weapon2 == WeaponType.none)
        {
            PlayerPrefs.SetString("Weapon2", "None");
        }
    }

    void loadOurWeaponLoadout()
    {
        if (PlayerPrefs.GetString("Weapon1", "Pistol") == "Pistol")
        {
            weapon1 = WeaponType.pistol;
        }
        else if (PlayerPrefs.GetString("Weapon1", "Pistol") == "MachineGun")
        {
            weapon1 = WeaponType.machinegun;
        }
        else if (PlayerPrefs.GetString("Weapon1", "Pistol") == "Shotgun")
        {
            weapon1 = WeaponType.shotgun;
        }
        else if (PlayerPrefs.GetString("Weapon1", "Pistol") == "Rocket")
        {
            weapon1 = WeaponType.rocket;
        }

        if (PlayerPrefs.GetString("Weapon2", "Pistol") == "Pistol")
        {
            weapon2 = WeaponType.pistol;
        }
        else if (PlayerPrefs.GetString("Weapon2", "Pistol") == "MachineGun")
        {
            weapon2 = WeaponType.machinegun;
        }
        else if (PlayerPrefs.GetString("Weapon2", "Pistol") == "Shotgun")
        {
            weapon2 = WeaponType.shotgun;
        }
        else if (PlayerPrefs.GetString("Weapon2", "Pistol") == "Rocket")
        {
            weapon2 = WeaponType.rocket;
        }
        else if (PlayerPrefs.GetString("Weapon2", "Pistol") == "None")
        {
            weapon2 = WeaponType.none;
        }
    }

    IEnumerator resetMGLowAmmoNoise()
    {
        canPlaymgOOASound = false;
        yield return new WaitForSeconds(.25f);
        canPlaymgOOASound = true;
    }
}

