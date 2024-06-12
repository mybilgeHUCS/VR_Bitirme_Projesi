//Description: StartLineEditor: Custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;

[CustomEditor(typeof(StartLine))]
public class StartLineEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty StartPosDistanceFromSTart;
    SerializedProperty listOffsetOnGrid;
    SerializedProperty ForwardDistanceFromOtherOneVehicle;

    SerializedProperty countdownDuration;
    SerializedProperty vehicleDistanceEachSecond;


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

    public List<String> StartLineNames = new List<string>();

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
        StartPosDistanceFromSTart = serializedObject.FindProperty("StartPosDistanceFromSTart");
        listOffsetOnGrid = serializedObject.FindProperty("listOffsetOnGrid");
        ForwardDistanceFromOtherOneVehicle = serializedObject.FindProperty("ForwardDistanceFromOtherOneVehicle");
        countdownDuration = serializedObject.FindProperty("countdownDuration");
        vehicleDistanceEachSecond = serializedObject.FindProperty("vehicleDistanceEachSecond");
        #endregion

        StartLineNames = GenerateStartLineNameList();
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

        InitStartLinePosition();
        EditorGUILayout.LabelField("");
        StartLineSize(listGUIStyle[0]);
        EditorGUILayout.LabelField("");
        SetupGrid(listGUIStyle[0]);
        EditorGUILayout.LabelField("");
        ShowCountdownDistance(listGUIStyle[0]);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }


    void InitStartLinePosition()
    {
        StartLine myScript = (StartLine)target;

        if (myScript.Grp_StartLineColliders &&
            myScript.Grp_StartLine_3DModels)
        {
            if (GUILayout.Button("Init StartLine Position"))
            {
                if (!myScript.pathRef)
                {
                    SerializedObject serializedObject4 = new UnityEditor.SerializedObject(myScript);
                    serializedObject4.Update();
                    SerializedProperty m_pathRef = serializedObject4.FindProperty("pathRef");

                    m_pathRef.objectReferenceValue = (PathRef)FindObjectOfType(typeof(PathRef));
                    serializedObject4.ApplyModifiedProperties();
                }

                if (helpBox.boolValue) HelpZone_01();
                if (myScript.pathRef.Track.checkpoints.Count > 0)
                {
                    //-> Move/Rotate: Grp_StartLine
                    SerializedObject serializedObject3 = new UnityEditor.SerializedObject(myScript.GetComponent<Transform>());
                    serializedObject3.Update();
                    SerializedProperty m_LocalPos3 = serializedObject3.FindProperty("m_LocalPosition");
                    SerializedProperty m_LocalRot3 = serializedObject3.FindProperty("m_LocalRotation");

                    m_LocalPos3.vector3Value = Vector3.zero;
                    m_LocalRot3.quaternionValue = Quaternion.identity;

                    serializedObject3.ApplyModifiedProperties();

                    //-> Move/Rotate: Grp_StartLineColliders
                    Undo.RegisterFullObjectHierarchyUndo(myScript.objStartLineColliders.gameObject, myScript.Grp_StartLineColliders.name);
                    myScript.objStartLineColliders.position = myScript.pathRef.Track.checkpoints[0].position;
                    myScript.objStartLineColliders.rotation = myScript.pathRef.Track.checkpoints[0].rotation;

                    //-> Move/Rotate: Grp_StartLine_3DModels
                    Undo.RegisterFullObjectHierarchyUndo(myScript.Grp_StartLine_3DModels.gameObject, myScript.Grp_StartLine_3DModels.name);
                    myScript.Grp_StartLine_3DModels.position = myScript.pathRef.Track.checkpoints[0].position;
                    myScript.Grp_StartLine_3DModels.rotation = myScript.pathRef.Track.checkpoints[0].rotation;

                    //-> Move/Rotate: Buffer ZoneIn
                    Undo.RegisterFullObjectHierarchyUndo(myScript.objBufferZoneIn.gameObject, myScript.objBufferZoneIn.name);
                   
                    Vector3 pos01 = myScript.pathRef.Track.checkpoints[myScript.pathRef.Track.checkpoints.Count - 1].transform.position;
                    Vector3 pos02 = myScript.pathRef.Track.checkpoints[0].transform.position;
                    Vector3 dir = pos02 - pos01;
                    float dist = Vector3.Distance(pos01, pos02);

                    Debug.Log("Dis: "+ dist);
                    Debug.Log("dir: " + dir);

                    myScript.objBufferZoneIn.transform.position = myScript.pathRef.Track.checkpoints[myScript.pathRef.Track.checkpoints.Count - 1].position;
                    myScript.objBufferZoneIn.transform.position += dir.normalized * dist * .2f;
                    myScript.objBufferZoneIn.transform.rotation = myScript.pathRef.Track.checkpoints[myScript.pathRef.Track.checkpoints.Count - 1].transform.GetChild(0).rotation;
               
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
            "Grp_StartLineCollider and/or Grp_StartLine_3DModels are not connected in the Inspector.", MessageType.Error);
        }
    }

    void StartLineSize(GUIStyle style_00)
    {
        EditorGUILayout.BeginVertical(style_00);

        StartLine myScript = (StartLine)target;

        if (myScript.Grp_StartLineColliders)
        {
            SerializedObject serializedObject0 = new UnityEditor.SerializedObject(myScript.Grp_StartLineColliders.GetComponent<Transform>());
            serializedObject0.Update();
            SerializedProperty m_LocalScale = serializedObject0.FindProperty("m_LocalScale");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start Line Collider Size:", GUILayout.Width(150));
            EditorGUILayout.PropertyField(m_LocalScale, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            serializedObject0.ApplyModifiedProperties();

            //-> Grp_StartLineColliders
            SerializedObject serializedObject1 = new UnityEditor.SerializedObject(myScript.objStartLineColliders.GetComponent<Transform>());
            serializedObject1.Update();
            SerializedProperty m_LocalScaleStartLine = serializedObject1.FindProperty("m_LocalScale");
            m_LocalScaleStartLine.vector3Value = m_LocalScale.vector3Value;
            serializedObject1.ApplyModifiedProperties();

            //-> Buffer ZoneIn
            SerializedObject serializedObject2 = new UnityEditor.SerializedObject(myScript.objBufferZoneIn.GetComponent<Transform>());
            serializedObject2.Update();
            SerializedProperty m_LocalScaleBufferIn = serializedObject2.FindProperty("m_LocalScale");
            m_LocalScaleBufferIn.vector3Value = m_LocalScale.vector3Value;
            serializedObject2.ApplyModifiedProperties();

        }
        else
        {
            EditorGUILayout.HelpBox(
            "Grp_StartLineCollider is not connected in the Inspector.", MessageType.Error);
        }

       
        EditorGUILayout.EndVertical();
    }

    void DistanceFromStartLineToTheGrid(GUIStyle style_00)
    {
        EditorGUILayout.BeginVertical(style_00);
        if (helpBox.boolValue) HelpZone_02();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Distance:", GUILayout.Width(85));
        //EditorGUILayout.PropertyField(StartPosDistanceFromSTart, new GUIContent(""), GUILayout.Width(30));

        StartLine myScript = (StartLine)target;
        if (myScript.pathRef.Track.checkpoints.Count > 0)
        {
            float TrackLength = myScript.pathRef.Track.pathLength;
            float LastPosDistance = myScript.pathRef.Track.DistanceFromCheckpointToStart(myScript.pathRef.Track.checkpoints.Count-1);
            StartPosDistanceFromSTart.floatValue = EditorGUILayout.Slider(StartPosDistanceFromSTart.floatValue, 1, TrackLength - LastPosDistance - 1);
        }
        else
        {
            EditorGUILayout.HelpBox(
           "Track must be connected to this script", MessageType.Warning);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    void SetupGrid(GUIStyle style_00)
    {
        EditorGUILayout.BeginVertical(style_00);
        if (helpBox.boolValue) HelpZone_03();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Distance between 2 lines:", GUILayout.Width(150));
        EditorGUILayout.PropertyField(ForwardDistanceFromOtherOneVehicle, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        for(var i = 0;i< listOffsetOnGrid.arraySize; i++)
        {
            if(i % 2 != 0)
                EditorGUILayout.LabelField("Even Position On Grid (right):");
            else
                EditorGUILayout.LabelField("Odd Position On Grid (Left):");
            EditorGUILayout.PropertyField(listOffsetOnGrid.GetArrayElementAtIndex(i), new GUIContent(""));
        }
       
        EditorGUILayout.EndVertical();
    }

    void ShowCountdownDistance(GUIStyle style_00)
    {
        EditorGUILayout.BeginVertical(style_00);
        if (helpBox.boolValue) HelpZone_04();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Countdown duration (s):", GUILayout.Width(150));
        EditorGUILayout.PropertyField(countdownDuration, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("The distance per second:", GUILayout.Width(150));
        EditorGUILayout.PropertyField(vehicleDistanceEachSecond, new GUIContent(""));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }


    List<string> GenerateStartLineNameList()
    {
        List<string> newList = new List<string>();
        StartLineNames.Clear();

        return newList;
    }


    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "Init the Start Line using checkpoint position 0", MessageType.Info);
    }


    private void HelpZone_02()
    {
        EditorGUILayout.HelpBox(
           "Choose the distance between the starting grid and the starting line" + "\n" +
           "The distance can't be bigger than the distance between the last checkpoint and checkpoint 0", MessageType.Info);
    }

    private void HelpZone_03()
    {
        EditorGUILayout.HelpBox(
           "Setup Odd and Even position on the grid", MessageType.Info);
    }

    private void HelpZone_04()
    {
        EditorGUILayout.HelpBox(
           "Show the countdown distance (Blue line)", MessageType.Info);
    }

    void OnSceneGUI()
    {
    }
}
#endif
