using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Excessives.Unity;

//[CreateAssetMenu(fileName = "ProjectileSetup", menuName = "AProjectiles/ProjectileData")]
[System.Serializable]
public class ProjectileData : ICloneable, IProjData //: ScriptableObject
{
	public float initVelocity;

	//[Tooltip("Use simple trig and online shot groups to figure this one out")]
	//public float spreadAngle;

	public bool useAdvancedDragModel = false;

	#region IProjData

	public ProjectileData ProjInfo { get { return this; } }

	#endregion

	#region Simple Drag

	public float dragCoefficient = 0.04f;

	#endregion

	#region Advanced Drag
	public float
		bulletDiameter,
		bulletLength,
		baseDiameter,
		cylinderLength,

		noseLength,
		noseVirtualLength, //Length of the nose if extended to a point
		meplatDiameter,
		ogiveRadius,
		ogiveTangentRadius,

		boatTailDiameter,
		boatTailAngle,
		boatTailLength,

		centerOfMass,

		driveBandDiameter,
		driveBandLength,
		driveBandBevel;

	public BoundaryLayerCode boundaryLayerCode = BoundaryLayerCode.LL;

	public int graphIterations = 100;

	public bool hasDriveBand = true;

	public AnimationCurve dragGraph;

	#endregion

	//{TODO} Correct unit of measurement
	float CrossSectionalArea;

	public float crossSectionalArea
	{
		get
		{
			return
				(this.CrossSectionalArea == 0.0f)
				? //If this is null, calculate it then return the value
				new Func<float>(
				() =>
				{
					//Find cross sectional area from the largest diameter
					this.CrossSectionalArea = Mathf.Pow(Mathf.Max(
						this.baseDiameter,
						this.boatTailDiameter,
						this.bulletDiameter,
						this.driveBandDiameter) / 2, 2) * Mathf.PI;

					return this.CrossSectionalArea;
				}
			)()
				: //If not null, just return whatever we have
			this.CrossSectionalArea;
		}
		set { this.CrossSectionalArea = value; }
	}

	float? SG;

	[Space(10)]
	//[Header ("Enter in either the bullet's mass, or tick ONE of the following")]
	public float bulletMass;

	#region Drag Graph Generator

	public bool isGraphGenerated { get; set; } = false;

	public void GenerateDragCoefficientGraph()
	{
		dragGraph = DragGraphGenerator2.GenerateDragCurve(
			this,
			100,
			4.0f
		);

		isGraphGenerated = true;
	}

	public void InitGraphSettings()
	{
		if (dragGraph == null)
			throw (new Exception("Cannot initialize drag graph if it is null"));
		dragGraph.postWrapMode = WrapMode.ClampForever;
		dragGraph.preWrapMode = WrapMode.ClampForever;
	}

	#endregion


	/// <summary>
	/// Gets the drag coefficient at any velocity magnitude.
	/// </summary>
	/// <returns>The drag coefficient at the given mach value.</returns>
	/// <param name="velocity">Magnitude of velocity.</param>
	public float GetDragCoefficient(float velocity)
	{
		float dragCoefficient = dragGraph.Evaluate(velocity);
		return (dragCoefficient >= 0)
			?
			dragCoefficient
			:
			0;
	}

	//{TODO} Need to setup material definitions
	public float SpeedOfSoundInObject(float density, float bulModulusOfElasticity)
		=> Mathf.Sqrt(bulModulusOfElasticity / density);

	public object Clone()
	{
		return (ProjectileData)MemberwiseClone();
	}


}
