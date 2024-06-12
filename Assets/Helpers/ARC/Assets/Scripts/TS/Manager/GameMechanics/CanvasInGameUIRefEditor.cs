//Description: CanvasInGameUIRefEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;

[CustomEditor(typeof(CanvasInGameUIRef))]
public class CanvasInGameUIRefEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;
    SerializedProperty listPlayerUIElements;
    SerializedProperty currentSelectedList;

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

    public List<String> ListNames = new List<string>();

    public string[] arrTypeName = new string[] { "RectTransform", "Camera" };

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

        listPlayerUIElements = serializedObject.FindProperty("listPlayerUIElements");
        currentSelectedList = serializedObject.FindProperty("currentSelectedList");

        ListNames = GenerateListNames();

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

        if (helpBox.boolValue) HelpZone(4);

        displaySections();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    List<string> GenerateListNames()
    {
        List<string> newList = new List<string>();
        ListNames.Clear();

        for (var i = 0; i < listPlayerUIElements.arraySize; i++)
        {
            SerializedProperty m_name = listPlayerUIElements.GetArrayElementAtIndex(i).FindPropertyRelative("name");
            newList.Add(m_name.stringValue);
        }


        return newList;
    }



    void displaySections()
    {
        currentSelectedList.intValue = EditorGUILayout.Popup(currentSelectedList.intValue, ListNames.ToArray());                // --> Display all methods

        SerializedProperty name = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("name");
        SerializedProperty cam = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("cam");
        SerializedProperty listRectTransform = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("listRectTransform");
        SerializedProperty listTexts = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("listTexts");
        SerializedProperty listImage = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("listImage");
        SerializedProperty b_EditName = listPlayerUIElements.GetArrayElementAtIndex(currentSelectedList.intValue).FindPropertyRelative("b_EditName");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("", GUILayout.Width(20)))
        {
            b_EditName.boolValue = !b_EditName.boolValue;
        }
        EditorGUILayout.LabelField("Current Selected List ID = " + currentSelectedList.intValue, GUILayout.Width(160));
        if (b_EditName.boolValue)
        {
            EditorGUILayout.LabelField(" -> name: ", GUILayout.Width(70));
            EditorGUILayout.PropertyField(name, new GUIContent(""));
        }
        EditorGUILayout.EndHorizontal();

        //-> Rect Transforms
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            listRectTransform.InsertArrayElementAtIndex(0);
            listRectTransform.GetArrayElementAtIndex(0).objectReferenceValue = null;
            listRectTransform.MoveArrayElement(0, listRectTransform.arraySize - 1);
        }
        EditorGUILayout.LabelField("Rect Transforms:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) HelpZone(1);

        for (var j = 0; j < listRectTransform.arraySize; j++)
        {
            SerializedProperty rectTransform = listRectTransform.GetArrayElementAtIndex(j);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(j + ":", GUILayout.Width(20));
            EditorGUILayout.PropertyField(rectTransform, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }

        //-> Texts
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            listTexts.InsertArrayElementAtIndex(0);
            listTexts.GetArrayElementAtIndex(0).objectReferenceValue = null;
            listTexts.MoveArrayElement(0, listTexts.arraySize - 1);
        }
        EditorGUILayout.LabelField("Texts:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) HelpZone(2);

        for (var j = 0; j < listTexts.arraySize; j++)
        {
            SerializedProperty text = listTexts.GetArrayElementAtIndex(j);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(j + ":", GUILayout.Width(20));
            EditorGUILayout.PropertyField(text, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }

        //->  Images
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            listImage.InsertArrayElementAtIndex(0);
            listImage.GetArrayElementAtIndex(0).objectReferenceValue = null;
            listImage.MoveArrayElement(0, listImage.arraySize - 1);
        }
        EditorGUILayout.LabelField("Images:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) HelpZone(3);

        for (var j = 0; j < listImage.arraySize; j++)
        {
            SerializedProperty im = listImage.GetArrayElementAtIndex(j);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(j + ":", GUILayout.Width(20));
            EditorGUILayout.PropertyField(im, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void HelpZone(int value)
    {
        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "How to access a camera:" + "\n" +
                "CanvasInGameUIRef.instance.listPlayerUIElements[" + currentSelectedList.intValue + "].cam", MessageType.Info);
                break;

            case 1:
                EditorGUILayout.HelpBox(
                "How to access a Rect Transform:" + "\n" +
                "CanvasInGameUIRef.instance.listPlayerUIElements[" + currentSelectedList.intValue + "].listRectTransform[int ID]", MessageType.Info);
                break;
            case 2:
                EditorGUILayout.HelpBox(
                "How to access a Text:" + "\n" +
                "CanvasInGameUIRef.instance.listPlayerUIElements[" + currentSelectedList.intValue + "].listTexts[int ID]", MessageType.Info);
                break;
            case 3:
                EditorGUILayout.HelpBox(
                "How to access an Image:" + "\n" +
                "CanvasInGameUIRef.instance.listPlayerUIElements[" + currentSelectedList.intValue + "].listImage[int ID]", MessageType.Info);
                break;
            case 4:
                EditorGUILayout.HelpBox(
                "Description: This script allows to easily access by script to the P1 and P2 UI elements.", MessageType.Info);
                break;
        }
    }

    void OnSceneGUI()
    {
    }
}
#endif
