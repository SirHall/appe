using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerminalBallisticCalculator : ICloneable {
	//Split this up into multiple calculators
	public virtual void ProcessTerminalHit(TerminalBallisticsData penData) {
		//penData.projectile.physicsTransform.Position = penData.hitInfo.point;

		#region Projectile Hitable

		//If object is IProjectileHitable, then 'hit' it

		IProjectileHitable hitable = penData.hitInfo.collider.GetComponent<IProjectileHitable>();

		if (hitable != null) {
			hitable.Hit(penData.projectile, penData.hitInfo);

			//{TODO} Properly setup how much kinetic energy we lose to the impact
			if (hitable.recieveImpulse) {
				penData.hitInfo.collider.attachedRigidbody.AddForceAtPosition(
					penData.projectile.physicsTransform.Velocity,
					penData.projectile.physicsTransform.Position,
					ForceMode.Impulse
					);
			}
		}


		#endregion
	}

	public virtual object Clone() {
		return MemberwiseClone() as TerminalBallisticCalculator;
	}
}

[System.Serializable]
public class TerminalBallisticsData {
	public RaycastHit hitInfo;
	public AffectedProjectile projectile;
	//{TODO} Add other required fields

}
