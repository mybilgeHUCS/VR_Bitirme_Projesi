//Description: SaveManagerEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty WhichDataType;


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
        WhichDataType = serializedObject.FindProperty("WhichDataType");


        SaveManager myScript = (SaveManager)target;
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

        WhichDataType.intValue = EditorGUILayout.Popup(WhichDataType.intValue, new string[] {".Dat","PlayerPrefs" });					// --> Display all scripts

        if(WhichDataType.intValue == 0)
        {
            if (GUILayout.Button("Show .Dat In Explorer"))
            {
                ShowDataInExplorer();
            }
        }
        else
        {
           
        }


        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");

        HelpZone_01();
        #endregion
    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }

    private void HelpZone_01()
    {
        
        EditorGUILayout.HelpBox(
            "Save specific datas and return True after Save Process: \nsaveAndReturnTrueAFterSaveProcess(string s_ObjectsDatas, string s_fileName)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Save specific datas: saveDAT(string s_ObjectsDatas, string s_fileName)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Load specific datas: LoadDAT(string s_fileName)", MessageType.Info);

       
    }





    void OnSceneGUI()
    {
    }
}
#endif
