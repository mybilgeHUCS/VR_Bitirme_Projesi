//Description : ButtonCustomEditor.cs. Use in association with ButtonCustom.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CanEditMultipleObjects]
[CustomEditor(typeof(ButtonCustom))]
public class ButtonCustomEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;
    SerializedProperty methodsList;
    SerializedProperty OnClick;
    SerializedProperty OnClickWrong;

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
        methodsList                     = serializedObject.FindProperty("methodsList");
        OnClick                         = serializedObject.FindProperty("OnClick");
        OnClickWrong                    = serializedObject.FindProperty("OnClickWrong");
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

        //-> Display conditions to allow the player to press the button
        SectionConditionToCheck();

        EditorGUILayout.LabelField("");

        if (helpBox.boolValue) HelpZone_01();
        //-> Display OnClick() section
        SectionOnClickOk();

        //-> Display OnClick() section
        SectionOnClickWrong();

        serializedObject.ApplyModifiedProperties();        
        EditorGUILayout.LabelField("");
        #endregion
    }

    //-> Display conditions to allow the player to press the button
    public void SectionConditionToCheck()
    {
        ButtonCustom myScript = (ButtonCustom)target;

        //-> Display conditions
        methodModule.displayMethodList("Conditions:",
                                   editorMethods,
                                    methodsList,
                                   myScript.methodsList,
                                   listGUIStyle[0],
                                   listGUIStyle[0],
                                   "All methods must be boolean methods. " +
                                   "\nOther methods will be ignored.",
                                   helpBox.boolValue);

    }

    //-> Display OnClick() section
    public void SectionOnClickOk()
    {
        EditorGUILayout.LabelField("Conditions return True:" ,EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(OnClick);
    }


    //-> Display OnClick() section
    public void SectionOnClickWrong()
    {
        EditorGUILayout.LabelField("Conditions return false:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(OnClickWrong);
    }

    

    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
          "OnClick() events are allowed only if all the conditions return true.", MessageType.Info);
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
