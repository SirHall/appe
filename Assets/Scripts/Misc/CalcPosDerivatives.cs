using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcPosDerivatives : MonoBehaviour
{

	public bool debug = false;

	public Vector3 posCurrent = Vector3.zero;
	public Vector3 posLast = Vector3.zero;

	public Vector3 velCurrent = Vector3.zero;
	public Vector3 velLast = Vector3.zero;

	public Vector3 accelCurrent = Vector3.zero;
	public Vector3 accelLast = Vector3.zero;

	public Vector3 jerkCurrent = Vector3.zero;
	public Vector3 jerkLast = Vector3.zero;

	public Vector3 jounceCurrent = Vector3.zero;
	public Vector3 jounceLast = Vector3.zero;

	void FixedUpdate () //Note: Note adjusted for time!
	{
		posCurrent = transform.position;

		velLast = velCurrent;
		velCurrent = (posCurrent - posLast) / Time.fixedDeltaTime;

		accelLast = accelCurrent;
		accelCurrent = (velCurrent - velLast) / Time.fixedDeltaTime;


		jerkLast = jerkCurrent;
		jerkCurrent = (accelCurrent - accelLast) / Time.fixedDeltaTime;

		jounceLast = jounceCurrent;
		jounceCurrent = (jerkCurrent - jerkLast) / Time.fixedDeltaTime;


		///Leave last!
		posLast = transform.position;

		if (debug) {
			//Velocity
			Debug.DrawRay (transform.position, velCurrent, Color.blue, Time.fixedDeltaTime, false);
			//Acceleration
			Debug.DrawRay (transform.position, accelCurrent, Color.red, Time.fixedDeltaTime, false);
			//Jerk
			Debug.DrawRay (transform.position, jerkCurrent, Color.gray, Time.fixedDeltaTime, false);
			//Jounce
			Debug.DrawRay (transform.position, jounceCurrent, Color.black, Time.fixedDeltaTime, false);
		}
	}

}
