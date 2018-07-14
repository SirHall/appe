using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnvironment : MonoBehaviour
{
	public static ProjectileEnvironment instance;

	public ProjectileEnvironmentData environment;

	void Awake ()
	{
		instance = this;
	}
}
