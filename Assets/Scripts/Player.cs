using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Transform bulletSpawnPoint;
    public GameObject bullet, shotgunPellet;
    public GameObject grenadeObject;
    public float gunPower;
    Camera mainCamera;
    public GameObject deadCam;

    public int pistolAmmoMax, shotgunAmmoMax, machinegunAmmoMax;
    public int pistolAmmo, shotgunAmmo, machinegunAmmo;
    public bool isReloading;
    public float reloadSpeed_Pistol, reloadSpeed_Shotgun, reloadSpeed_Machinegun;

    public float health;

    public Animator pistolAnim, shotgunAnim, machinegunAnim;
    public WeaponEffects pistolEffects, shotgunEffects, machinegunEffects;
    public Light pistolMuzzleLight, shotgunMuzzleLight, machinegunMuzzleLight;

    public GameObject pistolModel, shotgunModel, machinegunModel;

    public enum WeaponType {pistol, shotgun, machinegun, rocket, none};
    public WeaponType weapon1, weapon2;

    public float machinegunFireRate, mgFireRateCurrent;
    // bool for machine gun shooting
    bool mgCanShoot;

	// Use this for initialization
	void Start ()
    {
        // start with just a pistol
        //weapon1 = WeaponType.pistol;
        //weapon2 = WeaponType.none;

        mainCamera = Camera.main;
        //start with full clip
        pistolAmmo = pistolAmmoMax;
        shotgunAmmo = shotgunAmmoMax;
        machinegunAmmo = machinegunAmmoMax;

        // start our health at 1, cause idk how much we need
        health = 1;

        // get our pistol effects script from our anim, which is on the same object
        pistolEffects = pistolAnim.gameObject.GetComponent<WeaponEffects>();
        shotgunEffects = shotgunAnim.gameObject.GetComponent<WeaponEffects>();
        machinegunEffects = machinegunAnim.gameObject.GetComponent<WeaponEffects>();

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
                        FireWeapon();
                    }
                }
            }

            // grenade
            if (Input.GetButtonDown("Fire2"))
            {
                GameObject temp = Instantiate(grenadeObject, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
            }

            // reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isReloading)
                {
                    ReloadWeapon();
                }
            }

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
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            TakeDamage();
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
    }

    public void TakeDamage()
    {
        health--;
    }

    public void KillPlayer()
    {
        // we spawn a new camera that flops to the ground when we die
        GameObject tempCam = Instantiate(deadCam, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;
        // we add a slight random rotation to the camera to give a good effect
        tempCam.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10,30), Random.Range(10,30), Random.Range(10,30)));
        gameObject.SetActive(false);
    }

    IEnumerator MuzzleFlash(Light light)
    {
        light.intensity = 6;
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

    }

    void ChangeWeaponModel()
    {
        if (weapon1 == WeaponType.pistol)
        {
            pistolModel.SetActive(true);
            shotgunModel.SetActive(false);
            machinegunModel.SetActive(false);
        }
        if (weapon1 == WeaponType.shotgun)
        {
            pistolModel.SetActive(false);
            shotgunModel.SetActive(true);
            machinegunModel.SetActive(false);
        }
        if (weapon1 == WeaponType.machinegun)
        {
            pistolModel.SetActive(false);
            shotgunModel.SetActive(false);
            machinegunModel.SetActive(true);
        }
    }
}
