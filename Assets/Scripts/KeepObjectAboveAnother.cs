using UnityEngine;
using System.Collections;

public class KeepObjectAboveAnother : MonoBehaviour {

    public GameObject whatToStayAbove;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.position = whatToStayAbove.transform.position;
	}
}
