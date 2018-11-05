using System;
using OdinSerializer;

//[CreateAssetMenu(fileName = "ProjectileAffector", menuName = "AProjectiles/ProjectileAffector")]
[System.Serializable]
public class AffectorBase : ICloneable {
	//public AffectorBase() { }

	//public AffectorBase(AffectedProjectile projectile)
	//{
	//	this.proj = projectile;
	//}

	[NonSerialized]
	protected AffectedProjectile proj;

	public AffectedProjectile Projectile { set { this.proj = value; } }

	public virtual void Tick_PrePhysics(float deltaTime) { }

	public virtual void Tick_PostPhysics(float deltaTime) { }

	public virtual object Clone() {
		return (AffectorBase)MemberwiseClone();
	}
}
