//Description : LeaderboardEditor.cs. Use in association with Leaderboard.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(Leaderboard))]
public class LeaderboardEditorEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;
    SerializedProperty pageLeaderboard;
    SerializedProperty listGrpScore;
    SerializedProperty newTemplateName;
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
        moreOptions = serializedObject.FindProperty("moreOptions");
        pageLeaderboard = serializedObject.FindProperty("pageLeaderboard");
        listGrpScore = serializedObject.FindProperty("listGrpScore");
        newTemplateName = serializedObject.FindProperty("newTemplateName");
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

        //-> Display Leaderboard List
        DisplayLeaderboardList();

        //-> Section create a new leaderboard template
        CreateLeaderboardTemplate();

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");
        #endregion
    }

    public void CreateLeaderboardTemplate()
    {
        #region
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New leaderboard template", GUILayout.Width(180)))
        {
            //NewCheckStateNameEditor
            bool b_NameAlreadyExist = false;
            for (var i = 0; i < listGrpScore.arraySize; i++)
            {
                SerializedProperty m_name = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                if (newTemplateName.stringValue == m_name.stringValue)
                {
                    b_NameAlreadyExist = true;
                    break;
                }
            }

            if (b_NameAlreadyExist && EditorUtility.DisplayDialog("This name is already used",
                     "",
                 "Continue"))
            {

            }
            else
            {
                listGrpScore.InsertArrayElementAtIndex(0);

                SerializedProperty m_name = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("name");
                SerializedProperty m_scorePrefab = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("scorePrefab");
                SerializedProperty m_b_alternateBackgroundColor = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("b_alternateBackgroundColor");
                //SerializedProperty m_alternateBackgroundColor_Odd = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("alternateBackgroundColor_Odd");
                //SerializedProperty m_alternateBackgroundColor_Even = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("alternateBackgroundColor_Even");
                SerializedProperty m_b_alternateBackgroundSprite = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("b_alternateBackgroundSprite");
                //SerializedProperty m_alternateBackgroundSprite_Odd = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("alternateBackgroundSprite_Odd");
                //SerializedProperty m_alternateBackgroundSprite_Even = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("alternateBackgroundSprite_Even");
                SerializedProperty m_maxEntries = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("maxEntries");
                SerializedProperty m_setupLeaderboard = listGrpScore.GetArrayElementAtIndex(0).FindPropertyRelative("setupLeaderboard");



                m_name.stringValue = newTemplateName.stringValue;
                m_scorePrefab.objectReferenceValue = null;
                m_b_alternateBackgroundColor.boolValue = false;
                m_b_alternateBackgroundSprite.boolValue = false;
                m_maxEntries.intValue = 10;
                m_setupLeaderboard.ClearArray();

                listGrpScore.MoveArrayElement(0, listGrpScore.arraySize - 1);
            }
        }
        EditorGUILayout.PropertyField(newTemplateName, new GUIContent(""), GUILayout.MinWidth(100));
        EditorGUILayout.EndHorizontal();
        #endregion
    }

    public void DisplayLeaderboardList()
    {
        #region
        EditorGUILayout.HelpBox("Reference to the page ID in canvasMainMenuManager", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Page ID:", GUILayout.Width(50));
        EditorGUILayout.PropertyField(pageLeaderboard, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        for (var i = 0;i< listGrpScore.arraySize; i++)
        {
            SerializedProperty m_name = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("name");
            SerializedProperty m_scorePrefab = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("scorePrefab");
            SerializedProperty m_b_alternateBackgroundColor = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("b_alternateBackgroundColor");
            SerializedProperty m_alternateBackgroundColor_Odd = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("alternateBackgroundColor_Odd");
            SerializedProperty m_alternateBackgroundColor_Even = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("alternateBackgroundColor_Even");
            SerializedProperty m_b_alternateBackgroundSprite = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("b_alternateBackgroundSprite");
            SerializedProperty m_alternateBackgroundSprite_Odd = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("alternateBackgroundSprite_Odd");
            SerializedProperty m_alternateBackgroundSprite_Even = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("alternateBackgroundSprite_Even");
            SerializedProperty m_maxEntries = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("maxEntries");
            SerializedProperty m_setupLeaderboard = listGrpScore.GetArrayElementAtIndex(i).FindPropertyRelative("setupLeaderboard");

            EditorGUILayout.BeginVertical(listGUIStyle[2]);
            //-> Name
            EditorGUILayout.BeginVertical(listGUIStyle[0]);
            EditorGUILayout.BeginHorizontal();
            if (moreOptions.boolValue)
            {
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    listGrpScore.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
            EditorGUILayout.LabelField("ID: " + i, EditorStyles.boldLabel, GUILayout.Width(30));
            EditorGUILayout.PropertyField(m_name, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //-> Score prefab
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Score Prefab:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(m_scorePrefab, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            //-> Background color
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Background color:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(m_b_alternateBackgroundColor, new GUIContent(""), GUILayout.Width(30));
            if (m_b_alternateBackgroundColor.boolValue)
            {
                EditorGUILayout.LabelField("Odd:", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_alternateBackgroundColor_Odd, new GUIContent(""));
                EditorGUILayout.LabelField("Even:", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_alternateBackgroundColor_Even, new GUIContent(""));
            }
            EditorGUILayout.EndHorizontal();

            //-> Background sprite
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Background sprite:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(m_b_alternateBackgroundSprite, new GUIContent(""), GUILayout.Width(30));
            if (m_b_alternateBackgroundSprite.boolValue)
            {
                EditorGUILayout.LabelField("Odd:", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_alternateBackgroundSprite_Odd, new GUIContent(""));
                EditorGUILayout.LabelField("Even:", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_alternateBackgroundSprite_Even, new GUIContent(""));
            }
            EditorGUILayout.EndHorizontal();

            //-> Max Entries
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max entries:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(m_maxEntries, new GUIContent(""));
            EditorGUILayout.EndHorizontal();


            //-> Method call to init the leaderboard
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_setupLeaderboard, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("");
        }
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
