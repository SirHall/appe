using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerminalBallisticDetector
{
	//public AffectedProjectile projectile;

	public LayerMask layerMask;

	public TerminalBallisticsData Tick(AffectedProjectile proj)
	{
		RaycastHit rayHitInfo;

		//projectile.physicsTransform
		if (Physics.Raycast(
			proj.physicsTransform.Position,
			proj.physicsTransform.Velocity.normalized,
			out rayHitInfo,
			Vector3.Distance(proj.physicsTransform.Position, proj.physicsTransform.PrevPosition),
			layerMask))
		{
			return new TerminalBallisticsData() { projectile = proj, hitInfo = rayHitInfo };
		}
		return null;
	}
}
