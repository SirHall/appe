using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Equations related to the simulation of projectiles

public class ProjectileAffectors : MonoBehaviour
{

	// J/(kg.K)
	public const float dryAirGasConstant = 287.05f;
	public const float waterVapourGasConstant = 461.495f;
	public const float idealGasConstant = 8.31447f;

	#region Environment





	/// <summary>
	/// Densities the of dry air.
	/// </summary>
	/// <returns>The of dry air.</returns>
	/// <param name="airPressure">Air pressure (Pascals).</param>
	/// <param name="airTemperature">Air temperature (Kelvin).</param>
	public static float DensityOfDryAir(float airPressure, float airTemperature)
	{
		return airPressure / (dryAirGasConstant * airTemperature);
	}

	public static float DensityOfMoistAir(float dryAirPartialPressure, float waterVapourPartialPressure, float airTemperature)
	{
		/*
		 * pd = Partial Pressure of dry air
		 * T = Temperature (Kelvin)
		 * pv = Partial Pressure of water vapour
		 */

		return (dryAirPartialPressure / (dryAirGasConstant * airTemperature)) + (waterVapourPartialPressure / (waterVapourGasConstant * airTemperature));
	}


	public static float TemperatureLapseRate(
		float gravitationalAcceleration,
		float waterVapourPressureofSaturatedAir,
		float pressureOfSaturatedAir,
		float temperatureOfSaturatedAir,
		float specificHeatOfDryAirAtConstantPressure = 1003.5f, //J kg^-1 K^-1
		float heatVaporizationOfWater = 2501000f, // J/kg
		float specificGasConstantOfDryAir = 287f //J kg^-1 K^-1

	)
	{
		//https://en.wikipedia.org/wiki/Lapse_rate
		float elipson = specificGasConstantOfDryAir / specificHeatOfDryAirAtConstantPressure;
		float r =
			(elipson * waterVapourGasConstant) / (pressureOfSaturatedAir - waterVapourGasConstant);

		return
			gravitationalAcceleration
		*
		(
			(specificGasConstantOfDryAir * temperatureOfSaturatedAir * temperatureOfSaturatedAir) +
			(heatVaporizationOfWater * r * temperatureOfSaturatedAir)
		)
		/
		(
			(specificHeatOfDryAirAtConstantPressure * specificGasConstantOfDryAir *
			temperatureOfSaturatedAir * temperatureOfSaturatedAir) +
			(heatVaporizationOfWater * heatVaporizationOfWater * r * elipson)
		);

	}


	/// <summary>
	/// Air temperature at given altitude.
	/// </summary>
	/// <returns>The air temperature.</returns>
	public static float AltitudeAirTemperature(
		float altutide,
		float seaLevelAirTemperature = 288.15f, //Kelvin
		float temperatureLapseRate = 0.0065f //Kelvin/metre
	)
	{
		return seaLevelAirTemperature - temperatureLapseRate * altutide;
	}

	public static float AltitudeAirDensity(
		float altitude,
		float surfaceGravitationalAcceleration,
		float airTemperature,
		float seaLevelAtmoPressure = 101.325f, // kPa
		float seaLevelTemperature = 288.15f, // Kelvin
		float temperatureLapseRate = 0.0065f, // Kelvin/metre
		float idealGasConstant = 8.31447f, // Joules/(mol*Kelvin)
		float molarMassofDryAir = 0.0289644f // kg/mol
	)
	{
		float airPressure = seaLevelAtmoPressure
							* Mathf.Pow(
								1 - ((temperatureLapseRate * altitude) / seaLevelTemperature),
								(surfaceGravitationalAcceleration * molarMassofDryAir)
								/ (idealGasConstant * temperatureLapseRate)
							);

		//{TODO} Multiply by zero if projectile above the troposphere at
		//it's latitude
		return
			(airPressure * molarMassofDryAir)
		/
		(idealGasConstant * airTemperature);
	}

	/// <summary>
	/// Water's the vapour pressure.
	/// </summary>
	/// <returns>The vapour pressure (Celsius).</returns>
	/// <param name="dewPoint">Dew point.</param>
	public static float WaterVapourPressure(float dewPointTemperature)
	{
		// All info found at:
		// https://www.brisbanehotairballooning.com.au/calculate-air-density/

		return (0.99999683f + dewPointTemperature * //0
		(-0.90826951f * (10 ^ -2) + dewPointTemperature * //1
		(0.78736169f * (10 ^ -4) + dewPointTemperature * //2
		(-0.61117958f * (10 ^ -6) + dewPointTemperature * //3
		(0.43884187f * (10 ^ -8) + dewPointTemperature * //4
		(-0.29883885f * (10 ^ -10) + dewPointTemperature * //5
		(0.21874425f * (10 ^ -12) + dewPointTemperature * //6
		(-0.17892321f * (10 ^ -14) + dewPointTemperature * //7
		(0.11112018f * (10 ^ -16) + dewPointTemperature * //8
		(-0.30994571f * (10 ^ -19)))))))))));
	}

	#endregion

	#region Temperature/Heat

	public static float HeatDeltaToTempDelta(
		float heatEnergyDelta,
		float substanceMass,
		float substanceHeatCapacity
	)
	{
		return (heatEnergyDelta / (substanceMass * 1000)) / substanceHeatCapacity;
	}



	#endregion

}
