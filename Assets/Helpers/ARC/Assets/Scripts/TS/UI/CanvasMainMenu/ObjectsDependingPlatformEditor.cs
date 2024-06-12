//Description: ObjectsDependingPlatformEditor.cs. Use in association with ObjectsDependingPlatform.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(ObjectsDependingPlatform))]
public class ObjectsDependingPlatformEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;

    SerializedProperty listDesktopMobileObjects;


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
    public List<GUIStyle>   listGUIStyle = new List<GUIStyle>();
    private List<Color>     listColor = new List<Color>();
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
        SeeInspector                = serializedObject.FindProperty("SeeInspector");
        helpBox                     = serializedObject.FindProperty("helpBox");
        moreOptions                 = serializedObject.FindProperty("moreOptions");

        listDesktopMobileObjects    = serializedObject.FindProperty("listDesktopMobileObjects");


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

        if(helpBox.boolValue)HelpZone_01();

        displayObjectOnDesktopOrMobile(listGUIStyle[0]);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }




    public void displayObjectOnDesktopOrMobile(GUIStyle color_01)
    {
        EditorGUILayout.BeginVertical(color_01);
        if (GUILayout.Button("Add a new GameObject"))
        {
            listDesktopMobileObjects.InsertArrayElementAtIndex(0);
        }


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("GameObject enable on:", GUILayout.Width(150));
        EditorGUILayout.LabelField("Desktop:", GUILayout.Width(70));
        EditorGUILayout.LabelField("Mobile:", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < listDesktopMobileObjects.arraySize; i++)
        {
            SerializedProperty _Obj         = listDesktopMobileObjects.GetArrayElementAtIndex(i).FindPropertyRelative("_Obj");
            SerializedProperty b_Desktop    = listDesktopMobileObjects.GetArrayElementAtIndex(i).FindPropertyRelative("b_Desktop");
            SerializedProperty b_Mobile     = listDesktopMobileObjects.GetArrayElementAtIndex(i).FindPropertyRelative("b_Mobile");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_Obj, new GUIContent(""), GUILayout.Width(150));
            EditorGUILayout.PropertyField(b_Desktop, new GUIContent(""), GUILayout.Width(70));
            EditorGUILayout.PropertyField(b_Mobile, new GUIContent(""), GUILayout.Width(70));
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                listDesktopMobileObjects.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }


    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
            "This script allows to enable or disable objects depending the plaform selected in window w_GlobalParams.", MessageType.Info);
        EditorGUILayout.HelpBox(
          "This script is automaticaly called by PageInit.cs before the methods contained in PageInit.cs.", MessageType.Info);
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
