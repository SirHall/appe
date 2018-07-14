using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(ProjectileData))]
public class ProjectileDataEditor : PropertyDrawer
{

	Rect initDrawRect,
		currentRect = new Rect(0, 946, 0, 0); //946 - Total height of property when fully expanded

	#region Serialized Properties
	SerializedProperty
		bulletDiameter,
		bulletLength,
		baseDiameter,
		cylinderLength,

		noseLength,
		noseVirtualLength, //Length of the nose if extended to a sharp point
		meplatDiameter,
		ogiveRadius,
		ogiveTangentRadius,

		boatTailDiameter,
		boatTailAngle,
		boatTailLength,

		driveBandDiameter,
		driveBandLength,
		driveBandBevel,
		hasDriveBand,

		centerOfMass,
		boundaryLayerCode,

		bulletMass,
		graphIterations,
		dragGraph;


	#endregion

	bool isOpen = true;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		initDrawRect = position;
		currentRect = new Rect(position.x, position.y, position.width, 16); //{TODO} Update to the height of the drawing sqare

		currentRect.GetType();

		isOpen = EditorGUI.Foldout(currentRect, isOpen, "Affected Projectile");
		currentRect.y += 16;

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		if (isOpen)
		{

			GetAllSerializedProperties(property);

			//currentRect.y += 123;

			#region Fill in bullet's missing information

			/* Do not directly write to:
			 * cylinderLength
			 * ogiveTangentRadius
			 * noseVirtualLength
			 * boatTailAngle
			 * ogiveRadius (Should be calculated from noseLength)
			 */

			cylinderLength.floatValue = bulletLength.floatValue - (noseLength.floatValue + boatTailLength.floatValue);
			//Came right outa https://en.wikipedia.org/wiki/Nose_cone_design, and a little algebra ;)
			ogiveTangentRadius.floatValue = noseLength.floatValue * noseLength.floatValue + 0.25f; //{TODO} I *think* this is what the ogive tangent radius is


			//{TODO} Fix-up this equation
			//tex:
			//$$noseVirtualLength = noseLength + \sqrt{ogiveTangentRadius^2+ogiveTangentRadius*meplatDiameter+2ogiveRadius^2
			//-(\frac{meplatDiameter}{2})^2-meplatDiameter*ogiveRadius}$$

			//noseVirtualLength.floatValue = noseLength.floatValue + Mathf.Sqrt(
			//	Mathf.Pow(ogiveTangentRadius.floatValue, 2)
			//	+ ogiveTangentRadius.floatValue * meplatDiameter.floatValue
			//	+ 2 * ogiveRadius.floatValue * ogiveRadius.floatValue
			//	- Mathf.Pow(meplatDiameter.floatValue / 2, 2)
			//	- meplatDiameter.floatValue * ogiveRadius.floatValue
			//);

			noseVirtualLength.floatValue = noseLength.floatValue + (meplatDiameter.floatValue / 2.0f * Mathf.PI); //{TODO} Fix this

			ogiveRadius.floatValue =
				(
					((bulletDiameter.floatValue / 2.0f) * (bulletDiameter.floatValue / 2.0f))
					+ (noseVirtualLength.floatValue * noseVirtualLength.floatValue)
				) / bulletDiameter.floatValue;

			//tex:
			//$$boatAngle = \tan{(\frac{baseDiameter-boatTailDiameter}{2boatTailLength})}$$

			boatTailAngle.floatValue = Mathf.Tan(
				(baseDiameter.floatValue - boatTailDiameter.floatValue)
				/
				2 * boatTailLength.floatValue);

			//{TODO} Write up the equations to calculate these
			//ogiveRadius.floatValue = EditorGUI.FloatField(currentRect, "OgiveRadius", ogiveRadius.floatValue);
			//EditorGUI.LabelField(currentRect, "OgiveTangentRadius", ogiveTangentRadius.floatValue.ToString());
			//EditorGUI.LabelField(currentRect, "NoseVirtualLength", noseVirtualLength.floatValue.ToString());


			#endregion

			#region Clamp Values

			//ogiveRadius.floatValue = Mathf.Clamp(ogiveRadius.floatValue, 0.1f, float.PositiveInfinity);
			driveBandLength.floatValue = Mathf.Clamp(driveBandLength.floatValue, 0.1f, cylinderLength.floatValue * 0.5f);
			meplatDiameter.floatValue = Mathf.Clamp(meplatDiameter.floatValue, 0, float.PositiveInfinity);

			bulletDiameter.floatValue = Mathf.Clamp(bulletDiameter.floatValue, 0.1f, float.PositiveInfinity);
			bulletLength.floatValue = Mathf.Clamp(bulletLength.floatValue, 0.1f, float.PositiveInfinity);

			noseLength.floatValue = Mathf.Clamp(noseLength.floatValue, 0.1f, bulletLength.floatValue - boatTailLength.floatValue);
			boatTailLength.floatValue = Mathf.Clamp(boatTailLength.floatValue, 0.1f, bulletLength.floatValue - noseLength.floatValue);

			#endregion

			#region Draw Bullet Graphic
			//Render what the bullet would look like

			Texture2D bulletTex = new Texture2D(280, 210);

			float offsetScale = Mathf.Min(
									(bulletTex.width * 0.8f) / bulletLength.floatValue,
									(bulletTex.height * 0.8f) / Mathf.Max(boatTailDiameter.floatValue,
										driveBandDiameter.floatValue,
										bulletDiameter.floatValue,
										baseDiameter.floatValue)
								);

			Vector2 midPoint = new Vector2(
				(bulletTex.width / 2) - (noseLength.floatValue * offsetScale / 2) + (boatTailLength.floatValue * offsetScale / 2),
				(bulletTex.height / 2)
				);



			Color drawColor = Color.white;

			#region Prepare Texture

			for (int i = 0; i < bulletTex.width; i++)
				for (int j = 0; j < bulletTex.height; j++)
					bulletTex.SetPixel(
						i,
						j,
						Color.gray
						);
			#endregion

			#region Draw Cylinder

			DrawSquare(ref bulletTex,
				(int)(midPoint.x - cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y + baseDiameter.floatValue * 0.5f * offsetScale), //Up Left
				(int)(midPoint.x + cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y + bulletDiameter.floatValue * 0.5f * offsetScale), //Up Right
				(int)(midPoint.x + cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y - bulletDiameter.floatValue * offsetScale * 0.5f), //Down Right
				(int)(midPoint.x - cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y - baseDiameter.floatValue * offsetScale * 0.5f), //Down Left
				drawColor
			);
			#endregion

			#region Draw Boat Tail
			DrawSquare(ref bulletTex,
				(int)(midPoint.x - ((cylinderLength.floatValue * 0.5f) + boatTailLength.floatValue) * offsetScale),
				(int)(midPoint.y + boatTailDiameter.floatValue * 0.5f * offsetScale), //Up Left
				(int)(midPoint.x - cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y + baseDiameter.floatValue * 0.5f * offsetScale), //Up Right
				(int)(midPoint.x - cylinderLength.floatValue * 0.5f * offsetScale),
				(int)(midPoint.y - baseDiameter.floatValue * offsetScale * 0.5f), //Down Right
				(int)(midPoint.x - ((cylinderLength.floatValue * 0.5f) + boatTailLength.floatValue) * offsetScale),
				(int)(midPoint.y - boatTailDiameter.floatValue * offsetScale * 0.5f), //Down Left
				drawColor
			);
			#endregion

			#region Draw Drive Band
			if (hasDriveBand.boolValue)
			{
				DrawSquare(ref bulletTex, //Main square
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale), //Up Left
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale), //Up Right
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f), //Down Right
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f), //Down Left
					drawColor
				);

				DrawSquare(ref bulletTex, //Left Extension
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale - driveBandBevel.floatValue),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale - driveBandBevel.floatValue), //Up Left REWRITTEN
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale), //Up Right
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f), //Down Right
					(int)(midPoint.x - driveBandLength.floatValue * 0.5f * offsetScale - driveBandBevel.floatValue),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f + driveBandBevel.floatValue), //Down Left REWRITTEN
					drawColor
				);

				DrawSquare(ref bulletTex, //Right Extension
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale), //Up Left
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale + driveBandBevel.floatValue),
					(int)(midPoint.y + driveBandDiameter.floatValue * 0.5f * offsetScale - driveBandBevel.floatValue), //Up Right
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale + driveBandBevel.floatValue),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f + driveBandBevel.floatValue), //Down Right
					(int)(midPoint.x + driveBandLength.floatValue * 0.5f * offsetScale),
					(int)(midPoint.y - driveBandDiameter.floatValue * offsetScale * 0.5f), //Down Left
					drawColor
				);
			}
			#endregion

			#region Draw Nose

			/*
			 * We can assume that the tip and the base of the nose will form an isoceles triangle, where the sides of that triangle
			 * connecting to the center point are the Ogive Radius.
			 */

			#region OLD
			//Firstly, divide the distance between the nose tip and base edge by two to form a right angled triangle.

			//		float halfDistance = Vector2.Distance (
			//			                     new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
			//				                     midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
			//			                     new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
			//				                     midPoint.y)
			//		                     ) / 2;
			//
			//		//Find rotation between both positions
			//		float rotation = Vector2.SignedAngle (
			//			                 new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
			//				                 midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
			//			                 new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
			//				                 midPoint.y)
			//		                 );
			//
			//		DrawLine (ref bulletTex,
			//			(int)(midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale),
			//			(int)(midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
			//			(int)(midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale),
			//			(int)(midPoint.y),
			//			Color.black
			//		);
			//
			//		Vector2 ogiveMidPoint = (
			//		                            new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
			//			                            midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale) +
			//		                            new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
			//			                            midPoint.y)
			//		                        ) / 2;
			#endregion

			int noseDrawIterations = 200;

			//Make sure we only render the bullet where the y>=0
			//float xInt = 0;

			#region OLD2

			//for (int i = 0; i < noseDrawIterations; i++)
			//{
			//	//tex:
			//	// $$if \sqrt{ogiveRadius^2-(noseVirtualLength*i/noseDrawIterations)^2}
			//	// +ogiveTangentRadius - ogiveRadius >= 0$$

			//	if (
			//		(+Mathf.Sqrt(
			//			(ogiveRadius.floatValue * ogiveRadius.floatValue)
			//			- ((noseVirtualLength.floatValue * i / noseDrawIterations) * (noseVirtualLength.floatValue * i / noseDrawIterations))
			//		) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue)
			//		>= 0)
			//	{
			//		#region Upper Ogive
			//		DrawLine(ref bulletTex,

			//			//x-Start
			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * i / noseDrawIterations)) * offsetScale
			//		, 0, Mathf.Infinity)),

			//			//y-Start
			//			(int)(midPoint.y + (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * i / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),

			//			//x-Stop
			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * (i + 1) / noseDrawIterations)) * offsetScale
			//		, 0, Mathf.Infinity)),

			//			//y-Stop
			//			(int)(midPoint.y + (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * (i + 1) / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),

			//			drawColor
			//		);
			//		#endregion

			//		#region Lower Ogive
			//		DrawLine(ref bulletTex,

			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * i / noseDrawIterations)) * offsetScale
			//			, 0, Mathf.Infinity)),

			//			(int)(midPoint.y - (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * i / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),

			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * (i + 1) / noseDrawIterations)) * offsetScale
			//			, 0, Mathf.Infinity)),

			//			(int)(midPoint.y - (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * (i + 1) / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),
			//			drawColor

			//		);
			//		#endregion


			//	}
			//	else
			//	{
			//		//Draw a line between the two nose points(for when the ogive radius is really small),
			//		//and break out of the loop so we aren't uselessly trying
			//		//to draw more lines only to deny making them

			//		DrawLine(ref bulletTex,

			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * (i - 1) / noseDrawIterations)) * offsetScale
			//			, 0, Mathf.Infinity)),

			//			(int)(midPoint.y + (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * (i - 1) / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),

			//			(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * (i - 1) / noseDrawIterations)) * offsetScale
			//			, 0, Mathf.Infinity)),

			//			(int)(midPoint.y - (
			//				+Mathf.Sqrt(
			//					Mathf.Pow(ogiveRadius.floatValue, 2)
			//					- Mathf.Pow(noseVirtualLength.floatValue * (i - 1) / noseDrawIterations, 2)
			//				) + ogiveTangentRadius.floatValue - ogiveRadius.floatValue) * offsetScale),
			//			drawColor
			//		);
			//		break;
			//	}
			//}

			#endregion

			//(int)(midPoint.x + Mathf.Clamp(
			//				((cylinderLength.floatValue / 2) + (noseVirtualLength.floatValue * i / noseDrawIterations)) * offsetScale
			//		, 0, Mathf.Infinity))

			//Vector2Int
			//	upperOgivePrevPos = new Vector2Int(),
			//	lowerOgivePrevPos;


			int start =
				(int)(midPoint.x + Mathf.Clamp(
							(cylinderLength.floatValue / 2) * offsetScale
					, 0, Mathf.Infinity));

			int end = (int)(start + noseLength.floatValue * offsetScale);

			int currentPos = 0;


			for (float i = 0; i <= noseLength.floatValue; i += noseLength.floatValue / noseDrawIterations)
			{
				#region UpperOgive

				currentPos = (int)(
						midPoint.y +
							(
								Mathf.Sqrt((ogiveRadius.floatValue * ogiveRadius.floatValue) - (i * i))
								+ (bulletDiameter.floatValue / 2.0f) - ogiveRadius.floatValue
							)
						* offsetScale
						);

				bulletTex.SetPixel(
					start + (int)(i * offsetScale),
					currentPos,
					Color.white
					);

				#endregion

				#region LowerOgive

				currentPos = (int)(
						midPoint.y -
							(
								Mathf.Sqrt((ogiveRadius.floatValue * ogiveRadius.floatValue) - (i * i))
								+ (bulletDiameter.floatValue / 2.0f) - ogiveRadius.floatValue
							)
						* offsetScale
						);

				bulletTex.SetPixel(
					start + (int)(i * offsetScale),
					currentPos,
					Color.white
					);

				#endregion

				#region Meplat


				DrawLine(
					ref bulletTex,
					end,
					(int)(
					midPoint.y +
						(
							Mathf.Sqrt((ogiveRadius.floatValue * ogiveRadius.floatValue) - (noseLength.floatValue * noseLength.floatValue))
							+ (bulletDiameter.floatValue / 2.0f) - ogiveRadius.floatValue
						)
					* offsetScale
					),
					end,
					(int)(
					midPoint.y -
						(
							Mathf.Sqrt((ogiveRadius.floatValue * ogiveRadius.floatValue) - (noseLength.floatValue * noseLength.floatValue))
							+ (bulletDiameter.floatValue / 2.0f) - ogiveRadius.floatValue
						)
					* offsetScale
					),
					Color.white
					);



				#endregion

			}

			#endregion


			bulletTex.Apply();

			//Render to inspector GUI
			currentRect.width = bulletTex.width;
			currentRect.height = bulletTex.height;
			//EditorGUI.DrawTextureAlpha(currentRect, (Texture)bulletTex, ScaleMode.ScaleToFit);
			EditorGUI.LabelField(currentRect, new GUIContent(bulletTex));
			currentRect.y += bulletTex.height + 16;

			currentRect.height = 16;
			currentRect.width = position.width;

			#endregion

			#region Draw Drag Model Fields

			bulletDiameter.floatValue = EditorGUI.FloatField(currentRect, "BulletDiameter", bulletDiameter.floatValue);
			currentRect.y += 16;
			bulletLength.floatValue = EditorGUI.FloatField(currentRect, "BulletLength", bulletLength.floatValue);
			currentRect.y += 16;
			baseDiameter.floatValue = EditorGUI.FloatField(currentRect, "BaseDiameter", baseDiameter.floatValue);
			currentRect.y += 16;
			EditorGUI.LabelField(currentRect, "CylinderLength", cylinderLength.floatValue.ToString());
			currentRect.y += 32;

			noseLength.floatValue = EditorGUI.FloatField(currentRect, "NoseLength", noseLength.floatValue);
			currentRect.y += 16;
			meplatDiameter.floatValue = EditorGUI.FloatField(currentRect, "MeplatDiameter", meplatDiameter.floatValue);
			currentRect.y += 32;

			boatTailDiameter.floatValue = EditorGUI.FloatField(currentRect, "BoatTailDiameter", boatTailDiameter.floatValue);
			currentRect.y += 16;
			boatTailLength.floatValue = EditorGUI.FloatField(currentRect, "BoatTailLength", boatTailLength.floatValue);
			currentRect.y += 16;
			EditorGUI.LabelField(currentRect, "BoatTailAngle", boatTailAngle.floatValue.ToString());
			currentRect.y += 32;

			hasDriveBand.boolValue = EditorGUI.ToggleLeft(currentRect, "HasDriveBand", hasDriveBand.boolValue);
			currentRect.y += 16;
			driveBandDiameter.floatValue = EditorGUI.FloatField(currentRect, "DriveBandDiameter", driveBandDiameter.floatValue);
			currentRect.y += 16;
			driveBandLength.floatValue = EditorGUI.FloatField(currentRect, "DriveBandLength", driveBandLength.floatValue);
			currentRect.y += 16;
			driveBandBevel.floatValue = EditorGUI.FloatField(currentRect, "DriveBandBevel", driveBandBevel.floatValue);
			currentRect.y += 32;


			bulletMass.floatValue = EditorGUI.FloatField(currentRect, "BulletMass", bulletMass.floatValue);
			currentRect.y += 16;
			centerOfMass.floatValue = EditorGUI.FloatField(currentRect, "CenterOfMass", centerOfMass.floatValue);
			currentRect.y += 16;
			//Ugly...
			boundaryLayerCode.enumValueIndex =
				(int)(BoundaryLayerCode)EditorGUI.EnumPopup(
					currentRect,
					"BoundaryLayer",
					(BoundaryLayerCode)Enum.GetValues(typeof(BoundaryLayerCode))
						.GetValue(boundaryLayerCode.enumValueIndex)
					);

			currentRect.y += 32;

			#endregion

			#region Drag Graph
			if (GUI.Button(currentRect, "Generate Drag Curve"))
			{
				dragGraph.animationCurveValue = GenerateDragGraph(property);
			}

			currentRect.y += 16;

			graphIterations.intValue = EditorGUI.IntSlider(currentRect, graphIterations.intValue, 5, 1000);

			currentRect.y += 16;
			dragGraph.animationCurveValue = EditorGUI.CurveField(currentRect, "GraphIterations", dragGraph.animationCurveValue);
			currentRect.y += 16;

			#endregion
		}

		//Finish drawing
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return
			isOpen ?
				currentRect.y - initDrawRect.y + 32 //Self correcting size
				:
				16;
	}

	void GetAllSerializedProperties(SerializedProperty property)
	{
		bulletDiameter = property.FindPropertyRelative("bulletDiameter");
		bulletLength = property.FindPropertyRelative("bulletLength");
		baseDiameter = property.FindPropertyRelative("baseDiameter");
		cylinderLength = property.FindPropertyRelative("cylinderLength");

		noseLength = property.FindPropertyRelative("noseLength");
		noseVirtualLength = property.FindPropertyRelative("noseVirtualLength");
		meplatDiameter = property.FindPropertyRelative("meplatDiameter");
		ogiveRadius = property.FindPropertyRelative("ogiveRadius");
		ogiveTangentRadius = property.FindPropertyRelative("ogiveTangentRadius");

		boatTailDiameter = property.FindPropertyRelative("boatTailDiameter");
		boatTailAngle = property.FindPropertyRelative("boatTailAngle");
		boatTailLength = property.FindPropertyRelative("boatTailLength");

		driveBandDiameter = property.FindPropertyRelative("driveBandDiameter");
		driveBandLength = property.FindPropertyRelative("driveBandLength");
		driveBandBevel = property.FindPropertyRelative("driveBandBevel");
		hasDriveBand = property.FindPropertyRelative("hasDriveBand");

		centerOfMass = property.FindPropertyRelative("centerOfMass");
		boundaryLayerCode = property.FindPropertyRelative("boundaryLayerCode");

		bulletMass = property.FindPropertyRelative("bulletMass");
		graphIterations = property.FindPropertyRelative("graphIterations");
		dragGraph = property.FindPropertyRelative("dragGraph");
	}

	#region Line Renderers

	void DrawSquare(ref Texture2D tex, int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
	{
		DrawLine(ref tex,
			x0,
			y0,
			x1,
			y1,
			color
		);
		DrawLine(ref tex,
			x1,
			y1,
			x2,
			y2,
			color
		);
		DrawLine(ref tex,
			x2,
			y2,
			x3,
			y3,
			color
		);
		DrawLine(ref tex,
			x3,
			y3,
			x0,
			y0,
			color
		);
	}

	void DrawLine(ref Texture2D tex, int x0, int y0, int x1, int y1, Color col)
	{
		int dy = (int)(y1 - y0);
		int dx = (int)(x1 - x0);
		int stepx, stepy;

		if (dy < 0)
		{
			dy = -dy;
			stepy = -1;
		}
		else
		{
			stepy = 1;
		}
		if (dx < 0)
		{
			dx = -dx;
			stepx = -1;
		}
		else
		{
			stepx = 1;
		}
		dy <<= 1;
		dx <<= 1;

		float fraction = 0;

		tex.SetPixel(x0, y0, col);
		if (dx > dy)
		{
			fraction = dy - (dx >> 1);
			while (Mathf.Abs(x0 - x1) > 1)
			{
				if (fraction >= 0)
				{
					y0 += stepy;
					fraction -= dx;
				}
				x0 += stepx;
				fraction += dy;
				tex.SetPixel(x0, y0, col);
			}
		}
		else
		{
			fraction = dx - (dy >> 1);
			while (Mathf.Abs(y0 - y1) > 1)
			{
				if (fraction >= 0)
				{
					x0 += stepx;
					fraction -= dy;
				}
				y0 += stepy;
				fraction += dx;
				tex.SetPixel(x0, y0, col);
			}
		}
	}

	void DrawCircleSection(ref Texture2D tex, int x, int y, float radius, float lowerAngle, float upperAngle, int iterations, Color color)
	{
		float angleSection = (upperAngle - lowerAngle) / iterations;

		for (int i = 0; i <= iterations; i++)
		{

			DrawLine(ref tex, x + (int)(radius * Mathf.Cos(i * angleSection)), y + (int)(radius * Mathf.Sin(i * angleSection)), x + (int)(radius * Mathf.Cos((i + 1) * angleSection)), y + (int)(radius * Mathf.Sin((i + 1) * angleSection)), color);

		}
	}

	#endregion

	/// <summary>
	/// PolarForm -> Cartesian Form
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="rotation">Rotation in radians.</param>
	/// <param name="distance">Distance.</param>
	Vector2 DirectionalPosition(float rotation, float distance)
	{
		return new Vector2(distance * Mathf.Cos(rotation), distance * Mathf.Sin(rotation));
	}

	AnimationCurve GenerateDragGraph(SerializedProperty propery)
	{
		GetAllSerializedProperties(propery);

		ProjectileData proj = new ProjectileData();

		//Set all values
		proj.bulletDiameter = this.bulletDiameter.floatValue;
		proj.bulletLength = this.bulletLength.floatValue;
		proj.baseDiameter = this.baseDiameter.floatValue;
		proj.cylinderLength = this.cylinderLength.floatValue;
		proj.noseLength = this.noseLength.floatValue;
		proj.noseVirtualLength = this.noseVirtualLength.floatValue;
		proj.meplatDiameter = this.meplatDiameter.floatValue;
		proj.ogiveRadius = this.ogiveRadius.floatValue;
		proj.ogiveTangentRadius = this.ogiveTangentRadius.floatValue;
		proj.boatTailDiameter = this.boatTailDiameter.floatValue;
		proj.boatTailAngle = this.boatTailAngle.floatValue;
		proj.boatTailLength = this.boatTailLength.floatValue;
		proj.driveBandDiameter = this.driveBandDiameter.floatValue;
		proj.driveBandLength = this.driveBandLength.floatValue;
		proj.driveBandBevel = this.driveBandBevel.floatValue;
		proj.hasDriveBand = this.hasDriveBand.boolValue;
		proj.graphIterations = this.graphIterations.intValue;
		proj.bulletMass = this.bulletMass.floatValue;
		proj.centerOfMass = this.centerOfMass.floatValue;
		proj.boundaryLayerCode = (BoundaryLayerCode)this.boundaryLayerCode.enumValueIndex;

		proj.dragGraph =
			DragGraphGenerator2.GenerateDragCurve(
				proj,
				proj.graphIterations,
				4.0f //In mach
				);

		proj.InitGraphSettings();

		return proj.dragGraph;
	}

}

//[Obsolete("Old")]
//public class ProjectileDataEditorOLD : Editor
//{
//	Material mat;

//	void OnEnable()
//	{
//		Shader shader = Shader.Find("Hidden/Internal-Colored");
//		mat = new Material(shader);
//	}

//	void OnDisable()
//	{
//		DestroyImmediate(mat);
//	}

//	public override void OnInspectorGUI()
//	{
//		//		//New render system
//		//		Rect bulletRenderRect = GUILayoutUtility.GetRect (280, 210);
//		//		if (Event.current.type == EventType.Repaint) {
//		//			GUI.BeginClip (bulletRenderRect);
//		//			GL.PushMatrix ();
//		//			//GL.Clear (true, true, Color.black);
//		//			mat.SetPass (0);
//		//
//		//			GL.Begin (GL.LINES);
//		//			GL.Color (Color.blue);
//		//			GL.Vertex3 (0, 0, 0);
//		//			GL.Vertex3 (100, 100, 0);
//		//			GL.End ();
//		//			GUI.EndClip ();
//		//		}

//		ProjectileDataObject projectileDataObject = (ProjectileDataObject)target;

//		ProjectileData projectileData = projectileDataObject.projD;

//		projectileData.useAdvancedDragModel = EditorGUILayout.Toggle(
//			"Simple Drag Model",
//			projectileData.useAdvancedDragModel
//		);

//		if (!projectileData.useAdvancedDragModel)
//		{
//			#region Advanced Drag Model

//			#region Fill in bullet's missing information
//			projectileData.cylinderLength = projectileData.bulletLength - (projectileData.noseLength + projectileData.boatTailLength);
//			projectileData.driveBandLength = Mathf.Clamp(projectileData.driveBandLength, 0.1f, projectileData.cylinderLength * 0.5f);

//			//Came right outa https://en.wikipedia.org/wiki/Nose_cone_design, and a little algebra ;)
//			projectileData.ogiveTangentRadius = projectileData.bulletDiameter / 2; //I *think* this is what the ogive tangent radius is

//			projectileData.noseVirtualLength = projectileData.noseLength + Mathf.Sqrt(
//				Mathf.Pow(-projectileData.ogiveTangentRadius, 2)
//				+ projectileData.ogiveTangentRadius * projectileData.meplatDiameter
//				+ 2 * projectileData.ogiveRadius * projectileData.ogiveRadius
//				- Mathf.Pow(projectileData.meplatDiameter / 2, 2)
//				- projectileData.meplatDiameter * projectileData.ogiveRadius
//			);

//			projectileData.boatTailAngle = Mathf.Tan(
//				((projectileData.baseDiameter - projectileData.boatTailDiameter) / 2)
//				/
//				projectileData.boatTailLength);

//			projectileData.ogiveRadius = Mathf.Clamp(projectileData.ogiveRadius, 0.1f, Mathf.Infinity);

//			projectileData.meplatDiameter = Mathf.Clamp(projectileData.meplatDiameter, 0, Mathf.Infinity);

//			#endregion

//			#region Render bullet
//			//Render what the bullet would look like

//			Texture2D bulletTex = new Texture2D(280, 210);

//			Vector2 midPoint = new Vector2(bulletTex.width / 2, bulletTex.height / 2);

//			float offsetScale = Mathf.Min(
//									(bulletTex.width * 0.8f) / projectileData.bulletLength,
//									(bulletTex.height * 0.8f) / Mathf.Max(projectileData.boatTailDiameter,
//										projectileData.driveBandDiameter,
//										projectileData.bulletDiameter,
//										projectileData.baseDiameter)
//								);


//			#region Draw Cylinder

//			DrawSquare(ref bulletTex,
//				(int)(midPoint.x - projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y + projectileData.baseDiameter * 0.5f * offsetScale), //Up Left
//				(int)(midPoint.x + projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y + projectileData.bulletDiameter * 0.5f * offsetScale), //Up Right
//				(int)(midPoint.x + projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y - projectileData.bulletDiameter * offsetScale * 0.5f), //Down Right
//				(int)(midPoint.x - projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y - projectileData.baseDiameter * offsetScale * 0.5f), //Down Left
//				Color.blue
//			);
//			#endregion

//			#region Draw Boat Tail
//			DrawSquare(ref bulletTex,
//				(int)(midPoint.x - ((projectileData.cylinderLength * 0.5f) + projectileData.boatTailLength) * offsetScale),
//				(int)(midPoint.y + projectileData.boatTailDiameter * 0.5f * offsetScale), //Up Left
//				(int)(midPoint.x - projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y + projectileData.baseDiameter * 0.5f * offsetScale), //Up Right
//				(int)(midPoint.x - projectileData.cylinderLength * 0.5f * offsetScale),
//				(int)(midPoint.y - projectileData.baseDiameter * offsetScale * 0.5f), //Down Right
//				(int)(midPoint.x - ((projectileData.cylinderLength * 0.5f) + projectileData.boatTailLength) * offsetScale),
//				(int)(midPoint.y - projectileData.boatTailDiameter * offsetScale * 0.5f), //Down Left
//				Color.magenta
//			);
//			#endregion

//			#region Draw Drive Band
//			if (projectileData.hasDriveBand)
//			{
//				DrawSquare(ref bulletTex, //Main square
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale), //Up Left
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale), //Up Right
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f), //Down Right
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f), //Down Left
//					Color.black
//				);

//				DrawSquare(ref bulletTex, //Left Extension
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale - projectileData.driveBandBevel),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale - projectileData.driveBandBevel), //Up Left REWRITTEN
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale), //Up Right
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f), //Down Right
//					(int)(midPoint.x - projectileData.driveBandLength * 0.5f * offsetScale - projectileData.driveBandBevel),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f + projectileData.driveBandBevel), //Down Left REWRITTEN
//					Color.black
//				);

//				DrawSquare(ref bulletTex, //Right Extension
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale), //Up Left
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale + projectileData.driveBandBevel),
//					(int)(midPoint.y + projectileData.driveBandDiameter * 0.5f * offsetScale - projectileData.driveBandBevel), //Up Right
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale + projectileData.driveBandBevel),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f + projectileData.driveBandBevel), //Down Right
//					(int)(midPoint.x + projectileData.driveBandLength * 0.5f * offsetScale),
//					(int)(midPoint.y - projectileData.driveBandDiameter * offsetScale * 0.5f), //Down Left
//					Color.black
//				);
//			}
//			#endregion

//			#region Draw Nose

//			/*
//		 * We can assume that the tip and the base of the nose will form an isoceles triangle, where the sides of that triangle
//		 * connecting to the center point are the Ogive Radius.
//		 */

//			//Firstly, divide the distance between the nose tip and base edge by two to form a right angled triangle.

//			//		float halfDistance = Vector2.Distance (
//			//			                     new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
//			//				                     midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
//			//			                     new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
//			//				                     midPoint.y)
//			//		                     ) / 2;
//			//
//			//		//Find rotation between both positions
//			//		float rotation = Vector2.SignedAngle (
//			//			                 new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
//			//				                 midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
//			//			                 new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
//			//				                 midPoint.y)
//			//		                 );
//			//
//			//		DrawLine (ref bulletTex,
//			//			(int)(midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale),
//			//			(int)(midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale),
//			//			(int)(midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale),
//			//			(int)(midPoint.y),
//			//			Color.black
//			//		);
//			//
//			//		Vector2 ogiveMidPoint = (
//			//		                            new Vector2 (midPoint.x + bulletData.cylinderLength * 0.5f * offsetScale,
//			//			                            midPoint.y + bulletData.bulletDiameter * 0.5f * offsetScale) +
//			//		                            new Vector2 (midPoint.x + (bulletData.cylinderLength * 0.5f + bulletData.noseVirtualLength) * offsetScale,
//			//			                            midPoint.y)
//			//		                        ) / 2;
//			//Trust me, I have ample reason for setting this at 400. I would personally prefer the way the nose
//			//looks when this is at 1,000, but thats unnecessary
//			int noseDrawIterations = 400;

//			//Make sure we only render the bullet where the y>=0
//			//float xInt = 0;

//			for (int i = 0; i < noseDrawIterations; i++)
//			{
//				if (
//					(+Mathf.Sqrt(
//						Mathf.Pow(projectileData.ogiveRadius, 2)
//						- Mathf.Pow(projectileData.noseVirtualLength * i / noseDrawIterations, 2)
//					) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius)
//					>= 0)
//				{
//					#region Upper Ogive
//					DrawLine(ref bulletTex,

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * i / noseDrawIterations)) * offsetScale
//					, 0, Mathf.Infinity)),

//						(int)(midPoint.y + (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * i / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * (i + 1) / noseDrawIterations)) * offsetScale
//					, 0, Mathf.Infinity)),

//						(int)(midPoint.y + (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * (i + 1) / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),
//						Color.black

//					);
//					#endregion

//					#region Lower Ogive
//					DrawLine(ref bulletTex,

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * i / noseDrawIterations)) * offsetScale
//						, 0, Mathf.Infinity)),

//						(int)(midPoint.y - (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * i / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * (i + 1) / noseDrawIterations)) * offsetScale
//						, 0, Mathf.Infinity)),

//						(int)(midPoint.y - (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * (i + 1) / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),
//						Color.black

//					);
//					#endregion


//				}
//				else
//				{
//					//Draw a line between the two nose points(for when the ogive radius is really small),
//					//and break out of the loop so we aren't uselessly trying
//					//to draw more lines only to deny making them

//					DrawLine(ref bulletTex,

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * (i - 1) / noseDrawIterations)) * offsetScale
//						, 0, Mathf.Infinity)),

//						(int)(midPoint.y + (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * (i - 1) / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),

//						(int)(midPoint.x + Mathf.Clamp(
//							((projectileData.cylinderLength / 2) + (projectileData.noseVirtualLength * (i - 1) / noseDrawIterations)) * offsetScale
//						, 0, Mathf.Infinity)),

//						(int)(midPoint.y - (
//							+Mathf.Sqrt(
//								Mathf.Pow(projectileData.ogiveRadius, 2)
//								- Mathf.Pow(projectileData.noseVirtualLength * (i - 1) / noseDrawIterations, 2)
//							) + projectileData.ogiveTangentRadius - projectileData.ogiveRadius) * offsetScale),
//						Color.black

//					);
//					break;
//				}
//			}

//			#endregion


//			bulletTex.Apply();
//			//Render to inspector GUI
//			GUILayout.Box((Texture)bulletTex);

//			#endregion

//			#region Render Input
//			if (GUILayout.Button("Generate Drag Graph"))
//			{
//				//bulletData.dragGraph=DragGraphGenerator
//				projectileData.GenerateDragCoefficientGraph();
//				projectileData.InitGraphSettings();
//			}

//			///base.OnInspectorGUI ();

//			projectileData.bulletDiameter = EditorGUILayout.FloatField("Diameter", projectileData.bulletDiameter);
//			projectileData.baseDiameter = EditorGUILayout.FloatField("Base Diameter", projectileData.baseDiameter);
//			projectileData.bulletLength = EditorGUILayout.FloatField("Total Length", projectileData.bulletLength);
//			EditorGUILayout.LabelField("Cylinder Length", projectileData.cylinderLength.ToString());
//			EditorGUILayout.Space();

//			projectileData.noseLength = EditorGUILayout.FloatField("Nose Length", projectileData.noseLength);
//			projectileData.meplatDiameter = EditorGUILayout.FloatField("Meplat Diameter", projectileData.meplatDiameter);
//			EditorGUILayout.LabelField("Virtual Nose Length", projectileData.noseVirtualLength.ToString());
//			EditorGUILayout.Space();

//			projectileData.boatTailDiameter = EditorGUILayout.FloatField("Boat Tail Diameter", projectileData.boatTailDiameter);
//			projectileData.boatTailLength = EditorGUILayout.FloatField("Boat Tail Length", projectileData.boatTailLength);
//			EditorGUILayout.LabelField("Boat Tail Angle", projectileData.boatTailAngle.ToString());
//			EditorGUILayout.Space();

//			if (projectileData.hasDriveBand = EditorGUILayout.Toggle("Has Drive Band", projectileData.hasDriveBand))
//			{
//				projectileData.driveBandDiameter = EditorGUILayout.FloatField("Drive Band Diameter", projectileData.driveBandDiameter);
//				projectileData.driveBandLength = EditorGUILayout.FloatField("Drive Band Length", projectileData.driveBandLength);
//				projectileData.driveBandBevel = EditorGUILayout.FloatField("Drive Band Bevel", projectileData.driveBandBevel);
//			}
//			EditorGUILayout.Space();


//			projectileData.ogiveRadius = EditorGUILayout.FloatField("Secant Radius", projectileData.ogiveRadius);
//			//bulletData.ogiveRadiusTangent = EditorGUILayout.DelayedFloatField ("Ogive Radius Tangent", bulletData.ogiveRadiusTangent);
//			//EditorGUILayout.LabelField ("Ogive Radius", bulletData.ogiveRadius.ToString ());
//			EditorGUILayout.LabelField("Ogive Tangent Radius", projectileData.ogiveTangentRadius.ToString());

//			EditorGUILayout.Space();

//			projectileData.projectileMass = EditorGUILayout.FloatField("Bullet Mass", projectileData.projectileMass);
//			projectileData.dragGraph = EditorGUILayout.CurveField("Drag Curve", projectileData.dragGraph);
//			#endregion
//			#endregion
//		}
//		else
//		{
//			projectileData.dragCoefficient = EditorGUILayout.FloatField("Drag Coefficient", projectileData.dragCoefficient);
//			EditorGUILayout.Space();
//		}

//		EditorGUILayout.Space();

//	}

//	#region Line Renderers

//	void DrawSquare(ref Texture2D tex, int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
//	{
//		DrawLine(ref tex,
//			x0,
//			y0,
//			x1,
//			y1,
//			color
//		);
//		DrawLine(ref tex,
//			x1,
//			y1,
//			x2,
//			y2,
//			color
//		);
//		DrawLine(ref tex,
//			x2,
//			y2,
//			x3,
//			y3,
//			color
//		);
//		DrawLine(ref tex,
//			x3,
//			y3,
//			x0,
//			y0,
//			color
//		);
//	}

//	void DrawLine(ref Texture2D tex, int x0, int y0, int x1, int y1, Color col)
//	{
//		int dy = (int)(y1 - y0);
//		int dx = (int)(x1 - x0);
//		int stepx, stepy;

//		if (dy < 0)
//		{
//			dy = -dy;
//			stepy = -1;
//		}
//		else
//		{
//			stepy = 1;
//		}
//		if (dx < 0)
//		{
//			dx = -dx;
//			stepx = -1;
//		}
//		else
//		{
//			stepx = 1;
//		}
//		dy <<= 1;
//		dx <<= 1;

//		float fraction = 0;

//		tex.SetPixel(x0, y0, col);
//		if (dx > dy)
//		{
//			fraction = dy - (dx >> 1);
//			while (Mathf.Abs(x0 - x1) > 1)
//			{
//				if (fraction >= 0)
//				{
//					y0 += stepy;
//					fraction -= dx;
//				}
//				x0 += stepx;
//				fraction += dy;
//				tex.SetPixel(x0, y0, col);
//			}
//		}
//		else
//		{
//			fraction = dx - (dy >> 1);
//			while (Mathf.Abs(y0 - y1) > 1)
//			{
//				if (fraction >= 0)
//				{
//					x0 += stepx;
//					fraction -= dy;
//				}
//				y0 += stepy;
//				fraction += dx;
//				tex.SetPixel(x0, y0, col);
//			}
//		}
//	}

//	void DrawCircleSection(ref Texture2D tex, int x, int y, float radius, float lowerAngle, float upperAngle, int iterations, Color color)
//	{
//		float angleSection = (upperAngle - lowerAngle) / iterations;

//		for (int i = 0; i <= iterations; i++)
//		{

//			DrawLine(ref tex, x + (int)(radius * Mathf.Cos(i * angleSection)), y + (int)(radius * Mathf.Sin(i * angleSection)), x + (int)(radius * Mathf.Cos((i + 1) * angleSection)), y + (int)(radius * Mathf.Sin((i + 1) * angleSection)), color);

//		}
//	}

//	#endregion

//	/// <summary>
//	/// PolarForm -> Cartesian Form
//	/// </summary>
//	/// <returns>The position.</returns>
//	/// <param name="rotation">Rotation in radians.</param>
//	/// <param name="distance">Distance.</param>
//	Vector2 DirectionalPosition(float rotation, float distance)
//	{
//		return new Vector2(distance * Mathf.Cos(rotation), distance * Mathf.Sin(rotation));
//	}


//}
