using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "DragWindCubedAffector", menuName = "AProjectiles/Affectors/CubedDragWindAffector")]
[System.Serializable]
public class Affector_WindDragCubed : AffectorBase
{
	//public Affector_WindDragCubed() : base()
	//{ }

	#region Settings

	/// <summary>
	/// Whether or not to use the variables from the global 'ProjectileEnvironment' component.
	/// It's recommended that this be left on
	/// </summary>
	public bool useEnvironmentSettings = true;

	#region If Not Using Environment Settings

	/// <summary>
	/// The air density in kg/m3.
	/// </summary>
	public float airDensity;

	/// <summary>
	/// The velocity of the wind in m/s
	/// </summary>
	public Vector3 windVelocity = Vector3.zero;

	public Vector3 GetwindVelocity()
	{
		return useEnvironmentSettings ?
			(Application.isPlaying ? ProjectileEnvironment.instance.environment.windVelocity : Vector3.positiveInfinity)
			:
			this.windVelocity;

	}

	#endregion

	#endregion

	public DirectionalDragCube directionDragCube = new DirectionalDragCube();

	public override void Tick_PostPhysics(float deltaTime)
	{
		proj.physicsTransform.AddForce(
			GetDragAndWindAcceleration(
					proj.physicsTransform.Velocity, proj.physicsTransform.Position.y,
					proj.projectileData.dragCoefficient,
					proj.projectileData.crossSectionalArea,
					proj.projectileData.bulletMass),
			ForceMode.Acceleration,
			deltaTime
			);
	}

	#region Related Functions

	public Vector3 GetDragAndWindAcceleration(
		Vector3 velocity, float altitude,
		float dragCoefficient, float crossSectionalArea, float projectileMass)
	{
		Vector3 velDiff = windVelocity - velocity;

		return
			GetAirDensity(altitude) * dragCoefficient * crossSectionalArea *
			new Vector3(
				Mathf.Sign(velDiff.x) * velDiff.x * velDiff.x,
				Mathf.Sign(velDiff.y) * velDiff.y * velDiff.y,
				Mathf.Sign(velDiff.z) * velDiff.z * velDiff.z
				)
				/ (2 * projectileMass);

	}

	float GetAirDensity(float altitude)
	{
		return
			useEnvironmentSettings ?
			ProjectileEnvironment.instance.environment.GetAirDensity(altitude) :
			airDensity;

	}

	#endregion
}
