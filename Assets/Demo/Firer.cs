using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;
using Excessives;

public class Firer : MonoBehaviour
{
	//public IProjData projData;

	public AffectedProjectile affectedProj;
	//AffectedProjectile affectedProj;

	public Vector3 initialDirection = Vector3.right;

	public float initialVelocity = 250.0f;

	void Start()
	{
		affectedProj.physicsTransform
			.AddForce(initialDirection.normalized * initialVelocity, ForceMode.VelocityChange, Time.deltaTime);
		ProjectilePool.instance.FireProjectile(affectedProj);
	}

	void Update()
	{
		//affectedProj.Tick(Time.deltaTime);
	}
}
