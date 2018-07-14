using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class RenderBullet : MonoBehaviour
{

	public ProjectileData bullet;

	//public float width = 3;

	void OnSceneGUI ()
	{
		float width = HandleUtility.GetHandleSize (Vector3.zero) * 0.1f;
		Handles.DrawBezier (transform.position,
			Vector3.zero,
			Vector3.up,
			-Vector3.up,
			Color.red,
			null,
			width);
	}



	//	void DrawCircle (Vector3 centerPosition, Quaternion rotation, float radius, float time, int segments, Color color, bool depthTest)
	//	{
	//		float angle = 0;
	//
	//		for (int i = 0; i < segments + 1; i++) {
	//
	//			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
	//			y = Mathf.Cos (Mathf.Deg2Rad * angle) * _vertRadius;
	//
	//			switch (_axis) {
	//			case Axis.X:
	//				_line.SetPosition (i, new Vector3 (z, y, x));
	//				break;
	//			case Axis.Y:
	//				_line.SetPosition (i, new Vector3 (y, z, x));
	//				break;
	//			case Axis.Z:
	//				_line.SetPosition (i, new Vector3 (x, y, z));
	//				break;
	//			default:
	//				break;
	//			}
	//
	//			angle += (360f / _segments);
	//		}
	//	}


}
