using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandPos : MonoBehaviour
{
	public Vector3 lowerPos;

	public Vector3 upperPos;

	Vector3 goalPos;

	void ReCalcGoal ()
	{
		goalPos.x = Random.Range (lowerPos.x, upperPos.x);
		goalPos.y = Random.Range (lowerPos.y, upperPos.y);
		goalPos.z = Random.Range (lowerPos.z, upperPos.z);
	}


	void Update ()
	{
		if (transform.position == goalPos) {
			ReCalcGoal ();
		}

		transform.position = Vector3.MoveTowards (transform.position, goalPos, 2f);
	}
}
