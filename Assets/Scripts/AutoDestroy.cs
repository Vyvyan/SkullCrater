using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{

    public float timeUntilDestroy;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(TimedDestroy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(timeUntilDestroy);
        Destroy(gameObject);
    }
}
