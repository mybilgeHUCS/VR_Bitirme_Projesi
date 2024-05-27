//Description: CamDuringCoundownEditor: Custom editor
#if (UNITY_EDITOR)
using UnityEngine;

using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;


[CustomEditor(typeof(CamDuringCoundown))]
public class CamDuringCoundownEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty multiMethodsList;

    SerializedProperty editorSelectedList;
    SerializedProperty editorNewCountdownName;

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


    public List<String> countdownNames = new List<string>();

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
        multiMethodsList = serializedObject.FindProperty("multiMethodsList");

        editorSelectedList = serializedObject.FindProperty("editorSelectedList");
        editorNewCountdownName = serializedObject.FindProperty("editorNewCountdownName");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();


        countdownNames = GenerateCountdownNameList();
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

        Selection(listGUIStyle[0], listGUIStyle[1]);

        LoadSection(listGUIStyle[0], listGUIStyle[1]);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    private void Selection(GUIStyle style_00, GUIStyle style_01)
    {
        EditorGUILayout.BeginVertical(style_00);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Choose a countdown: ", GUILayout.Width(130));
        editorSelectedList.intValue = EditorGUILayout.Popup(editorSelectedList.intValue, countdownNames.ToArray());				// --> Display all methods
        EditorGUILayout.EndHorizontal();

        if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create new countdown"))
            {
                bool b_Allowed = true;
                for (var i = 0; i < multiMethodsList.arraySize; i++)
                {
                    SerializedProperty m_Name = multiMethodsList.GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
                    if (m_Name.stringValue == editorNewCountdownName.stringValue ||
                        editorNewCountdownName.stringValue == "")
                    {
                        b_Allowed = false;
                    }
                }

                if (b_Allowed)
                {
                    int howmanylist = multiMethodsList.arraySize;
                    multiMethodsList.InsertArrayElementAtIndex(0);
                    multiMethodsList.GetArrayElementAtIndex(0).FindPropertyRelative("_Name").stringValue = editorNewCountdownName.stringValue;
                    multiMethodsList.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("methodsList").ClearArray();
                    multiMethodsList.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("methodsList").InsertArrayElementAtIndex(0);

                    multiMethodsList.MoveArrayElement(0, multiMethodsList.arraySize - 1);
                    countdownNames = GenerateCountdownNameList();
                    
                    editorSelectedList.intValue = howmanylist;
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("The name is already use.",
                     "Choose a unique name.", "Continue"))
                    {
                    }
                }
            }
            EditorGUILayout.PropertyField(editorNewCountdownName, new GUIContent(""));

            EditorGUILayout.EndHorizontal();

            SerializedProperty m_Name2 = multiMethodsList.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("_Name");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Edit countdown name: ", GUILayout.Width(150));
            EditorGUILayout.PropertyField(m_Name2, new GUIContent(""));
            EditorGUILayout.EndHorizontal();



        }
        EditorGUILayout.EndVertical();

        if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
            EditorGUILayout.LabelField("");
    }

    //--> display a list of methods (Load Section)
    private void LoadSection(GUIStyle style_00, GUIStyle style_01)
    {
        #region
        //--> Display feedback
        for (var i = 0; i < multiMethodsList.arraySize; i++)
        {
            if (editorSelectedList.intValue == i)
            {
                CamDuringCoundown myScript = (CamDuringCoundown)target;

                methodModule.displayMethodList("(boolean methods only):",
                                               editorMethods,
                                               multiMethodsList.GetArrayElementAtIndex(editorSelectedList.intValue).FindPropertyRelative("methodsList"),
                                               myScript.multiMethodsList[editorSelectedList.intValue].methodsList,
                                               style_00,
                                               style_01,
                                               "The methods are called in the same order as the list. " +
                                               "\nAll methods must be boolean methods. " +
                                               "\nOther methods will be ignored.",
                                               moreOptions.boolValue,
                                               true);
            }
        }
        #endregion
    }

   
    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "Methods that load datas when the game is launched:" + "\n" +
           "Language, Player progression, Sound volumes, Inputs,...", MessageType.Info);
    }


    List<string> GenerateCountdownNameList()
    {
        List<string> newList = new List<string>();
        countdownNames.Clear();

        for (var i = 0; i < multiMethodsList.arraySize; i++)
        {
            SerializedProperty m_Name = multiMethodsList.GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
            newList.Add(m_Name.stringValue);
        }
        return newList;
    }


    void OnSceneGUI()
    {
    }
}
#endif
