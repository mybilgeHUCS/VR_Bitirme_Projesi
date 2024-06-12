//Description : AltPathEditor.cs : Work in association with AltPath.cs . Allow to create a car alt path
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(AltPath))]
public class AltPathEditor : Editor
{
	SerializedProperty SeeInspector;                                            // use to draw default Inspector
	SerializedProperty moreOptions;
	SerializedProperty helpBox;

	SerializedProperty checkpointStart;
	SerializedProperty tmpCheckpoints;
	SerializedProperty checkpointEnd;


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
		SeeInspector = serializedObject.FindProperty("SeeInspector");
		moreOptions = serializedObject.FindProperty("moreOptions");
		helpBox = serializedObject.FindProperty("helpBox");
		checkpointStart = serializedObject.FindProperty("checkpointStart");
		tmpCheckpoints = serializedObject.FindProperty("tmpCheckpoints");
		checkpointEnd = serializedObject.FindProperty("checkpointEnd");

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
		DisplayMainPathInfo();
		EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField("");
		EditorGUILayout.BeginVertical(listGUIStyle[0]);
		DisplayCheckpoints();
		EditorGUILayout.EndVertical();
		EditorGUILayout.LabelField("");

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.LabelField("");
		#endregion
	}

    void DisplayMainPathInfo()
    {
		EditorGUILayout.LabelField("Main Path Info:", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Checkpoint (Start):", GUILayout.Width(110));
		EditorGUILayout.PropertyField(checkpointStart, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		if (checkpointStart.objectReferenceValue && GUILayout.Button("Update Grp_AltPath using start position"))
		{
			AltPath myScript = (AltPath)target;
			Transform checkpointStartTrans = checkpointStart.objectReferenceValue as Transform;
			Undo.RegisterFullObjectHierarchyUndo(myScript.gameObject, myScript.name);

			myScript.transform.position = checkpointStartTrans.position;
			myScript.transform.rotation = checkpointStartTrans.rotation;
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Checkpoint (End):", GUILayout.Width(110));
		EditorGUILayout.PropertyField(checkpointEnd, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		if (checkpointEnd.objectReferenceValue == null)
		{
			EditorGUILayout.HelpBox("Select the checkpoint on the Main Path that corresponds to the end of the Alternative path.", MessageType.Warning);
		}
	}


	void DisplayCheckpoints()
    {
        if (!Application.isPlaying)
        {
			EditorGUILayout.LabelField("Alt Checkpoints list:",EditorStyles.boldLabel);
			for (var i = 0; i < tmpCheckpoints.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(i+ ": ", GUILayout.Width(20));

				
				if (GUILayout.Button("+", GUILayout.Width(20)))
				{
					Transform obj = (Transform)tmpCheckpoints.GetArrayElementAtIndex(i).objectReferenceValue;

					GameObject newAltPath = Instantiate((GameObject)obj.gameObject, obj.transform.parent) as GameObject;
					Undo.RegisterCreatedObjectUndo(newAltPath, "newAltPath");

					newAltPath.name = obj.name + "(Bis)";

					tmpCheckpoints.InsertArrayElementAtIndex(i);
					tmpCheckpoints.GetArrayElementAtIndex(i + 1).objectReferenceValue = newAltPath;

					break;
				}
				

				EditorGUILayout.PropertyField(tmpCheckpoints.GetArrayElementAtIndex(i), new GUIContent("")/*,  GUILayout.Width(30)*/);

                if (GUILayout.Button("-", GUILayout.Width(20)))
				{
					if (tmpCheckpoints.arraySize > 1)
					{
						Transform obj = (Transform)tmpCheckpoints.GetArrayElementAtIndex(i).objectReferenceValue;

						tmpCheckpoints.DeleteArrayElementAtIndex(i);
						if (obj)
						{
							

							Undo.DestroyObjectImmediate(obj.gameObject);
						}
						tmpCheckpoints.DeleteArrayElementAtIndex(i);
					}

					break;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
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