//Description: LoadSceneEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(LoadScene))]
public class LoadSceneEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;

    SerializedProperty multiMethodList;

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
        #region
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        helpBox = serializedObject.FindProperty("helpBox");
        multiMethodList = serializedObject.FindProperty("multiMethodList");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

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

        displayMultiMethodsLists(listGUIStyle[0], listGUIStyle[1]);

        EditorGUILayout.LabelField("");

        if (GUILayout.Button("Create a list of methods"))
        {
            if(multiMethodList.arraySize == 0)
                multiMethodList.InsertArrayElementAtIndex(0);
            else
                multiMethodList.InsertArrayElementAtIndex(multiMethodList.arraySize);
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
        LoadScene myScript = (LoadScene)target;

        for(var i = 0; i < multiMethodList.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
           methodModule.displayMethodList("List " + i + " of methods is called just before loading the new scene:",
                                       editorMethods,
                                       multiMethodList.GetArrayElementAtIndex(i).FindPropertyRelative("methodsList"),
                                       myScript.multiMethodList[i].methodsList,
                                       style_Blue,
                                       style_Yellow_01,
                                       "The methods are called in the same order as the list. " +
                                       "\nAll methods must be boolean methods. " +
                                       "\nOther methods will be ignored.");

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                multiMethodList.DeleteArrayElementAtIndex(i);

            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }



    

    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
           "Load a new Scene with a custom method List:" + "\n" +
           "LoadScene.instance.StartCoroutine(" +
           "LoadScene.instance.LoadSceneWithSceneNumberAndSpecificCustomMethodList(int index,int whichCustomMethodList))", MessageType.Info);
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
