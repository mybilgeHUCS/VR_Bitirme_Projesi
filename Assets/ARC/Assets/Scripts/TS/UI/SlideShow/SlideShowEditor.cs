//Description : SlideShowEditor.cs. Use in association with SlideShow.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(SlideShow))]
public class SlideShowEditor : Editor
{
    SerializedProperty          SeeInspector;                                            // use to draw default Inspector
    SerializedProperty          helpBox;
    SerializedProperty          moreOptions;

    SerializedProperty          initEvents;
    SerializedProperty          methodsListNewEntry;
    SerializedProperty          methodsListHowManyEntries;
    SerializedProperty          methodsListGetCurrentSelection;
    SerializedProperty          methodsListSetCurrentSelection;
    public EditorMethods_Pc     editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc   methodModule;

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

        initEvents                      = serializedObject.FindProperty("initEvents");
        methodsListNewEntry             = serializedObject.FindProperty("methodsListNewEntry");
        methodsListHowManyEntries       = serializedObject.FindProperty("methodsListHowManyEntries");

        methodsListGetCurrentSelection  = serializedObject.FindProperty("methodsListGetCurrentSelection");
        methodsListSetCurrentSelection  = serializedObject.FindProperty("methodsListSetCurrentSelection");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        #endregion
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


        displayHowManyEntries(listGUIStyle[0], listGUIStyle[1]);

        displayGetCurrentSelection(listGUIStyle[0], listGUIStyle[1]);

        displaySetCurrentSelection(listGUIStyle[0], listGUIStyle[1]);

        EditorGUILayout.LabelField("");

        displayInitEvents(listGUIStyle[0], listGUIStyle[1]);

        displayAllMethodsToInitNewEntry(listGUIStyle[0], listGUIStyle[1]);

        serializedObject.ApplyModifiedProperties();        
        EditorGUILayout.LabelField("");
        #endregion
    }

    //--> display a list of methods
    private void displayGetCurrentSelection(GUIStyle styleZero, GUIStyle StyleOne)
    {
        #region
        //--> Display feedback
        SlideShow myScript = (SlideShow)target;

        methodModule.displayOneMethod("1b-Get the current selected entry:",
                                       editorMethods,
                                       methodsListGetCurrentSelection,
                                       myScript.methodsListGetCurrentSelection,
                                       styleZero,
                                       StyleOne,
                                       "Use Only Int methods." +
                                       "\nOther methods will be ignored.",
                                       true);

        #endregion
    }

    //--> display a list of methods
    private void displaySetCurrentSelection(GUIStyle styleZero, GUIStyle StyleOne)
    {
        #region
        //--> Display feedback
        SlideShow myScript = (SlideShow)target;

        methodModule.displayOneMethod("1c-Set the current selected entry:",
                                       editorMethods,
                                       methodsListSetCurrentSelection,
                                       myScript.methodsListSetCurrentSelection,
                                       styleZero,
                                       StyleOne,
                                       "Use ONLY Void methods." +
                                       "\nOther methods will be ignored.",
                                       true);

        #endregion
    }


    //--> display a list of methods
    private void displayHowManyEntries(GUIStyle styleZero, GUIStyle StyleOne)
    {
        #region
        //--> Display feedback
        SlideShow myScript = (SlideShow)target;

        methodModule.displayOneMethod("1a-Get how many entries available:",
                                       editorMethods,
                                       methodsListHowManyEntries,
                                       myScript.methodsListHowManyEntries,
                                       styleZero,
                                       StyleOne,
                                       "Use ONLY Int methods." +
                                       "\nOther methods will be ignored.");

        #endregion
    }

    //--> display a list of methods
    private void displayInitEvents(GUIStyle styleZero, GUIStyle StyleOne)
    {
        #region
        EditorGUILayout.BeginVertical(styleZero);

        EditorGUILayout.LabelField("2a-Events called during the Initialisation:",EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Use ONLY Void Methods", MessageType.Info);
        EditorGUILayout.PropertyField(initEvents, new GUIContent(""));
       
        EditorGUILayout.EndVertical();
        #endregion
    }


    //--> display a list of methods
    private void displayAllMethodsToInitNewEntry(GUIStyle styleZero, GUIStyle StyleOne)
    {
        #region
        //--> Display feedback
        SlideShow myScript = (SlideShow)target;

        methodModule.displayMethodList("2b-Methods called when a new entry is displayed:",
                                       editorMethods,
                                       methodsListNewEntry,
                                       myScript.methodsListNewEntry,
                                       styleZero,
                                       StyleOne,
                                       "The methods are called in the same order as the list. " +
                                       "\nAll methods must be boolean methods. " +
                                       "\nOther methods will be ignored.");

        #endregion
    }

    void OnSceneGUI()
    {
    }
}
#endif
