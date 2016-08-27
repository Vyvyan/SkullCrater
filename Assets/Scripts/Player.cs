using UnityEngine;
using System.Collections;

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

    public int pistolAmmoMax, shotgunAmmoMax, machinegunAmmoMax, rocketAmmoMax;
    public int pistolAmmo, shotgunAmmo, machinegunAmmo, rocketAmmo;
    public bool isReloading;
    public float reloadSpeed_Pistol, reloadSpeed_Shotgun, reloadSpeed_Machinegun, reloadSpeed_Rocket;

    public float health;

    public float grenadeJuiceMax, GrenadeJuiceCurrent, grenadeJuicePerKill;
    public bool hasGrenadeReady;
    bool canGainGrenadeJuice;

    public Animator pistolAnim, shotgunAnim, machinegunAnim, rocketAnim;
    public WeaponEffects pistolEffects, shotgunEffects, machinegunEffects;
    public Light pistolMuzzleLight, shotgunMuzzleLight, machinegunMuzzleLight;

    public GameObject pistolModel, shotgunModel, machinegunModel, rocketModel;

    public enum WeaponType {pistol, shotgun, machinegun, rocket, none};
    public WeaponType weapon1, weapon2;

    public float machinegunFireRate, mgFireRateCurrent;
    // bool for machine gun shooting
    bool mgCanShoot;

	// Use this for initialization
	void Start ()
    {
        // start with just a pistol
        weapon1 = WeaponType.pistol;
        weapon2 = WeaponType.none;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

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
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
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
                    StartCoroutine(GrenadeJuiceCoolDown());
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
            Destroy(other.gameObject);
        }

        // switch game modes when we hit the planet
        if (GameManager.gameState == GameManager.GameState.PreGame)
        {
            if (other.gameObject.tag == "StartGameTrigger")
            {
                GameManager.gameState = GameManager.GameState.Playing;
                // turn on player light
                playerLightAnim.SetTrigger("LightFadeUp");
            }
        }

        // stores gold when touching the beam
        if (other.gameObject.tag == "Beam")
        {
            if (GameManager.heldGold > 0)
            {
                GameManager.thisSessionGoldGained += GameManager.heldGold;
                GameManager.storedGold += GameManager.heldGold;
                PlayerPrefs.SetInt("storedGold", GameManager.storedGold);
                GameManager.heldGold = 0;
                gameManager.goldBonusLevel = 0;
            }
        }
    }

    IEnumerator Reload()
    {
        if (weapon1 == WeaponType.pistol)
        {
            isReloading = true;
            pistolAnim.SetBool("ReloadGun", true);
            yield return new WaitForSeconds(reloadSpeed_Pistol);
            pistolAnim.SetBool("ReloadGun", false);
            pistolAmmo = pistolAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.shotgun)
        {
            isReloading = true;
            shotgunAnim.SetBool("ReloadGun", true);
            yield return new WaitForSeconds(reloadSpeed_Shotgun);
            shotgunAnim.SetBool("ReloadGun", false);
            shotgunAmmo = shotgunAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.machinegun)
        {
            isReloading = true;
            machinegunAnim.SetBool("ReloadGun", true);
            yield return new WaitForSeconds(reloadSpeed_Machinegun);
            machinegunAnim.SetBool("ReloadGun", false);
            machinegunAmmo = machinegunAmmoMax;
            isReloading = false;
        }
        if (weapon1 == WeaponType.rocket)
        {
            isReloading = true;
            rocketAnim.SetBool("ReloadGun", true);
            yield return new WaitForSeconds(reloadSpeed_Rocket);
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
    }

    public void KillPlayer()
    {
        // we spawn a new camera that flops to the ground when we die
        GameObject tempCam = Instantiate(deadCam, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;
        // we add a slight random rotation to the camera to give a good effect
        tempCam.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10,30), Random.Range(10,30), Random.Range(10,30)));
        gameObject.SetActive(false);
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

                StartCoroutine(MuzzleFlash(pistolMuzzleLight));
                GameObject temp = Instantiate(bullet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                pistolAmmo--;
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

                    StartCoroutine(MuzzleFlash(machinegunMuzzleLight));
                    GameObject temp = Instantiate(bullet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                    temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);

                    machinegunAmmo--;
                    mgCanShoot = false;
                    // reset our timer
                    mgFireRateCurrent = machinegunFireRate;
                }
                
            }
        }
        // ROCKET
        if (weapon1 == WeaponType.rocket)
        {
            if (rocketAmmo > 0)
            {
                rocketAnim.SetTrigger("FireGun");

                GameObject temp = Instantiate(rocketProjectile, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                // no need to add force to the rocket, since we'll have a script that moves it on the rocket itself
                //temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                rocketAmmo--;
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
        if (canGainGrenadeJuice)
        {
            if (GrenadeJuiceCurrent < grenadeJuiceMax)
            {
                GrenadeJuiceCurrent += grenadeJuicePerKill;
            }
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
                        gameManager.ChangeEquipButtonText();
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
                        gameManager.ChangeEquipButtonText();
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
                    ChangeWeaponModel();
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
                            gameManager.ChangeEquipButtonText();
                        }
                    }
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
                            gameManager.ChangeEquipButtonText();
                        }
                    }
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
                            gameManager.ChangeEquipButtonText();
                        }
                    }
                }
            }
        }
    }

    IEnumerator GrenadeJuiceCoolDown()
    {
        canGainGrenadeJuice = false;
        yield return new WaitForSeconds(3);
        canGainGrenadeJuice = true;
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
}

