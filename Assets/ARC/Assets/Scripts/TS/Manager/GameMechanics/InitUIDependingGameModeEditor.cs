//Description: InitUIDependingGameModeEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;
 
[CustomEditor(typeof(InitUIDependingGameMode))]
public class InitUIDependingGameModeEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty listInitGameMode;
    SerializedProperty currentModeSelected;
    SerializedProperty newModeName;
    SerializedProperty currentTypeToAddToTheList;

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

    public List<String> GameModeNames = new List<string>();

    public string[] arrTypeName = new string[] { "RectTransform", "Camera" };

    void OnEnable()
    {
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

        listInitGameMode = serializedObject.FindProperty("listInitGameMode");
        currentModeSelected = serializedObject.FindProperty("currentModeSelected");
        newModeName = serializedObject.FindProperty("newModeName");
        currentTypeToAddToTheList = serializedObject.FindProperty("currentTypeToAddToTheList");

        GameModeNames = GenerateGameModeNameList();
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

        if (helpBox.boolValue) HelpZone(0);
        else EditorGUILayout.LabelField("");

        GameModeSelection();
        EditorGUILayout.LabelField("");

        InitGameModeUI(currentModeSelected.intValue);

        CreateNewStep(currentModeSelected.intValue);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    List<string> GenerateGameModeNameList()
    {
        List<string> newList = new List<string>();
        GameModeNames.Clear();

        for (var i = 0;i< listInitGameMode.arraySize; i++)
        {
            SerializedProperty m_name = listInitGameMode.GetArrayElementAtIndex(i).FindPropertyRelative("name");
            newList.Add(m_name.stringValue);
        }
        return newList;
    } 

    void GameModeSelection()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select a mode: ", GUILayout.Width(100));
        currentModeSelected.intValue = EditorGUILayout.Popup(currentModeSelected.intValue, GameModeNames.ToArray());				// --> Display all methods

        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            if (listInitGameMode.arraySize > 1)
                listInitGameMode.DeleteArrayElementAtIndex(currentModeSelected.intValue);

            currentModeSelected.intValue = 0;
            GameModeNames = GenerateGameModeNameList();
        }
        if (GUILayout.Button("Update", GUILayout.Width(60)))
        {
            DisplaySelectedModeOnScreen(currentModeSelected.intValue);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Create new mode -> Name: ", GUILayout.Width(160));
        EditorGUILayout.PropertyField(newModeName, new GUIContent(""), GUILayout.Width(100));
        if (GUILayout.Button("Create", GUILayout.Width(60)))
        {
            bool b_NameAlredayExist = false;

            for (var i = 0; i < GameModeNames.Count; i++)
            {
                if (newModeName.stringValue == GameModeNames[i])
                {
                    b_NameAlredayExist = true;
                    break;
                }
            }

            if (!b_NameAlredayExist)
            {
                listInitGameMode.InsertArrayElementAtIndex(0);
                listInitGameMode.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = newModeName.stringValue;

                SerializedProperty steps = listInitGameMode.GetArrayElementAtIndex(0).FindPropertyRelative("steps");
                SerializedProperty listRectTransform = listInitGameMode.GetArrayElementAtIndex(0).FindPropertyRelative("listRectTransform");
                steps.ClearArray();
                listRectTransform.ClearArray();

                listInitGameMode.MoveArrayElement(0, listInitGameMode.arraySize - 1);
                currentModeSelected.intValue = listInitGameMode.arraySize - 1;
                GameModeNames = GenerateGameModeNameList();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Name already exist",
                "Each Mode must have a unique name.", "Continue", ""))
                {
                }
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    void CreateNewStep(int whichMode)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add new step"))
        {
            SerializedProperty steps = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps");
            SerializedProperty listRectTransform = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listRectTransform");
            SerializedProperty listCamera = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listCamera");


            steps.InsertArrayElementAtIndex(0);

            switch (currentTypeToAddToTheList.intValue)
            {
                // RectTransform
                case 0:
                    steps.GetArrayElementAtIndex(0).FindPropertyRelative("type").intValue = currentTypeToAddToTheList.intValue;
                    steps.GetArrayElementAtIndex(0).FindPropertyRelative("idInTheList").intValue = listRectTransform.arraySize;

                    steps.MoveArrayElement(0, steps.arraySize - 1);

                    listRectTransform.InsertArrayElementAtIndex(0);
                    listRectTransform.GetArrayElementAtIndex(0).FindPropertyRelative("description").stringValue = "";
                    listRectTransform.GetArrayElementAtIndex(0).FindPropertyRelative("rect").objectReferenceValue = null;
                    listRectTransform.MoveArrayElement(0, listRectTransform.arraySize - 1);
                    break;
                // Camera
                case 1:
                    steps.GetArrayElementAtIndex(0).FindPropertyRelative("type").intValue = currentTypeToAddToTheList.intValue;
                    steps.GetArrayElementAtIndex(0).FindPropertyRelative("idInTheList").intValue = listCamera.arraySize;

                    steps.MoveArrayElement(0, steps.arraySize - 1);

                    listCamera.InsertArrayElementAtIndex(0);
                    listCamera.GetArrayElementAtIndex(0).FindPropertyRelative("description").stringValue = "";
                    listCamera.GetArrayElementAtIndex(0).FindPropertyRelative("cam").objectReferenceValue = null;
                    listCamera.MoveArrayElement(0, listCamera.arraySize - 1);
                    break;
            } 
        }

        currentTypeToAddToTheList.intValue = EditorGUILayout.Popup(currentTypeToAddToTheList.intValue, arrTypeName);
        EditorGUILayout.EndHorizontal();
    }

    void InitGameModeUI(int whichMode)
    {
        SerializedProperty steps = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps");

        SerializedProperty m_name = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(m_name, new GUIContent(""));

        for (var i = 0; i < steps.arraySize; i++)
        {
            SerializedProperty type = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps").GetArrayElementAtIndex(i).FindPropertyRelative("type");
            SerializedProperty idInTheList = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps").GetArrayElementAtIndex(i).FindPropertyRelative("idInTheList");
           
            switch (type.intValue)
            {
                #region RectTransform
                case 0:
                    SerializedProperty listRectTransform = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listRectTransform");
                    SerializedProperty b_ShowInEditor = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listRectTransform").GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_ShowInEditor");


                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("rect").objectReferenceValue = null;
                        steps.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.LabelField("Step " + i +": ",EditorStyles.boldLabel, GUILayout.Width(80));
                    SerializedProperty description = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("description");
                    EditorGUILayout.PropertyField(description, new GUIContent(""));
                    if (GUILayout.Button("", GUILayout.Width(20)))
                    {
                        b_ShowInEditor.boolValue = !b_ShowInEditor.boolValue;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();


                    if (b_ShowInEditor.boolValue)
                    {
                        //-> Reference to Rect Transform
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Rect Transform:", GUILayout.Width(100));
                        SerializedProperty refRectTransform = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("rect");
                        EditorGUILayout.PropertyField(refRectTransform, new GUIContent(""));
                        EditorGUILayout.EndHorizontal();

                        //-> Enable or disable the object
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Enable:", GUILayout.Width(100));
                        SerializedProperty Enabled = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Enabled");
                        EditorGUILayout.PropertyField(Enabled, new GUIContent(""), GUILayout.Width(30));
                        EditorGUILayout.EndHorizontal();

                        //-> Change Rectransform Scale
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Change Scale:", GUILayout.Width(100));
                        SerializedProperty b_Scale = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Scale");
                        EditorGUILayout.PropertyField(b_Scale, new GUIContent(""), GUILayout.Width(30));
                        if (b_Scale.boolValue)
                        {
                            SerializedProperty newScale = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newScale");
                            EditorGUILayout.PropertyField(newScale, new GUIContent(""), GUILayout.Width(130));
                        }
                        EditorGUILayout.EndHorizontal();

                        //-> Change Rectransform Anchors
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Change Anchors:", GUILayout.Width(100));
                        SerializedProperty b_Anchors = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Anchors");
                        EditorGUILayout.PropertyField(b_Anchors, new GUIContent(""), GUILayout.Width(30));
                        if (b_Anchors.boolValue)
                        {
                            SerializedProperty newAnchorMin = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newAnchorMin");
                            EditorGUILayout.LabelField("Min:", GUILayout.Width(30));
                            EditorGUILayout.PropertyField(newAnchorMin, new GUIContent(""), GUILayout.Width(100));
                            SerializedProperty newAnchorMax = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newAnchorMax");
                            EditorGUILayout.LabelField("Max:", GUILayout.Width(30));
                            EditorGUILayout.PropertyField(newAnchorMax, new GUIContent(""), GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();


                        //-> Change Rectransform Pivot
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Change Pivot:", GUILayout.Width(100));
                        SerializedProperty b_Pivot = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Pivot");
                        EditorGUILayout.PropertyField(b_Pivot, new GUIContent(""), GUILayout.Width(30));
                        if (b_Pivot.boolValue)
                        {
                            SerializedProperty newPivot = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newPivot");
                            EditorGUILayout.PropertyField(newPivot, new GUIContent(""), GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    

                    break;
                #endregion

                #region Camera
                case 1:
                    // listCamera
                    // b_ShowInEditorCam
                    SerializedProperty listCamera = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listCamera");
                    SerializedProperty b_ShowInEditorCam = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listCamera").GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_ShowInEditor");

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("cam").objectReferenceValue = null;
                        steps.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.LabelField("Step " + i + ": ", EditorStyles.boldLabel, GUILayout.Width(80));
                    SerializedProperty descriptionCam = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("description");
                    EditorGUILayout.PropertyField(descriptionCam, new GUIContent(""));
                    if (GUILayout.Button("", GUILayout.Width(20)))
                    {
                        b_ShowInEditorCam.boolValue = !b_ShowInEditorCam.boolValue;
                        break;
                    }
                    EditorGUILayout.EndHorizontal();


                    if (b_ShowInEditorCam.boolValue)
                    {
                        //-> Reference to Rect Transform
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Camera:", GUILayout.Width(100));
                        SerializedProperty CamTransform = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("cam");
                        EditorGUILayout.PropertyField(CamTransform, new GUIContent(""));
                        EditorGUILayout.EndHorizontal();

                        //-> Enable or disable the object
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Enable:", GUILayout.Width(100));
                        SerializedProperty Enabled = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Enabled");
                        EditorGUILayout.PropertyField(Enabled, new GUIContent(""), GUILayout.Width(30));
                        EditorGUILayout.EndHorizontal();

                        //-> Change Rectransform Scale
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Viewport Rect:", GUILayout.Width(100));
                        SerializedProperty b_ViewportRect = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_ViewportRect");
                        EditorGUILayout.PropertyField(b_ViewportRect, new GUIContent(""), GUILayout.Width(30));
                        if (b_ViewportRect.boolValue)
                        {
                            SerializedProperty m_ViewportRectX = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectX");
                            SerializedProperty m_ViewportRectY = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectY");
                            SerializedProperty m_ViewportRectW = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectW");
                            SerializedProperty m_ViewportRectH = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectH");

                            EditorGUILayout.LabelField("X:", GUILayout.Width(20));
                            EditorGUILayout.PropertyField(m_ViewportRectX, new GUIContent(""), GUILayout.Width(30));
                            EditorGUILayout.LabelField("Y:", GUILayout.Width(20));
                            EditorGUILayout.PropertyField(m_ViewportRectY, new GUIContent(""), GUILayout.Width(30));
                            EditorGUILayout.LabelField("W:", GUILayout.Width(20));
                            EditorGUILayout.PropertyField(m_ViewportRectW, new GUIContent(""), GUILayout.Width(30));
                            EditorGUILayout.LabelField("H:", GUILayout.Width(20));
                            EditorGUILayout.PropertyField(m_ViewportRectH, new GUIContent(""), GUILayout.Width(30));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    break;
                    #endregion
            }
        }
    }


    void DisplaySelectedModeOnScreen(int whichMode)
    {
        SerializedProperty steps = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps");

        SerializedProperty m_name = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("name");
        EditorGUILayout.PropertyField(m_name, new GUIContent(""));
        InitUIDependingGameMode myScript = (InitUIDependingGameMode)target;

        for (var i = 0; i < steps.arraySize; i++)
        {
            SerializedProperty type = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps").GetArrayElementAtIndex(i).FindPropertyRelative("type");
            SerializedProperty idInTheList = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("steps").GetArrayElementAtIndex(i).FindPropertyRelative("idInTheList");

            switch (type.intValue)
            {
                #region RectTransform
                case 0:
                    SerializedProperty listRectTransform = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listRectTransform");
                    GameObject obj = myScript.listInitGameMode[whichMode].listRectTransform[idInTheList.intValue].rect.gameObject;

                    if (obj)
                    {
                        //-> Enable/Disable rectTransform
                        SerializedProperty Enabled = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Enabled");
                        SerializedObject serializedObject2 = new SerializedObject(obj);
                        serializedObject2.Update();
                        SerializedProperty m_IsActive = serializedObject2.FindProperty("m_IsActive");
                        m_IsActive.boolValue = Enabled.boolValue;
                        serializedObject2.ApplyModifiedProperties();

                        //-> Change Scale
                        SerializedProperty b_Scale = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Scale");
                        if (b_Scale.boolValue)
                        {
                            SerializedProperty newScale = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newScale");
                            SerializedObject serializedObject3 = new SerializedObject(obj.GetComponent<RectTransform>());
                            serializedObject3.Update();
                            SerializedProperty m_LocalScale = serializedObject3.FindProperty("m_LocalScale");
                            m_LocalScale.vector3Value = newScale.vector3Value;
                            serializedObject3.ApplyModifiedProperties();
                        }

                        //-> Change Anchor
                        SerializedProperty b_Anchors = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Anchors");
                        if (b_Anchors.boolValue)
                        {
                            SerializedProperty newAnchorMin = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newAnchorMin");
                            SerializedProperty newAnchorMax = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newAnchorMax");
                            SerializedObject serializedObject3 = new SerializedObject(obj.GetComponent<RectTransform>());
                            serializedObject3.Update();
                            SerializedProperty m_AnchorMin = serializedObject3.FindProperty("m_AnchorMin");
                            SerializedProperty m_AnchorMax = serializedObject3.FindProperty("m_AnchorMax");
                            m_AnchorMin.vector2Value = newAnchorMin.vector2Value;
                            m_AnchorMax.vector2Value = newAnchorMax.vector2Value;

                            serializedObject3.ApplyModifiedProperties();
                        }

                        //-> Change Pivot
                        SerializedProperty b_Pivot = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Pivot");
                        if (b_Pivot.boolValue)
                        {
                            SerializedProperty newPivot = listRectTransform.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("newPivot");
                            SerializedObject serializedObject3 = new SerializedObject(obj.GetComponent<RectTransform>());
                            serializedObject3.Update();
                            SerializedProperty m_Pivot = serializedObject3.FindProperty("m_Pivot");
                            m_Pivot.vector2Value = newPivot.vector2Value;

                            serializedObject3.ApplyModifiedProperties();
                        }
                    }
                    

                    break;
                #endregion

                #region Camera
                case 1:
                    SerializedProperty listCamera = listInitGameMode.GetArrayElementAtIndex(whichMode).FindPropertyRelative("listCamera");
                    Camera objCam = myScript.listInitGameMode[whichMode].listCamera[idInTheList.intValue].cam;

                    if (objCam)
                    { //-> Enable/Disable rectTransform
                        SerializedProperty EnabledCam = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_Enabled");
                        SerializedObject serializedObject2B = new SerializedObject(objCam.gameObject);
                        serializedObject2B.Update();
                        SerializedProperty m_IsActiveCam = serializedObject2B.FindProperty("m_IsActive");
                        m_IsActiveCam.boolValue = EnabledCam.boolValue;
                        serializedObject2B.ApplyModifiedProperties();


                        //-> Change Viewport rect
                        SerializedProperty b_ViewportRect = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("b_ViewportRect");
                        if (b_ViewportRect.boolValue)
                        {
                            SerializedProperty m_ViewportRectX = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectX");
                            SerializedProperty m_ViewportRectY = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectY");
                            SerializedProperty m_ViewportRectW = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectW");
                            SerializedProperty m_ViewportRectH = listCamera.GetArrayElementAtIndex(idInTheList.intValue).FindPropertyRelative("ViewportRectH");
                            SerializedObject serializedObject3 = new SerializedObject(objCam);
                            serializedObject3.Update();
                            SerializedProperty m_NormalizedViewPortRect = serializedObject3.FindProperty("m_NormalizedViewPortRect");
                            m_NormalizedViewPortRect.rectValue = new Rect(
                                m_ViewportRectX.floatValue,
                                m_ViewportRectY.floatValue,
                                m_ViewportRectW.floatValue,
                                m_ViewportRectH.floatValue);

                            serializedObject3.ApplyModifiedProperties();
                        }
                    }
                    
                    break;
                    #endregion
            }
        }
    }

    private void HelpZone(int value)
    {
        //CanvasInGameUIRef myScript = (CanvasInGameUIRef)target;
        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "Description: This script is used to display the needed P1 and P2 UI depending the selected game mode." + "\n" +
                "It allows to enable|disable rect Transform. Change Scale|Anchor|Pivot. Change the ViewportRect of a camera.", MessageType.Info);
                break;

            
        }

    }

    void OnSceneGUI()
    {
    }
}
#endif
