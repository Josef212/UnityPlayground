using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonSpawner : MonoBehaviour 
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float spawnDistance;
    [SerializeField] private Nucleon[] nucleonPrefabs;

    private float timeSinceLastSpawn;

    //========================================================

    void Start () 
	{
		
	}
	

	void FixedUpdate () 
	{
        timeSinceLastSpawn += Time.deltaTime;
        if(timeSinceLastSpawn >= timeBetweenSpawns)
        {
            SpawnNucleon();
            timeSinceLastSpawn -= timeBetweenSpawns;
        }
	}

	//========================================================

	private void SpawnNucleon()
    {
        Nucleon obj = Instantiate<Nucleon>(nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)], transform);
        obj.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
