using UnityEngine;
using UnityEditor;
using System.Linq;
using Excessives.LinqE;

//[CustomPropertyDrawer (typeof(ConverterFieldBase))]
using System;


public class ConverterFieldEditor : Editor
{
	string[] enumNames;

	//	public override void OnInspectorGUI ()
	//	{
	//		ConverterFieldBase converter = (ConverterFieldBase)target;
	//
	//		EditorGUI.EnumMaskPopup (new Rect (0, 0, 128, 16), "Convert From", converter.usedEnum);
	//
	//	}

}
