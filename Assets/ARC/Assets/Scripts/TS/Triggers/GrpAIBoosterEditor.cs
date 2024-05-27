// Description: GrpAIBoosterEditor
// Allows to create an AI booster (Trigger) in the Hierarchy
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(GrpAIBooster))]
public class GrpAIBoosterEditor : Editor
{
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty trigger_AIBooster;


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
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");

        trigger_AIBooster = serializedObject.FindProperty("trigger_AIBooster");

        #endregion
    }

    public override void OnInspectorGUI()
    {
        #region
        DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));

        if (EditorPrefs.GetBool("MoreOptions") == true)
        {
            EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
            EditorGUILayout.PropertyField(moreOptions, new GUIContent(""), GUILayout.Width(30));
        }
        EditorGUILayout.EndHorizontal();

        CreateNewGrpAIBooster();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    void CreateNewGrpAIBooster()
    {
        if (GUILayout.Button("Create a New AI Booster", GUILayout.Height(30)))
        {
            GrpAIBooster myScript = (GrpAIBooster)target;
            GameObject newGroup = PrefabUtility.InstantiatePrefab((GameObject)trigger_AIBooster.objectReferenceValue, myScript.transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(newGroup, "newGroup");

            newGroup.transform.localScale = new Vector3(30, 30, 3);

            Selection.activeGameObject = newGroup;
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
