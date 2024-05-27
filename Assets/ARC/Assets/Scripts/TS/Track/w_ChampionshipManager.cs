// Description : w_ChampionshipManager.cs :  Custom editor for w_ChampionshipManager window
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;
using System;

public class w_ChampionshipManager : EditorWindow
{
    private Vector2             scrollPosAll;

    ChampionshipModeData        _ChampionshipModeData;
    SerializedObject            serializedObject;
    SerializedProperty          HelpBox;
    SerializedProperty          MoreOptions;
    SerializedProperty          listOfChampionship;
    SerializedProperty          pointsWinDependingFinalPosition;


    globalTextDatas             _globalTextDatas;
    SerializedObject            serializedObjectTxtRef;

    public EditorMethods_Pc     editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc   methodModule;

    TracksData                  _TracksData;


    //-> Difficulty Manager
    SerializedObject serializedObjectDiffManager;
    SerializedProperty difficultyParamsList;
    DifficultyManagerData difficultyManagerData;
    public List<String> difficultyNamesList = new List<string>();


    [MenuItem("Tools/TS/w_ChampionshipManager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_ChampionshipManager));
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
        //-> Access Championship Data
        string objectPath = "Assets/ARC/Assets/Datas/Ref/GameMode/ChampionshipModeData.asset";
        _ChampionshipModeData = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as ChampionshipModeData;

       
        if (_ChampionshipModeData) {
            serializedObject = new UnityEditor.SerializedObject(_ChampionshipModeData);
            HelpBox = serializedObject.FindProperty("HelpBox");
            MoreOptions = serializedObject.FindProperty("MoreOptions");
            listOfChampionship = serializedObject.FindProperty("listOfChampionship");
            pointsWinDependingFinalPosition = serializedObject.FindProperty("pointsWinDependingFinalPosition");
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            editorMethods = new EditorMethods_Pc();
            methodModule = new AP_MethodModule_Pc();
        }

        //-> Access Multi language Text data
        string objectPathTxtData = "Assets/ARC/Assets/Datas/Ref/globalTextDatas.asset";
        _globalTextDatas = AssetDatabase.LoadAssetAtPath(objectPathTxtData, typeof(UnityEngine.Object)) as globalTextDatas;

        if (_globalTextDatas)
        {
            serializedObjectTxtRef = new UnityEditor.SerializedObject(_globalTextDatas);
            serializedObjectTxtRef.Update();
            serializedObjectTxtRef.ApplyModifiedProperties();
        }

        //-> Access Track data
        objectPath = "Assets/ARC/Assets/Datas/Ref/TracksData.asset";
        _TracksData = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as TracksData;

        if (difficultyManagerData)
        {
            serializedObjectDiffManager = new UnityEditor.SerializedObject(difficultyManagerData);

            difficultyParamsList = serializedObjectDiffManager.FindProperty("difficultyParamsList");
            serializedObjectDiffManager.Update();
            difficultyNamesList = GenerateDifficultyNamesListList();
            serializedObjectDiffManager.ApplyModifiedProperties();
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

        if (_ChampionshipModeData && _globalTextDatas)
        {
            serializedObject.Update();
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
            m_tab.intValue = GUILayout.Toolbar(m_tab.intValue, new string[] { "Global Params", "Championship Params" }, GUILayout.MinWidth(30));

            EditorGUILayout.LabelField("");

            switch (m_tab.intValue)
            {
                case 0:
                    GlobalParams();
                    break;
                case 1:
                    ShowChampionshipList();
                    break;
            }
            ;
            EditorGUILayout.LabelField("");

            serializedObject.ApplyModifiedProperties();
        }

        if (difficultyManagerData)
        {
            serializedObjectDiffManager.Update();
            difficultyNamesList = GenerateDifficultyNamesListList();
            serializedObjectDiffManager.ApplyModifiedProperties();
        }

        EditorGUILayout.EndScrollView();
        #endregion
    }

    void GlobalParams()
    {
        #region
        EditorGUILayout.BeginVertical(listGUIStyle[0]);
        EditorGUILayout.BeginHorizontal(listGUIStyle[1]);
        EditorGUILayout.LabelField("Points win after a race:",EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
       
        for(var i = 0;i < pointsWinDependingFinalPosition.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pos " + (i+1).ToString() + ":", GUILayout.Width(50));
            EditorGUILayout.PropertyField(pointsWinDependingFinalPosition.GetArrayElementAtIndex(i), new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Create New Value"))
        {
            pointsWinDependingFinalPosition.InsertArrayElementAtIndex(0);
            pointsWinDependingFinalPosition.MoveArrayElement(0, Mathf.Clamp(pointsWinDependingFinalPosition.arraySize-1, 0,pointsWinDependingFinalPosition.arraySize));
        }

        EditorGUILayout.EndVertical();

        #endregion
    }

    void ShowChampionshipList()
    {
        #region
        for (var i = 0; i < listOfChampionship.arraySize; i++)
        {
            SerializedProperty m_ListTexts                  = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("listTexts");
            SerializedProperty m_showInCustomEditor         = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("showInCustomEditor");
            SerializedProperty m_listTrackParams            = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("listTrackParams");
            SerializedProperty m_championshipIcon           = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("championshipIcon");
            SerializedProperty m_Unlock                     = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("Unlock");
            SerializedProperty m_bUnlockANewChampionship    = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("bUnlockANewChampionship");
            SerializedProperty m_whichChampionshipToUnlock  = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("whichChampionshipToUnlock");
            SerializedProperty m_whichPlayerPosToUnlock     = listOfChampionship.GetArrayElementAtIndex(i).FindPropertyRelative("whichPlayerPosToUnlock");

            EditorGUILayout.BeginVertical(listGUIStyle[0]);
            EditorGUILayout.BeginHorizontal(listGUIStyle[3]);
            EditorGUILayout.LabelField(i + ":", GUILayout.Width(15));
            EditorGUILayout.PropertyField(m_showInCustomEditor, new GUIContent(""), GUILayout.Width(20));
            //-> Select the name of the track using the multilanguage system
            string sName = ReturnTrackName(
                m_ListTexts.GetArrayElementAtIndex(0).FindPropertyRelative("listID").intValue,
                m_ListTexts.GetArrayElementAtIndex(0).FindPropertyRelative("EntryID").intValue);

            EditorGUILayout.LabelField(sName, EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                listOfChampionship.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();


            //-> Display championship info
            if (m_showInCustomEditor.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: ", GUILayout.Width(40));
                EditorGUILayout.LabelField("List: ", GUILayout.Width(38));
                EditorGUILayout.PropertyField(m_ListTexts.GetArrayElementAtIndex(0).FindPropertyRelative("listID"), new GUIContent(""));
                EditorGUILayout.LabelField("ID: ", GUILayout.Width(30));
                EditorGUILayout.PropertyField(m_ListTexts.GetArrayElementAtIndex(0).FindPropertyRelative("EntryID"), new GUIContent(""));
                if (GUILayout.Button("TextEditor"))
                {
                    EditorWindow.GetWindow(typeof(w_TextCreator));
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite: ", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_championshipIcon, new GUIContent(""));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(listGUIStyle[1]);
                EditorGUILayout.LabelField("", GUILayout.Height(.3f));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Is Unlocked: ", GUILayout.Width(80));
                EditorGUILayout.PropertyField(m_Unlock, new GUIContent(""));
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

                //-> Select how to unlock a championship
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_bUnlockANewChampionship, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Condition to Unlock a new Championship:");
                EditorGUILayout.EndHorizontal();
               

                if (m_bUnlockANewChampionship.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Player Pos: At Least", GUILayout.Width(120));
                    EditorGUILayout.PropertyField(m_whichPlayerPosToUnlock, new GUIContent(""), GUILayout.MinWidth(20));

                    EditorGUILayout.LabelField("Unlocked Championship: ", GUILayout.Width(150));
                    EditorGUILayout.PropertyField(m_whichChampionshipToUnlock, new GUIContent(""), GUILayout.MinWidth(20));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                //-> Display Tracks 
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    m_listTrackParams.InsertArrayElementAtIndex(0);
                    break;
                }
                EditorGUILayout.LabelField("----> Track List", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
              
                TracksListing(i);

                //-> Display Coin options
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
               
                Coins(i);
            }
            EditorGUILayout.EndVertical();

             if (m_showInCustomEditor.boolValue)
                EditorGUILayout.LabelField("");
          
        }

        AddNewChampionship();
        #endregion
    }

    void AddNewChampionship()
    {
        if (GUILayout.Button("Create New Championship", GUILayout.Height(30)))
        {
            listOfChampionship.InsertArrayElementAtIndex(0);
            SerializedProperty m_ListTexts                  = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("listTexts");
            SerializedProperty m_showInCustomEditor         = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("showInCustomEditor");
            SerializedProperty m_listTrackParams            = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("listTrackParams");
            SerializedProperty m_championshipIcon           = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("championshipIcon");
            SerializedProperty m_Unlock                     = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("Unlock");
            SerializedProperty m_bUnlockANewChampionship    = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("bUnlockANewChampionship");
            SerializedProperty m_whichChampionshipToUnlock  = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("whichChampionshipToUnlock");
            SerializedProperty m_whichPlayerPosToUnlock     = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("whichPlayerPosToUnlock");
            SerializedProperty methodsCallWhenChampionshipIsFinished = listOfChampionship.GetArrayElementAtIndex(0).FindPropertyRelative("methodsCallWhenChampionshipIsFinished");

            m_listTrackParams.ClearArray();
            m_championshipIcon.objectReferenceValue = null;
            m_showInCustomEditor.boolValue = true;
            m_Unlock.boolValue = true;
            m_bUnlockANewChampionship.boolValue = true;
            m_whichChampionshipToUnlock.intValue = 0;
            m_whichPlayerPosToUnlock.intValue = 3;
            methodsCallWhenChampionshipIsFinished.ClearArray();

            listOfChampionship.MoveArrayElement(0, listOfChampionship.arraySize - 1);
        }
    }

    void TracksListing(int value)
    {
        EditorGUILayout.BeginVertical();
        SerializedProperty m_listTrackParams = listOfChampionship.GetArrayElementAtIndex(value).FindPropertyRelative("listTrackParams");

        for(var j = 0;j< m_listTrackParams.arraySize; j++)
        {
            SerializedProperty mTrack                       = m_listTrackParams.GetArrayElementAtIndex(j);
            SerializedProperty m_TrackID                    = mTrack.FindPropertyRelative("TrackID");
            SerializedProperty m_AI_Difficulty              = mTrack.FindPropertyRelative("AI_Difficulty");
            SerializedProperty m_howManyVehicleByRace       = mTrack.FindPropertyRelative("howManyVehicleByRace");
            SerializedProperty m_UnlockTrackOnArcadeMode    = mTrack.FindPropertyRelative("UnlockTrackOnArcadeMode");
            SerializedProperty m_UnlockTrackOnTimeTrialMode = mTrack.FindPropertyRelative("UnlockTrackOnTimeTrialMode");
            SerializedProperty m_methodsCallWhenRaceIsFinished = mTrack.FindPropertyRelative("methodsCallWhenRaceIsFinished");
            SerializedProperty m_bShowTrackInfoInEditor     = mTrack.FindPropertyRelative("bShowTrackInfoInEditor");
            SerializedProperty m_winCoinsTrack              = mTrack.FindPropertyRelative("winCoinsTrack");


            EditorGUILayout.BeginVertical(listGUIStyle[2]);
            //-> Display Track Name
            if (m_TrackID.intValue < _TracksData.listTrackParams.Count)
            {
                int listID = _TracksData.listTrackParams[m_TrackID.intValue].selectedListMultiLanguage;
                int entryID = _TracksData.listTrackParams[m_TrackID.intValue].NameIDMultiLanguage;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_bShowTrackInfoInEditor, new GUIContent(""), GUILayout.Width(20));
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    m_listTrackParams.DeleteArrayElementAtIndex(j);
                    break;
                }
                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    m_listTrackParams.MoveArrayElement(j,Mathf.Clamp(j-1,0, m_listTrackParams.arraySize-1));
                    break;
                }
                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    m_listTrackParams.MoveArrayElement(j, Mathf.Clamp(j + 1, 0, m_listTrackParams.arraySize - 1));
                    break;
                }
                EditorGUILayout.LabelField("Track ID:", GUILayout.Width(60));
                EditorGUILayout.PropertyField(m_TrackID, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("(" + ReturnTrackName(listID, entryID) + ")", EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();
            }
            else{
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_bShowTrackInfoInEditor, new GUIContent(""), GUILayout.Width(20));
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    m_listTrackParams.DeleteArrayElementAtIndex(j);
                    break;
                }
                EditorGUILayout.LabelField("Track ID:", GUILayout.Width(60));
                EditorGUILayout.PropertyField(m_TrackID, new GUIContent(""), GUILayout.Width(20));

                EditorGUILayout.LabelField("(Track Doesn't exist)", EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            if (m_bShowTrackInfoInEditor.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("AI Difficulty:", GUILayout.Width(120));
                m_AI_Difficulty.intValue = EditorGUILayout.Popup(m_AI_Difficulty.intValue, difficultyNamesList.ToArray(), GUILayout.Width(100));				// --> Display all methods
                //EditorGUILayout.PropertyField(m_AI_Difficulty, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("How Many Vehicles:", GUILayout.Width(120));
                EditorGUILayout.PropertyField(m_howManyVehicleByRace, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unlock In Arcade:", GUILayout.Width(120));
                EditorGUILayout.PropertyField(m_UnlockTrackOnArcadeMode, new GUIContent(""), GUILayout.Width(20));

                EditorGUILayout.LabelField("Unlock In Time Trial:", GUILayout.Width(120));
                EditorGUILayout.PropertyField(m_UnlockTrackOnTimeTrialMode, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Win Coins (Track):", GUILayout.Width(120));
                EditorGUILayout.PropertyField(m_winCoinsTrack, new GUIContent(""), GUILayout.Width(100));

                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

    }

    //-> Display coins rules for each Championship
    void Coins(int value)
    {
        SerializedProperty m_listChampionshipCoins = listOfChampionship.GetArrayElementAtIndex(value).FindPropertyRelative("listChampionshipCoins");
        SerializedProperty m_showCoinsInEditor = listOfChampionship.GetArrayElementAtIndex(value).FindPropertyRelative("showCoinsInEditor");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_showCoinsInEditor, new GUIContent(""), GUILayout.Width(20));

        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            m_listChampionshipCoins.InsertArrayElementAtIndex(0);
            m_listChampionshipCoins.MoveArrayElement(0, m_listChampionshipCoins.arraySize - 1);
        }
        if (GUILayout.Button("-", GUILayout.Width(20)))
        {
            if (m_listChampionshipCoins.arraySize > 0)
                m_listChampionshipCoins.DeleteArrayElementAtIndex(Mathf.Clamp(m_listChampionshipCoins.arraySize - 1, 0, m_listChampionshipCoins.arraySize - 1));
        }

        EditorGUILayout.LabelField("----> Coins (Won when the championship ended)", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();      

        if (m_showCoinsInEditor.boolValue) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Default"))
            {
                SerializedProperty m_DefaultChampionshipCoins = serializedObject.FindProperty("listEndChampionshipCoins");
                m_listChampionshipCoins.ClearArray();

                for (int k = 0; k < m_DefaultChampionshipCoins.arraySize; k++)
                    m_listChampionshipCoins.InsertArrayElementAtIndex(0);

                for (int k = 0; k < m_DefaultChampionshipCoins.arraySize; k++)
                    m_listChampionshipCoins.GetArrayElementAtIndex(k).intValue = m_DefaultChampionshipCoins.GetArrayElementAtIndex(k).intValue;
            }
            if (GUILayout.Button("Clear"))
            {
                m_listChampionshipCoins.ClearArray();
            }
            EditorGUILayout.EndHorizontal();


            //-> Display Coins depending position
            for (var j = 0; j < m_listChampionshipCoins.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Pos " + (j + 1) + ":", GUILayout.Width(40));
                EditorGUILayout.PropertyField(m_listChampionshipCoins.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.MinWidth(40));
                EditorGUILayout.EndHorizontal();
            }
        }
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