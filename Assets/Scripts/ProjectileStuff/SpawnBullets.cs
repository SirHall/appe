using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBullets : MonoBehaviour
{

	public GameObject bullet;

	[Range (-90, 90)]
	float fireAngle = -15f;

	[Range (-90, 90)]
	public float maxRotateRandom = 0;

	public int numberOfBullets = 1;

	public float spawnTime = 0;

	float spawnTimeClock = 0;

	int spawned = 0;



	void Start ()
	{
		spawnTimeClock = spawnTime;
		for (int i = 0; i < numberOfBullets; i++) {
			Instantiate (bullet, Vector3.zero, Quaternion.Euler (fireAngle + Random.Range (-maxRotateRandom, maxRotateRandom), Random.Range (-maxRotateRandom, maxRotateRandom), Random.Range (-maxRotateRandom, maxRotateRandom)));
		}

	}

	void Update ()
	{
		spawnTimeClock -= Time.deltaTime;


		/*if (spawnTimeClock <= 0 && spawned < numberOfBullets) {
			Instantiate (bullet, Vector3.zero, Quaternion.Euler (fireAngle + Random.Range (-maxRotateRandom, maxRotateRandom), Random.Range (-maxRotateRandom, maxRotateRandom), Random.Range (-maxRotateRandom, maxRotateRandom)));
			spawned += 1;
		}

		if (spawned >= numberOfBullets) {
			Debug.LogError ("We've spawned: " + numberOfBullets + " bullets");
		}*/
	}
}
