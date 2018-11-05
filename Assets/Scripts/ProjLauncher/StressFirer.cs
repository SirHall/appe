using Excessives;
using Excessives.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressFirer : MonoBehaviour {
	//public IProjData projData;

	public AffectedProjectile affectedProj = new AffectedProjectile();
	//AffectedProjectile affectedProj;

	public Vector3
		initialDirection1 = new Vector3(0.0f, 1.0f, 5.0f),
		initialDirection2 = new Vector3(0.0f, 5.0f, 1.0f);

	public float initialVelocity = 50.0f;

	public int instances = 10;

	void Start() {
		Fire();
	}

	void Update() {
		if (KeyCode.F.Pressed())
			Fire();
	}

	void Fire() {
		AffectedProjectile newInst;
		for (int i = 0; i < instances; i++) {
			newInst = affectedProj.Clone() as AffectedProjectile;
			Vector3 dir = Vector3.Lerp(initialDirection1, initialDirection2, (float)CryptoRand.Range()).normalized;
			newInst.physicsTransform.AddForce(dir * initialVelocity, ForceMode.VelocityChange, Time.deltaTime);

			ProjectilePool.instance.FireProjectile(newInst);
			newInst.Initial(); //{TODO} This is dirty
		}
	}
}
