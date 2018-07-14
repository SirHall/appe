using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu (fileName = "Gas", menuName = "AProjectiles/Reactions/Gas")]
[System.Serializable]
public class ReactionGas //: ScriptableObject
{
	public const float idealGasConstant = 8.3144598f;

	public float molarMass;

	/// <summary>
	/// Pressure of any gas in Pascals
	/// </summary>
	/// <param name="mass">Mass in grams.</param>
	/// <param name="temperature">Temperature in Kelvin.</param>
	/// <param name="volume">Volume in cubic metres.</param>
	/// <param name="molarMass">Molar mass in grams per mole.</param>
	public static float Pressure (
		float mass, float temperature,
		float volume, float molarMass
	)
	{
		return 
		(mass * idealGasConstant * temperature)
		/
		(volume * molarMass);
	}
}
