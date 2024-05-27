// Description:  w_GlobalParams:(window) Display Global project parameters
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

public class w_GlobalParams : EditorWindow
{
    private Vector2 scrollPosAll;
    SerializedObject serializedObject;
    SerializedProperty helpBox;

    SerializedProperty selectedPlatform;
    SerializedProperty b_MobileUIEnterSelect;
    SerializedProperty b_MobilePageInitSetSelected;
    SerializedProperty b_MobilePageInSetSelected;
    SerializedProperty b_MobilePageOutSetSelected;

    SerializedProperty b_DesktopOtherUIEnterSelect;
    SerializedProperty b_DesktopOtherPageInitSetSelected;
    SerializedProperty b_DesktopOtherPageInSetSelected;
    SerializedProperty b_DesktopOtherPageOutSetSelected;

    SerializedProperty mainMenuScenesInBuildID;

    GlobalDatas globalDatas;

    [MenuItem("Tools/TS/Other/w_GlobalSettings")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_GlobalParams));
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
        #region

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


        string objectPath = "Assets/ARC/Assets/Datas/Ref/globalTSDatas.asset";
        globalDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as GlobalDatas;

        if (globalDatas)
        {
            serializedObject = new UnityEditor.SerializedObject(globalDatas);
            helpBox = serializedObject.FindProperty("helpBox");

            selectedPlatform = serializedObject.FindProperty("selectedPlatform");
            b_MobileUIEnterSelect = serializedObject.FindProperty("b_MobileUIEnterSelect");
            b_MobilePageInitSetSelected = serializedObject.FindProperty("b_MobilePageInitSetSelected");
            b_MobilePageInSetSelected = serializedObject.FindProperty("b_MobilePageInSetSelected");
            b_MobilePageOutSetSelected = serializedObject.FindProperty("b_MobilePageOutSetSelected");


            b_DesktopOtherUIEnterSelect = serializedObject.FindProperty("b_DesktopOtherUIEnterSelect");
            b_DesktopOtherPageInitSetSelected = serializedObject.FindProperty("b_DesktopOtherPageInitSetSelected");
            b_DesktopOtherPageInSetSelected = serializedObject.FindProperty("b_DesktopOtherPageInSetSelected");
            b_DesktopOtherPageOutSetSelected = serializedObject.FindProperty("b_DesktopOtherPageOutSetSelected");
            mainMenuScenesInBuildID = serializedObject.FindProperty("mainMenuScenesInBuildID");
            serializedObject.ApplyModifiedProperties();
        }


        #endregion
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
        
        serializedObject.Update();
       
        GlobalParameters();
       
        EditorGUILayout.LabelField("");

        AccessSavedDataFolder();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();
        #endregion
    }


    void AccessSavedDataFolder()
    {
        EditorGUILayout.LabelField("Access Saved Data Folder:", EditorStyles.boldLabel);
        if (GUILayout.Button("Show .Dat In Explorer"))
        {
            ShowDataInExplorer();
        }

        EditorGUILayout.LabelField("Delete (Data):", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Inputs"))
        {
            InfoInputs infoInputs = FindObjectOfType<InfoInputs>();
            if (!infoInputs)
            {
                if (EditorUtility.DisplayDialog("Action is not possible","There is not InfoInput object in the Hierarchy", "Contine")){}
            }
            else
            {
                if (EditorUtility.DisplayDialog("Delete all the saved files",
                                                        "Are you sure you delete " + infoInputs.SaveName + ".Dat", "Yes", "No"))
                {
                    string itemPath = Application.persistentDataPath;
                    itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                    FileUtil.DeleteFileOrDirectory(itemPath + "/" + infoInputs.SaveName + ".dat");
                }
            }
        }
        if (GUILayout.Button("Audio"))
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (!soundManager)
            {
                if (EditorUtility.DisplayDialog("Action is not possible", "There is not InfoInput object in the Hierarchy", "Contine")) { }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Delete all the saved files",
                                                        "Are you sure you delete " + soundManager.SaveName + ".Dat", "Yes", "No"))
                {
                    string itemPath = Application.persistentDataPath;
                    itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                    FileUtil.DeleteFileOrDirectory(itemPath + "/" + soundManager.SaveName + ".dat");
                }
            }
        }
        if (GUILayout.Button("Player Progression"))
        {
            LoadSavePlayerProgession playerProgression = FindObjectOfType<LoadSavePlayerProgession>();
            if (!playerProgression)
            {
                if (EditorUtility.DisplayDialog("Action is not possible", "There is not InfoInput object in the Hierarchy", "Contine")) { }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Delete all the saved files",
                                                        "Are you sure you delete " + playerProgression.SaveName + ".Dat", "Yes", "No"))
                {
                    for (var i = 0 ;i < 50; i++)
                    {
                        string itemPath = Application.persistentDataPath;
                        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                        FileUtil.DeleteFileOrDirectory(itemPath + "/" + playerProgression.SaveName + i + ".dat");
                    }
                }
            }
        }

        if (GUILayout.Button("Leadearboard"))
        {
            for (var i = 0; i < 50; i++)
            {
                string itemPath = Application.persistentDataPath;
                itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                FileUtil.DeleteFileOrDirectory(itemPath + "/" + "/TL_" + i + ".dat");
            }
        }

        EditorGUILayout.EndHorizontal();
    }
     

    void GlobalParameters()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Global Settings:", EditorStyles.boldLabel, GUILayout.Width(120));
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(60));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");
        UIParams();

        EditorGUILayout.EndVertical();
    }

    void UIParams()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[1]);
       
        EditorGUILayout.LabelField("");

        if (helpBox.boolValue) { HelpZone(4);/* EditorGUILayout.Space();*/}
        if (GUILayout.Button("Update current Scene", GUILayout.Height(30)))
        {
            DesktopMobileObject(selectedPlatform.intValue);
        }

        EditorGUILayout.LabelField("");

        if (selectedPlatform.intValue == 0)
        {
            DesktopOtherOptions();
        }

        if (selectedPlatform.intValue == 1)
        {
            MobileOptions();
        }


        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Main Menu: Scene In Build ID:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(mainMenuScenesInBuildID, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();


        
        EditorGUILayout.EndVertical();
    }

    void MobileOptions()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Mobile UI Options:", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField("|Bypass", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        if (helpBox.boolValue) { EditorGUILayout.Space(); HelpZone(0); }
        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField(new GUIContent("UI Button Navigation:", "Here is a tooltip"), GUILayout.Width(200));
        EditorGUILayout.LabelField("UI Button Navigation:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_MobileUIEnterSelect, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { EditorGUILayout.Space(); HelpZone(1);}
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PageInit SetSelectedGameObject:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_MobilePageInitSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { EditorGUILayout.Space(); HelpZone(2); }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PageIn Set First Select Button:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_MobilePageInSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { EditorGUILayout.Space(); HelpZone(3);}
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pageout Set First Select Button:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_MobilePageOutSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        
    }


    void DesktopOtherOptions()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Desktop | Other UI Options:",EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField("|Bypass", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("UI Button Navigation:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_DesktopOtherUIEnterSelect, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { HelpZone(0); EditorGUILayout.Space(); }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PageInit SetSelectedGameObject:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_DesktopOtherPageInitSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { HelpZone(1); EditorGUILayout.Space(); }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PageIn Set First Select Button:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_DesktopOtherPageInSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { HelpZone(2); EditorGUILayout.Space(); }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pageout Set First Select Button:", GUILayout.Width(200));
        EditorGUILayout.PropertyField(b_DesktopOtherPageOutSetSelected, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
        if (helpBox.boolValue) { HelpZone(3); EditorGUILayout.Space(); }
    }

    static void DesktopMobileObject(int selectedPlatform)
    {
        ObjectsDependingPlatform[] allObject = FindObjectsOfType<ObjectsDependingPlatform>();

        for (var i = 0; i < allObject.Length; i++)
        {

            for (var j = 0; j < allObject[i].listDesktopMobileObjects.Count; j++)
            {
                SerializedObject serializedObject2 = new UnityEditor.SerializedObject(allObject[i]);
                serializedObject2.Update();
                SerializedProperty m_Obj = serializedObject2.FindProperty("listDesktopMobileObjects").GetArrayElementAtIndex(j).FindPropertyRelative("_Obj");

                if (allObject[i].listDesktopMobileObjects[j]._Obj)
                {
                    //SerializedProperty m_IsActive = serializedObject2.FindProperty("listDesktopMobileObjects").GetArrayElementAtIndex(j).FindPropertyRelative("_Obj").FindPropertyRelative("m_IsActive");
                    SerializedProperty m_StateMobile = serializedObject2.FindProperty("listDesktopMobileObjects").GetArrayElementAtIndex(j).FindPropertyRelative("b_Mobile");
                    SerializedProperty m_StateDesktop = serializedObject2.FindProperty("listDesktopMobileObjects").GetArrayElementAtIndex(j).FindPropertyRelative("b_Desktop");

                    SerializedObject serializedObject3 = new UnityEditor.SerializedObject((GameObject)m_Obj.objectReferenceValue);
                    serializedObject3.Update();
                    SerializedProperty m_IsActive = serializedObject3.FindProperty("m_IsActive");

                    SerializedProperty m_State = m_StateMobile;

                    if (selectedPlatform == 0) m_State = m_StateDesktop;

                    //Debug.Log("m_State.boolValue: " + m_State.boolValue);

                    if (m_State.boolValue) m_IsActive.boolValue = true;
                    else m_IsActive.boolValue = false;

                    serializedObject3.ApplyModifiedProperties();
                }
                serializedObject2.ApplyModifiedProperties();
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }



    private void HelpZone(int value)
    {
        #region

        switch (value)
        {
            case 0:
                EditorGUILayout.HelpBox(
                "If False: Events set on ButtonNavigation.cs script are disabled.", MessageType.Info);
                break;
            case 1:
                EditorGUILayout.HelpBox(
                "If False: The method SetSelectedGameObject is disabled on script PageInit.", MessageType.Info);
                break;
            case 2:
                EditorGUILayout.HelpBox(
                "If False: The method Set First Select Button is disabled on script PageIn.", MessageType.Info);
                break;
            case 3:
                EditorGUILayout.HelpBox(
                "If False: The method Set First Select Button is disabled on script PageOut.", MessageType.Info);
                break;
            case 4:
                EditorGUILayout.HelpBox(
                "After a modification. Don't forget to update each scene all your build scenes.", MessageType.Warning);
                break;
        }

        


        #endregion
    }

}
#endif