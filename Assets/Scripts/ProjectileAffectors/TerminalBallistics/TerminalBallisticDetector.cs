using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerminalBallisticDetector : ICloneable {
	//public AffectedProjectile projectile;

	public LayerMask layerMask = new LayerMask();

	public TerminalBallisticsData Tick(AffectedProjectile proj, bool debugHits = false) {
		RaycastHit rayHitInfo;

		//projectile.physicsTransform
		if (Physics.Raycast(
			proj.physicsTransform.PrevPosition,
			proj.physicsTransform.Position - proj.physicsTransform.PrevPosition,
			out rayHitInfo,
			Vector3.Distance(proj.physicsTransform.Position, proj.physicsTransform.PrevPosition),
			layerMask)
			) {

			if (debugHits)
				Debug.DrawRay(
					proj.physicsTransform.PrevPosition,
					proj.physicsTransform.PrevPosition - proj.physicsTransform.Position,
					Color.cyan,
					10.0f,
					false
					);

			return new TerminalBallisticsData() { projectile = proj, hitInfo = rayHitInfo };
		}
		return null;
	}

	#region ICloneable

	public virtual object Clone() {
		return (TerminalBallisticDetector)MemberwiseClone();
	}

	#endregion

}
