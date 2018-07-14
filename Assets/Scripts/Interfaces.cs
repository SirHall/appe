using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileHitable
{
	/// <summary>
	/// If this returns true, and is placed on a MonoBehaviour on the same GameObject as a rigidbody, this will
	/// also recieve 'Hit' updates if a collider on a child object is hit.
	/// </summary>
	bool updateOnIndirectHits { get; }
	/// <summary>
	/// Whether or not the projectile should impart velocity into our rigidbody
	/// </summary>
	bool recieveImpulse { get; }
	/// <summary>
	/// Get's activated hen we're hit
	/// </summary>
	/// <param name="affectedProjectile"></param>
	void Hit(AffectedProjectile affectedProjectile, RaycastHit hitInfo);
}
