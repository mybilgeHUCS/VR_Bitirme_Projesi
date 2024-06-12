//Description: CountdownEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;


[CustomEditor(typeof(Countdown))]
public class CountdownEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;
    SerializedProperty multiListUnityEvents;
    SerializedProperty editorSelectedList;
    SerializedProperty editorNewCountdownName;


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

    public List<String> countdownNames = new List<string>();

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
        multiListUnityEvents = serializedObject.FindProperty("multiListUnityEvents");
        editorSelectedList = serializedObject.FindProperty("editorSelectedList");
        editorNewCountdownName = serializedObject.FindProperty("editorNewCountdownName");
        #endregion

        countdownNames = GenerateCountdownNameList();
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

        if (helpBox.boolValue) HelpZone_01();

        Selection(listGUIStyle[0], listGUIStyle[1]);
        
        DisplayCountdownList(listGUIStyle[0], listGUIStyle[1]);


        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    private void Selection(GUIStyle style_00, GUIStyle style_01)
    {
        EditorGUILayout.BeginVertical(style_00);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Choose a countdown: ", GUILayout.Width(130));
        editorSelectedList.intValue = EditorGUILayout.Popup(editorSelectedList.intValue, countdownNames.ToArray());				// --> Display all methods
        EditorGUILayout.EndHorizontal();

        if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
        {

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create new countdown"))
            {
                bool b_Allowed = true;
                for (var i = 0; i < multiListUnityEvents.arraySize; i++)
                {
                    SerializedProperty m_Name = multiListUnityEvents.GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
                    if (m_Name.stringValue == editorNewCountdownName.stringValue ||
                        editorNewCountdownName.stringValue == "")
                    {
                        b_Allowed = false;
                    }
                }


                if (b_Allowed)
                {
                    multiListUnityEvents.InsertArrayElementAtIndex(0);
                    multiListUnityEvents.GetArrayElementAtIndex(0).FindPropertyRelative("_Name").stringValue = editorNewCountdownName.stringValue;
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").ClearArray();
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").InsertArrayElementAtIndex(0);
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("StepDuration").floatValue = 1;

                    multiListUnityEvents.MoveArrayElement(0, multiListUnityEvents.arraySize - 1);
                    countdownNames = GenerateCountdownNameList();
                    editorSelectedList.intValue = multiListUnityEvents.arraySize - 1;
                }
                else
                {
                    if (EditorUtility.DisplayDialog("The name is already use.",
                     "Choose a unique name.", "Continue"))
                    {
                    }
                }
            }
            EditorGUILayout.PropertyField(editorNewCountdownName, new GUIContent(""));

            EditorGUILayout.EndHorizontal();

            SerializedProperty m_Name2 = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("_Name");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Edit countdown name: ", GUILayout.Width(150));
            EditorGUILayout.PropertyField(m_Name2, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
            EditorGUILayout.LabelField("");
    }

    //--> display a list of methods (Load Section)
    private void DisplayCountdownList(GUIStyle style_00, GUIStyle style_01)
    {
        #region
        for (var i = 0;i< multiListUnityEvents.arraySize; i++)
        {
            if(editorSelectedList.intValue == i)
            {
                SerializedProperty m_unityEventsList = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList");
                SerializedProperty m_StepDuration = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("StepDuration");

                EditorGUILayout.LabelField("");
                EditorGUILayout.BeginVertical(style_00);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Step Duration:", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_StepDuration, new GUIContent(""), GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField("");

                for (var j = 0; j < m_unityEventsList.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Step " + j + ":", EditorStyles.boldLabel, GUILayout.Width(50));
                    EditorGUILayout.PropertyField(m_unityEventsList.GetArrayElementAtIndex(j), new GUIContent(""));
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Create a new Step"))
                {
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").InsertArrayElementAtIndex(0);
                    int arraySize = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").arraySize;
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").MoveArrayElement(0, arraySize - 1);
                }
                if (GUILayout.Button("Remove last Step") && multiListUnityEvents.arraySize > 0)
                {
                    int arraySize = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").arraySize;
                    multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsList").DeleteArrayElementAtIndex(arraySize - 1);
                }

                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("Do something after the countdown:", EditorStyles.boldLabel);
                SerializedProperty m_unityEventsAfterCountdown = multiListUnityEvents.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("unityEventsAfterCountdown");
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(m_unityEventsAfterCountdown, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion
    }

    List<string> GenerateCountdownNameList()
    {
        List<string> newList = new List<string>();
        countdownNames.Clear();

        for (var i = 0; i < multiListUnityEvents.arraySize; i++)
        {
            SerializedProperty m_Name = multiListUnityEvents.GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
            newList.Add(m_Name.stringValue);
        }

        return newList;
    }


    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "Call the countdown: Countdown.instance.BCountdown(int CountdownID = 0)", MessageType.Info);
    }

    void OnSceneGUI()
    {
    }
}
#endif
