//Description : PowerUpsSystemEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(PowerUpsSystem))]
public class PowerUpsSystemEditor : Editor
{
    SerializedProperty puInitUIList;
    SerializedProperty objPUInitList;
    SerializedProperty puDisableList;
    SerializedProperty puAIUpdateList;
    SerializedProperty puplayerUpdateList;
    SerializedProperty puOnTriggerEnterList;
    SerializedProperty puAllowToChangePUList;
    SerializedProperty tab;
    // Init Power-up UI
    // Init All Power-up
    // Disable All Power-up
    // Update AI Power-up
    // Update Player Power-up
    // OnTriggerEnter Power-up


    string[] tabName = new string[7] {
        "0: Init UI",
        "1: Init All Power-ups",
        "2: Disable All Power-ups",
        "3: Update AI Power-ups",
        "4: Update Player Power-up",
        "5: OnTriggerEnter Power-ups",
        "6: Allow To Change Power-Ups"};


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
        puInitUIList = serializedObject.FindProperty("puInitUIList");
        objPUInitList = serializedObject.FindProperty("objPUInitList");
        puDisableList = serializedObject.FindProperty("puDisableList");
        puAIUpdateList = serializedObject.FindProperty("puAIUpdateList");
        puplayerUpdateList = serializedObject.FindProperty("puplayerUpdateList");
        puOnTriggerEnterList = serializedObject.FindProperty("puOnTriggerEnterList");
        puAllowToChangePUList = serializedObject.FindProperty("puAllowToChangePUList");
        tab = serializedObject.FindProperty("editorTab");
        
        #endregion
    }




    public override void OnInspectorGUI()
    {
        #region
        DrawDefaultInspector();

        EditorGUILayout.LabelField("");

        OtherParams();


        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    void OtherParams()
    {

        tab.intValue = EditorGUILayout.Popup(tab.intValue, tabName, GUILayout.MinWidth(100));				// --> Display all methods


        switch (tab.intValue)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "Call these methods when the scene starts to initialize UI for each Power-up type.", MessageType.Info);
                DisplaySP(puInitUIList);
                break;
            case 1:
                EditorGUILayout.HelpBox(
                "Call these methods when all Power-up types are initialized during the game.", MessageType.Info);
                DisplaySP(objPUInitList);
                break;
            case 2:
                EditorGUILayout.HelpBox(
                "Call these methods when all Power-up types are Disabled (Reset) during the game.", MessageType.Info);
                DisplaySP(puDisableList);
                break;
            case 3:
                EditorGUILayout.HelpBox(
                "Call these methods when all Power-up types are initialized during the game.", MessageType.Info);
                DisplaySP(puAIUpdateList);
                break;
            case 4:
                EditorGUILayout.HelpBox(
                "Call one of these methods when a Power-up is selected by an AI.", MessageType.Info);
                DisplaySP(puplayerUpdateList);
                break;
            case 5:
                EditorGUILayout.HelpBox(
                "Call one of these methods when a Power-up is selected by a Player.", MessageType.Info);
                DisplaySP(puOnTriggerEnterList);
                break;
            case 6:
                EditorGUILayout.HelpBox(
                "Call these methods to check if the vehicle is allowed to choose a power-up.", MessageType.Info);
                DisplaySP(puAllowToChangePUList);
                break;
        }
    }

    void DisplaySP(SerializedProperty sp)
    {
        for(var i = 0;i< sp.arraySize; i++)
        {
            SerializedProperty name = sp.GetArrayElementAtIndex(i).FindPropertyRelative("name");
            SerializedProperty objSelectionsRules = sp.GetArrayElementAtIndex(i).FindPropertyRelative("objSelectionsRules");
            SerializedProperty ID = sp.GetArrayElementAtIndex(i).FindPropertyRelative("ID");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(name, new GUIContent(""), GUILayout.MinWidth(120));
            EditorGUILayout.LabelField("ID:", GUILayout.Width(20));
            EditorGUILayout.PropertyField(ID, new GUIContent(""), GUILayout.Width(30));
            EditorGUILayout.PropertyField(objSelectionsRules, new GUIContent(""), GUILayout.MinWidth(50));

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                sp.DeleteArrayElementAtIndex(i);
                break;
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                sp.InsertArrayElementAtIndex(i);
                sp.GetArrayElementAtIndex(i + 1).FindPropertyRelative("name").stringValue = "";
                sp.GetArrayElementAtIndex(i + 1).FindPropertyRelative("objSelectionsRules").objectReferenceValue = null;
                sp.GetArrayElementAtIndex(i + 1).FindPropertyRelative("ID").intValue = 0;

                break;
            }
            if (GUILayout.Button("^", GUILayout.Width(20)))
            {
                sp.MoveArrayElement(i, Mathf.Clamp(i - 1, 0, sp.arraySize - 1));
                break;
            }
            if (GUILayout.Button("v", GUILayout.Width(20)))
            {
                sp.MoveArrayElement(i, Mathf.Clamp(i + 1, 0, sp.arraySize - 1));
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }


    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "", MessageType.Info);
    }

    void OnSceneGUI()
    {
    }
}
#endif

