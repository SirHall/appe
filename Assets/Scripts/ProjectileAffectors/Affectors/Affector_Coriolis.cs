using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CoriolisAffector", menuName = "AProjectiles/Affectors/CoriolisAffector")]
[System.Serializable]
public class Affector_Coriolis : AffectorBase
{
	//public Affector_Coriolis() : base()
	//{ }

	public override void Tick_PostPhysics(float deltaTime)
	{
		proj.physicsTransform.AddForce(
			GetCoriolisAcceleration(proj.physicsTransform.Velocity),
			ForceMode.Acceleration,
			deltaTime
		);
	}

	public Vector3 GetCoriolisAcceleration(Vector3 velocity)
	{
		return CoriolisAcceleration(
			velocity,
			Vector3.up,
			ProjectileEnvironment.instance.environment.latitude,
			ProjectileEnvironment.instance.environment.planetaryLengthOfDay
			);
	}

	public static Vector3 CoriolisAcceleration(
		Vector3 velocity,
		Vector3 upVector,
		float latitude,
		float lengthOfDay
	)
	{
		return
			Vector3.Cross(
			-2 * ((Quaternion.Euler(90 - latitude, 0, 0) * upVector).normalized * (2 * Mathf.PI) / lengthOfDay),
			velocity
		);
	}

}
