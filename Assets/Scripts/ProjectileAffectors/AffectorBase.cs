using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "ProjectileAffector", menuName = "AProjectiles/ProjectileAffector")]
public abstract class AffectorBase : ScriptableObject
{
	public abstract void Tick(AffectedProjectile proj, float deltaTime);
}
