using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GravityAffector", menuName = "AProjectiles/Affectors/GravityAffector")]
public class Affector_Gravity : AffectorBase
{
	public bool useAdvancedGravity;

	public Vector3 gravity;

	public override void Tick(AffectedProjectile proj, float deltaTime)
	{
		proj.AddForce(
		 useAdvancedGravity ? GetGravity(proj.position.y) : gravity,
			ForceMode.Acceleration,
			deltaTime
			);
	}

	public Vector3 GetGravity(float altitude)
	{
		return ProjectileEnvironment.instance.environment.GetGravity(altitude);
	}
}
