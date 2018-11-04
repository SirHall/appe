using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SpinTwistAffector", menuName = "AProjectiles/Affectors/SpinTwistAffector")]
[System.Serializable]
public class Affector_SpinTwist : AffectorBase
{
	//public Affector_SpinTwist() : base()
	//{
	//}

	public override void Tick_PostPhysics(float deltaTime)
	{
		Vector3 pos = proj.physicsTransform.Position;

		pos +=
			Quaternion.FromToRotation(Vector3.forward, proj.physicsTransform.Velocity.normalized) *
			new Vector3(
				GetSpinDrift(
						GetMillerStability(
							proj.projectileData.bulletMass,
							proj.projectileData.bulletDiameter,
							proj.projectileData.bulletLength,
							proj.physicsTransform.SpinVelocity
							),
					deltaTime,
					proj.physicsTransform.SpinVelocity),
				0, 0);
		//proj.physicsTransform.Position = pos;
	}

	#region Related Methods

	public float GetSpinDrift(float millerStability, float deltaTime, float twist)
	{
		// For the source of this equation please visit
		// https://loadoutroom.com/13415/reaper-tips-spin-drift-coriolis-effect/

		//Please note that, when viewed from the rear:
		//The bullet will drift to the right if spinning clockwise
		//The bullet will drift to the left if spinning anticlockwise
		//If this is being calculated on a bullet each physics tick,
		//TOF can be replaced by the time delta between those ticks

		//The 'SG' value can be calculated using the 'MillerStability()' function
		//(Multiplied by 0.0254 to convert from inches to metres)

		return 0.03175f * (millerStability + 1.2f) * Mathf.Pow(deltaTime, 1.83f) * Mathf.Sign(twist);

	}

	float l, t; //Only initialize these once

	//Add in multiple stability methods

	/// <summary>
	/// Millers the stability.
	/// </summary>
	/// <returns>The gyrospoic stability of the bullet.</returns>
	/// <param name="bulletMass">Bullet mass (kg).</param>
	/// <param name="bulletDiameter">Bullet diameter (meters).</param>
	/// <param name="bulletLength">Bullet length (meters).</param>
	/// <param name="twist">Twist (meters per turn).</param>
	public float GetMillerStability(float bulletMass, float bulletDiameter, float bulletLength, float twist)
	{
		// Equation data taken from:
		// https://en.wikipedia.org/wiki/Miller_twist_rule#References


		//Firstly, convert the inputted SI values into their imperial counterparts (For the equation)
		bulletMass *= 15432.4f; //kg to grains
		bulletDiameter *= 39.3701f; //metres to inches
		bulletLength *= 39.3701f; //metres to inches
		twist *= 39.3701f; //metres to inches

		//BulletLength(calibers) = BulletLength(inches) / BulletDiameter(inches)
		//Twist(calibers ber turn) = Twist(inches per turn) / BulletDiameter(inches)

		l = bulletLength / bulletDiameter;
		t = twist / bulletDiameter;

		return
			(30 * bulletMass) /
			(
				t * t *
				bulletDiameter * bulletDiameter * bulletDiameter *
				l *
				(1 + (l * l))
			);
	}


	#endregion

}
