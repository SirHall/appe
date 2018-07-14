using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Obsolete("Use DragGraphGenerator2")]
public class DragGraphGenerator
{
	//This code was based off the MC DRAG paper which can be found here:
	//http://oai.dtic.mil/oai/oai?verb=getRecord&metadataPrefix=html&identifier=ADA098110

	public static AnimationCurve GenerateDragCoefficientGraph(
		ProjectileData bulletData,
		float speedOfSound,
		float upperSpeedRange = 1543.5f,
		int iterations = 100,
		//bool optimize = true,
		float ratioOfSpecificHeats = 1.4f)
	{
		AnimationCurve graph = new AnimationCurve();


		for (int i = 0; i < iterations; i++)
		{ //Need to have a key at the very bottom and top of the curve
			graph.AddKey(
				(upperSpeedRange / iterations) * i,
				TotalDragCoefficient(
					bulletData,
					speedOfSound,
					(upperSpeedRange / iterations) * i,
					ratioOfSpecificHeats
				)
			);
		}

		//{TODO} Add graph optimization (Remove unecessary nodes)


		return graph;
	}

	#region Calculate drag coefficients at point

	public static float TotalDragCoefficient(ProjectileData projectileData, float speedOfSound, float projectileSpeed, float ratioOfSpecificHeats = 1.4f)
	{
		//print ("==>" + ProjectileHeadDrag (bulletData, speedOfSound, projectileSpeed, ratioOfSpecificHeats).ToString ());

		//Return sum of coefficients
		return
			Check(ProjectileHeadDrag(projectileData, speedOfSound, projectileSpeed, ratioOfSpecificHeats))
		+ Check(ProjectileBoatTailDrag(projectileData, speedOfSound, projectileSpeed, ratioOfSpecificHeats))
		+ Check(ProjectileRotatingBandDrag(projectileData, speedOfSound, projectileSpeed))
		+ Check(ProjectileSkinWettedSurfaceDrag(projectileData, speedOfSound, projectileSpeed))
		+ Check(ProjectileBluntBaseDrag(projectileData, speedOfSound, projectileSpeed, ratioOfSpecificHeats));
	}

	public static float ProjectileHeadDrag(ProjectileData projectileData, float speedOfSound, float projectileSpeed, float ratioOfSpecificHeats)
	{
		//Pages 14, 16

		float ogiveRadiusPerTangent =
			(projectileData.ogiveRadius / projectileData.bulletDiameter)
			/
			(projectileData.ogiveTangentRadius / projectileData.bulletDiameter); //We are going to use this repeatedly, we can expect this to be 0 for a cone, and 1 for a tangent ogive nose

		float machNumber = projectileSpeed / speedOfSound;

		float c1 = 0.7156f - 0.5313f * ogiveRadiusPerTangent + 0.595f * ogiveRadiusPerTangent * ogiveRadiusPerTangent;
		float c2 = 0.0796f + 0.0779f * ogiveRadiusPerTangent;
		float c3 = 1.587f + 0.049f * ogiveRadiusPerTangent;
		float c4 = 0.1122f + 0.1658f * ogiveRadiusPerTangent;

		//{TODO} Clean up/simplify this equation

		//tex:
		//$$t=\frac{1-\frac{\frac{meplatDiameter}{bulletDiameter}}{bulletDiameter}}{\frac{noseLength}{bulletDiameter}}$$

		float t = (1 - ((projectileData.meplatDiameter / projectileData.bulletDiameter) / projectileData.bulletDiameter)) / (projectileData.noseLength / projectileData.bulletDiameter);


		float Cps = 1f; //{TODO} FINISH OFF

		//tex:
		//$$if\frac{projectileSpeed}{speedOfSound}>\left(1+0.552t^{\frac{4}{5}}\right)^{-0.5}$$

		if (projectileSpeed / speedOfSound > Mathf.Pow((1 + 0.552f * Mathf.Pow(t, 4f / 5f)), -0.5f))
		{
			//Transonic
			//tex:
			//$$0.368t^{\frac{9}{5}}+\frac{1.6t\cdot machNumber^2-1}{\left(ratioOfSpecificHeats+1\right)machNumber^2}$$

			return
			#region Equation
				0.368f * Mathf.Pow(t, 9f / 5f) +
			(
				(1.6f * t * (Mathf.Pow(machNumber, 2) - 1))
				/
				((ratioOfSpecificHeats + 1) * Mathf.Pow(machNumber, 2))
			);

			#endregion
		}
		else
		{
			//Supersonic
			//tex:
			//$$\frac{c_1-c_2\cdot t^2}{machNumber^2-1}\left(t\sqrt{machNumber^2-1}\right)^{c_3+c_4t}+\frac{0.75\pi}{4}\left(\frac{meplatDiameter}{bulletDiameter}\right)^2Cps$$

			return
			#region Equation
				(c1 - c2 * Mathf.Pow(t, 2)) / (Mathf.Pow(machNumber, 2) - 1)
			* Mathf.Pow(t * Mathf.Sqrt(Mathf.Pow(machNumber, 2) - 1), c3 + c4 * t)
			+ (Mathf.PI / 4 * 0.75f * Mathf.Pow(projectileData.meplatDiameter / projectileData.bulletDiameter, 2) * Cps);
			#endregion
		}
	}

	public static float ProjectileBoatTailDrag(ProjectileData projectileData, float speedOfSound, float projectileSpeed, float ratioOfSpecificHeats)
	{
		float t = (1 - ((projectileData.meplatDiameter / projectileData.bulletDiameter) / projectileData.bulletDiameter)) / (projectileData.noseVirtualLength / projectileData.bulletDiameter);

		float machNumber = projectileSpeed / speedOfSound;

		//tex:
		//$$A_1=\left(1-\frac{3\frac{\frac{ogiveTangentRadius}{bulletDiameter}}{\frac{ogiveRadius}{bulletDiameter}}}{5machNumber}\right)\left(\frac{5t}{6machNumber}\left(\frac{\pi}{2}\right)^2-\frac{0.7435}{machNumber^2}\left(t\cdot machNumber\right)^2\right)$$
		#region Equation
		float A1 = (1 -
				   ((3 * ((projectileData.ogiveTangentRadius / projectileData.bulletDiameter) / (projectileData.ogiveRadius / projectileData.bulletDiameter))) / (5 * machNumber))
				   )
				   * (
					   ((5 * t) / (6 * machNumber))
					   * Mathf.Pow(Mathf.PI / 2, 2)
					   - (0.7435f / Mathf.Pow(machNumber, 2))
					   * Mathf.Pow(t * machNumber, 1.6f)
				   );
		#endregion

		float A = A1 * Mathf.Exp(Mathf.Sqrt(Mathf.Pow(2 / (ratioOfSpecificHeats * Mathf.Pow(machNumber, 2)), projectileData.cylinderLength)))
				  + ((2 * Mathf.Tan(projectileData.boatTailAngle / projectileData.bulletDiameter)) / Mathf.Sqrt(Mathf.Pow(machNumber, 2) - 1))
				  -
				  (((ratioOfSpecificHeats + 1) * Mathf.Pow(machNumber, 4) - 4 * (Mathf.Pow(machNumber, 2) - 1)) * Mathf.Pow(Mathf.Tan(projectileData.boatTailAngle), 2)) /
				  (2 * Mathf.Pow(Mathf.Pow(machNumber, 2) - 1, 2));

		float k = 0.85f / Mathf.Sqrt(Mathf.Pow(machNumber, 2) - 1);

		return ((4 * A * Mathf.Tan(projectileData.boatTailAngle)) / k)
		* (
			(1 - Mathf.Exp(-k * projectileData.boatTailLength / projectileData.bulletDiameter))
			+ 2 * Mathf.Tan(projectileData.boatTailAngle / projectileData.bulletDiameter)
			* (
				Mathf.Exp(-k * projectileData.boatTailLength / projectileData.bulletDiameter) *
				((projectileData.boatTailLength / projectileData.bulletDiameter) + (1 / k))
				- (1 / k)
			)
		);
	}

	public static float ProjectileRotatingBandDrag(ProjectileData projectileData, float speedOfSound, float projectileSpeed)
	{
		AnimationCurve animCurve = new AnimationCurve();
		//Get in inmportant points
		animCurve.AddKey(0.0f, 0.0f);
		animCurve.AddKey(0.4f, 0.0f);
		animCurve.AddKey(0.9f, 0.5f);
		animCurve.AddKey(1.2f, 3.1f);
		animCurve.AddKey(2.0f, 2.5f);
		animCurve.AddKey(4.0f, 2.5f);

		//Really could simplify below expression
		return animCurve.Evaluate(projectileSpeed / speedOfSound) * ((projectileData.driveBandDiameter / projectileData.bulletDiameter) / (projectileData.bulletDiameter / projectileData.bulletDiameter));
	}

	public static float ProjectileSkinWettedSurfaceDrag(ProjectileData projectileData, float speedOfSound, float projectileSpeed)
	{
		float machNumber = projectileSpeed / speedOfSound;

		//Reynolds number                                                      ///Hmm, these cancel out here.
		float RE = 23296.3f * machNumber * (projectileData.bulletLength / projectileData.bulletDiameter) * projectileData.bulletDiameter; ///Pg.20 variable 'dREF' has been replaced with bulletData.bulletDiameter here, verify that this is correct

		//Turbulent frictional coefficient (I'd assume that at such high velocities, the air would be mildly turbulent) [Add in Laminar Frictional Coefficient]
		float CF = (0.445f / Mathf.Pow(RE, 2.58f)) * Mathf.Pow(1 + 0.25f * Mathf.Pow(machNumber, 2), -0.32f);

		//SW (Wetted Surface area) would be the sum of multiple components

		//Wetted surface area of the nose
		float SWN = (Mathf.PI / 2) * (projectileData.noseLength / projectileData.bulletDiameter)
					* (1 + (1 / (8 * Mathf.Pow(projectileData.noseLength / projectileData.bulletDiameter, 2))))
					* (
						1 + ((1 / 3) + (1 / (50 * Mathf.Pow(projectileData.noseLength / projectileData.bulletDiameter, 2))))
						* ((projectileData.ogiveTangentRadius / projectileData.bulletDiameter) / (projectileData.ogiveRadius / projectileData.bulletDiameter))
					);

		//Wetted surface area of the cylinder
		float SWCYL = Mathf.PI * ((projectileData.bulletLength / projectileData.bulletDiameter) - (projectileData.noseLength / projectileData.bulletDiameter));

		return (4 / Mathf.PI) * CF * (SWN + SWCYL);
	}

	public static float ProjectileBluntBaseDrag(ProjectileData projectileData, float speedOfSound, float projectileSpeed, float ratioOfSpecificHeats)
	{
		float machNumber = projectileSpeed / speedOfSound;
		//MachNumber^2
		float machNumberSq = Mathf.Pow(machNumber, 2);

		//BasePressure to FreeStreamPressure Ratio
		float pressureRatio =
			(
				1 + 0.09f * machNumberSq
				* (1 - Mathf.Exp(-projectileData.cylinderLength / projectileData.bulletDiameter))
			)
			* (1 + (machNumberSq / 2) * (1 - (projectileData.baseDiameter / projectileData.bulletDiameter)));

		//Find base drag coefficient
		return (2 * Mathf.Pow(projectileData.baseDiameter / projectileData.bulletDiameter, 2)) / (ratioOfSpecificHeats * machNumberSq)
		* (1 - pressureRatio);
	}

	#endregion

	public static float Check(float numb)
	{
		return (numb == Mathf.Infinity || float.IsNaN(numb)) ?
			0
			:
			Mathf.Abs(numb);
	}
}

