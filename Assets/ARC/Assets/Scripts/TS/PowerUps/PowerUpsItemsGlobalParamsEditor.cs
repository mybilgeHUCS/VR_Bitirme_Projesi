// Description: PowerUpsItemsGlobalParamsEditor: Custom editor.
// Create a power group. Single power-up.
// Power-up rotation parameters
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(PowerUpsItemsGlobalParams))]
public class PowerUpsItemsGlobalParamsEditor : Editor
{
    SerializedProperty refPowerUpPrefab;
    SerializedProperty listPowerUpPrefab;
    SerializedProperty currentPowerUpSelection;

    string[] listPowerUpsName = new string[5]{ "Repair","Machine Gun","Shield","Mine","Missile"};


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
        refPowerUpPrefab = serializedObject.FindProperty("refPowerUpPrefab");

        listPowerUpPrefab = serializedObject.FindProperty("listPowerUpPrefab");
        currentPowerUpSelection = serializedObject.FindProperty("currentPowerUpSelection");
        #endregion
    }




    public override void OnInspectorGUI()
    {
        #region
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        EditorGUILayout.LabelField("General Parameters:",EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        DrawDefaultInspector();
        EditorGUILayout.EndVertical();

        serializedObject.Update();

        EditorGUILayout.LabelField("");

        CreateNewPowerUpsGroup();

        EditorGUILayout.LabelField("");
        CreateSinglePowerUp();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    void CreateNewPowerUpsGroup()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        EditorGUILayout.LabelField("Create a group of Power-ups:", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Create a new Power-ups Group", GUILayout.Height(30)))
        {
            PowerUpsItemsGlobalParams myScript = (PowerUpsItemsGlobalParams)target;
            GameObject newGroup = PrefabUtility.InstantiatePrefab((GameObject)refPowerUpPrefab.objectReferenceValue, myScript.transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(newGroup, "newGroup");

            GrpPowerUp triggerAILookAtPowerUp = newGroup.GetComponent<GrpPowerUp>();

            TS.Generics.Path path = GameObject.FindObjectOfType<TS.Generics.Path>();

            if (path)
            {
                triggerAILookAtPowerUp.path = path;
            }

            Selection.activeGameObject = newGroup;
        }

        EditorGUILayout.EndVertical();
    }


    void CreateSinglePowerUp()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginVertical(listGUIStyle[2]);
        EditorGUILayout.LabelField("Create a single of Power-up:", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Power-up:", GUILayout.Width(115));
        currentPowerUpSelection.intValue = EditorGUILayout.Popup(currentPowerUpSelection.intValue, listPowerUpsName);


        
    

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create a new Power-up " + "(" + listPowerUpsName[currentPowerUpSelection.intValue] + ")", GUILayout.Height(30)))
        {
            PowerUpsItemsGlobalParams myScript = (PowerUpsItemsGlobalParams)target;
            GameObject newGroup = PrefabUtility.InstantiatePrefab((GameObject)listPowerUpPrefab.GetArrayElementAtIndex(currentPowerUpSelection.intValue).objectReferenceValue, myScript.transform) as GameObject;
            Undo.RegisterCreatedObjectUndo(newGroup, "newGroup");

            Selection.activeGameObject = newGroup;
        }

        EditorGUILayout.EndVertical();
    }


    private void HelpZone_01()
    {
        //PowerUpsItemsGlobalParams myScript = (PowerUpsItemsGlobalParams)target;
        EditorGUILayout.HelpBox(
           "", MessageType.Info);
    }

    void OnSceneGUI()
    {
    }
}
#endif
