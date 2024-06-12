//Description :
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

[CustomEditor(typeof(VehiclePrefabInit))]
public class VehiclePrefabInitEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;

    SerializedProperty initVehiclePrefabList;
    SerializedProperty currentSelectedList;
    SerializedProperty currentDisplayedList;

    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

    public List<string> StepsListName = new List<string>();

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
            listTex.Add(MakeTex(2, 2, inspectorColor.ReturnColor(i)));
            listGUIStyle.Add(new GUIStyle());
            listGUIStyle[i] = new GUIStyle(); listGUIStyle[i].normal.background = listTex[i];
        }
        #endregion

        #region
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        helpBox = serializedObject.FindProperty("helpBox");
        initVehiclePrefabList = serializedObject.FindProperty("initVehiclePrefabList");

        currentSelectedList = serializedObject.FindProperty("currentSelectedList");
        currentDisplayedList = serializedObject.FindProperty("currentDisplayedList");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        //VehiclePrefabInit myScript = (VehiclePrefabInit)target;


       

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
        EditorGUILayout.EndHorizontal();

        if(helpBox.boolValue) HelpZone_01();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Sequence:", GUILayout.Width(120));
        EditorGUILayout.PropertyField(currentSelectedList, new GUIContent(""), GUILayout.Width(30));
        SerializedProperty m_SceneStepsListName = initVehiclePrefabList.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("name");
        EditorGUILayout.LabelField("(" + m_SceneStepsListName.stringValue + ")");

        EditorGUILayout.EndHorizontal();

          StepsListName.Clear();
          for (var i = 0; i < initVehiclePrefabList.arraySize; i++)
              StepsListName.Add(initVehiclePrefabList.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID: " + currentDisplayedList.intValue, GUILayout.Width(30));
        currentDisplayedList.intValue = EditorGUILayout.Popup(currentDisplayedList.intValue, StepsListName.ToArray()); ;
        EditorGUILayout.EndHorizontal();


        displayMultiMethodsLists(listGUIStyle[0], listGUIStyle[1]);

        EditorGUILayout.LabelField("");

        if (GUILayout.Button("Create a list of methods"))
        {
            initVehiclePrefabList.InsertArrayElementAtIndex(0);
            initVehiclePrefabList.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = "New: " + initVehiclePrefabList.arraySize.ToString();
            initVehiclePrefabList.MoveArrayElement(0, initVehiclePrefabList.arraySize-1);
            currentDisplayedList.intValue = initVehiclePrefabList.arraySize - 1;

        }

        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.LabelField("");
        #endregion
    }


    //--> display multiple list of methods call when the scene starts
    private void displayMultiMethodsLists(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        
        VehiclePrefabInit myScript = (VehiclePrefabInit)target;

        for(var i = 0; i < initVehiclePrefabList.arraySize; i++)
        {
            if (currentDisplayedList.intValue == i)
            {
                EditorGUILayout.BeginHorizontal();
                methodModule.displayMethodList("List " + i + " of methods is called just before loading the new scene:",
                                            editorMethods,
                                            initVehiclePrefabList.GetArrayElementAtIndex(i).FindPropertyRelative("methodsList"),
                                            myScript.initVehiclePrefabList[i].methodsList,
                                            style_Blue,
                                            style_Yellow_01,
                                            "The methods are called in the same order as the list. " +
                                            "\nAll methods must be boolean methods. " +
                                            "\nOther methods will be ignored.");

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    initVehiclePrefabList.DeleteArrayElementAtIndex(i);

                }
                EditorGUILayout.EndHorizontal();
            }
               
        }
        #endregion
    }



    

    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
           "", MessageType.Info);
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
