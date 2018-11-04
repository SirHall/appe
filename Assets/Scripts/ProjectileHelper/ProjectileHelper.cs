using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProjectileHelper
{
	//Is compatible with compound-colliders
	public static ThicknessData FindThickness(Vector3 origin, Vector3 direction, Func<RaycastHit, bool> isColliderValid, float maxDist = 100, int layerMask = int.MaxValue)
	{
		//Damn Linq can be beautiful
		RaycastHit[] casts =
			Physics.RaycastAll(origin, direction, maxDist, layerMask) //Forward cast
			.Union(Physics.RaycastAll(origin + (direction.normalized * maxDist), -direction, maxDist, layerMask)) //Reverse cast
			.Where(n => isColliderValid(n)) //Only valid colliders
			.OrderBy(n => Vector3.Distance(origin, n.point)) //Order by distance
			.ToArray();

		foreach (var cast in casts)
			DrawingFuncs.DrawStar(cast.point, Color.blue, 1.0f, float.PositiveInfinity);

		//Find exit
		int depth = 1; //For the depth in the colliders
		for (int i = 0; i < casts.Length; i++)
		{
			//We use Vector3.Dot to tell which direction the face is pointing relative to our fire direction
			depth += (int)Mathf.Sign(Vector3.Dot(direction, -casts[i].normal)); //Damn I love one-liners
			if (depth == 0)
				return new ThicknessData(Vector3.Distance(origin, casts[i].point), casts[i].point);
		}

		//If a proper exit is not found
		return default(ThicknessData);
	}
}

public struct ThicknessData
{
	public ThicknessData(float thickness, Vector3 exitPosition)
	{
		this.thickness = thickness;
		this.exitPosition = exitPosition;
	}

	public float thickness;
	public Vector3 exitPosition;
}