using Excessives.Unity;
using System;
using UnityEngine;

//The purely simulated projectile

[System.Serializable]
public class AffectedProjectile : ICloneable
{
	//Gun specific
	public float spinTwist;

	public ProjectileData projectileData;

	//{TODO} Do we need this?
	public float minVelocityCutoff = 0.01f; //When the projectile falls below this velocity, it is disabled

	public LayerMask rayCastLayerMask;

	public AffectorBase[] affectors;

	public float latitude
	{
		get
		{
			//{TODO} There is a function somewhere that calculates latitude properly, find it!
			return ProjectileEnvironment.instance.environment.latitude;
		}
	}

	//Only initialize once as not to trigger the GC
	//(Also allows anything that we collide into to get the full collision info)
	RaycastHit hitInfo;

	#region Initialization

	public AffectedProjectile(
		Vector3 position,
		Vector3 direction,
		ProjectileData projectileData,
		//GameObject gameObjectRep,
		float spinTwist
	//bool initialyActive
	)
	{
		Initial(position, direction, projectileData, spinTwist);

		//active = initialyActive;
	}


	public void Initial(
		Vector3 position,
		Vector3 direction,
		ProjectileData projectileData,
		//GameObject gameObjectRep,
		float twist
	)
	{
		this.position = position;

		this.velocity = //Properly will result in a circle shot group, rather than square
						//Quaternion.Euler(UnityEngine.Random.Range(0, projectileData.spreadAngle), 0, 0) //{TODO} Use this maths on the gunnery end
						//*
						//Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.forward)
						//*
		(direction * projectileData.initVelocity);


		//("  Initial: " + velocity.ToString()).Log();


		this.projectileData = projectileData;

		//if (this.gameObjectRep != gameObjectRep)
		//{
		//    GameObject.Destroy(this.gameObjectRep);
		//    this.gameObjectRep = GameObject.Instantiate(
		//        gameObjectRep,
		//        position,
		//        Quaternion.FromToRotation(Vector3.forward, direction)
		//    );
		//}

		this.spinTwist = twist;

		//active = true;
	}

	#endregion

	public void Tick(float deltaTime)
	{

		hitInfo = default(RaycastHit);

		prevVelocity = velocity;
		prevPosition = position;

		//Affectors

		for (int i = 0; i < affectors.Length; i++)
		{
			affectors[i].Tick(this, deltaTime);
		}

		position += velocity * deltaTime;

		if (Physics.Raycast(//If there is an object between us and the next position
				prevPosition,
				prevVelocity,
				out hitInfo,
				Vector3.Distance(prevPosition, position),
				rayCastLayerMask))
		{

			position = hitInfo.point;

			#region Projectile Hitable

			//If object is IProjectileHitable, then 'hit' it

			IProjectileHitable hitable = hitInfo.collider.attachedRigidbody.GetComponent<IProjectileHitable>();

			if (hitable != null && hitable.updateOnIndirectHits)
			{
				hitable.Hit(this, hitInfo);
			}

			hitable = hitInfo.collider.gameObject.GetComponent<IProjectileHitable>();

			if (hitable != null)
			{
				hitable.Hit(this, hitInfo);
			}


			#endregion

			#region Send Impulse to RigidBody

			//{TODO} Properly setup how much kinetic energy we lose to the impact

			if (hitable.recieveImpulse)
			{
				hitInfo.collider.attachedRigidbody.AddForceAtPosition(
					velocity,
					position,
					ForceMode.Impulse
					);
			}

			#endregion

			#region Richochet
			//{TODO} Tie this up together with how much energy we lose to the rigidbody
			//Restitution dependant richochet
			PhysicMaterial physMat =
				hitInfo.collider.gameObject.GetComponent<PhysicMaterial>();
			velocity =
				(physMat == null)
				? //If the hit object does not have a physics material, then just do a normal reflect
				Vector3.Reflect(velocity, hitInfo.normal)
				: //However, if the object does have a physics material, lerp from a reflect and simply stopping
				  //{TODO} Make this more realistic
				Vector3.Lerp(
				Vector3.zero,
				Vector3.Reflect(velocity, hitInfo.normal),
				physMat.bounciness
			);

			#endregion
		}


		Debug.DrawLine(prevPosition, position, Color.blue, 20);

		DrawingFuncs.DrawStar(position, Color.green, .5f, 20);
	}

	#region Physics

	[HideInInspector]
	public Vector3
		position = Vector3.zero,
		velocity = Vector3.zero,
		prevPosition = Vector3.zero,
		prevVelocity = Vector3.zero;

	public void AddForce(Vector3 force, ForceMode mode, float deltaTime)
	{
		switch (mode)
		{
			case ForceMode.Acceleration:
				velocity += force * deltaTime;
				return;
			case ForceMode.Force:
				velocity += (force / projectileData.bulletMass) * deltaTime;
				return;
			case ForceMode.Impulse:
				velocity += force / projectileData.bulletMass;
				return;
			case ForceMode.VelocityChange:
				velocity += force;
				return;
		}
	}

	#endregion

	public object Clone()
	{
		return (AffectedProjectile)MemberwiseClone();
	}
}