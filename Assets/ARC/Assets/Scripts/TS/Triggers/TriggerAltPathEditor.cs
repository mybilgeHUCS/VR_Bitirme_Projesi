//Description : TriggerAltPathEditor.cs : Works in association with TriggerAltPath.cs . Allows to create a car path
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(TriggerAltPath))]
public class TriggerAltPathEditor : Editor
{
	SerializedProperty SeeInspector;                                            // use to draw default Inspector
	SerializedProperty moreOptions;
	SerializedProperty helpBox;

	SerializedProperty AltPathList;
	SerializedProperty GrpAltPath;
	SerializedProperty bestPath;


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
	

	public Color _cRed2 = new Color(1f, .35f, 0f, 1f);
	public Color _cRed = new Color(1f, .5f, 0f, .5f);
	public Color _cGray = new Color(.9f, .9f, .9f, 1);
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

		#region
		// Setup the SerializedProperties.
		SeeInspector    = serializedObject.FindProperty("SeeInspector");
		moreOptions     = serializedObject.FindProperty("moreOptions");
		helpBox         = serializedObject.FindProperty("helpBox");

		AltPathList     = serializedObject.FindProperty("AltPathList");
		GrpAltPath      = serializedObject.FindProperty("GrpAltPath");
		bestPath        = serializedObject.FindProperty("bestPath");
		#endregion
	}

	public override void OnInspectorGUI()
	{

		#region
		if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		serializedObject.Update();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("See Inspector:", GUILayout.Width(85));
		EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
		EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
		EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));

		if (EditorPrefs.GetBool("MoreOptions") == true)
		{
			EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
			EditorGUILayout.PropertyField(moreOptions, new GUIContent(""), GUILayout.Width(30));
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginVertical(listGUIStyle[0]);
		BestPathSelection();
		EditorGUILayout.EndVertical();
		EditorGUILayout.LabelField("");

		EditorGUILayout.BeginVertical(listGUIStyle[0]);
		DisplayMainPathInfo();
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.LabelField("");
		#endregion
	}

    void BestPathSelection()
    {
		EditorGUILayout.HelpBox("-1: Main Path" + "\n" +
            "0 to x: Alt Path", MessageType.Info);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Select Best AI Path:", GUILayout.Width(120));
		EditorGUILayout.PropertyField(bestPath, new GUIContent(""));
		EditorGUILayout.EndHorizontal();
	}


	void DisplayMainPathInfo()
    {
		EditorGUILayout.LabelField("Alternative Paths list:", EditorStyles.boldLabel);
		for (var i = 0; i < AltPathList.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));

			EditorGUILayout.PropertyField(AltPathList.GetArrayElementAtIndex(i), new GUIContent(""));

			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				if (AltPathList.GetArrayElementAtIndex(i).objectReferenceValue != null)
				{
					AltPath altPath = (AltPath)AltPathList.GetArrayElementAtIndex(i).objectReferenceValue;
					GameObject obj = altPath.gameObject;
					TriggerAltPath myScript = (TriggerAltPath)target;
					Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
					Undo.DestroyObjectImmediate(obj);
					AltPathList.DeleteArrayElementAtIndex(i);
				}

				AltPathList.DeleteArrayElementAtIndex(i);
				break;
			}

			EditorGUILayout.EndHorizontal();
		}
        //
		if (GUILayout.Button("Create new Alternative Path", GUILayout.Height(20)))
		{
			AltPathList.InsertArrayElementAtIndex(0);

			TriggerAltPath myScript = (TriggerAltPath)target;

			GameObject newGroup = PrefabUtility.InstantiatePrefab((GameObject)GrpAltPath.objectReferenceValue, myScript.transform.parent.parent.parent) as GameObject;
			Undo.RegisterCreatedObjectUndo(newGroup, "newGroup");

			AltPathList.GetArrayElementAtIndex(0).objectReferenceValue = newGroup;

			newGroup.GetComponent<AltPath>().triggerAltPath = myScript;

			AltPathList.MoveArrayElement(0,Mathf.Clamp(AltPathList.arraySize-1, 0,AltPathList.arraySize));

		}
		if (GUILayout.Button("Create new empty slot"))
		{
			AltPathList.InsertArrayElementAtIndex(0);
			AltPathList.GetArrayElementAtIndex(0).objectReferenceValue = null;
		}
	}


	void OnSceneGUI()
	{
	
	}

	private void HelpZone(int value)
	{
        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "Description: This script is used to display the needed P1 and P2 UI depending the selected game mode." + "\n" +
                "It allows to enable|disable rect Transform. Change Scale|Anchor|Pivot. Change the ViewportRect of a camera.", MessageType.Info);
                break;
        }
    }
}
#endif