using UnityEngine;
using System.Collections;
using System;

//{TODO} Remove this
[System.Obsolete]
public class AffectorFactorySingleton
{
	public static AffectorFactorySingleton instance;

	void Awake()
	{
		instance = this;
	}

	//AffectorFactory affectorFactory = new AffectorFactory();

	//public AffectorBase GetNewAffector(Type type, params object[] parameters)
	//{
	//	//return affectorFactory.GetNewInstance(type, parameters);
	//}

}
