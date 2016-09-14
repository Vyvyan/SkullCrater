using UnityEngine;
using System.Collections;

public class SkullBossParent : MonoBehaviour {

    BlackSkull blackSkull;
    public ParticleSystem spawnParticles;
    public GameObject bossRocks;

	// Use this for initialization
	void Start ()
    {
        blackSkull = GetComponentInChildren<BlackSkull>();
        spawnParticles.Stop();
        bossRocks.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void ChangeStateAfterSpawnAnimation()
    {
        blackSkull.bossState = BlackSkull.BossState.idle;
    }

    public void StartParticles()
    {
        spawnParticles.Play();
    }

    public void StopParticles()
    {
        spawnParticles.Stop();
    }

    public void SpawnRocks()
    {
        bossRocks.SetActive(true);
    }
}
