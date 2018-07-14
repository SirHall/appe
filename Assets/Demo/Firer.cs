using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excessives.LinqE;
using Excessives;

public class Firer : MonoBehaviour
{
	public IProjData projData;

	AffectedProjectile affectedProj;

	public Vector3 initialDirection = Vector3.right;

	public float initialVelocity = 250.0f;

	void Start()
	{
		affectedProj = new AffectedProjectile(Vector3.zero, Vector3.right, projData.ProjInfo, 5.0f);
		affectedProj.AddForce(initialDirection.normalized * initialVelocity, ForceMode.VelocityChange, Time.deltaTime);
	}

	void Update()
	{
		affectedProj.Tick(Time.deltaTime);
	}
}
