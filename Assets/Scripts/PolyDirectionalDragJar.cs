
using UnityEngine;

//{TODO} Is this class needed

class PolyDirectionalDragJar
{
	public PolyDirectionalDragJar()
	{

	}

	DirectionalDrag[] directions = new DirectionalDrag[0];

	public Vector3 EvaluateLocalDrag(Vector3 localVel, float[] crossSectionalArea, float mass, float fluidDensity)
	{
		Vector3 localDragAccel = Vector3.zero;



		return localDragAccel;
	}


	float FindDragAlongAxis(Vector3 localVel, Vector3 axis, float[] crossSectionalArea, float mass, float fluidDensity)
	{
		float dragOnAxis = 0.0f;

		for (int i = 0; i < directions.Length; i++)
		{
			//{TODO} Redo this
			//dragOnAxis = Vector3.Project(directions[i].EvaluateDragCoefficient(localVel), axis).magnitude;
		}

		return dragOnAxis;
	}

}

public class DirectionalDrag
{
	public DirectionalDrag() { }

	public virtual Vector3 direction { get; private set; }
	public virtual float EvaluateDragCoefficient(Vector3 localVelocity)
	{
		return 0.1f;
	}
}