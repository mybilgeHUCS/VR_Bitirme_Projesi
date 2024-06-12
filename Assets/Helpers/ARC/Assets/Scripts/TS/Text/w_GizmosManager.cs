// Description: window to set up Gizmo global parameters in the project 
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

public class w_GizmosManager : EditorWindow
{
    private Vector2 scrollPosAll;
  
    [MenuItem("Tools/TS/Other/w_GizmosManager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_GizmosManager));
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

    Path path;


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
    }

    

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

        UpdateInfo();


        EditorGUILayout.EndScrollView();
        #endregion
    }

    void UpdateInfo()
    {
        if (path == null)
        {
            path = FindObjectOfType<Path>();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Main Path:", GUILayout.Width(60));

            if(path.gizmoShowPath)
            {
                if (GUILayout.Button("Enable"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(path, "path");
                    path.gizmoShowPath = !path.gizmoShowPath;
                }
            }
            else
            {
                if (GUILayout.Button("Disable"))
                {
                    Undo.RegisterFullObjectHierarchyUndo(path, "path");
                    path.gizmoShowPath = !path.gizmoShowPath;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void HelpZone(int value)
    {
        #region

        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "", MessageType.Info);
                break;
        }
        #endregion
    }

}
#endif