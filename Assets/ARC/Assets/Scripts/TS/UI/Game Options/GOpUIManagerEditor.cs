//Description : GOpUIManagerEditor. Custom editor for GOpUIManager.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(GOpUIManager))]
public class GOpUIManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty singleLanguageList;


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
        #region
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        
        helpBox = serializedObject.FindProperty("helpBox");

        singleLanguageList = serializedObject.FindProperty("singleLanguageList");
        #endregion
    }

    public override void OnInspectorGUI()
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
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();


        if (helpBox.boolValue)                         // If true Default Inspector is drawn on screen
            HelpZone_01();

        EditorGUILayout.LabelField("");

        displayLanguage();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");

        
        #endregion
    }

    void displayLanguage()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        if (GUILayout.Button("Add a new language", GUILayout.Height(30)))
        {
            singleLanguageList.InsertArrayElementAtIndex(0);
            singleLanguageList.MoveArrayElement(0, singleLanguageList.arraySize-1);
            for (var i = 0; i < singleLanguageList.arraySize; i++)
            {
                singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").InsertArrayElementAtIndex(0);
                singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").GetArrayElementAtIndex(0).stringValue = "";
                int aSize = singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").arraySize;
                singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").MoveArrayElement(0,aSize - 1);
            }

            singleLanguageList.GetArrayElementAtIndex(singleLanguageList.arraySize - 1).FindPropertyRelative("languageList").ClearArray();
            for (var i = 0; i < singleLanguageList.arraySize; i++)
            {
                singleLanguageList.GetArrayElementAtIndex(singleLanguageList.arraySize - 1).FindPropertyRelative("languageList").InsertArrayElementAtIndex(0);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.LabelField("");

        for (var i = 0;i< singleLanguageList.arraySize; i++)
        {
            EditorGUILayout.BeginVertical(listGUIStyle[0]);
            EditorGUILayout.BeginVertical(listGUIStyle[2]);
            EditorGUILayout.LabelField("Language " + i + ":", GUILayout.Width(100));
            EditorGUILayout.EndVertical();
            for (var j = 0; j < singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").arraySize; j++)
            {
                EditorGUILayout.PropertyField(singleLanguageList.GetArrayElementAtIndex(i).FindPropertyRelative("languageList").GetArrayElementAtIndex(j), new GUIContent("")) ;
            }   
            EditorGUILayout.EndVertical();
        }
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
