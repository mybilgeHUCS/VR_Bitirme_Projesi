// Description : 
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.SceneManagement;

public class w_Version : EditorWindow
{
    private Vector2 scrollPosAll;
  



    [MenuItem("Tools/TS/Other/w_Version")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Version));
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



    void OnEnable()
    {
        #region

        #endregion
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
        //--> Window description

        Version();

      

        EditorGUILayout.LabelField("");


        EditorGUILayout.BeginHorizontal();
       
        EditorGUILayout.LabelField("Experimental Options: ", GUILayout.Width(120));
        if (GUILayout.Button("On"))
        {
            EditorPrefs.SetBool("MoreOptions", true);
        }
        if (GUILayout.Button("Off"))
        {
            EditorPrefs.SetBool("MoreOptions", false);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
        #endregion
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

   void Version()
    {
        EditorGUILayout.LabelField("Current", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Version 1.0.2");
        EditorGUILayout.LabelField("-[Correction] Enable Limit on Track 01");
        EditorGUILayout.LabelField("-[script] (modifcation) NewPUExample.cs");
        EditorGUILayout.LabelField("-[New] Random Power-ups");


        EditorGUILayout.LabelField("");






        EditorGUILayout.LabelField("Old",EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Version 1.0.1");
        EditorGUILayout.LabelField("-[New] New garage camera");
        EditorGUILayout.LabelField("-[Correction] Leaderboard (Load)");
        EditorGUILayout.LabelField("-[Correction] Invert Up down");

        EditorGUILayout.LabelField("");

        EditorGUILayout.LabelField("Version 1.0");
        EditorGUILayout.LabelField("-First Version");
    }

}
#endif