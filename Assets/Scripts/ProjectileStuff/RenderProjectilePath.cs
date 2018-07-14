using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderProjectilePath : MonoBehaviour
{

	void Start ()
	{
		float mass = 5;
		float k = 500 / (2 * mass);

		float velocity = 150;
		float windVelocity = 0;
		float angle = Mathf.PI / 5;

		float prevPos = 0;

		for (float i = 0; i <= 10000; i += 0.1f) {

//			float dragPos =
//				(mass / k)
//				* Mathf.Log (
//					((k * i * (velocity * windVelocity) * Mathf.Abs (Mathf.Sin (angle)))
//					/
//					mass)
//					+ 1
//				)
//				* Mathf.Sign (angle);
		
			float nextPos = velocity - (9.81f * Mathf.Pow (i, 2) / 2);

			Debug.DrawLine (
				new Vector3 (0, prevPos, i - 0.1f),
				new Vector3 (0, nextPos, i),
				Color.red,
				Mathf.Infinity,
				true
			);

			prevPos = nextPos;
		}
	}

	public static float ProjectileYAtTime (
		float time, 
		float angle, //Radians
		float velocity, //Single axis
		float mass,
		float initialCoriolis, //Single axis
		float windVelocity, //Single axis
		float latitude, 
		float planetaryRotationalVelocity,
		float dragCoefficient,
		float airDensity,
		float crossSectionalArea,
		float SG,
		float twist
	)
	{	
		float k = (dragCoefficient * crossSectionalArea * airDensity) / (2 * mass);

		float dragPos =
			(mass / k)
			* Mathf.Log (
				((k * time * (velocity * windVelocity) * Mathf.Abs (Mathf.Sin (angle)))
				/
				mass)
				+ 1
			)
			* Mathf.Sign (angle);


		return 0;
	}
}
