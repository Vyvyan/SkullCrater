using UnityEngine;
using System.Collections;

public class BackWarner : MonoBehaviour {

    public Light warningLight;
    public float lightTimer;
    public GameObject warningImage;
    Collider col;

	// Use this for initialization
	void Start ()
    {
        col = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	    if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;
            warningLight.intensity = 6;
        }
        else
        {
            warningLight.intensity = 0;
        }
        

        if (col.enabled == false)
        {
            lightTimer -= Time.deltaTime;
            if (warningImage.activeSelf == false)
            {
                warningImage.SetActive(true);
            }
        }
        else
        {
            if (warningImage.activeSelf == true)
            {
                warningImage.SetActive(false);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        /*
        if (other.tag == "Enemy")
        {
            lightTimer = 1f;
        }
        */
        if (other.tag == "Enemy")
        {
            col.enabled = false;
            lightTimer = .3f;
        }
    }

    void LateUpdate()
    {
        col.enabled = true;
    }

}
