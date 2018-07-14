using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcVelocity : MonoBehaviour
{

	[HideInInspector]
	public Vector3 velocity = Vector3.zero;

	Vector3 lastPosition;

	void Update ()
	{
		velocity = (transform.position - lastPosition) / Time.deltaTime;

		lastPosition = transform.position;
	}
}
