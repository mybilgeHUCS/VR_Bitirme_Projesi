//Description : InfoPlayerTSEditor: InfoPlayerTS Custom editor 
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(InfoPlayerTS))]
public class InfoPlayerTSEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty listCheckStates;
    SerializedProperty moreOptions;
    SerializedProperty helpBox;
    SerializedProperty currentCheckStateDisplayed;
    SerializedProperty NewCheckStateNameEditor;
    SerializedProperty editNameEditor;
    
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

    private List<Texture2D>     listTex = new List<Texture2D>();
    public List<GUIStyle>       listGUIStyle = new List<GUIStyle>();
    private List<Color>         listColor = new List<Color>();
    #endregion


    List<string>                listEditor = new List<string>();

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
        SeeInspector                = serializedObject.FindProperty("SeeInspector");
        moreOptions                 = serializedObject.FindProperty("moreOptions");
        helpBox                     = serializedObject.FindProperty("helpBox");
        listCheckStates             = serializedObject.FindProperty("listCheckStates");
        NewCheckStateNameEditor     = serializedObject.FindProperty("NewCheckStateNameEditor");
        currentCheckStateDisplayed  = serializedObject.FindProperty("currentCheckStateDisplayed");
        editNameEditor              = serializedObject.FindProperty("editNameEditor");
        #endregion

        updateListCheckState();
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

        //-> Section display selection
        displayCheckState();

        EditorGUILayout.LabelField("");

        //-> Section Create New CheckState
        CreateNewCheckState();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    public void displayCheckState()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        for (var i = 0;i< listCheckStates.arraySize; i++)
        {
            if(currentCheckStateDisplayed.intValue == i)
            {
                SerializedProperty s_Name = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("s_Name");
                SerializedProperty b_Use_IsSentenceInProcess = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_Use_IsSentenceInProcess");
                SerializedProperty b_IsSentenceInProcess = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_IsSentenceInProcess");

                SerializedProperty b_Use_IsPageInProcess = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_Use_IsPageInProcess");
                SerializedProperty b_IsPageCustomPartInProcess = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_IsPageCustomPartInProcess");

                SerializedProperty b_Use_IsAvailableToDoSomething = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_Use_IsAvailableToDoSomething");
                SerializedProperty b_IsAvailableToDoSomething = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_IsAvailableToDoSomething");

                SerializedProperty b_Use_ProcessToDisplayNewPageInProgress = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_Use_ProcessToDisplayNewPageInProgress");
                SerializedProperty b_ProcessToDisplayNewPageInProgress = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("b_ProcessToDisplayNewPageInProgress");

                EditorGUILayout.BeginHorizontal();
                currentCheckStateDisplayed.intValue = EditorGUILayout.Popup(currentCheckStateDisplayed.intValue, listEditor.ToArray());
                EditorGUILayout.LabelField("Edit Name: ", GUILayout.Width(65));
                EditorGUILayout.PropertyField(editNameEditor, new GUIContent(""), GUILayout.Width(20));

                if (moreOptions.boolValue)
                {
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        listCheckStates.DeleteArrayElementAtIndex(i);
                        currentCheckStateDisplayed.intValue = 0;
                        updateListCheckState();
                        break;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (editNameEditor.boolValue)
                    EditorGUILayout.PropertyField(s_Name, new GUIContent(""));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("|Boolean: ",EditorStyles.boldLabel, GUILayout.Width(200));
                EditorGUILayout.LabelField("|Use: ", EditorStyles.boldLabel, GUILayout.Width(50));
                EditorGUILayout.LabelField("|State: ", EditorStyles.boldLabel, GUILayout.Width(50));
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IsSentenceInProcess: ", GUILayout.Width(200));
                EditorGUILayout.PropertyField(b_Use_IsSentenceInProcess, new GUIContent(""), GUILayout.Width(50));
                if (b_Use_IsSentenceInProcess.boolValue)
                {
                    if (b_IsSentenceInProcess.boolValue && GUILayout.Button("True")) { b_IsSentenceInProcess.boolValue = !b_IsSentenceInProcess.boolValue; }
                    else if (!b_IsSentenceInProcess.boolValue && GUILayout.Button("False", GUILayout.MinWidth(50))) { b_IsSentenceInProcess.boolValue = !b_IsSentenceInProcess.boolValue; }
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("DisplayNewPageInProgress: ", GUILayout.Width(200));
                EditorGUILayout.PropertyField(b_Use_ProcessToDisplayNewPageInProgress, new GUIContent(""), GUILayout.Width(50));
                if (b_Use_ProcessToDisplayNewPageInProgress.boolValue)
                {
                    if (b_ProcessToDisplayNewPageInProgress.boolValue && GUILayout.Button("True")) { b_ProcessToDisplayNewPageInProgress.boolValue = !b_ProcessToDisplayNewPageInProgress.boolValue; }
                    else if (!b_ProcessToDisplayNewPageInProgress.boolValue && GUILayout.Button("False", GUILayout.MinWidth(50))) { b_ProcessToDisplayNewPageInProgress.boolValue = !b_ProcessToDisplayNewPageInProgress.boolValue; }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("PageCustomPartInProcess: ", GUILayout.Width(200));
                EditorGUILayout.PropertyField(b_Use_IsPageInProcess, new GUIContent(""), GUILayout.Width(50));
                if (b_Use_IsPageInProcess.boolValue)
                {
                    if (b_IsPageCustomPartInProcess.boolValue && GUILayout.Button("True")) { b_IsPageCustomPartInProcess.boolValue = !b_IsPageCustomPartInProcess.boolValue; }
                    else if (!b_IsPageCustomPartInProcess.boolValue && GUILayout.Button("False", GUILayout.MinWidth(50))) { b_IsPageCustomPartInProcess.boolValue = !b_IsPageCustomPartInProcess.boolValue; }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IsAvailableToDoSomething: ", GUILayout.Width(200));
                EditorGUILayout.PropertyField(b_Use_IsAvailableToDoSomething, new GUIContent(""), GUILayout.Width(50));
                if (b_Use_IsAvailableToDoSomething.boolValue)
                {
                    if (b_IsAvailableToDoSomething.boolValue && GUILayout.Button("True")) { b_IsAvailableToDoSomething.boolValue = !b_IsAvailableToDoSomething.boolValue; }
                    else if (!b_IsAvailableToDoSomething.boolValue && GUILayout.Button("False", GUILayout.MinWidth(50))) { b_IsAvailableToDoSomething.boolValue = !b_IsAvailableToDoSomething.boolValue; }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }


    public void updateListCheckState()
    {
        listEditor.Clear();
        for (var i = 0; i < listCheckStates.arraySize; i++)
        {
            listEditor.Add(listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("s_Name").stringValue);
        }
    }

    public void CreateNewCheckState()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Page", GUILayout.Width(120)))
        {
            //NewCheckStateNameEditor
            bool b_NameAlreadyExist = false;
            for (var i = 0; i < listCheckStates.arraySize; i++)
            {
                SerializedProperty s_Name = listCheckStates.GetArrayElementAtIndex(i).FindPropertyRelative("s_Name");
                if(NewCheckStateNameEditor.stringValue == s_Name.stringValue)
                {
                    b_NameAlreadyExist = true;
                    break;
                }
            }

            if (b_NameAlreadyExist && EditorUtility.DisplayDialog("This name is already used",
                     "",
                 "Continue"))
            {
               
            }
            else
            {
                listCheckStates.InsertArrayElementAtIndex(0);
                SerializedProperty b_Use_IsSentenceInProcess = listCheckStates.GetArrayElementAtIndex(0).FindPropertyRelative("b_Use_IsSentenceInProcess");
                SerializedProperty b_IsSentenceInProcess = listCheckStates.GetArrayElementAtIndex(0).FindPropertyRelative("b_IsSentenceInProcess");
                SerializedProperty b_Use_IsAvailableToDoSomething = listCheckStates.GetArrayElementAtIndex(0).FindPropertyRelative("b_Use_IsAvailableToDoSomething");
                SerializedProperty b_IsAvailableToDoSomething = listCheckStates.GetArrayElementAtIndex(0).FindPropertyRelative("b_IsAvailableToDoSomething");
                SerializedProperty s_Name = listCheckStates.GetArrayElementAtIndex(0).FindPropertyRelative("s_Name");

                s_Name.stringValue = NewCheckStateNameEditor.stringValue;
                b_Use_IsSentenceInProcess.boolValue = false;
                b_IsSentenceInProcess.boolValue = false;
                b_Use_IsAvailableToDoSomething.boolValue = true;
                b_IsAvailableToDoSomething.boolValue = false;

                listCheckStates.MoveArrayElement(0, listCheckStates.arraySize - 1);
                currentCheckStateDisplayed.intValue = listCheckStates.arraySize - 1;

                updateListCheckState();
            }
        }
        EditorGUILayout.PropertyField(NewCheckStateNameEditor, new GUIContent(""), GUILayout.MinWidth(100));
        EditorGUILayout.EndHorizontal();
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
