//Description : ButtonNavigationEditorEditor.cs. Use in association with ButtonNavigationEditor.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CanEditMultipleObjects]
[CustomEditor(typeof(ButtonNavigation))]
public class ButtonNavigationEditorEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;

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
        SeeInspector                    = serializedObject.FindProperty("SeeInspector");
        helpBox                         = serializedObject.FindProperty("helpBox");
        moreOptions                     = serializedObject.FindProperty("moreOptions");
        #endregion

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();
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

        EditorGUILayout.LabelField("");

       
        serializedObject.ApplyModifiedProperties();        
        EditorGUILayout.LabelField("");
        #endregion
    }

   
    void OnSceneGUI()
    {
    }
}
#endif
