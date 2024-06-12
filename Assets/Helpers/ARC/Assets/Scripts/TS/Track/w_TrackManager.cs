// Description : w_TrackManager.cs :  window to manage tracks
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;
using System.IO;

public class w_TrackManager : EditorWindow
{
    private Vector2         scrollPosAll;
    TracksData              _TracksData;
    SerializedObject        serializedObject;
    SerializedProperty      HelpBox;
    SerializedProperty      MoreOptions;
    SerializedProperty      listTrackParams;

    globalTextDatas         _globalTextDatas;
    SerializedObject         serializedObjectTxtRef;

    PlayerMainMenuSelection _PlayerMainMenuSelection;
    SerializedObject        soPlayerMMSelec;
    SerializedProperty      HowManyPlayer;
    SerializedProperty currentGameMode;
    SerializedProperty howManyVehicleInSelectedGameMode;
    SerializedProperty currentDifficulty;

    //-> Difficulty Manager
    SerializedObject serializedObjectDiffManager;
    SerializedProperty difficultyParamsList;
    DifficultyManagerData difficultyManagerData;
    public List<String> difficultyNamesList = new List<string>();


    [MenuItem("Tools/TS/w_TrackManager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_TrackManager));
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
        //-> Access Track data
        string objectPath = "Assets/ARC/Assets/Datas/Ref/TracksData.asset";
        _TracksData = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as TracksData;

       
        if (_TracksData) {
            serializedObject = new UnityEditor.SerializedObject(_TracksData);
            HelpBox = serializedObject.FindProperty("HelpBox");
            MoreOptions = serializedObject.FindProperty("MoreOptions");
            listTrackParams = serializedObject.FindProperty("listTrackParams");
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        //-> Access Multi Language texts
        string objectPathTxtData = "Assets/ARC/Assets/Datas/Ref/globalTextDatas.asset";
        _globalTextDatas = AssetDatabase.LoadAssetAtPath(objectPathTxtData, typeof(UnityEngine.Object)) as globalTextDatas;

        if (_globalTextDatas)
        {
            serializedObjectTxtRef = new UnityEditor.SerializedObject(_globalTextDatas);
            serializedObjectTxtRef.Update();
            serializedObjectTxtRef.ApplyModifiedProperties();
        }

        objectPath = "Assets/ARC/Assets/Datas/Ref/DifficultyManagerData.asset";
        difficultyManagerData = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as DifficultyManagerData;

        if (difficultyManagerData)
        {
            serializedObjectDiffManager = new UnityEditor.SerializedObject(difficultyManagerData);

            difficultyParamsList = serializedObjectDiffManager.FindProperty("difficultyParamsList");
            serializedObjectDiffManager.Update();
            difficultyNamesList = GenerateDifficultyNamesListList();
            serializedObjectDiffManager.ApplyModifiedProperties();
        }



        objectPath = "Assets/ARC/Assets/Datas/Ref/PlayerMainMenuSelection.asset";
        _PlayerMainMenuSelection = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as PlayerMainMenuSelection;

        if (_PlayerMainMenuSelection)
        {
            soPlayerMMSelec = new UnityEditor.SerializedObject(_PlayerMainMenuSelection);

            HowManyPlayer = soPlayerMMSelec.FindProperty("HowManyPlayer");
            currentGameMode = soPlayerMMSelec.FindProperty("currentGameMode");
            howManyVehicleInSelectedGameMode = soPlayerMMSelec.FindProperty("howManyVehicleInSelectedGameMode");
            currentDifficulty = soPlayerMMSelec.FindProperty("currentDifficulty");

            soPlayerMMSelec.Update();
            soPlayerMMSelec.ApplyModifiedProperties();
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
            listGUIStyle[i] = new GUIStyle(); listGUIStyle[i].normal.background =listTex[i];
        }
        #endregion
    }


    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

        serializedObject.Update();
        if (_TracksData && _globalTextDatas)
        {
            if (listTex.Count == 0 ||
                listTex.Count > 0 && listTex[0] == null)
            {
                InitColorStyle();
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
            EditorGUILayout.PropertyField(HelpBox, new GUIContent(""), GUILayout.Width(30));

            if (EditorPrefs.GetBool("MoreOptions") == true)
            {
                EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
                EditorGUILayout.PropertyField(MoreOptions, new GUIContent(""), GUILayout.Width(30));
            }
          
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            SerializedProperty m_tab = serializedObject.FindProperty("tab");
            m_tab.intValue = GUILayout.Toolbar(m_tab.intValue, new string[] { "Global Params", "Tracks Params" }, GUILayout.MinWidth(30));

            EditorGUILayout.LabelField("");

            switch (m_tab.intValue)
            {
                case 0:
                    SerializedProperty m_showGlobalParamsInEditor = serializedObject.FindProperty("showGlobalParamsInEditor");
                    EditorGUILayout.BeginVertical(listGUIStyle[0]);
                    EditorGUILayout.BeginHorizontal(listGUIStyle[3]);
                    EditorGUILayout.PropertyField(m_showGlobalParamsInEditor, new GUIContent(""), GUILayout.Width(20));
                    EditorGUILayout.LabelField("Global Parameters:", EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();

                    CurrentRaceInfo(m_showGlobalParamsInEditor.boolValue);

                    EditorGUILayout.EndVertical();
                    break;
                case 1:
                    ShowDataAndReset();
                    ShowItemsList();
                    AddATrackToTheList();
                    break;
            }

            EditorGUILayout.LabelField("");           
        }

        if (difficultyManagerData)
        {
            serializedObjectDiffManager.Update();
            difficultyNamesList = GenerateDifficultyNamesListList();
            serializedObjectDiffManager.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
        #endregion
    }


    void CurrentRaceInfo(bool bShow)
    {
        if (bShow)
        {
            soPlayerMMSelec.Update();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Game Mode:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(currentGameMode, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            if (HelpBox.boolValue)
            {
                EditorGUILayout.HelpBox("Game Mode:" +
                "\n0: Arcade Mode" +
                "\n1: Time Trail Mode" +
                "\n2: Championship Mode" +
                "\n3: Test (Play with AI. A path must be set up)" +
                "\n4: Main Menu Mode" +
                "\n5: Test (P1 + No collision. No Path needed)", MessageType.Info);
                EditorGUILayout.LabelField("");
            }
               

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("How Many Player:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(HowManyPlayer, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            if(HowManyPlayer.intValue > howManyVehicleInSelectedGameMode.intValue)
            EditorGUILayout.HelpBox("HowManyPlayer must not be > howManyVehicleInSelectedGameMode", MessageType.Error);

            if (HelpBox.boolValue)
            {
                EditorGUILayout.HelpBox("1: P1 | 2: P1 + P2 (Spiltscreen)", MessageType.Info);
                EditorGUILayout.LabelField("");
            }
               

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Vehicles (Total):", GUILayout.Width(120));
            EditorGUILayout.PropertyField(howManyVehicleInSelectedGameMode, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            if (HelpBox.boolValue) {
                EditorGUILayout.HelpBox("Choose the number of vehicle Players + AIs", MessageType.Info);
                EditorGUILayout.LabelField("");
            }
                

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Difficulty:", GUILayout.Width(120));
            EditorGUILayout.PropertyField(currentDifficulty, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            if (HelpBox.boolValue) {
                EditorGUILayout.HelpBox("Choose AI difficulty: " +
                    "\n0: Easy" +
                    "\n1: Medium" +
                    "\n2: Hard", MessageType.Info);
            }
               
            soPlayerMMSelec.ApplyModifiedProperties();
        }
    }

    void GlobalTrackParameters()
    {
        SerializedProperty m_showGlobalParamsInEditor = serializedObject.FindProperty("showGlobalParamsInEditor");

        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginHorizontal(listGUIStyle[3]);
        EditorGUILayout.PropertyField(m_showGlobalParamsInEditor, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.LabelField("Global Parameters:", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();


        if (m_showGlobalParamsInEditor.boolValue)
        {
            SerializedProperty m_listCoinMultiplier = serializedObject.FindProperty("listCoinMultiplier");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                m_listCoinMultiplier.InsertArrayElementAtIndex(0);
                m_listCoinMultiplier.MoveArrayElement(0, m_listCoinMultiplier.arraySize - 1);
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (m_listCoinMultiplier.arraySize > 0)
                    m_listCoinMultiplier.DeleteArrayElementAtIndex(Mathf.Clamp(m_listCoinMultiplier.arraySize - 1, 0, m_listCoinMultiplier.arraySize - 1));
            }
            EditorGUILayout.LabelField("Difficulty Multiplier", GUILayout.MinWidth(120));

            EditorGUILayout.EndHorizontal();


            //-> Arcade Mode: Display Coins depending position
            for (var j = 0; j < m_listCoinMultiplier.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Difficulty " + j + ":", GUILayout.Width(70));
                EditorGUILayout.PropertyField(m_listCoinMultiplier.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.MinWidth(40));
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void ShowDataAndReset()
    {
        if (GUILayout.Button("Show .Dat In Explorer"))
        {
            ShowDataInExplorer();
        }

        EditorGUILayout.BeginHorizontal();
 
        if (GUILayout.Button("Delete Player Progression + Leaderboards"))
        {
            if (EditorUtility.DisplayDialog("Delete Player Progression + Leaderboards", "Are you sure?", "Yes", "No"))
            {
                DeleteData();
            }

        }
        EditorGUILayout.EndHorizontal();


       
    }

    public void DeleteData()
    {
        int howManySaveSlot = 3;
        for (var i = 0; i < howManySaveSlot; i++)
        {
            //-> Delete Player Progression
            string filePath = Application.persistentDataPath + "/PP_" + i + ".dat";

            //-> Delete .Dat
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                UnityEditor.AssetDatabase.Refresh();
            }

            //-> Delete Player Prefs
            if (PlayerPrefs.HasKey("PP_" + i))
                PlayerPrefs.DeleteKey("PP_" + i);
        }


        //-> Delete Leaderboards
        for (var i = 0; i < howManySaveSlot; i++)
        {
            //-> Delete Player Progression
            string itemPath = Application.persistentDataPath + "/TL_" + i + ".dat";

            //-> Delete .Dat
            if (File.Exists(itemPath))
            {
                itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                FileUtil.DeleteFileOrDirectory(itemPath);
                UnityEditor.AssetDatabase.Refresh();
            }

            //-> Delete Player Prefs
            if (PlayerPrefs.HasKey("TL_" + i))
                PlayerPrefs.DeleteKey("TL_" + i);
        }
    }


    //-> Create a new track at the end of the list
    public void AddATrackToTheList()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[3]);
        if (GUILayout.Button("Add a new Track to the List", GUILayout.Height(50)))
        {
            listTrackParams.InsertArrayElementAtIndex(listTrackParams.arraySize);
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("listLeadeboardEntries").ClearArray();
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("selectedListMultiLanguage").intValue = 0;
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("NameIDMultiLanguage").intValue = 71;
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("sceneName").stringValue = "Scene Name";
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("aIDifficulty").intValue = 0;
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("trackSprite").objectReferenceValue = null;
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("fullScreenSprite").objectReferenceValue = null;
            
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("showInEditor").boolValue = true;
            listTrackParams.GetArrayElementAtIndex(listTrackParams.arraySize - 1).FindPropertyRelative("showLeaderboardInEditor").boolValue = true;
        }
        
        EditorGUILayout.EndVertical();
    }

    void ShowItemsList()
    {
        #region
        for (var i = 0; i < listTrackParams.arraySize; i++)
        {
            SerializedProperty m_selectedListMultiLanguage = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("selectedListMultiLanguage");
            SerializedProperty m_NameIDMultiLanguage = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("NameIDMultiLanguage");
            SerializedProperty m_sceneName = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("sceneName");
            SerializedProperty m_isUnlockedArcade = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("isUnlockedArcade");
            SerializedProperty m_isUnlockedTimeTrial = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("isUnlockedTimeTrial");
            
            SerializedProperty m_trackSprite = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("trackSprite");
            SerializedProperty m_fullScreenSprite = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("fullScreenSprite");
            
            SerializedProperty m_listLeadeboardEntries = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("listLeadeboardEntries");
            SerializedProperty m_showLeaderboardInEditor = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("showLeaderboardInEditor");
            SerializedProperty m_showInEditor = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("showInEditor");
            SerializedProperty m_showCoinsInEditor = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("showCoinsInEditor");
            SerializedProperty m_listArcadeCoins = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("listArcadeCoins");
            SerializedProperty m_listTimeTrialCoins = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("listTimeTrialCoins");

            SerializedProperty m_ListTexts = listTrackParams.GetArrayElementAtIndex(i).FindPropertyRelative("listTexts");

            EditorGUILayout.BeginVertical(listGUIStyle[0]);
            EditorGUILayout.BeginHorizontal(listGUIStyle[3]);
            EditorGUILayout.LabelField(i + ":" , GUILayout.Width(15));
            EditorGUILayout.PropertyField(m_showInEditor, new GUIContent(""), GUILayout.Width(20));

            //-> Select the name of the track using the multilanguage system
            string sName = ReturnTrackName(m_selectedListMultiLanguage.intValue, m_NameIDMultiLanguage.intValue);
            EditorGUILayout.LabelField(sName, EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                listTrackParams.DeleteArrayElementAtIndex(i);
                DeleteData();
                break;
            }
            EditorGUILayout.EndHorizontal();

           
            if (m_showInEditor.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text: ", GUILayout.Width(40));
                EditorGUILayout.LabelField("List: ", GUILayout.Width(38));
                EditorGUILayout.PropertyField(m_selectedListMultiLanguage, new GUIContent(""));
                EditorGUILayout.LabelField("ID: ", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_NameIDMultiLanguage, new GUIContent(""));
                if (GUILayout.Button("TextEditor"))
                {
                    EditorWindow.GetWindow(typeof(w_TextCreator));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Scene Name: ", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_sceneName, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(listGUIStyle[1]);
                EditorGUILayout.LabelField("", GUILayout.Height(.3f));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unlocked: ", GUILayout.Width(80));
                EditorGUILayout.LabelField("Arcade: ", GUILayout.Width(50));
                EditorGUILayout.PropertyField(m_isUnlockedArcade, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Time Trial: ", GUILayout.Width(60));
                EditorGUILayout.PropertyField(m_isUnlockedTimeTrial, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                //-> Display Unlock Text
                EditorGUILayout.BeginHorizontal();
                sName = ReturnTrackName(
                m_ListTexts.GetArrayElementAtIndex(1).FindPropertyRelative("listID").intValue,
                m_ListTexts.GetArrayElementAtIndex(1).FindPropertyRelative("EntryID").intValue);

                EditorGUILayout.LabelField("Unlock Text: ", GUILayout.Width(80));
                EditorGUILayout.LabelField(sName, EditorStyles.textArea);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text: ", GUILayout.Width(40));
                EditorGUILayout.LabelField("List: ", GUILayout.Width(38));
                EditorGUILayout.PropertyField(m_ListTexts.GetArrayElementAtIndex(1).FindPropertyRelative("listID"), new GUIContent(""));
                EditorGUILayout.LabelField("ID: ", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_ListTexts.GetArrayElementAtIndex(1).FindPropertyRelative("EntryID"), new GUIContent(""));
                if (GUILayout.Button("TextEditor"))
                {
                    EditorWindow.GetWindow(typeof(w_TextCreator));
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(listGUIStyle[1]);
                EditorGUILayout.LabelField("", GUILayout.Height(.3f));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Thumb Sprite: ", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_trackSprite, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Full Sprite: ", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_fullScreenSprite, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                //-> Display Leaderboard
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_showLeaderboardInEditor, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Leaderboard", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
                if (m_showLeaderboardInEditor.boolValue)
                {                
                    if (m_listLeadeboardEntries.arraySize == 0 && GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        m_listLeadeboardEntries.InsertArrayElementAtIndex(0);
                        break;
                    }

                    for (var j = 0; j < m_listLeadeboardEntries.arraySize; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            m_listLeadeboardEntries.InsertArrayElementAtIndex(j);
                            break;
                        }
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            m_listLeadeboardEntries.DeleteArrayElementAtIndex(j);
                            break;
                        }
                        EditorGUILayout.PropertyField(m_listLeadeboardEntries.GetArrayElementAtIndex(j).FindPropertyRelative("name"), new GUIContent(""), GUILayout.Width(100));
                     
                        ShowConvertissor(m_listLeadeboardEntries.GetArrayElementAtIndex(j));

                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(listGUIStyle[0]);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                //-> Display Coins
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_showCoinsInEditor, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Coins", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
                if (m_showCoinsInEditor.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Default Value"))
                    {
                        // Arcade
                        SerializedProperty m_ArcadeDefaultCoin = serializedObject.FindProperty("listArcadeCoins");
                        m_listArcadeCoins.ClearArray();

                        for(int k = 0;k< m_ArcadeDefaultCoin.arraySize;k++)
                            m_listArcadeCoins.InsertArrayElementAtIndex(0);

                        for (int k = 0; k < m_ArcadeDefaultCoin.arraySize; k++)
                            m_listArcadeCoins.GetArrayElementAtIndex(k).intValue = m_ArcadeDefaultCoin.GetArrayElementAtIndex(k).intValue;


                        // Time Trial
                        SerializedProperty m_TTDefaultCoin = serializedObject.FindProperty("listTimeTrialCoins");
                        m_listTimeTrialCoins.ClearArray();

                        for (int k = 0; k < m_TTDefaultCoin.arraySize; k++)
                            m_listTimeTrialCoins.InsertArrayElementAtIndex(0);

                        for (int k = 0; k < m_TTDefaultCoin.arraySize; k++)
                            m_listTimeTrialCoins.GetArrayElementAtIndex(k).intValue = m_TTDefaultCoin.GetArrayElementAtIndex(k).intValue;
                    }
                    if (GUILayout.Button("Clear"))
                    {
                        m_listArcadeCoins.ClearArray();
                        m_listTimeTrialCoins.ClearArray();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginVertical(listGUIStyle[0]);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        m_listArcadeCoins.InsertArrayElementAtIndex(0);
                        m_listArcadeCoins.MoveArrayElement(0,m_listArcadeCoins.arraySize-1);
                        break;
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                      if(m_listArcadeCoins.arraySize>0)
                            m_listArcadeCoins.DeleteArrayElementAtIndex(Mathf.Clamp(m_listArcadeCoins.arraySize - 1,0, m_listArcadeCoins.arraySize - 1));
                        break;
                    }
                    EditorGUILayout.LabelField("Arcade", GUILayout.MinWidth(70));
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    
                    //-> Arcade Mode: Display Coins depending position
                    for (var j = 0; j < m_listArcadeCoins.arraySize; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Pos " + (j + 1) + ":", GUILayout.Width(40));
                        EditorGUILayout.PropertyField(m_listArcadeCoins.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.MinWidth(40));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginVertical(listGUIStyle[0]);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        m_listTimeTrialCoins.InsertArrayElementAtIndex(0);
                        m_listTimeTrialCoins.MoveArrayElement(0, m_listTimeTrialCoins.arraySize - 1);
                        break;
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        if (m_listTimeTrialCoins.arraySize > 0)
                            m_listTimeTrialCoins.DeleteArrayElementAtIndex(Mathf.Clamp(m_listTimeTrialCoins.arraySize - 1, 0, m_listTimeTrialCoins.arraySize - 1));
                        break;
                    }
                    EditorGUILayout.LabelField("Time Trial", GUILayout.MinWidth(70));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    //-> Time Trial: Display Coins depending position
                    for (var j = 0; j < m_listTimeTrialCoins.arraySize; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Time " + (j +1), GUILayout.Width(40));
                        EditorGUILayout.PropertyField(m_listTimeTrialCoins.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.MinWidth(40));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            if (m_showInEditor.boolValue)
                EditorGUILayout.LabelField("");
        }
        #endregion
    }

    string ReturnTrackName(int _ListNumber,int _ID)
    {
        if (_globalTextDatas.textDatasList.Count > _ListNumber &&
            _globalTextDatas.textDatasList[_ListNumber].TextsList.Count > _ID)
        {
            return _globalTextDatas.textDatasList[_ListNumber].TextsList[_ID].multiLanguage[0];
        }
        else
        {
            return "Wrong Name (Text doesn't exist).";
        }
    }

    string FormatTime(int time)
    {
        int intTime = (int)time;
        int seconds = (intTime / 1000)  %60;       
        int minutes = (intTime / 1000) / 60;
        float milli = (intTime % 1000);
        string timeText = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milli);
        return timeText;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void ShowConvertissor(SerializedProperty LeaderboardID)
    {
        EditorGUI.BeginChangeCheck();
        SerializedProperty m_Minutes = LeaderboardID.FindPropertyRelative("minutes");
        SerializedProperty m_Seconds = LeaderboardID.FindPropertyRelative("seconds");
        SerializedProperty m_Milliseconds = LeaderboardID.FindPropertyRelative("milliseconds");

        EditorGUILayout.PropertyField(m_Minutes, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.LabelField(":", GUILayout.Width(5));
        EditorGUILayout.PropertyField(m_Seconds, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.LabelField(":", GUILayout.Width(5));
        EditorGUILayout.PropertyField(m_Milliseconds, new GUIContent(""), GUILayout.Width(30));

        if (EditorGUI.EndChangeCheck())
        {
            ApplyConvertissor(LeaderboardID);
        }
    }

    void ApplyConvertissor(SerializedProperty LeaderboardID)
    {
        SerializedProperty m_Minutes = LeaderboardID.FindPropertyRelative("minutes");
        SerializedProperty m_Seconds = LeaderboardID.FindPropertyRelative("seconds");
        SerializedProperty m_Milliseconds = LeaderboardID.FindPropertyRelative("milliseconds");
        SerializedProperty m_Time = LeaderboardID.FindPropertyRelative("time");

        int minutes     = m_Minutes.intValue*1000 *60;
        int seconds     = m_Seconds.intValue* 1000;
        int milliseconds = m_Milliseconds.intValue;

        m_Time.intValue = minutes + seconds + milliseconds;

    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }

    List<string> GenerateDifficultyNamesListList()
    {
        List<string> newList = new List<string>();
        difficultyNamesList.Clear();

        for (var i = 0; i < difficultyParamsList.arraySize; i++)
        {
            SerializedProperty m_Name = difficultyParamsList.GetArrayElementAtIndex(i).FindPropertyRelative("name");
            newList.Add(m_Name.stringValue);
        }

        return newList;
    }
}
#endif