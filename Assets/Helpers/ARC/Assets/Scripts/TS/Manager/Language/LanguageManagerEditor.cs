//Description: LanguageManagerEditor. Custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(LanguageManager))]
public class LanguageManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty currentLanguage;
    SerializedProperty helpBox;
    SerializedProperty _GlobalTextDatas;

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
        currentLanguage = serializedObject.FindProperty("currentLanguage");
        helpBox = serializedObject.FindProperty("helpBox");
        _GlobalTextDatas = serializedObject.FindProperty("_GlobalTextDatas");
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

        GlobalParams(listGUIStyle[1]);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");

        
        #endregion
    }

    void GlobalParams(GUIStyle color_01)
    {
        #region
        EditorGUILayout.BeginVertical(color_01);
        //
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GlobalTextDatas: ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(_GlobalTextDatas, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Language:", GUILayout.Width(120));
        EditorGUILayout.PropertyField(currentLanguage, new GUIContent(""), GUILayout.Width(30));

        if (GUILayout.Button("Reset language"))
        {
            if (EditorUtility.DisplayDialog("Delete PlayerPrefs",
                                        "Are you sure?", "Yes", "No"))
            {
                PlayerPrefs.SetInt("LG", 0);
            }
            currentLanguage.intValue = 0;
        }
        if (GUILayout.Button("Set language to current"))
        {
            PlayerPrefs.SetInt("LG", currentLanguage.intValue);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        #endregion
    }

  
    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
            "Return a text: LanguageManager.instance.String_ReturnText(int _WhichTextFolder, int _Entry)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Update all texts: LanguageManager.instance.Bool_UpdateAllTexts()", MessageType.Info);
        

        EditorGUILayout.HelpBox(
            "Access current Language (variable): LanguageManager.instance.currentLanguage", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Used to init the language when the game starts: Bool_InitLanguage()", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Update the current language: Bool_UpdateSelectedLanguage(int value)", MessageType.Info);


        EditorGUILayout.HelpBox(
            "Save Load current Language: PlayerPrefs.GetInt(LG)", MessageType.Info);

        #endregion
    }





    void OnSceneGUI()
    {
    }
}
#endif
