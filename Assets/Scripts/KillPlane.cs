using UnityEngine;
using System.Collections;

public class KillPlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent)
        {
            Destroy(other.transform.parent.gameObject);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
