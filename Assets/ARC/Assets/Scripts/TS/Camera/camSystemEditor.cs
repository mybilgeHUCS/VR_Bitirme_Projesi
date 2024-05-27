//Description : Custom editor for camSystem.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(camSystem))]
public class camSystemEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
   
    SerializedProperty moreOptions;
    SerializedProperty helpBox;
    SerializedProperty camPresetList;
    SerializedProperty currentPresetEditor;
    SerializedProperty tabEditor;
    SerializedProperty presetName;
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



    void OnEnable()
    {
        #region Init Inspector Color
        listColor.Clear();
        listGUIStyle.Clear();
        for (var i = 0; i < inspectorColor.listColor.Length; i++)
        {
            listTex.Add(MakeTex(2, 2, inspectorColor.listColor[i]));
            listGUIStyle.Add(new GUIStyle());
            listGUIStyle[i] = new GUIStyle(); listGUIStyle[i].normal.background = listTex[i];
        }
        #endregion

        #region
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");
        camPresetList = serializedObject.FindProperty("camPresetList");
        currentPresetEditor = serializedObject.FindProperty("currentPresetEditor");
        tabEditor = serializedObject.FindProperty("tabEditor");
        presetName = serializedObject.FindProperty("presetName");
        #endregion
    }




    public override void OnInspectorGUI()
    {
        #region
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

        //if (helpBox.boolValue) HelpZone_01();

        EditorGUILayout.LabelField("");

        tabEditor.intValue = GUILayout.Toolbar(tabEditor.intValue, new string[] { "Edit Preset", "Create Preset" }, GUILayout.MinWidth(60));

        switch (tabEditor.intValue)
        {
            case 0:
                EditPreset();
                break;
            case 1:
                CreatePreset();
                break;
        }
       

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");

        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();
        #endregion
    }

    void CreatePreset()
    {
        EditorGUILayout.LabelField("");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name:", GUILayout.Width(50));
        EditorGUILayout.PropertyField(presetName, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create New Preset"))
        {
            bool bAlreadyExist = false;
            for(var i = 0;i< camPresetList.arraySize; i++)
            {
                if(camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == presetName.stringValue)
                {
                    bAlreadyExist = true;
                    break;
                }
            }


            if (!bAlreadyExist)
            {
                camPresetList.InsertArrayElementAtIndex(0);
                camPresetList.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = presetName.stringValue;
                int index = Mathf.Clamp(camPresetList.arraySize, 0, camPresetList.arraySize-1);
                camPresetList.MoveArrayElement(0,index);
                currentPresetEditor.intValue = index;
                tabEditor.intValue = 0;
            }
            else if (EditorUtility.DisplayDialog("This name is already used",
                  "",
              "Continue"))
            {
            }
        }
    }

    void EditPreset()
    {
        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        //currentPresetEditor
        List<string> nameList = new List<string>();
        for (var i = 0; i < camPresetList.arraySize; i++)
            nameList.Add(camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);

        currentPresetEditor.intValue = EditorGUILayout.Popup(currentPresetEditor.intValue, nameList.ToArray());


        

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview Camera Position"))
        {
            camSystem myScript = (camSystem)target;
            SerializedObject serializedObject2 = new SerializedObject(myScript.previewCam.gameObject);
            serializedObject2.Update();
            SerializedProperty m_IsActive = serializedObject2.FindProperty("m_IsActive");
            m_IsActive.boolValue = true;
            serializedObject2.ApplyModifiedProperties();

            Undo.RegisterFullObjectHierarchyUndo(myScript.previewCam.transform, myScript.previewCam.name);

            float dist = myScript.camPresetList[currentPresetEditor.intValue].distance + myScript.camPresetList[currentPresetEditor.intValue].accelerationDistance;


            Vector3 dir = myScript.camPresetList[currentPresetEditor.intValue].followTargetPos.transform.forward;

            if (!myScript.camPresetList[currentPresetEditor.intValue].b_IsCamRotationLocked)
            {
                // Preview Position
                myScript.previewCam.position = myScript.camPresetList[currentPresetEditor.intValue].followTargetPos.position - dir * (dist + 2);
                // Preview Look at
                myScript.previewCam.transform.LookAt(myScript.camPresetList[currentPresetEditor.intValue].targetLookAt);
            }
            // Fixed target
            else
            {
                // Preview Position
                myScript.previewCam.position = myScript.camPresetList[currentPresetEditor.intValue].followTargetPos.position - dir * dist;
                //Preview Rotation
                myScript.previewCam.rotation = myScript.camPresetList[currentPresetEditor.intValue].targetLookAt.rotation;
            }



           
        }

        if (GUILayout.Button("Close Preview"))
        {
            camSystem myScript = (camSystem)target;
            SerializedObject serializedObject2 = new SerializedObject(myScript.previewCam.gameObject);
            serializedObject2.Update();
            SerializedProperty m_IsActive = serializedObject2.FindProperty("m_IsActive");
            m_IsActive.boolValue = false;
            serializedObject2.ApplyModifiedProperties();
        }

        
        EditorGUILayout.EndHorizontal();

        for (var i = 0;i< camPresetList.arraySize; i++)
        {
            if(currentPresetEditor.intValue == i)
            {
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                SerializedProperty name = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                SerializedProperty targetLookAt = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("targetLookAt");
                SerializedProperty distance = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("distance");
                SerializedProperty speedToReachDefaultDistance = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("speedToReachDefaultDistance");
                SerializedProperty accelerationDistance = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("accelerationDistance");
                SerializedProperty namspeedToReachAccelerationDistancee = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("speedToReachAccelerationDistance");
                SerializedProperty followTargetPos = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("followTargetPos");
                SerializedProperty bSmoothTransition = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("bSmoothTransition");
                SerializedProperty b_IsCamRotationLocked = camPresetList.GetArrayElementAtIndex(i).FindPropertyRelative("b_IsCamRotationLocked");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(name, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cam Rotation Locked:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(b_IsCamRotationLocked, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Smooth Transition:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(bSmoothTransition, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Camera (Look At):", GUILayout.Width(200));
                EditorGUILayout.PropertyField(targetLookAt, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Camera (Target followed):", GUILayout.Width(200));
                EditorGUILayout.PropertyField(followTargetPos, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Break position:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(distance, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Acceleration pos:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(accelerationDistance, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Speed to reach break pos:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(speedToReachDefaultDistance, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
               
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Speed to reach Acceleration pos:", GUILayout.Width(200));
                EditorGUILayout.PropertyField(namspeedToReachAccelerationDistancee, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                
               
               
                EditorGUILayout.EndVertical();


            }

           
        }
        EditorGUILayout.EndVertical();
    }

    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "", MessageType.Info);
    }





    void OnSceneGUI()
    {
    }
}
#endif

