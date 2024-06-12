// Description : Work in association with LayerSelectorEditor.cs : Allow to setup some parameters from LayerSelector.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CanEditMultipleObjects]
[CustomEditor(typeof(LayerSelector))]
public class LayerSelectorEditor : Editor
{

	SerializedProperty SeeInspector;
	SerializedProperty moreOptions;
	SerializedProperty helpBox;
	SerializedProperty layerName;

	LayersRef objLayerRef;
	List<string> listOfLayersNames = new List<string>();

	private Texture2D MakeTex(int width, int height, Color col)
	{       // use to change the GUIStyle
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

	void OnEnable()
	{
		// Setup the SerializedProperties.
		SeeInspector = serializedObject.FindProperty("SeeInspector");
		moreOptions = serializedObject.FindProperty("moreOptions");
		helpBox = serializedObject.FindProperty("helpBox");
		layerName = serializedObject.FindProperty("layerName");
		objLayerRef = FindObjectOfType<LayersRef>();

        if (objLayerRef)
        {
            for(var i = 0;i< objLayerRef.layersListData.listLayerInfo.Count; i++)
            {
				listOfLayersNames.Add(objLayerRef.layersListData.listLayerInfo[i].name);
			}
        }
	}


	public override void OnInspectorGUI()
	{
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

		DisplayerLayersInfo();


		serializedObject.ApplyModifiedProperties();
	}

	void DisplayerLayersInfo()
	{
        if (objLayerRef && objLayerRef.isActiveAndEnabled)
        {
            if(listOfLayersNames.Count > 0)
			    layerName.intValue = EditorGUILayout.Popup(layerName.intValue, listOfLayersNames.ToArray());
		}
		else
        {
			EditorGUILayout.HelpBox(
		   "Prefab LayersRef is needed in the current to access the Inspector options.", MessageType.Info);
		}

		/*LayerSelector myScript = (LayerSelector)target;
		EditorGUILayout.PropertyField(layersListData, new GUIContent(""));

		EditorGUILayout.LabelField("");

		if (layersListData.objectReferenceValue)
		{
			SerializedObject serializedObject2 = new SerializedObject(myScript.layersListData);

			serializedObject2.Update();
			SerializedProperty listLayerInfo = serializedObject2.FindProperty("listLayerInfo");

			for (var i = 0; i < listLayerInfo.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));
				EditorGUILayout.PropertyField(listLayerInfo.GetArrayElementAtIndex(i).FindPropertyRelative("name"), new GUIContent(""), GUILayout.Width(120));
				EditorGUILayout.LabelField(" ID: ", GUILayout.Width(40));
				EditorGUILayout.PropertyField(listLayerInfo.GetArrayElementAtIndex(i).FindPropertyRelative("layerID"), new GUIContent(""), GUILayout.Width(30));
				EditorGUILayout.EndHorizontal();
			}



			serializedObject2.ApplyModifiedProperties();
		}
        */

	}


	void OnSceneGUI()
	{

	}

	private void HelpZone_01()
	{
		EditorGUILayout.HelpBox(
		   "", MessageType.Info);
	}

}
#endif