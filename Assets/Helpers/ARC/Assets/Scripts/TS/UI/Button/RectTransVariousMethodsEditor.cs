//Description : RectTransVariousMethodsEditorEditor.cs. Use in association with RectTransVariousMethodsEditor.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CanEditMultipleObjects]
[CustomEditor(typeof(RectTransVariousMethods))]
public class RectTransVariousMethodsEditorEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;


    SerializedProperty scaleSpeed;
    SerializedProperty vectorThreeaList;
    SerializedProperty bShowEditorScale;
    SerializedProperty pivotSpeed;
    SerializedProperty pivotaList;
    SerializedProperty bShowEditorPivot;

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
    public  List<GUIStyle>      listGUIStyle = new List<GUIStyle>();
    private List<Color>         listColor = new List<Color>();
    #endregion

    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

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
        helpBox                         = serializedObject.FindProperty("helpBox");
        moreOptions                     = serializedObject.FindProperty("moreOptions");
        scaleSpeed                      = serializedObject.FindProperty("scaleSpeed");
        vectorThreeaList                = serializedObject.FindProperty("vectorThreeaList");
        bShowEditorScale                = serializedObject.FindProperty("bShowEditorScale");
        pivotSpeed                      = serializedObject.FindProperty("pivotSpeed");
        pivotaList                      = serializedObject.FindProperty("pivotaList");
        bShowEditorPivot                = serializedObject.FindProperty("bShowEditorPivot");
        #endregion

        editorMethods                   = new EditorMethods_Pc();
        methodModule                    = new AP_MethodModule_Pc();
    }

    public override void OnInspectorGUI()
    {
        #region
        //-> Custom editor visualisation parameters
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

        DisplayInfo();

        EditorGUILayout.LabelField("");

        serializedObject.ApplyModifiedProperties();        
        EditorGUILayout.LabelField("");
        #endregion
    }

    void DisplayInfo()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(bShowEditorScale, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.LabelField("Change Scale:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (bShowEditorScale.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scale speed:", GUILayout.Width(80));
            EditorGUILayout.PropertyField(scaleSpeed, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            for(var i = 0;i< vectorThreeaList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID " + i + ":", GUILayout.Width(30));
                EditorGUILayout.PropertyField(vectorThreeaList.GetArrayElementAtIndex(i), new GUIContent(""));
                EditorGUILayout.EndHorizontal();
            }
           
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(bShowEditorPivot, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.LabelField("Change Pivot:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (bShowEditorPivot.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pivot speed:", GUILayout.Width(80));
            EditorGUILayout.PropertyField(pivotSpeed, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < pivotaList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID " + i + ":", GUILayout.Width(40));
                EditorGUILayout.PropertyField(pivotaList.GetArrayElementAtIndex(i), new GUIContent(""));
                EditorGUILayout.EndHorizontal();
            }

        }
        EditorGUILayout.EndVertical();
    }

    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
          "Useful methods:\n" +
          "ChangePivotSmooth(int vectorID)\n" +
          "ChangePivotStraight(int vectorID)\n" +
          "ChangeScaleSmooth(int vectorID)\n" +
          "ChangeScaleStraight(int vectorID)\n" +
          "", MessageType.Info);
        #endregion
    }
    

    void OnSceneGUI()
    {
    }
}
#endif
