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
using System.IO;

[CustomEditor(typeof(LoadSavePlayerProgession))]
public class LoadSavePlayerProgessionEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty methodsListSave;
    SerializedProperty methodsListLoad;


    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

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
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");
        methodsListSave = serializedObject.FindProperty("methodsListSave");
        methodsListLoad = serializedObject.FindProperty("methodsListLoad");

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

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Show .Dat In Explorer"))
        {
            ShowDataInExplorer();
        }
        if (GUILayout.Button("Delete Player Progression Datas"))
        {
            DeletePlayerProgressionDatas();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        //-> Section display methods
        SaveSection(listGUIStyle[0], listGUIStyle[1]);
        EditorGUILayout.LabelField("");
        LoadSection(listGUIStyle[0], listGUIStyle[1]);


        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    //--> display a list of methods (Save Section)
    private void SaveSection(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        LoadSavePlayerProgession myScript = (LoadSavePlayerProgession)target;

        methodModule.displayMethodList("Methods to save Player progression (string methods only):",
                                       editorMethods,
                                       methodsListSave,
                                       myScript.methodsListSave,
                                       style_Blue,
                                       style_Yellow_01,
                                       "The methods are called in the same order as the list. " +
                                       "\nAll methods must be string methods. " +
                                       "\nOther methods will be ignored.",
                                       moreOptions.boolValue);

        #endregion
    }

   //--> display a list of methods (Load Section)
    private void LoadSection(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        LoadSavePlayerProgession myScript = (LoadSavePlayerProgession)target;

        methodModule.displayMethodList("Methods to load Player progression (boolean methods only):",
                                       editorMethods,
                                       methodsListLoad,
                                       myScript.methodsListLoad,
                                       style_Blue,
                                       style_Yellow_01,
                                       "The methods are called in the same order as the list. " +
                                       "\nAll methods must be boolean methods. " +
                                       "\nOther methods will be ignored.",
                                       moreOptions.boolValue,
                                       false);

        #endregion
    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }

    public void DeletePlayerProgressionDatas()
    {
        #region
        LoadSavePlayerProgession myScript = (LoadSavePlayerProgession)target;
        
        string filePath = Application.persistentDataPath + "/PP_" + myScript.currentSelectedSlot +".dat";
        if (EditorUtility.DisplayDialog("Delete Player Progression", "Are you sure?", "Yes", "No"))
        {
            //-> Delete .Dat
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                UnityEditor.AssetDatabase.Refresh();
            }

            //-> Delete Player Prefs
            if (PlayerPrefs.HasKey("PP_" + myScript.currentSelectedSlot))
                PlayerPrefs.DeleteKey("PP_" + myScript.currentSelectedSlot);
        }
        #endregion
    }

    private void HelpZone_01()
    {
        LoadSavePlayerProgession myScript = (LoadSavePlayerProgession)target;
        EditorGUILayout.HelpBox(
           "Player Progression are saved with the name: PP_" + myScript.currentSelectedSlot, MessageType.Info);
    }





    void OnSceneGUI()
    {
    }
}
#endif
