// Description : Work in association with GrpPowerUpEditor.cs : Allow to setup some parameters from GrpPowerUp.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CanEditMultipleObjects]
[CustomEditor(typeof(GrpPowerUp))]
public class GrpPowerUpEditor : Editor
{

	SerializedProperty SeeInspector;
	SerializedProperty moreOptions;
	SerializedProperty helpBox;
	SerializedProperty currentPowerUp;
	SerializedProperty currentRandomPUSelected;
	SerializedProperty listPowerUpCreation;

	SerializedProperty objInsertBefore;
	SerializedProperty objInsertAfter;
	SerializedProperty path;
	SerializedProperty m_tab;
	SerializedProperty objSelectionRulesList;

	SerializedObject serializedObject2;
	PowerUpsDatas powerUpsDatas;
	SerializedProperty listPowerUps;
	SerializedProperty m_PURandomPrefab;
	SerializedProperty m_PURandomSeqRef;
	SerializedProperty m_PURandomSeq;
	List<string> listPowerUpsName = new List<string>();

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
		currentPowerUp = serializedObject.FindProperty("currentPowerUp");
		currentRandomPUSelected = serializedObject.FindProperty("currentRandomPUSelected");
		listPowerUpCreation = serializedObject.FindProperty("listPowerUpCreation");
		objInsertAfter = serializedObject.FindProperty("objInsertAfter");
		objInsertBefore = serializedObject.FindProperty("objInsertBefore");
		path = serializedObject.FindProperty("path");

		m_tab = serializedObject.FindProperty("currentEditorTab");

		objSelectionRulesList = serializedObject.FindProperty("objSelectionRulesList");

		string objectPath = "Assets/ARC/Assets/Datas/Ref/PowerUpsDatas.asset";
		powerUpsDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as PowerUpsDatas;

		if (powerUpsDatas)
		{
			
			serializedObject2 = new UnityEditor.SerializedObject(powerUpsDatas);

			listPowerUps = serializedObject2.FindProperty("listPowerUps");
			m_PURandomPrefab = serializedObject2.FindProperty("puRandomPrefab");
			m_PURandomSeqRef = serializedObject2.FindProperty("randomSeqRef");
			m_PURandomSeq = serializedObject2.FindProperty("randomSeq");

			serializedObject2.ApplyModifiedProperties();

			for (var i = 0; i < listPowerUps.arraySize; i++)
				listPowerUpsName.Add(listPowerUps.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);

			listPowerUpsName.Add("Random");
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


		
		m_tab.intValue = GUILayout.Toolbar(m_tab.intValue, new string[] { "Setup", "Other" }, GUILayout.MinWidth(30));

		EditorGUILayout.LabelField("");

		switch (m_tab.intValue)
		{
			case 0:
				SectionVariables();
				EditorGUILayout.LabelField("");
				SectionCreateNewPowerUp();
				break;
			case 1:
				RulesSection();
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}

    void RulesSection()
    {
		EditorGUILayout.HelpBox(
		   "This section allows to choose the order and the rules used to choose AI Power-up when the AI vehicle enter on trigger Trigger_AILookAtPowerUp_In", MessageType.Info);


		for (var i = 0; i< objSelectionRulesList.arraySize; i++)
        {
			SerializedProperty name = objSelectionRulesList.GetArrayElementAtIndex(i).FindPropertyRelative("name");
			SerializedProperty objSelectionsRules = objSelectionRulesList.GetArrayElementAtIndex(i).FindPropertyRelative("objSelectionsRules");
			SerializedProperty ID = objSelectionRulesList.GetArrayElementAtIndex(i).FindPropertyRelative("ID");

	        EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(i + ":", GUILayout.Width(15));
			EditorGUILayout.PropertyField(name, new GUIContent(""), GUILayout.Width(80));
			EditorGUILayout.LabelField("ID:", GUILayout.Width(20));
			EditorGUILayout.PropertyField(ID, new GUIContent(""), GUILayout.Width(30));
			EditorGUILayout.PropertyField(objSelectionsRules, new GUIContent(""), GUILayout.MinWidth(50));

			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				objSelectionRulesList.DeleteArrayElementAtIndex(i);
				break;
			}
			if (GUILayout.Button("^", GUILayout.Width(20)))
			{
				objSelectionRulesList.MoveArrayElement(i, Mathf.Clamp(i - 1, 0, objSelectionRulesList.arraySize));
			}
			if (GUILayout.Button("v", GUILayout.Width(20)))
			{
				objSelectionRulesList.MoveArrayElement(i, Mathf.Clamp(i + 1, 0, objSelectionRulesList.arraySize));
			}
			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				objSelectionRulesList.InsertArrayElementAtIndex(i);
				objSelectionRulesList.GetArrayElementAtIndex(i+1).FindPropertyRelative("name").stringValue = "";
				objSelectionRulesList.GetArrayElementAtIndex(i+1).FindPropertyRelative("objSelectionsRules").objectReferenceValue = null;
				objSelectionRulesList.GetArrayElementAtIndex(i+1).FindPropertyRelative("ID").intValue = 0;
				break;
			}

			EditorGUILayout.EndHorizontal();
		}

		if (objSelectionRulesList.arraySize == 0)
			if (GUILayout.Button("Add rules", GUILayout.Height(20)))
			{
				objSelectionRulesList.InsertArrayElementAtIndex(0);
			}
	}


	void SectionVariables()
    {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("path:", GUILayout.Width(100));
		EditorGUILayout.PropertyField(path, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("");

		if (GUILayout.Button("Open window PathGenerator"))
		{
			EditorWindow.GetWindow(typeof(w_PathGenerator));
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Insert (Start point):", GUILayout.Width(110));
		EditorGUILayout.PropertyField(objInsertAfter, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Grp_Power_Up", GUILayout.Width(110));
		//EditorGUILayout.PropertyField(objInsertAfter, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Insert (End point):", GUILayout.Width(110));
		EditorGUILayout.PropertyField(objInsertBefore, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

        if(objInsertAfter.objectReferenceValue && objInsertBefore.objectReferenceValue)
        {
			Transform objRefAfter = (Transform)objInsertAfter.objectReferenceValue;
			Transform objRefBefore = (Transform)objInsertBefore.objectReferenceValue;


			if (objRefAfter.parent.GetComponent<AltPath>() && objRefBefore.parent.GetComponent<Path>()
                ||
				objRefAfter.parent.GetComponent<Path>() && objRefBefore.parent.GetComponent<AltPath>())
			{
				EditorGUILayout.HelpBox(
				"Set up error: InsertAfter and InsertBefore objects must be both on Main path or both on alt path", MessageType.Error);
			}
		}

	}

	void SectionCreateNewPowerUp()
	{
		currentPowerUp.intValue = EditorGUILayout.Popup(currentPowerUp.intValue, listPowerUpsName.ToArray());

		// Random case
		if(currentPowerUp.intValue == listPowerUpsName.Count - 1)
        {
			

			EditorGUILayout.BeginHorizontal();
			//m_PURandomSeqRef	
			serializedObject2.Update();
			for (var i = 0; i < m_PURandomSeq.arraySize; i++) 
				EditorGUILayout.LabelField(m_PURandomSeq.GetArrayElementAtIndex(i).intValue + " | ", GUILayout.Width(20));


			if (GUILayout.Button("<", GUILayout.Width(20)))
            {
				if(m_PURandomSeq.arraySize > 1)
					m_PURandomSeq.DeleteArrayElementAtIndex(m_PURandomSeq.arraySize - 1);
			}

			if (GUILayout.Button("Add", GUILayout.Width(40)))
			{
				if(m_PURandomSeq.GetArrayElementAtIndex(0).intValue != 0 || m_PURandomSeq.GetArrayElementAtIndex(0).intValue != 4)
                {
					m_PURandomSeq.InsertArrayElementAtIndex(0);
					m_PURandomSeq.GetArrayElementAtIndex(0).intValue = currentRandomPUSelected.intValue;
				}
				m_PURandomSeq.MoveArrayElement(0, m_PURandomSeq.arraySize - 1);
			}

			List<string> tmpList = new List<string>();
			for (var i = 0; i < listPowerUpsName.Count - 1; i++)
            {
				if (i == 0 || i == 4) tmpList.Add(".");
				else tmpList.Add(listPowerUpsName[i]);
			}
				

			if (currentRandomPUSelected.intValue >= listPowerUpsName.Count)
				currentRandomPUSelected.intValue = 0;

			currentRandomPUSelected.intValue = EditorGUILayout.Popup(currentRandomPUSelected.intValue, tmpList.ToArray(), GUILayout.MinWidth(100));

			if (GUILayout.Button("Reset", GUILayout.MinWidth(50)))
			{
				m_PURandomSeq.ClearArray();

				for (var i = 0; i < m_PURandomSeqRef.arraySize; i++)
					m_PURandomSeq.InsertArrayElementAtIndex(0);

				for (var i = 0; i < m_PURandomSeqRef.arraySize; i++)
					m_PURandomSeq.GetArrayElementAtIndex(i).intValue = m_PURandomSeqRef.GetArrayElementAtIndex(i).intValue;
			}

			serializedObject2.ApplyModifiedProperties();

			

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.LabelField("");

		EditorGUILayout.BeginHorizontal();

		
		if (GUILayout.Button("Add to the list"))
		{
            if(currentPowerUp.intValue!= 0)
            {
				listPowerUpCreation.InsertArrayElementAtIndex(0);
				listPowerUpCreation.GetArrayElementAtIndex(0).intValue = currentPowerUp.intValue;
				listPowerUpCreation.MoveArrayElement(0, listPowerUpCreation.arraySize-1);
			}
		}

		if (GUILayout.Button("Default"))
		{
			listPowerUpCreation.ClearArray();
			for(var i = 0;i< listPowerUps.arraySize; i++)
			{
                if(i != 0 && i != 4)
                {
					listPowerUpCreation.InsertArrayElementAtIndex(0);
					listPowerUpCreation.GetArrayElementAtIndex(0).intValue = i;
					listPowerUpCreation.MoveArrayElement(0, listPowerUpCreation.arraySize - 1);
				}
			}

		}
		if (GUILayout.Button("Clear list"))
		{
    		listPowerUpCreation.ClearArray();
		}
		EditorGUILayout.EndHorizontal();


		DisplayThePowerUpsList();

		if (GUILayout.Button("Create Power-ups list"))
        {
            if(!objInsertBefore.objectReferenceValue || !objInsertAfter.objectReferenceValue || !path.objectReferenceValue )
            {
				if (EditorUtility.DisplayDialog("Warning!!!",
				"Power-ups group is not correctly set up?", "Continue"))
				{
				}
			}
            else
            {
				if (EditorUtility.DisplayDialog("Power-ups Group",
								"Are you sure you want to delete and replace the current power-ups group?", "Yes", "No"))
				{
					CreateANewGroupOfPowerUps();
				}
			}     
		}

		if (moreOptions.boolValue && GUILayout.Button("Create Gizmo Path"))
		{
			FindPointsList();
		}
	}

    void DisplayThePowerUpsList()
    {
		#region
		for(var i = 0;i< listPowerUpCreation.arraySize; i++)
        {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("^", GUILayout.Width(20)))
			{
				listPowerUpCreation.MoveArrayElement(i, i - 1);
				break;
			}
			if (GUILayout.Button("v", GUILayout.Width(20)))
			{
				listPowerUpCreation.MoveArrayElement(i, i + 1);
				break;
			}
			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				if (listPowerUpCreation.arraySize > 1)
				{
					listPowerUpCreation.DeleteArrayElementAtIndex(i);
				}

				break;
			}

			if (GUILayout.Button("Replace", GUILayout.Width(70)))
			{
				listPowerUpCreation.GetArrayElementAtIndex(i).intValue = currentPowerUp.intValue;
				break;
			}

			EditorGUILayout.LabelField(i + ": " +  listPowerUpsName[listPowerUpCreation.GetArrayElementAtIndex(i).intValue]);



			EditorGUILayout.EndHorizontal();
		}
		#endregion
	}

    void CreateANewGroupOfPowerUps()
    {
		GrpPowerUp myScript = (GrpPowerUp)target;

		Transform[] existingPowerUps = myScript.gameObject.GetComponentsInChildren<Transform>(true);

		List<Vector3> tmpPosition = new List<Vector3>();
	    List<Quaternion> tmpRotation = new List<Quaternion>();

		foreach (Transform obj in existingPowerUps)
        {
            if(obj && obj.GetComponent<PowerUpsItems>())
            {
				tmpPosition.Add(obj.position);
				tmpRotation.Add(obj.rotation);
				Undo.DestroyObjectImmediate(obj.gameObject);
			}
		}
		//Debug.Log("existingPowerUps: " + existingPowerUps.Length);

		int counter = 0;
		int newXPos = 14;

		float distance = Vector3.Distance(myScript.objInsertAfter.position, myScript.objInsertBefore.position);
		Vector3 dir = (myScript.objInsertBefore.position - myScript.objInsertAfter.position).normalized;

		Vector3 refPos = myScript.transform.position;

		myScript.transform.position = myScript.objInsertAfter.position;
		myScript.transform.rotation = Quaternion.LookRotation(dir);

		myScript.transform.GetChild(0).position = myScript.objInsertAfter.position;
		myScript.transform.GetChild(0).rotation = Quaternion.LookRotation(dir);

		SerializedProperty listLookAtPowerUp = serializedObject.FindProperty("listLookAtPowerUp");

		listLookAtPowerUp.ClearArray();

		for (var i = 0;i< listPowerUpCreation.arraySize; i++)
        {
			GameObject newObj = null;
			if(listPowerUpCreation.GetArrayElementAtIndex(i).intValue != listPowerUpsName.Count - 1)
            {
				newObj = (GameObject)PrefabUtility.InstantiatePrefab(
				listPowerUps.GetArrayElementAtIndex(listPowerUpCreation.GetArrayElementAtIndex(i).intValue).FindPropertyRelative("powerUpPrefab").objectReferenceValue,
				myScript.transform);
            }
			// Random Power_up case
            else
            {
				newObj = (GameObject)PrefabUtility.InstantiatePrefab(
				m_PURandomPrefab.objectReferenceValue,
				myScript.transform);

				newObj.GetComponent<PowerUpsItems>().randomPU = true;
				newObj.GetComponent<PowerUpsItems>().PowerType = 1000;


				List<int> tmpList = new List<int>();
				for (var j = 0; j < m_PURandomSeq.arraySize; j++)
				{
					tmpList.Add(m_PURandomSeq.GetArrayElementAtIndex(j).intValue);
				}


				newObj.GetComponent<PowerUpsItems>().randomPUList = tmpList;
			}

			listLookAtPowerUp.InsertArrayElementAtIndex(0);


			if (tmpPosition.Count > i && myScript.transform.position == refPos)
            {
				newObj.transform.rotation = tmpRotation[i];
				newObj.transform.position = tmpPosition[i];

				listLookAtPowerUp.GetArrayElementAtIndex(0).FindPropertyRelative("powerUpToLookAT").objectReferenceValue = newObj.transform;
			}
            else
            {
				newObj.transform.rotation = Quaternion.LookRotation(dir);
				if (i == 0)
					newObj.transform.localPosition = new Vector3(0, 0, 0);
				else if (counter == 0)
					newObj.transform.localPosition = new Vector3(-newXPos, 0, 0);
				else if (counter == 1)
					newObj.transform.localPosition = new Vector3(newXPos, 0, 0);


				newObj.transform.Translate(Vector3.forward * distance * .5f);
				
				newObj.transform.localEulerAngles = new Vector3(-myScript.transform.localEulerAngles.x, 0, 0);

				listLookAtPowerUp.GetArrayElementAtIndex(0).FindPropertyRelative("powerUpToLookAT").objectReferenceValue = newObj.transform;
			}

			newObj.GetComponent<PowerUpShowPath>().grpPowerUp = myScript;
			newObj.GetComponent<PowerUpShowPath>().path = myScript.path;

			listLookAtPowerUp.GetArrayElementAtIndex(0).FindPropertyRelative("powerUpToLookAT").objectReferenceValue = newObj.transform;
			listLookAtPowerUp.GetArrayElementAtIndex(0).FindPropertyRelative("b_PowerUpAvailable").boolValue = true;


			int endArray = Mathf.Clamp(listLookAtPowerUp.arraySize, 0, listLookAtPowerUp.arraySize-1);
			listLookAtPowerUp.MoveArrayElement(0, endArray);

			if (i != 0) {
				counter++;
				counter %= 2;
				if (counter == 0) newXPos += 14;
			}

			Undo.RegisterCreatedObjectUndo(newObj, newObj.name);
		}

		FindPointsList();

	}

    void FindPointsList()
    {
		GrpPowerUp myScript = (GrpPowerUp)target;
		// Trouver si le 1er point c'est un point sur sub path ou path principal
		Transform objRef = (Transform)objInsertAfter.objectReferenceValue;

		bool bMainPath = true;
		if (objRef.parent.GetComponent<AltPath>())
        {
			Debug.Log("Alt Path");
			bMainPath = false;

		}
        else if(objRef.parent.GetComponent<Path>())
        {
			Debug.Log("Main Path");
			bMainPath = true;
		}

		// Main Path
		SerializedProperty listLookAtPowerUp = serializedObject.FindProperty("listLookAtPowerUp");
		if (bMainPath)
        {
			for (var j = 0; j < listLookAtPowerUp.arraySize; j++)
            {
				SerializedProperty powerUpToLookAT = listLookAtPowerUp.GetArrayElementAtIndex(j).FindPropertyRelative("powerUpToLookAT");
				//string sList = "Null";
				if (powerUpToLookAT.objectReferenceValue)
                {
					List<Transform> checkpointsRefTmp = new List<Transform>(objRef.parent.GetComponent<Path>().checkpoints);

					//Debug.Log("checkpointsRefTmp: " + checkpointsRefTmp.Count);

					for (var i = 0; i < checkpointsRefTmp.Count; i++)
					{
						if (checkpointsRefTmp[i] == objRef)
						{
							checkpointsRefTmp.Insert(i+1, powerUpToLookAT.objectReferenceValue as Transform);
							break;
						}
					}
					
					SerializedProperty checkpointsRef = listLookAtPowerUp.GetArrayElementAtIndex(j).FindPropertyRelative("checkpointsRef");

					for (var i = 0; i < checkpointsRefTmp.Count; i++)
					{
						checkpointsRef.InsertArrayElementAtIndex(0);
						checkpointsRef.GetArrayElementAtIndex(0).objectReferenceValue = checkpointsRefTmp[i];

						int endArray = Mathf.Clamp(checkpointsRef.arraySize, 0, checkpointsRef.arraySize - 1);
						checkpointsRef.MoveArrayElement(0, endArray);
					}
				}
				//Debug.Log(sList);
			}
				
		}

		// Alt Path
		if (!bMainPath)
		{
			for (var j = 0; j < listLookAtPowerUp.arraySize; j++)
			{
				SerializedProperty powerUpToLookAT = listLookAtPowerUp.GetArrayElementAtIndex(j).FindPropertyRelative("powerUpToLookAT");
				if (powerUpToLookAT.objectReferenceValue)
				{
					//sList = "";
					List<Transform> checkpointsRefTmp = new List<Transform>(myScript.path.checkpoints);


					List<Transform> checkpointsAltPathRefTmp = new List<Transform>(objRef.parent.GetComponent<AltPath>().tmpCheckpoints);

					for (var i = 0; i < checkpointsRefTmp.Count; i++)
					{
						if (checkpointsRefTmp[i] == objRef.parent.GetComponent<AltPath>().checkpointStart)
						{
							for (var k = 0; k < checkpointsAltPathRefTmp.Count; k++)
                            {
								checkpointsRefTmp.Insert(i+1+k, checkpointsAltPathRefTmp[k]);
							}
								
							break;
						}
					}


					for (var i = 0; i < checkpointsRefTmp.Count; i++)
					{
						if (checkpointsRefTmp[i] == objRef)
						{
							checkpointsRefTmp.Insert(i+1, powerUpToLookAT.objectReferenceValue as Transform);
							break;
						}
					}

					SerializedProperty checkpointsRef = listLookAtPowerUp.GetArrayElementAtIndex(j).FindPropertyRelative("checkpointsRef");

					for (var i = 0; i < checkpointsRefTmp.Count; i++)
					{
						checkpointsRef.InsertArrayElementAtIndex(0);
						checkpointsRef.GetArrayElementAtIndex(0).objectReferenceValue = checkpointsRefTmp[i];

						int endArray = Mathf.Clamp(checkpointsRef.arraySize, 0, checkpointsRef.arraySize - 1);
						checkpointsRef.MoveArrayElement(0, endArray);
					}
				}
				//Debug.Log(sList);
			}
		}
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