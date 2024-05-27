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

[CustomEditor(typeof(VehiclePrefabPause))]
public class VehiclePrefabPauseEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;

    SerializedProperty PauseMethodsList;
    SerializedProperty UnPauseMethodsList;


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

        PauseMethodsList = serializedObject.FindProperty("PauseMethodsList");
        UnPauseMethodsList = serializedObject.FindProperty("UnPauseMethodsList");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        //VehiclePrefabPause myScript = (VehiclePrefabPause)target;


       

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


        displayPauseMethodsList(listGUIStyle[0], listGUIStyle[1]);
        displayUnPauseMethodsList(listGUIStyle[0], listGUIStyle[1]);
       
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.LabelField("");
        #endregion
    }


    private void displayPauseMethodsList(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        
        VehiclePrefabPause myScript = (VehiclePrefabPause)target;

        methodModule.displayMethodList("Pause Vehicle",
                                           editorMethods,
                                           PauseMethodsList,
                                           myScript.PauseMethodsList,
                                           style_Blue,
                                           style_Yellow_01,
                                           "The methods are called in the same order as the list. " +
                                           "\nAll methods must be boolean methods. " +
                                           "\nOther methods will be ignored.");
        #endregion
    }

    private void displayUnPauseMethodsList(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback

        VehiclePrefabPause myScript = (VehiclePrefabPause)target;

        methodModule.displayMethodList("UnPause Vehicle",
                                           editorMethods,
                                           UnPauseMethodsList,
                                           myScript.UnPauseMethodsList,
                                           style_Blue,
                                           style_Yellow_01,
                                           "The methods are called in the same order as the list. " +
                                           "\nAll methods must be boolean methods. " +
                                           "\nOther methods will be ignored.");
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
