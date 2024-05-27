// Description:  w_PathGenerator: Window to create the track path
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

public class w_PathGenerator : EditorWindow
{
    private Vector2 scrollPosAll;
    public Path trackPath;

    [MenuItem("Tools/TS/w_PathGenerator")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_PathGenerator));
    }

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

    public string[] listItemType = new string[] { };

    public List<string> _test = new List<string>();
    public int page = 0;
    public int numberOfIndexInAPage = 50;
    public int seachSpecificID = 0;

    public Color _cGreen = new Color(1f, .8f, .4f, 1);
    public Color _cGray = new Color(.9f, .9f, .9f, 1);


    public Texture2D eye;
    public Texture2D currentItemDisplay;
    public int intcurrentItemDisplay = 0;
    public bool b_UpdateProcessDone = false;
    public bool b_AllowUpdateScene = false;



    void OnEnable()
    {
        Init();
    }


    void Init()
    {
        #region Init Inspector Color
        //listColor.Clear();


        Path allPath = FindObjectOfType<Path>();

        if (allPath)
        {
            trackPath = allPath;
            Undo.RegisterFullObjectHierarchyUndo(trackPath.gameObject, "n");
        }
        #endregion
    }


    void InitColorStyle()
    {
        #region Init Inspector Color
        listGUIStyle.Clear();
        listTex.Clear();
        for (var i = 0; i < inspectorColor.listColor.Length; i++)
        {
            listTex.Add(MakeTex(2, 2, inspectorColor.ReturnColor(i)));
            listGUIStyle.Add(new GUIStyle());
            listGUIStyle[i] = new GUIStyle(); listGUIStyle[i].normal.background = listTex[i];
        }
        #endregion
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

        if (listTex.Count == 0 || listTex.Count > 0 && listTex[0] == null)
        {
            InitColorStyle();
        }


        UpdateInfo();
       
        CreateANewCheckpoint();

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndScrollView();
        #endregion
    }

    void UpdateInfo()
    {
        if(!trackPath && !Application.isPlaying)
        {
            Path allPath = FindObjectOfType<Path>();

            if (allPath)
            {
                trackPath = allPath;
            }
        }
    }

    void CreateANewCheckpoint()
    {
        if (trackPath)
        {
            SerializedObject serializedObject0 = new UnityEditor.SerializedObject(trackPath);
            serializedObject0.Update();
            SerializedProperty checkpoints = serializedObject0.FindProperty("checkpoints");
            SerializedProperty prefabCheckpoint = serializedObject0.FindProperty("prefabCheckpoint");

            //-> Add a new checkpoint to the main path
            if (GUILayout.Button("New checkpoint at the end of the list", GUILayout.Height(50)))
            {
                Path myScript = trackPath;
                GameObject newCheckpoint = PrefabUtility.InstantiatePrefab((GameObject)prefabCheckpoint.objectReferenceValue, myScript.transform) as GameObject;
                Undo.RegisterCreatedObjectUndo(newCheckpoint, "newAltPath");

                int pos = Mathf.Clamp(myScript.checkpoints.Count - 1, 0, myScript.checkpoints.Count - 1);

                //Debug.Log("pos: " + pos);
                if (myScript.checkpoints.Count > 0)
                {
                    newCheckpoint.transform.position = myScript.checkpoints[pos].transform.position;
                    newCheckpoint.transform.rotation = myScript.checkpoints[pos].transform.rotation;

                    string _name = (myScript.checkpoints.Count).ToString();
                    if (myScript.checkpoints.Count < 10) _name = "0" + _name;
                    newCheckpoint.name = "cp_" + _name;
                    checkpoints.InsertArrayElementAtIndex(0);
                    checkpoints.GetArrayElementAtIndex(0).objectReferenceValue = newCheckpoint;
                    checkpoints.MoveArrayElement(0, checkpoints.arraySize - 1);
                }
                else
                {
                    newCheckpoint.transform.position = myScript.transform.position;
                    newCheckpoint.transform.rotation = myScript.transform.rotation;
                    checkpoints.InsertArrayElementAtIndex(0);
                    checkpoints.GetArrayElementAtIndex(0).objectReferenceValue = newCheckpoint;
                    newCheckpoint.name = "cp_00";
                }

                Selection.activeGameObject = newCheckpoint;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update Checkpoint Direction"))
            {
                for (var i = 0; i < checkpoints.arraySize; i++)
                {
                    Transform trans01 = checkpoints.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                    Transform trans02 = checkpoints.GetArrayElementAtIndex((i + 1) % checkpoints.arraySize).objectReferenceValue as Transform;

                    Undo.RegisterFullObjectHierarchyUndo(trans01.gameObject, trans01.name);
                    trans01.LookAt(trans02);
                    trans01.localEulerAngles = new Vector3(0, trans01.localEulerAngles.y + 180, 0);
                }

                if (EditorUtility.DisplayDialog("Process Done.",
                "Update Checkpoint Direction Done.", "Continue"))
                {
                }
            }

            if (GUILayout.Button("Rename Checkpoints"))
            {
                for (var i = 0; i < checkpoints.arraySize; i++)
                {

                    Transform trans01 = checkpoints.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                    Transform trans02 = checkpoints.GetArrayElementAtIndex((i + 1) % checkpoints.arraySize).objectReferenceValue as Transform;

                    Undo.RegisterFullObjectHierarchyUndo(trans01.gameObject, trans01.name);
                    string number = "0";
                    if (i > 9) number = "";
                    trans01.name = "cp_" + number + i;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.Width(20));
            EditorGUILayout.LabelField("Checkpoints list:", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < checkpoints.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if(Selection.activeGameObject && Selection.activeGameObject.transform == checkpoints.GetArrayElementAtIndex(i).objectReferenceValue)
                    EditorGUILayout.LabelField(i + ": ", listGUIStyle[5], GUILayout.Width(20));
                else
                    EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));

                if (GUILayout.Button("*", GUILayout.Width(20)))
                {
                   
                    Path myScript = trackPath;
                    if (myScript.checkpoints[i] != Selection.activeObject)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(myScript.checkpoints[i].gameObject, myScript.name);
                        myScript.checkpoints[i].transform.position = Selection.activeTransform.position;
                        myScript.checkpoints[i].transform.rotation = Selection.activeTransform.rotation;
                    }   
                }

                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    Path myScript = trackPath;
                    GameObject newCheckpoint = PrefabUtility.InstantiatePrefab((GameObject)prefabCheckpoint.objectReferenceValue, myScript.transform) as GameObject;
                    Undo.RegisterCreatedObjectUndo(newCheckpoint, "newAltPath");

                    newCheckpoint.transform.position = myScript.checkpoints[i].transform.position;
                    newCheckpoint.transform.rotation = myScript.checkpoints[i].transform.rotation;

                    newCheckpoint.name = myScript.checkpoints[i].name + "b";

                    int childPosition = myScript.checkpoints[i].transform.GetSiblingIndex();
                    newCheckpoint.transform.SetSiblingIndex(childPosition + 1);

                    checkpoints.InsertArrayElementAtIndex(0);
                    checkpoints.GetArrayElementAtIndex(0).objectReferenceValue = newCheckpoint;
                    checkpoints.MoveArrayElement(0, i + 1);

                    Selection.activeGameObject = newCheckpoint;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    Path myScript = trackPath;
                    Selection.activeGameObject = null;
                    Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
                    if (checkpoints.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
                        Undo.DestroyObjectImmediate(myScript.checkpoints[i].gameObject);
                    }

                    myScript.checkpoints.RemoveAt(i);
                    
                    break;
                }

                EditorGUILayout.PropertyField(checkpoints.GetArrayElementAtIndex(i), new GUIContent("")/*,  GUILayout.Width(30)*/);
                EditorGUILayout.LabelField("Alt Path: ", GUILayout.Width(50));

                Transform refTrans = (Transform)checkpoints.GetArrayElementAtIndex(i).objectReferenceValue;

                if (refTrans && refTrans.GetChild(0).childCount > 0)
                {
                    GameObject AltPath = refTrans.GetChild(0).GetChild(0).gameObject;
                    SerializedObject serializedObject1 = new UnityEditor.SerializedObject(AltPath);
                    serializedObject1.Update();
                    SerializedProperty m_IsActive = serializedObject1.FindProperty("m_IsActive");

                    if (m_IsActive.boolValue && GUILayout.Button("", listGUIStyle[5], GUILayout.Width(20)))
                    {
                        m_IsActive.boolValue = !m_IsActive.boolValue;
                    }
                    else if (!m_IsActive.boolValue && GUILayout.Button("", listGUIStyle[0], GUILayout.Width(20)))
                    {
                        m_IsActive.boolValue = !m_IsActive.boolValue;
                    }

                    serializedObject1.ApplyModifiedProperties();

                    if (m_IsActive.boolValue)
                    {
                        if (GUILayout.Button("Select", GUILayout.Width(50)))
                        {
                            Selection.activeGameObject = refTrans.GetChild(0).GetChild(0).gameObject;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(50));
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            serializedObject0.ApplyModifiedProperties();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }



    private void HelpZone(int value)
    {
        #region

        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "This section allows to create checkpoints on the main Path", MessageType.Info);
                break;
        }
        #endregion
    }

}
#endif