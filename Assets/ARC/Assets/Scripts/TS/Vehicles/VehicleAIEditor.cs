// Description : Work in association with VehiculeAI.cs : Allow to setup some parameters from VehiculeAI.cs
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.SceneManagement;
using TS.Generics;

[CustomEditor(typeof(VehicleAI))]
public class VehicleAIEditor : Editor {

	SerializedProperty 		SeeInspector;
	//
	SerializedProperty carWaitBeforeBackwardDuration;
	SerializedProperty 		carBackwardDuration;

	SerializedProperty methodsList;
	public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
	public AP_MethodModule_Pc methodModule;


	#region Init Inspector Color
	private Texture2D MakeTex(int width, int height, Color col)
	{                       // use to change the GUIStyle
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	private List<Texture2D> listTex = new List<Texture2D>();
	public List<GUIStyle> listGUIStyle = new List<GUIStyle>();
	private List<Color> listColor = new List<Color>();
	#endregion

	void OnEnable () {
		#region Init Inspector Color
		listColor.Clear();
		listGUIStyle.Clear();
		for (var i = 0; i < inspectorColor.listColor.Length; i++)
		{
			listTex.Add(MakeTex(2, 2, inspectorColor.ReturnColor(i)));
			listGUIStyle.Add(new GUIStyle());
			listGUIStyle[i] = new GUIStyle(); listGUIStyle[i].normal.background = listTex[i];
		}
		#endregion

		// Setup the SerializedProperties.
		SeeInspector = serializedObject.FindProperty ("SeeInspector");
		//vehicleAIEulerRotation 				= serializedObject.FindProperty ("vehicleAIEulerRotation");
		carWaitBeforeBackwardDuration 	= serializedObject.FindProperty ("carWaitBeforeBackwardDuration");
		carBackwardDuration 			= serializedObject.FindProperty ("carBackwardDuration");

		methodsList = serializedObject.FindProperty("methodsList");

		editorMethods = new EditorMethods_Pc();
		methodModule = new AP_MethodModule_Pc();
	}


	public override void OnInspectorGUI()
	{
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();
		serializedObject.Update ();


		EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Inspector :",GUILayout.Width(90));
			EditorGUILayout.PropertyField(SeeInspector, new GUIContent (""),GUILayout.Width(30));
		EditorGUILayout.EndHorizontal();


		displayMethodsLists(listGUIStyle[0], listGUIStyle[1]);

		serializedObject.ApplyModifiedProperties ();
	}

	//--> display multiple list of methods call when the scene starts
	private void displayMethodsLists(GUIStyle style_Yellow_01, GUIStyle style_Blue)
	{
		#region
		//--> Display feedback

		VehicleAI myScript = (VehicleAI)target;

		methodModule.displayMethodList("Improve AI behaviour (void)",
											editorMethods,
											methodsList,
											myScript.methodsList,
											style_Blue,
											style_Yellow_01,
											"");
		#endregion
	}


	void OnSceneGUI( )
	{

	}
}
#endif