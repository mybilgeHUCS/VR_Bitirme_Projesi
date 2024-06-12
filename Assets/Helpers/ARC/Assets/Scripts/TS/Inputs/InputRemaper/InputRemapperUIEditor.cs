//Description: InputRemapperUIEditor: Custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;

[CustomEditor(typeof(InputRemapperUI))]
public class InputRemapperUIEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty whichInputModeEditor;
    SerializedProperty whichInputTypeEditor;
    SerializedProperty whichInputToCreateEditor;

    public String[] arrInputMode = new string[2] {"Keyboard","Gamepad"};
    public String[] arrInputType = new string[3] { "Button", "Checkbox", "Slider"};

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
        whichInputModeEditor = serializedObject.FindProperty("whichInputModeEditor");
        whichInputTypeEditor = serializedObject.FindProperty("whichInputTypeEditor");
        whichInputToCreateEditor = serializedObject.FindProperty("whichInputToCreateEditor");


        #endregion
    }

    public override void OnInspectorGUI()
    {
        #region
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();


        InputRemapperUI myScript = (InputRemapperUI)target;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        // HelpZone_01();
        EditorGUILayout.BeginHorizontal();

        whichInputModeEditor.intValue = EditorGUILayout.Popup(whichInputModeEditor.intValue, arrInputMode, GUILayout.Width(85));
        whichInputTypeEditor.intValue = EditorGUILayout.Popup(whichInputTypeEditor.intValue, arrInputType, GUILayout.Width(85));

        EditorGUILayout.LabelField("Input Number:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(whichInputToCreateEditor, new GUIContent(""), GUILayout.Width(30));


        if (GUILayout.Button("Create"))
        {
            if (whichInputTypeEditor.intValue == 0)
            {
               
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(myScript.refInputTypeList[0], myScript.refPosList[0].transform);
                newObj.transform.SetSiblingIndex(myScript.refPosList[2 + whichInputModeEditor.intValue].transform.GetSiblingIndex());

                newObj.name = "Grp_Button";

                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);


                SerializedObject serializedObject2 = new SerializedObject(newObj.transform.GetChild(1).GetChild(1).GetComponent<btnRempInfo>());

                serializedObject2.Update();
                SerializedProperty m_whichDevice = serializedObject2.FindProperty("whichDevice");
                SerializedProperty m_whichType = serializedObject2.FindProperty("whichType");
                SerializedProperty m_whichInput = serializedObject2.FindProperty("whichInput");

                m_whichDevice.intValue = whichInputModeEditor.intValue;
                if (m_whichDevice.intValue == 0) m_whichType.intValue = 0;  // Keyboard
                if (m_whichDevice.intValue == 1) m_whichType.intValue = 1;  // Gamepad
                if (m_whichDevice.intValue == 2) m_whichType.intValue = 0;  // Mobile
                if (m_whichDevice.intValue == 3) m_whichType.intValue = 0;  // Other

                m_whichInput.intValue = whichInputToCreateEditor.intValue;

                serializedObject2.ApplyModifiedProperties();
            }

            if (whichInputTypeEditor.intValue == 1)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(myScript.refInputTypeList[1], myScript.refPosList[0].transform);
                newObj.transform.SetSiblingIndex(myScript.refPosList[2 + whichInputModeEditor.intValue].transform.GetSiblingIndex());
                newObj.name = "Grp_CheckBox";
                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);

                SerializedObject serializedObject2 = new SerializedObject(newObj.transform.GetChild(1).GetChild(1).GetComponent<btnRempInfo>());

                serializedObject2.Update();
                SerializedProperty m_whichDevice = serializedObject2.FindProperty("whichDevice");
                SerializedProperty m_whichType = serializedObject2.FindProperty("whichType");
                SerializedProperty m_whichInput = serializedObject2.FindProperty("whichInput");

                m_whichDevice.intValue = whichInputModeEditor.intValue;
                m_whichInput.intValue = whichInputToCreateEditor.intValue;

                serializedObject2.ApplyModifiedProperties();
            }

            if (whichInputTypeEditor.intValue == 2)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(myScript.refInputTypeList[2], myScript.refPosList[0].transform);
                newObj.transform.SetSiblingIndex(myScript.refPosList[2 + whichInputModeEditor.intValue].transform.GetSiblingIndex());
                newObj.name = "Grp_Slider";
                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);

                SerializedObject serializedObject2 = new SerializedObject(newObj.transform.GetChild(1).GetChild(1).GetComponent<btnRempInfo>());

                serializedObject2.Update();
                SerializedProperty m_whichDevice = serializedObject2.FindProperty("whichDevice");
                SerializedProperty m_whichType = serializedObject2.FindProperty("whichType");
                SerializedProperty m_whichInput = serializedObject2.FindProperty("whichInput");

                m_whichDevice.intValue = whichInputModeEditor.intValue;
                m_whichInput.intValue = whichInputToCreateEditor.intValue;

                serializedObject2.ApplyModifiedProperties();
            }

           
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }




    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
            "", MessageType.Info);

        EditorGUILayout.HelpBox(
            "", MessageType.Info);

        #endregion
    }





    void OnSceneGUI()
    {
    }
}
#endif
