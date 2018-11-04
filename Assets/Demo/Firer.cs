using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;
using Excessives;

public class Firer : MonoBehaviour
{
	//public IProjData projData;

	public AffectedProjectile affectedProj = new AffectedProjectile();
	//AffectedProjectile affectedProj;

	public Vector3 initialDirection = new Vector3(0.0f, 10.0f, 50.0f);

	public float initialVelocity = 250.0f;

	void Start()
	{
		affectedProj.physicsTransform
			.AddForce(initialDirection.normalized * initialVelocity, ForceMode.VelocityChange, Time.deltaTime);
		ProjectilePool.instance.FireProjectile(affectedProj);
		affectedProj.Initial(); //{TODO} This is dirty
	}

	void Update()
	{
		//affectedProj.Tick(Time.deltaTime);
	}
}
