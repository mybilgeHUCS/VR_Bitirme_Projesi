// Description: PowerUpsItemsEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(PowerUpsItems))]
public class PowerUpsItemsEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty PowerType;

    PowerUpsDatas powerUpsDatas;
    SerializedProperty listPowerUps;

    List<string> listPowerUpsName = new List<string>();

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
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");

        PowerType = serializedObject.FindProperty("PowerType");


        string objectPath = "Assets/ARC/Assets/Datas/Ref/PowerUpsDatas.asset";
        powerUpsDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as PowerUpsDatas;

        if (powerUpsDatas)
        {
            SerializedObject serializedObject2;
            serializedObject2 = new UnityEditor.SerializedObject(powerUpsDatas);

            listPowerUps = serializedObject2.FindProperty("listPowerUps");

            serializedObject2.ApplyModifiedProperties();

            for (var i = 0; i < listPowerUps.arraySize; i++)
                listPowerUpsName.Add(listPowerUps.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue);

        }


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


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Power-up:", GUILayout.Width(115));
        PowerType.intValue = EditorGUILayout.Popup(PowerType.intValue, listPowerUpsName.ToArray());

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
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
