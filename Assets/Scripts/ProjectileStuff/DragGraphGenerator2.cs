using MCDRAG_Calc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGraphGenerator2 : MonoBehaviour
{
	public static AnimationCurve GenerateDragCurve(
		ProjectileData projData,
		int iterations = 100,
		float upperSpeedRange = 4f //In mach
		)
	{
		AnimationCurve graph = new AnimationCurve();
		MCDRAG dragCalcor = new MCDRAG(); //{TODO} Should I make this a static field instead of recreating it?

		dragCalcor.SetIterations(iterations, upperSpeedRange, 0.0, true);

		dragCalcor.InputValues(
			projData.bulletDiameter,
			projData.bulletLength / projData.bulletDiameter,
			projData.noseLength / projData.bulletDiameter,
			projData.ogiveTangentRadius / projData.ogiveRadius,
			projData.boatTailLength / projData.bulletDiameter,
			projData.baseDiameter / projData.bulletDiameter,
			projData.meplatDiameter / projData.bulletDiameter,
			projData.hasDriveBand ? projData.driveBandDiameter / projData.bulletDiameter : 1,
			projData.centerOfMass / projData.bulletDiameter,
			projData.boundaryLayerCode,
			""
			);

		dragCalcor.GenerateCoefficients();

		for (int i = 0; i < dragCalcor.M.Length; i++)
			graph.AddKey((float)dragCalcor.M[i], (float)dragCalcor.C1[i]);

		return graph;
	}
}
