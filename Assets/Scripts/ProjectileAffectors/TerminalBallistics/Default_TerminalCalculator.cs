using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//{TODO} Clean up everything in this class
[System.Serializable]
public class Default_TerminalCalculator : TerminalBallisticCalculator {
	public override void ProcessTerminalHit(TerminalBallisticsData penData) {
		//penData.projectile.physicsTransform.Position = penData.hitInfo.point + (penData.hitInfo.normal * 0.01f);
		//penData.projectile.physicsTransform.Position = penData.projectile.physicsTransform.PrevPosition;

		base.ProcessTerminalHit(penData);

		Tuple<bool, float> willPenInfo = WillPen(penData);


		if (willPenInfo.Item1)
			Penetrate(penData, willPenInfo.Item2);
		else
			Richochet(penData);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="penData"></param>
	/// <returns>bool willPenetrate, float penetrationDepth</returns>
	protected virtual Tuple<bool, float> WillPen(TerminalBallisticsData penData) {
		ProjectileData projData = penData.projectile.projectileData;
		VirtualPhysicsTransform physicsTransform = penData.projectile.physicsTransform;

		//(assuming 0 degrees is hitting straight on)
		//If angleWeHitTarget >= 80 degrees, then richochet
		if (Vector3.Angle(physicsTransform.Velocity.normalized, -penData.hitInfo.normal) >= 80)
			return new Tuple<bool, float>(false, 0.0f); //When Unity supports C#7, switch this to the new syntax

		//Otherwise, do some more testing

		float targetDensity = 1000.0f; //{TODO} Hook this up

		//tex:
		//$$stoppingDist=\sqrt[3]{\frac{projMass * initVel^2}{targetDensity*projCrossArea * projDragCoefficient}} $$
		//Heavily simplified model

		float stoppingDist =
			Mathf.Pow( //3rd root
				(projData.bulletMass * physicsTransform.VelocityMagnitude * physicsTransform.VelocityMagnitude) //{TODO} Make velocity relative to target
				/
				(targetDensity * projData.CrossSectionalArea * projData.GetDragCoefficient(physicsTransform.Velocity.magnitude / 343.0f)) //{TODO} Rewrite correctly!
				, (1.0f / 3.0f)
			);

		return new Tuple<bool, float>(stoppingDist >= projData.noseLength, stoppingDist);
	}

	//{TODO} Damn this method is long...
	protected virtual void Penetrate(TerminalBallisticsData penData, float stoppingDist) {
		ProjectileData projData = penData.projectile.projectileData;
		VirtualPhysicsTransform physicsTransform = penData.projectile.physicsTransform;
		float targetDensity = 1000.0f; //{TODO} Hook this up

		//343.0f - Temporary hardcoded value - 1 mach in air ~= 343 m/s

		ThicknessData objectThickness =
			ProjectileHelper.FindThickness(
			physicsTransform.Position,
			physicsTransform.Velocity.normalized,
			(rCH) => rCH.collider.gameObject == penData.hitInfo.collider.gameObject); //Only colliders on this object

		Debug.Log($"Thickness: {objectThickness.thickness}m");

		//{TODO} Add some check to see if the object thickness was even found

		//If the projectile stops mid-object, de-activate it
		if (stoppingDist <= objectThickness.thickness) {
			penData.projectile.Active = false;
			Debug.Log($"Stopped after penetrating {stoppingDist}m");
			return;
		}

		//Otherwise, see how fast we come out the other side
		physicsTransform.Position = objectThickness.exitPosition;

		//tex:
		//$$ v=\sqrt{-\frac{ACpx^3}{m}+u^2}$$

		physicsTransform.VelocityMagnitude =
			Mathf.Sqrt(
				-
				(
					(
						 projData.CrossSectionalArea *
						 projData.GetDragCoefficient(physicsTransform.Velocity.magnitude / 343.0f) *
						 targetDensity *
						 objectThickness.thickness * objectThickness.thickness * objectThickness.thickness
					)
					/
					projData.bulletMass
				)
				+ (physicsTransform.VelocityMagnitude * physicsTransform.VelocityMagnitude)
				);

		Debug.Log($"Stopping distance: {stoppingDist}");
		Debug.Log($"Fully penetrated with an exit velocity of {physicsTransform.Velocity.magnitude}m/s");
	}

	protected virtual void Richochet(TerminalBallisticsData penData) {

		//Move us back so we don't morph through the ground
		penData.projectile.physicsTransform.Position = penData.hitInfo.point + (penData.hitInfo.normal * 0.01f);
		//{TODO} Tie this up together with how much energy we lose to the rigidbody
		//Restitution dependant richochet
		PhysicMaterial physMat =
			penData.hitInfo.collider.gameObject.GetComponent<BoxCollider>().material;
		penData.projectile.physicsTransform.Velocity =
			(physMat == null)
			? //If the hit object does not have a physics material, then just do a normal reflect
			Vector3.Reflect(penData.projectile.physicsTransform.Velocity, penData.hitInfo.normal)
			: //However, if the object does have a physics material, lerp from a reflect and simply stopping
			  //{TODO} Make this more realistic
			Vector3.Lerp(
			Vector3.zero,
			Vector3.Reflect(penData.projectile.physicsTransform.Velocity, penData.hitInfo.normal),
			physMat.bounciness *
			Vector3.Cross(penData.projectile.physicsTransform.Velocity.normalized, penData.hitInfo.normal).magnitude //Bounce more if we hit the surface at a sharp angle
		);
	}
}
