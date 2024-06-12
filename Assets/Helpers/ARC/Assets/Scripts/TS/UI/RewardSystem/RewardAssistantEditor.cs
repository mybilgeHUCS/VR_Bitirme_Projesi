//Description: RewardAssistantEditor: Custm Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(RewardAssistant))]
public class RewardAssistantEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty methodsList;

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
        methodsList = serializedObject.FindProperty("methodsList");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        //RewardAssistant myScript = (RewardAssistant)target;
        #endregion
    }

    public override void OnInspectorGUI()
    {
        #region
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        displayMultiMethodsLists();

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");
        #endregion
    }



    //--> display multiple list of methods call when the scene starts
    private void displayMultiMethodsLists()
    {
        #region
        //--> Display feedback
        RewardAssistant myScript = (RewardAssistant)target;

        EditorGUILayout.BeginHorizontal();
        methodModule.displayMethodList("List of methods is called just before loading the new scene:",
                                    editorMethods,
                                    methodsList,
                                    myScript.methodsList,
                                    listGUIStyle[0],
                                    listGUIStyle[1],
                                    "The methods are called in the same order as the list. " +
                                    "\nAll methods must be boolean methods. " +
                                    "\nOther methods will be ignored.");

        
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    void OnSceneGUI()
    {
    }
}
#endif
