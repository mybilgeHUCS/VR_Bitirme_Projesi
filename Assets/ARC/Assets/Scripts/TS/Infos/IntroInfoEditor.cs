//Description : IntroInfoEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;


[CustomEditor(typeof(IntroInfo))]
public class IntroInfoEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty introAlreadyLoaded;
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
        introAlreadyLoaded = serializedObject.FindProperty("introAlreadyLoaded");
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


        EditorGUILayout.LabelField("");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Intro loaded:",EditorStyles.boldLabel, GUILayout.Width(85));
        EditorGUILayout.PropertyField(introAlreadyLoaded, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.LabelField("");
        if (helpBox.boolValue) HelpZone_01();
        LoadSection(listGUIStyle[0], listGUIStyle[1]);


        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    //--> display a list of methods (Load Section)
    private void LoadSection(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        IntroInfo myScript = (IntroInfo)target;

        methodModule.displayMethodList("(boolean methods only):",
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

   
    private void HelpZone_01()
    {
        //IntroInfo myScript = (IntroInfo)target;
        EditorGUILayout.HelpBox(
           "Methods that load datas when the game is launched:" + "\n" +
           "Language, Player progression, Sound volumes, Inputs,...", MessageType.Info);
    }

    void OnSceneGUI()
    {
    }
}
#endif
