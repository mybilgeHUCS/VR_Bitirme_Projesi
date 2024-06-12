//Description : PathEditor.cs : Works in association with Path.cs . Allows to create a car path
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
	SerializedProperty SeeInspector;                                            // use to draw default Inspector
	SerializedProperty moreOptions;
	SerializedProperty helpBox;

	SerializedProperty additionalPathsList;
	SerializedProperty checkpoints;
	SerializedProperty AltPathList;
	SerializedProperty currentSelectedCheckpoint;
	SerializedProperty prefabCheckpoint;
	SerializedProperty showCheckpoints;


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
		SeeInspector                = serializedObject.FindProperty("SeeInspector");
		moreOptions                 = serializedObject.FindProperty("moreOptions");
		helpBox                     = serializedObject.FindProperty("helpBox");
		additionalPathsList         = serializedObject.FindProperty("additionalPathsList");
		checkpoints                 = serializedObject.FindProperty("checkpoints");
		AltPathList                 = serializedObject.FindProperty("AltPathList");
		currentSelectedCheckpoint   = serializedObject.FindProperty("currentSelectedCheckpoint");
		prefabCheckpoint            = serializedObject.FindProperty("prefabCheckpoint");
		showCheckpoints             = serializedObject.FindProperty("showCheckpoints");
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

		EditorGUILayout.LabelField("");

		DisplayAltPath();

		if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
			DisplayAdditionalPaths();

		serializedObject.ApplyModifiedProperties();
          
		EditorGUILayout.LabelField("");
		#endregion
	}

    void DisplayAdditionalPaths()
    {
		EditorGUILayout.LabelField("");

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Show All"))
		{
			for (var i = 0; i < additionalPathsList.arraySize; i++)
			{
				SerializedProperty b_Show = additionalPathsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_Show");
				b_Show.boolValue = true;
			}
		}
		if (GUILayout.Button("Hide All"))
		{
			for (var i = 0; i < additionalPathsList.arraySize; i++)
			{
				SerializedProperty b_Show = additionalPathsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_Show");
				b_Show.boolValue = false;
			}
		}
		EditorGUILayout.EndHorizontal();

		for (var i = 0;i< additionalPathsList.arraySize; i++)
        {
			SerializedProperty b_Show = additionalPathsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_Show");
			SerializedProperty offset = additionalPathsList.GetArrayElementAtIndex(i).FindPropertyRelative("offset");
			SerializedProperty color = additionalPathsList.GetArrayElementAtIndex(i).FindPropertyRelative("color");
			

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("-", GUILayout.Width(20)))
            {
              
            }
			EditorGUILayout.LabelField(i + ":", GUILayout.Width(20));

			b_Show.boolValue = EditorGUILayout.Toggle(b_Show.boolValue, GUILayout.Width(20));

            if (b_Show.boolValue)
            {
				EditorGUILayout.PropertyField(offset, new GUIContent(""));
				EditorGUILayout.PropertyField(color, new GUIContent(""));
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.LabelField("");
		if (GUILayout.Button("Add New Alternative Path"))
		{
			
		}
	}

    void DisplayCheckpoints()
    {
        if (!Application.isPlaying)
        {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.Width(20));
			EditorGUILayout.LabelField("Checkpoints list:", EditorStyles.boldLabel, GUILayout.Width(100));
			EditorGUILayout.PropertyField(showCheckpoints, new GUIContent(""), GUILayout.Width(20));
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Update Checkpoint Direction"))
			{
				for (var i = 0; i < checkpoints.arraySize; i++)
                {

					Transform trans01 = checkpoints.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
					Transform trans02 = checkpoints.GetArrayElementAtIndex((i+1) % checkpoints.arraySize).objectReferenceValue as Transform;

					Undo.RegisterFullObjectHierarchyUndo(trans01.gameObject, trans01.name);
					trans01.LookAt(trans02);
					trans01.localEulerAngles = new Vector3(0, trans01.localEulerAngles.y, 0);
				}
			}

			

			if (showCheckpoints.boolValue)
            {
				for (var i = 0; i < checkpoints.arraySize; i++)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));
					if (GUILayout.Button("+", GUILayout.Width(20)))
					{
						Path myScript = (Path)target;
						GameObject newCheckpoint = PrefabUtility.InstantiatePrefab((GameObject)prefabCheckpoint.objectReferenceValue, myScript.transform) as GameObject;
						Undo.RegisterCreatedObjectUndo(newCheckpoint, "newAltPath");

						newCheckpoint.transform.position = myScript.checkpoints[i].transform.position;
						newCheckpoint.transform.rotation = myScript.checkpoints[i].transform.rotation;

						newCheckpoint.name = myScript.checkpoints[i].name + " B";

						int childPosition = myScript.checkpoints[i].transform.GetSiblingIndex();
						newCheckpoint.transform.SetSiblingIndex(childPosition + 1);

						checkpoints.InsertArrayElementAtIndex(0);
						checkpoints.GetArrayElementAtIndex(0).objectReferenceValue = newCheckpoint;
						checkpoints.MoveArrayElement(0, i + 1);
					}

					if (GUILayout.Button("-", GUILayout.Width(20)))
					{
						Path myScript = (Path)target;
						Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
						if (checkpoints.GetArrayElementAtIndex(i).objectReferenceValue != null)
						{
							Undo.DestroyObjectImmediate(myScript.checkpoints[i].gameObject);
						}

						myScript.checkpoints.RemoveAt(i);
						break;
					}

					EditorGUILayout.PropertyField(checkpoints.GetArrayElementAtIndex(i), new GUIContent("")/*,  GUILayout.Width(30)*/);
					EditorGUILayout.LabelField("Alt Path Trigger: ", GUILayout.Width(100));

					Transform refTrans = (Transform)checkpoints.GetArrayElementAtIndex(i).objectReferenceValue;

					if (refTrans && refTrans.GetChild(0).childCount > 0)
					{
						GameObject AltPath = refTrans.GetChild(0).GetChild(0).gameObject;
						SerializedObject serializedObject0 = new UnityEditor.SerializedObject(AltPath);
						serializedObject0.Update();
						SerializedProperty m_IsActive = serializedObject0.FindProperty("m_IsActive");

						if (m_IsActive.boolValue && GUILayout.Button("", listGUIStyle[5], GUILayout.Width(20)))
						{
							m_IsActive.boolValue = !m_IsActive.boolValue;
						}
						else if (!m_IsActive.boolValue && GUILayout.Button("", listGUIStyle[0], GUILayout.Width(20)))
						{
							m_IsActive.boolValue = !m_IsActive.boolValue;
						}

						serializedObject0.ApplyModifiedProperties();

						if (m_IsActive.boolValue)
						{
							if (GUILayout.Button("Select", GUILayout.Width(50)))
							{
								Selection.activeGameObject = refTrans.GetChild(0).GetChild(0).gameObject;
							}
						}
						else
						{
							EditorGUILayout.LabelField("", GUILayout.Width(50));
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}
			}
		
		}
	}

	void DisplayAltPath()
	{
		if (!Application.isPlaying)
		{
			EditorGUILayout.LabelField("Alt Path List:", EditorStyles.boldLabel);
			for (var i = 0; i < AltPathList.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));
				EditorGUILayout.PropertyField(AltPathList.GetArrayElementAtIndex(i), new GUIContent("")/*,  GUILayout.Width(30)*/);
				if (GUILayout.Button("-", GUILayout.Width(20)))
                {
					Path myScript = (Path)target;
					Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
					if (AltPathList.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
						Undo.DestroyObjectImmediate(myScript.AltPathList[i].gameObject);
					}

					myScript.AltPathList.RemoveAt(i);
					break;
				}
			    EditorGUILayout.EndHorizontal();
			}

			CreateNewAltPath();
		}
	}

    void CreateNewAltPath()
    {
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create a new Alt Path using the checkpoint: "))
		{
			if (currentSelectedCheckpoint.intValue < checkpoints.arraySize)
            {
                //-> Instantiate anew ALt Path Prefab
				AltPathList.InsertArrayElementAtIndex(0);
				Path myScript = (Path)target;

				PathRef pathRef = myScript.transform.parent.GetComponent<PathRef>();
				GameObject newAltPath = PrefabUtility.InstantiatePrefab((GameObject)pathRef.prefabAltPath_Ref, pathRef.transform) as GameObject;
				Undo.RegisterCreatedObjectUndo(newAltPath, "newAltPath");

				AltPathList.GetArrayElementAtIndex(0).objectReferenceValue = newAltPath;
				AltPathList.MoveArrayElement(0, AltPathList.arraySize - 1);

				newAltPath.name = "Checkpoint_" + currentSelectedCheckpoint.intValue + "_AltPath_" + (AltPathList.arraySize - 1);

                //-> Activate the Alt Path Trigger
				Transform refTrans = (Transform)checkpoints.GetArrayElementAtIndex(currentSelectedCheckpoint.intValue).objectReferenceValue;

				if (refTrans.GetChild(0).childCount > 0)
				{
					GameObject AltPathTrigger = refTrans.GetChild(0).GetChild(0).gameObject;
					SerializedObject serializedObject0 = new UnityEditor.SerializedObject(AltPathTrigger);
					serializedObject0.Update();
					SerializedProperty m_IsActive = serializedObject0.FindProperty("m_IsActive");

					m_IsActive.boolValue = true;

					serializedObject0.ApplyModifiedProperties();
				}

				//-> Connect the new Alt Path to the Trigger ALt Path object
				if (refTrans.GetChild(0).childCount > 0)
				{
					GameObject AltPathTrigger = refTrans.GetChild(0).GetChild(0).gameObject;
					SerializedObject serializedObject0 = new UnityEditor.SerializedObject(AltPathTrigger.GetComponent<TriggerAltPath>());
					serializedObject0.Update();
					SerializedProperty m_AltPathList = serializedObject0.FindProperty("AltPathList");

					m_AltPathList.InsertArrayElementAtIndex(0);
					m_AltPathList.GetArrayElementAtIndex(0).objectReferenceValue = newAltPath;
					m_AltPathList.MoveArrayElement(0, m_AltPathList.arraySize - 1);

					serializedObject0.ApplyModifiedProperties();
				}


				//-> Auto-Connect chepointStart
				newAltPath.GetComponent<AltPath>().checkpointStart = (Transform)checkpoints.GetArrayElementAtIndex(currentSelectedCheckpoint.intValue).objectReferenceValue;
				newAltPath.transform.position = myScript.checkpoints[currentSelectedCheckpoint.intValue].transform.position;

				//-> Auto-Connect altPathTrigger object
				if (refTrans.GetChild(0).childCount > 0)
				{
					GameObject AltPathTrigger = refTrans.GetChild(0).GetChild(0).gameObject;
					newAltPath.GetComponent<AltPath>().triggerAltPath = AltPathTrigger.GetComponent<TriggerAltPath>();
				}

				Transform PlayerTrigger = newAltPath.transform.GetChild(0);
				PlayerTrigger.rotation = newAltPath.GetComponent<AltPath>().checkpointStart.rotation;

				Selection.activeGameObject = newAltPath;
			}
		}
		EditorGUILayout.PropertyField(currentSelectedCheckpoint, new GUIContent(""),  GUILayout.Width(30));
		EditorGUILayout.EndHorizontal();
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