using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_VirtualPhysicsTransform : VirtualPhysicsTransform
{

	public LayerMask collisionMask;

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);
	}

}
