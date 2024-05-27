//Description: TransitionManagerEditor.cs. Use in association with TransitionManager.cs
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

[CustomEditor(typeof(TransitionManager))]
public class TransitionManagerEditor : Editor
{
    SerializedProperty          SeeInspector;                                            // use to draw default Inspector
    SerializedProperty          listMultiMethods;
    SerializedProperty          editorCurrentSelectedTransition;
    SerializedProperty          editorNewTransitionName;
    SerializedProperty          editorAllowsScriptedTransition;
    SerializedProperty          currentTab;
    SerializedProperty          moreOptions;
    SerializedProperty          helpBox;
    SerializedProperty          editName;

    public EditorMethods_Pc     editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc   methodModule;

    public string[]             listTransition = new string[2] {
            "Use Scripting",
            "Use Animation"};

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
        SeeInspector                    = serializedObject.FindProperty("SeeInspector");
        listMultiMethods                = serializedObject.FindProperty("listMultiMethods");
        editorCurrentSelectedTransition = serializedObject.FindProperty("editorCurrentSelectedTransition");
        editorAllowsScriptedTransition  = serializedObject.FindProperty("editorAllowsScriptedTransition");
        moreOptions                     = serializedObject.FindProperty("moreOptions");
        helpBox                         = serializedObject.FindProperty("helpBox");
        editName                        = serializedObject.FindProperty("editName");
        editorNewTransitionName         = serializedObject.FindProperty("editorNewTransitionName");
        currentTab = serializedObject.FindProperty("currentTab");
        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();
        #endregion
    }


    public override void OnInspectorGUI()
    {
        #region
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        //-> Custom editor visualisation parameters
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

        currentTab.intValue = GUILayout.SelectionGrid(currentTab.intValue, new string[] { "Edit", "Create" }, 2);

        switch (currentTab.intValue)
        {
            //-> Section that display parameters for each transition
            case 0:
                DisplayTransition();
                EditorGUILayout.LabelField("");
                break;
            //-> Section to create a new transition
            case 1:
                CreateNewTransition();
                EditorGUILayout.LabelField("");
                break;
        }

        serializedObject.ApplyModifiedProperties();
        #endregion
    }


    //--> Create a new transition
    void CreateNewTransition()
    {
        #region
        EditorGUILayout.LabelField("");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Transition Name:", GUILayout.Width(100));
        EditorGUILayout.PropertyField(editorNewTransitionName, new GUIContent(""), GUILayout.MinWidth(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Create New Transition", GUILayout.Height(30)))
        {
            bool b_NameAlredayExist = false;
        
            for (var i = 0; i < listMultiMethods.arraySize; i++)
            {
                if (editorNewTransitionName.stringValue == listMultiMethods.GetArrayElementAtIndex(i).FindPropertyRelative("_Name").stringValue)
                {
                    b_NameAlredayExist = true;
                    break;
                }
            }

            if (!b_NameAlredayExist)
            {
               listMultiMethods.InsertArrayElementAtIndex(0);
                listMultiMethods.GetArrayElementAtIndex(0).FindPropertyRelative("_Name").stringValue = editorNewTransitionName.stringValue;
                listMultiMethods.GetArrayElementAtIndex(0).FindPropertyRelative("methodsListPart1").GetArrayElementAtIndex(0).FindPropertyRelative("obj").objectReferenceValue = null;
                listMultiMethods.GetArrayElementAtIndex(0).FindPropertyRelative("methodsListPart2").GetArrayElementAtIndex(0).FindPropertyRelative("obj").objectReferenceValue = null;
                listMultiMethods.GetArrayElementAtIndex(0).FindPropertyRelative("whichTransitionType").intValue = 1;
                listMultiMethods.GetArrayElementAtIndex(0).FindPropertyRelative("bPauseAnimatorUntilNewPageIsDisplayed").boolValue = true;
                listMultiMethods.MoveArrayElement(0, listMultiMethods.arraySize - 1);
                editorCurrentSelectedTransition.intValue = listMultiMethods.arraySize - 1;
                currentTab.intValue = 0;
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Name already exist",
                "Each Transition must have a unique name.", "Continue", ""))
                {
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    //-> Section that display parameters for each transition
    void DisplayTransition()
    {
        #region
        EditorGUILayout.LabelField("");
        //-> Transition Section
        TransitionManager myScript = (TransitionManager)target;

        //-> Transition Popup
        List<string> listTransitionName = new List<string>();
        for (var i = 0; i < listMultiMethods.arraySize; i++)
            listTransitionName.Add(listMultiMethods.GetArrayElementAtIndex(i).FindPropertyRelative("_Name").stringValue);

        //-> Current Selected transition
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID: " + editorCurrentSelectedTransition.intValue + ":", GUILayout.Width(40));
        editorCurrentSelectedTransition.intValue = EditorGUILayout.Popup(editorCurrentSelectedTransition.intValue, listTransitionName.ToArray());
        EditorGUILayout.LabelField("Edit Name: ", GUILayout.Width(70));
        EditorGUILayout.PropertyField(editName, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();

        //-> Transition name
        if (editName.boolValue)
            EditorGUILayout.PropertyField(listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("_Name"), new GUIContent(""));

        //-> Which transition type (0: USe Scripting | 1: Use animation)
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
       
      
        SerializedProperty whichTransitionType = listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("whichTransitionType");

        if (editorAllowsScriptedTransition.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Transition Type:", GUILayout.Width(200));
            whichTransitionType.intValue = EditorGUILayout.Popup(whichTransitionType.intValue, listTransition.ToArray(), GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();
        }
          
        //-> 0: Use scripting
        if (whichTransitionType.intValue == 0) 
        {
            //-> Transition Part 1
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Part 1:", GUILayout.Width(50));
            editorMethods.DisplayMethodsOnEditor(
                myScript.listMultiMethods[editorCurrentSelectedTransition.intValue].methodsListPart1,
                listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("methodsListPart1"),
                listGUIStyle[0],
                true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            //-> Transition Part 2
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Part 2:", GUILayout.Width(50));
            editorMethods.DisplayMethodsOnEditor(
                myScript.listMultiMethods[editorCurrentSelectedTransition.intValue].methodsListPart2,
                listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("methodsListPart2"),
                listGUIStyle[0],
                true);
            EditorGUILayout.EndHorizontal();
        }
        //-> 1: Use Animation
        else
        {
            SerializedProperty _animator = listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("_animator");
            SerializedProperty _bPauseAnimatorUntilNewPageIsDisplayed = listMultiMethods.GetArrayElementAtIndex(editorCurrentSelectedTransition.intValue).FindPropertyRelative("bPauseAnimatorUntilNewPageIsDisplayed");
            // Display animation
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Animator component: ", GUILayout.Width(130));
            EditorGUILayout.PropertyField(_animator, new GUIContent(""), GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pause Animation: ", GUILayout.Width(130));
            EditorGUILayout.PropertyField(_bPauseAnimatorUntilNewPageIsDisplayed, new GUIContent(""), GUILayout.MinWidth(100));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
