// Description: w_EditLimit. Window to create game Limit
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

public class w_EditLimit : EditorWindow
{
    private Vector2 scrollPosAll;

    public LimitCollider source;

    [MenuItem("Tools/TS/w_EditLimit")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_EditLimit));
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



    SerializedObject grpLimit;
    SerializedProperty meshList;
    SerializedProperty editorTab;
    

    void OnEnable()
    {
        Init();
    }


    void Init()
    {
        
        LimitCollider objLimit = GameObject.FindObjectOfType<LimitCollider>();
        if (objLimit)
        {
            //
            source = objLimit;
            grpLimit = new UnityEditor.SerializedObject(source);
            meshList = grpLimit.FindProperty("meshList");
            editorTab = grpLimit.FindProperty("editorTab");
        }

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


        if (!source)
            Init();
        else
        {

        }

        source = EditorGUILayout.ObjectField(source, typeof(LimitCollider), true) as LimitCollider;

        if (source)
        {
            editorTab.intValue = GUILayout.SelectionGrid(editorTab.intValue, new string[] { "Path Limit", "Single Collider" }, 2);
            EditorGUILayout.LabelField("");

            switch (editorTab.intValue)
            {
                case 0:
                    grpLimit.Update();

                    //-> General Options
                    SectionOption();

                    //->  Generate 
                    if (GUILayout.Button("Generate Mesh", GUILayout.Height(40)))
                        GenerateMesh(false);

                    if (source.b_EditMode && source.posList.Count > 0)
                        GenerateMesh(true);

                    GenerateCollider();

                    EditorGUILayout.LabelField("");

                    //-> Add Spot
                    AddSpot();

                    EditorGUILayout.LabelField("");

                    //-> Reset
                    if (GUILayout.Button("Reset"))
                    {
                        Reset();
                    }
                      

                    grpLimit.ApplyModifiedProperties();
                    break;
                case 1:
                    CreateSingleCollider();
                    break;
                
            }
        }

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndScrollView();
        #endregion
    }

    void CreateSingleCollider()
    {
        if (GUILayout.Button("Create Single Collider", GUILayout.Height(40)))
        {
            GameObject newSingleLimit  = (GameObject)PrefabUtility.InstantiatePrefab(source.objSingleLimit);
            newSingleLimit.transform.localScale = new Vector3(150, 50);
            Undo.RegisterCreatedObjectUndo(newSingleLimit, newSingleLimit.name);
        }
    }

    void Reset()
    {
        if (source)
        {
            for (var i = 0; i < source.posList.Count; i++)
            {
                if (source.posList[i])
                    Undo.DestroyObjectImmediate(source.posList[i].gameObject);
            }
            source.posList.Clear();

            for (var i = 0; i < source.meshList.Count; i++)
            {
                if (source.meshList[i].objMesh)
                {
                    Undo.RegisterFullObjectHierarchyUndo(source.meshList[i].objMesh, source.meshList[i].objMesh.name);
                    source.meshList[i].objMesh.GetComponent<MeshFilter>().mesh = null;
                    source.meshList[i].objMesh.GetComponent<MeshCollider>().sharedMesh = null;
                }
            }

            source.vertices = new Vector3[0];
        }
    }


    public void SectionOption()
    {
        if (meshList == null)
            Init();
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("|Name ", GUILayout.Width(40));
            EditorGUILayout.LabelField("|Generate ", GUILayout.MinWidth(60));
            EditorGUILayout.LabelField("|Show ", GUILayout.MinWidth(60));
            EditorGUILayout.LabelField("|Obj ", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            for (var i = 0; i < meshList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty name = meshList.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                SerializedProperty objMesh = meshList.GetArrayElementAtIndex(i).FindPropertyRelative("objMesh");
                SerializedProperty bGenerate = meshList.GetArrayElementAtIndex(i).FindPropertyRelative("bGenerate");

                EditorGUILayout.LabelField(name.stringValue, GUILayout.Width(40));

                if (GUILayout.Button(ReturnTorF(bGenerate), GUILayout.MinWidth(60)))
                {
                    bGenerate.boolValue = !bGenerate.boolValue;
                }

                GameObject obj = objMesh.objectReferenceValue as GameObject;
                SerializedObject mh = new UnityEditor.SerializedObject(obj);
                mh.Update();
                SerializedProperty m_IsActive = mh.FindProperty("m_IsActive");
                if (GUILayout.Button(ReturnTorF(m_IsActive), GUILayout.MinWidth(60)))
                {
                    m_IsActive.boolValue = !m_IsActive.boolValue;
                }

                EditorGUILayout.PropertyField(objMesh, new GUIContent(""), GUILayout.Width(50));

                mh.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("");
        }

        EditorGUILayout.BeginHorizontal();
        SerializedProperty limitWidth = meshList.GetArrayElementAtIndex(0).FindPropertyRelative("distanceToPath");
        SerializedProperty limiHeight = meshList.GetArrayElementAtIndex(1).FindPropertyRelative("distanceToPath");
        EditorGUILayout.LabelField("|Path width:", GUILayout.MinWidth(80));
        EditorGUILayout.PropertyField(limitWidth, new GUIContent(""), GUILayout.MinWidth(50));
        EditorGUILayout.LabelField("|Path height:", GUILayout.MinWidth(80));
        EditorGUILayout.PropertyField(limiHeight, new GUIContent(""), GUILayout.MinWidth(50));
        EditorGUILayout.EndHorizontal();
    }

    string ReturnTorF(SerializedProperty prop)
    {
        if (!prop.boolValue) return "False";
        return "True";
    }

    public void AddSpot()
    {
        if (GUILayout.Button("Add Spot", GUILayout.Height(40)))
        {
            if (Selection.activeTransform)
            {
                for (var i = 0; i < source.posList.Count; i++)
                {
                    if (source.posList[i] == Selection.activeTransform)
                    {
                        int howManySpots = source.posList.Count / 4;

                        List<GameObject> tmpList = new List<GameObject>();
                        for (var j = 0; j < 4; j++)
                        {
                            GameObject newSpot = (GameObject)PrefabUtility.InstantiatePrefab(source.refSpot, source.transform);

                            int pos_01 = i % howManySpots + howManySpots * j;
                            int pos_02 = i % howManySpots + 1 + howManySpots * j;

                            Vector3 dir = source.posList[pos_02 % source.posList.Count].position - source.posList[pos_01].position;

                            float dist = Vector3.Distance(source.posList[pos_01].position, source.posList[pos_02 % source.posList.Count].position);

                            newSpot.transform.position = source.posList[pos_01 % source.posList.Count].position + dir.normalized * dist * .5f;

                            newSpot.name = "spot_Ploup_" + i;
                            newSpot.transform.LookAt(source.path.checkpoints[(i + 1) % howManySpots].position);

                            newSpot.transform.localEulerAngles = new Vector3(
                            0,
                            newSpot.transform.localEulerAngles.y,
                            newSpot.transform.localEulerAngles.z);

                            switch (j)
                            {
                                case 0:
                                    newSpot.name = "spot_0";
                                    newSpot.transform.SetSiblingIndex(pos_01 + 1 );
                                    break;
                                case 1:
                                    newSpot.name = "spot_1";
                                    newSpot.transform.SetSiblingIndex(pos_01 + 2);
                                    break;
                                case 2:
                                    newSpot.name = "spot_2";
                                    newSpot.transform.SetSiblingIndex(pos_01 + 3);
                                    break;
                                case 3:
                                    newSpot.name = "spot_3";
                                    newSpot.transform.SetSiblingIndex(pos_01 + 4);
                                    break;
                            }

                            Undo.RegisterCreatedObjectUndo(newSpot, "spot_Ploup_" + i);
                            tmpList.Add(newSpot.gameObject);
                        }


                        Undo.RegisterFullObjectHierarchyUndo(source.gameObject,source.name);
                        source.posList.Clear();
                        for (var j = 0; j < source.transform.childCount; j++)
                        {
                            source.posList.Add(source.transform.GetChild(j));
                        }

                        break;
                    }
                }
            }
        }
    }

    public void GenerateCollider()
    {
        if (GUILayout.Button("Generate Collider", GUILayout.Height(40)))
        {
            for (var k = 0; k < source.meshList.Count; k++)
            {
                // Create the mesh
                MeshCollider meshCol = source.meshList[k].objMesh.GetComponent<MeshCollider>();
                meshCol.sharedMesh = source.meshList[k].objMesh.GetComponent<MeshFilter>().sharedMesh;
            }

            if (EditorUtility.DisplayDialog("Generate Collider","Done", "Continue"))
            {}
        }
    }

    public void GenerateMesh(bool bEdit)
    {
        int howManySpots = source.path.checkpoints.Count;
        source.spots = howManySpots - 1;


        if (!bEdit)
        {
            for (var i = 0; i < source.posList.Count; i++)
            {
                if (source.posList[i])
                    Undo.DestroyObjectImmediate(source.posList[i].gameObject);
            }

            source.posList.Clear();

            for(var j = 0; j < 4; j++)
            {
                for (var i = 0; i < source.path.checkpoints.Count; i++)
                {
                    GameObject newSpot = (GameObject)PrefabUtility.InstantiatePrefab(source.refSpot, source.transform);

                    newSpot.transform.position = source.path.checkpoints[i].position;
                    newSpot.transform.LookAt(source.path.checkpoints[(i + 1) % howManySpots].position);
                    newSpot.transform.localEulerAngles = new Vector3(
                        0,
                        newSpot.transform.localEulerAngles.y,
                        newSpot.transform.localEulerAngles.z);

                    switch (j)
                    {
                        case 0:
                            newSpot.name = "spot_0";
                            newSpot.transform.localPosition += newSpot.transform.right * -source.meshList[0].distanceToPath;
                            break;
                        case 1:
                            newSpot.name = "spot_1";
                            newSpot.transform.localPosition += newSpot.transform.right * -source.meshList[0].distanceToPath + newSpot.transform.up * source.meshList[1].distanceToPath;
                            break;
                        case 2:
                            newSpot.name = "spot_2";
                            newSpot.transform.localPosition += newSpot.transform.right * source.meshList[0].distanceToPath + newSpot.transform.up * source.meshList[1].distanceToPath;
                            break;
                        case 3:
                            newSpot.name = "spot_3";
                            newSpot.transform.localPosition += newSpot.transform.right * source.meshList[0].distanceToPath;
                            break;
                    }
                    
                    source.posList.Add(newSpot.transform);
                    Undo.RegisterCreatedObjectUndo(newSpot, "spot_" + i);
                }
            }
        }

        if (source.posList.Count > 1)
        {
            howManySpots = source.posList.Count / 4;
            source.spots = howManySpots - 1;
            for (var k = 0; k < 4; k++)
            {
                // Create the mesh
                Mesh mesh = source.meshList[k].objMesh.GetComponent<Mesh>();
                source.meshList[k].objMesh.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
                mesh.name = "Limit" + k;


                //-> Create the vertices array
                source.vertices = new Vector3[(howManySpots + 1) * 2];


                //Debug.Log("vertices: " + source.vertices.Length);
                int counter = (howManySpots) * k;

                for (int x = 0; x <= howManySpots * 2; x++)
                {
                    //Debug.Log("counter: " + counter);
                    source.vertices[x] = new Vector3(
                        -source.transform.position.x + source.posList[counter % source.posList.Count].position.x,
                        -source.transform.position.y + source.posList[counter % source.posList.Count].position.y,
                        -source.transform.position.z + source.posList[counter % source.posList.Count].position.z);

                    counter++;
                }

                mesh.vertices = source.vertices;


                //-> Create Triangles
                int[] triangles = new int[(source.spots + 2) * 6];
                for (var i = 0; i <= source.spots; i++)
                {
                    //Debug.Log("spots: " + i + " -> " + (i * 6).ToString());
                    if (i < source.spots)
                    {
                        //First Triangle
                        triangles[0 + i * 6] = i;
                        triangles[1 + i * 6] = i + source.spots + 1;
                        triangles[2 + i * 6] = i + 1;
                        //Second Triangle
                        triangles[3 + i * 6] = i + 1;
                        triangles[4 + i * 6] = i + source.spots + 1;
                        triangles[5 + i * 6] = i + source.spots + 2;
                    }
                    else
                    {
                        //First Triangle
                        triangles[0 + i * 6] = source.spots;
                        triangles[1 + i * 6] = source.spots + source.spots + 1;
                        triangles[2 + i * 6] = 0;
                        //Second Triangle
                        triangles[3 + i * 6] = 0;
                        triangles[4 + i * 6] = source.spots + source.spots + 1;
                        triangles[5 + i * 6] = source.spots + 1;
                    }

                }
                mesh.triangles = triangles;
            }
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
            case 1:
                EditorGUILayout.HelpBox(
                "Grp_Limit doesn't exist in the Hierarchy. This window only works in a gameplay scene.", MessageType.Warning);
                break;
        }
        #endregion
    }

}
#endif