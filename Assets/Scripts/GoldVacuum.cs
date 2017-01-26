using UnityEngine;
using System.Collections;

public class GoldVacuum : MonoBehaviour {

    GameObject player;
    bool isInRangeOfPlayer;
    Rigidbody goldRB;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        goldRB = GetComponentInParent<Rigidbody>();
        isInRangeOfPlayer = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInRangeOfPlayer)
        {
            goldRB.AddForce((player.transform.position - gameObject.transform.position).normalized * .35f, ForceMode.VelocityChange);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isInRangeOfPlayer = true;
        }
    }

    /*
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isInRangeOfPlayer = false;
        }
    }
    */

}
