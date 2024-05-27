// Description: PauseManagerEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(PauseManager))]
public class PauseManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;
    SerializedProperty listOfPause;
    SerializedProperty Bool_IsGamePaused;

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
        listOfPause = serializedObject.FindProperty("listOfPause");
        helpBox = serializedObject.FindProperty("helpBox");
        Bool_IsGamePaused = serializedObject.FindProperty("Bool_IsGamePaused");
        moreOptions = serializedObject.FindProperty("moreOptions");

        //PauseManager myScript = (PauseManager)target;

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
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(65));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));


        if (EditorPrefs.GetBool("MoreOptions") == true)
        {
            EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
            EditorGUILayout.PropertyField(moreOptions, new GUIContent(""), GUILayout.Width(30));
        }
        EditorGUILayout.EndHorizontal();
        
       

        if(helpBox.boolValue) HelpZone_01();


        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is The Game Paused:", GUILayout.Width(130));
        EditorGUILayout.PropertyField(Bool_IsGamePaused, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        for (var i = 0;i< listOfPause.arraySize; i++)
        {
            SerializedProperty m_Name = listOfPause.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name");
            SerializedProperty m_Pause = listOfPause.GetArrayElementAtIndex(i).FindPropertyRelative("m_Pause");
            SerializedProperty m_UnPause = listOfPause.GetArrayElementAtIndex(i).FindPropertyRelative("m_Unpause");

            EditorGUILayout.BeginVertical(listGUIStyle[0]);
            EditorGUILayout.BeginVertical(listGUIStyle[1]);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pause " + i + ": ", EditorStyles.boldLabel, GUILayout.Width(85));
            EditorGUILayout.PropertyField(m_Name, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pause:");
            EditorGUILayout.LabelField("Unpause:");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_Pause, new GUIContent(""));
            EditorGUILayout.PropertyField(m_UnPause, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }


        if (moreOptions.boolValue)
        {
            if (GUILayout.Button("Add New Pause (EventSystem)"))
            {
                listOfPause.InsertArrayElementAtIndex(listOfPause.arraySize - 1);
            }

            if (GUILayout.Button("Remove Last"))
            {
                listOfPause.DeleteArrayElementAtIndex(listOfPause.arraySize - 1);
            }
        }
        


       
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");


        #endregion
    }




    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
            "Call Pause: TS.Generics.PauseManager.instance.PauseGame();", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Call Unpause: TS.Generics.PauseManager.instance.UnpauseGame();", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Know if Pause is enabled:\n" +
            "TS.Generics.PauseManager.instance.Bool_IsGamePaused", MessageType.Info);

        #endregion
    }





    void OnSceneGUI()
    {
    }
}
#endif
