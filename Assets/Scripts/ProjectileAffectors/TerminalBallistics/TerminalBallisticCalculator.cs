using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerminalBallisticCalculator
{
	public virtual void ProcessTerminalHit(TerminalBallisticsData penData)
	{
		penData.projectile.physicsTransform.Position = penData.hitInfo.point;

		#region Projectile Hitable

		//If object is IProjectileHitable, then 'hit' it

		IProjectileHitable hitable = penData.hitInfo.collider.attachedRigidbody.GetComponent<IProjectileHitable>();

		if (hitable != null && hitable.updateOnIndirectHits)
		{
			hitable.Hit(penData.projectile, penData.hitInfo);
		}

		hitable = penData.hitInfo.collider.gameObject.GetComponent<IProjectileHitable>();

		if (hitable != null)
		{
			hitable.Hit(penData.projectile, penData.hitInfo);
		}


		#endregion

		#region Send Impulse to RigidBody

		//{TODO} Properly setup how much kinetic energy we lose to the impact

		if (hitable.recieveImpulse)
		{
			penData.hitInfo.collider.attachedRigidbody.AddForceAtPosition(
				penData.projectile.physicsTransform.Velocity,
				penData.projectile.physicsTransform.Position,
				ForceMode.Impulse
				);
		}

		#endregion

		#region Richochet
		//{TODO} Tie this up together with how much energy we lose to the rigidbody
		//Restitution dependant richochet
		PhysicMaterial physMat =
			penData.hitInfo.collider.gameObject.GetComponent<PhysicMaterial>();
		penData.projectile.physicsTransform.Velocity =
			(physMat == null)
			? //If the hit object does not have a physics material, then just do a normal reflect
			Vector3.Reflect(penData.projectile.physicsTransform.Velocity, penData.hitInfo.normal)
			: //However, if the object does have a physics material, lerp from a reflect and simply stopping
			  //{TODO} Make this more realistic
			Vector3.Lerp(
			Vector3.zero,
			Vector3.Reflect(penData.projectile.physicsTransform.Velocity, penData.hitInfo.normal),
			physMat.bounciness
		);

		#endregion
	}
}


public class TerminalBallisticsData
{
	public RaycastHit hitInfo;
	public AffectedProjectile projectile;
	//{TODO} Add other required fields

}
