using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public Transform bulletSpawnPoint;
    public GameObject bullet;
    public GameObject grenadeObject;
    public float gunPower;
    Camera mainCamera;
    public GameObject deadCam;

    public int pistolAmmoMax;
    public int pistolAmmo;
    public bool isReloading;
    public float reloadSpeed;

    public float health;


	// Use this for initialization
	void Start ()
    {
        mainCamera = Camera.main;
        //start with full clip
        pistolAmmo = pistolAmmoMax;
        // start our health at 1, cause idk how much we need
        health = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            // fire a shot
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isReloading)
                {
                    if (pistolAmmo > 0)
                    {
                        GameObject temp = Instantiate(bullet, bulletSpawnPoint.position, mainCamera.transform.rotation) as GameObject;
                        temp.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * gunPower, ForceMode.Impulse);
                        pistolAmmo--;
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
                if (pistolAmmo < pistolAmmoMax)
                {
                    if (!isReloading)
                    {
                        StartCoroutine(Reload());
                    }
                }
            }
        }

        // if we die
        if (health <= 0)
        {
            GameManager.gameState = GameManager.GameState.Dead;
            KillPlayer();
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
        isReloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        pistolAmmo = pistolAmmoMax;
        isReloading = false;
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
}
