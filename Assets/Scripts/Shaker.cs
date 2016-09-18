using UnityEngine;
using System.Collections;

public class Shaker : MonoBehaviour
{
    public float shakeTimer;
    Vector3 originalPosition;

    void Start()
    {
        originalPosition = gameObject.transform.position;
        shakeTimer = 8;
    }

    void LateUpdate()
    {
        if (shakeTimer > 0)
        {
            Vector3 newPosition = Random.insideUnitSphere * .05f;
            newPosition.x += transform.position.x;
            newPosition.y += transform.position.y;
            newPosition.z += transform.position.z;
            gameObject.transform.position = newPosition;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = originalPosition;
            Destroy(this);
        }
    }
}