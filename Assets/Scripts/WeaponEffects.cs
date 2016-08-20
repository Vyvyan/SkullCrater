using UnityEngine;
using System.Collections;

public class WeaponEffects : MonoBehaviour {

    public GameObject muzzleFlash;
    public Transform muzzleFlashTransform;

	// Use this for initialization
	void Start ()
    {
        muzzleFlashTransform = transform.GetChild(0).transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void CreateMuzzleFlash()
    {
        GameObject temp = Instantiate(muzzleFlash, muzzleFlashTransform.position, muzzleFlashTransform.rotation) as GameObject;
        temp.transform.SetParent(gameObject.transform.parent.transform);
    }
}
