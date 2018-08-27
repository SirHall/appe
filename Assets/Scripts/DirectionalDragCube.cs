using UnityEngine;

[System.Serializable]
public class DirectionalDragCube
{
	public DirectionalDragCube()
	{

	}

	/// <summary>
	/// Finds the drag acceleration experienced by 'the cube'
	/// </summary>
	/// <param name="localVel"></param>
	/// <param name="crossSectionalArea"></param>
	/// <param name="mass"></param>
	/// <param name="fluidDensity"></param>
	/// <returns></returns>
	public virtual Vector3 EvaluateLocalDrag(Vector3 localVel, float mass, float fluidDensity)
	{
		return new Vector3(
				EvaluateXDrag(localVel.x, crossSectionalArea.x, mass, fluidDensity),
				EvaluateYDrag(localVel.y, crossSectionalArea.y, mass, fluidDensity),
				EvaluateZDrag(localVel.z, crossSectionalArea.z, mass, fluidDensity)
				);
	}

	/// <summary>
	/// The cross sectional area on each axis
	/// </summary>
	protected Vector3 crossSectionalArea;

	protected virtual float EvaluateXDrag(float localVel, float crossSectionalArea, float mass, float fluidDensity) { return 0.0f; }
	protected virtual float EvaluateYDrag(float localVel, float crossSectionalArea, float mass, float fluidDensity) { return 0.0f; }
	protected virtual float EvaluateZDrag(float localVel, float crossSectionalArea, float mass, float fluidDensity) { return 0.0f; }

	/// <summary>
	/// Wrapper for Vector3.Project()
	/// </summary>
	/// <param name="localDrag"></param>
	/// <param name="axis"></param>
	/// <returns></returns>
	public virtual Vector3 GetLocalDragAlongAxis(Vector3 localDrag, Vector3 axis)
	{
		return Vector3.Project(localDrag, axis);
	}
}