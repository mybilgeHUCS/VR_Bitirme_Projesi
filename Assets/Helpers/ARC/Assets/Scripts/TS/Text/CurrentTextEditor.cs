//Description: CurrentTextEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TS.Generics;

[CustomEditor(typeof(CurrentText))]
public class CurrentTextEditor : Editor
{
    SerializedProperty      SeeInspector;                                            // use to draw default Inspector
    SerializedProperty      _Entry;
    SerializedProperty      _WhichTextData;
    SerializedProperty      tab;
    SerializedProperty      bBypass;

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

    private Texture2D Tex_01;
    private Texture2D Tex_02;
    private Texture2D Tex_03;
    private Texture2D Tex_04;
    private Texture2D Tex_05;

    public TextVariousMethods txtVariousMethods;

    void OnEnable()
    {
        #region
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        _Entry = serializedObject.FindProperty("_Entry");
        _WhichTextData = serializedObject.FindProperty("_WhichTextData");
        bBypass = serializedObject.FindProperty("bBypass");
        tab = serializedObject.FindProperty("tab");

        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {

        }

        txtVariousMethods = new TextVariousMethods();
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

        GUIStyle style_Color01 = new GUIStyle(GUI.skin.box); style_Color01.normal.background = Tex_01;
        GUIStyle style_Color02 = new GUIStyle(GUI.skin.box); style_Color02.normal.background = Tex_02;
        GUIStyle style_Color03 = new GUIStyle(GUI.skin.box); style_Color03.normal.background = Tex_03;
        GUIStyle style_Color04 = new GUIStyle(GUI.skin.box); style_Color04.normal.background = Tex_04;
        GUIStyle style_Color05 = new GUIStyle(GUI.skin.box); style_Color05.normal.background = Tex_05;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bypass:", GUILayout.Width(60));
        EditorGUILayout.PropertyField(bBypass, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();


        if (!bBypass.boolValue)
        {
            tab.intValue = GUILayout.Toolbar(tab.intValue, new string[2] { "Simple Text", "Text Manage By Script" });


            if (tab.intValue == 0)   // 0: Text is always the same
            {
                GameObject _TextManager = FindObjectOfType<LanguageManager>().gameObject;
                if (_TextManager)
                    txtVariousMethods.DisplaySpecificEntry(_WhichTextData, _Entry, _TextManager);


                CurrentText myScript = (CurrentText)target;

                if (GUILayout.Button("Apply Text"))
                {
                    SerializedObject serializedObject2 = new UnityEditor.SerializedObject(myScript.GetComponent<Text>());
                    SerializedProperty m_Text = serializedObject2.FindProperty("m_Text");

                    serializedObject2.Update();
                    m_Text.stringValue = txtVariousMethods.ReturnSpecificEntry(_WhichTextData, _Entry, _TextManager);
                    serializedObject2.ApplyModifiedProperties();

                }
            }
            else if (tab.intValue == 1) //1: Text is manage by script
            {

            }

        }


        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");

       
        #endregion
    }










    void OnSceneGUI()
    {
    }
}
#endif
