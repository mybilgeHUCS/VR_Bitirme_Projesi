// Description : w_TextCreator.cs : (window) Allow to create multilanguage texts
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;


public class w_TextCreator : EditorWindow
{
    private Vector2 scrollPosAll;
    private Vector2 scrollPosEntrySection;

    SerializedObject serializedObject;

    SerializedProperty HelpBox;
    SerializedProperty MoreOptions;
    SerializedProperty currentelectedDatas;
    SerializedProperty toggleLanguageList;
    SerializedProperty StartPos;
    SerializedProperty EndPos;
    SerializedProperty tab;
    SerializedProperty newEntryTextList;

    globalTextDatas _globalTextDatas;

    public List<string> DataName = new List<string>();
    public string[] arrTab = new string[3] { "Edit Texts", "Create New Text", "Other" };

    [System.Serializable]
    public class DataClass
    {
        public string data;
    }

    [MenuItem("Tools/TS/w_TextCreator")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_TextCreator));
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

        string objectPath = "Assets/ARC/Assets/Datas/Ref/globalTextDatas.asset";
        _globalTextDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as globalTextDatas;

        if (_globalTextDatas) {
            serializedObject = new UnityEditor.SerializedObject(_globalTextDatas);
            HelpBox = serializedObject.FindProperty("HelpBox");
            MoreOptions = serializedObject.FindProperty("MoreOptions");
            currentelectedDatas = serializedObject.FindProperty("currentelectedDatas");
            toggleLanguageList = serializedObject.FindProperty("toggleLanguageList");
            StartPos = serializedObject.FindProperty("StartPos");
            EndPos = serializedObject.FindProperty("EndPos");
            tab = serializedObject.FindProperty("tab");
            newEntryTextList = serializedObject.FindProperty("newEntryTextList");
            serializedObject.Update();
            UpdateLanguageToggleList();
            UpdateDataList();
            serializedObject.ApplyModifiedProperties();
        }
    }

    void UpdateLanguageToggleList()
    {
        toggleLanguageList.ClearArray();
        newEntryTextList.ClearArray();

        SerializedObject serializedObject3 = new UnityEditor.SerializedObject(_globalTextDatas.textDatasList[0]);
        SerializedProperty m_TextsList = serializedObject3.FindProperty("TextsList");
        serializedObject3.Update();
      
        int howManyLanguage = m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").arraySize;
        //Debug.Log("howManyLanguage: " + howManyLanguage);
        for (var i = 0; i < howManyLanguage; i++)
        {
            toggleLanguageList.InsertArrayElementAtIndex(0);
            toggleLanguageList.GetArrayElementAtIndex(0).boolValue = true;
            newEntryTextList.InsertArrayElementAtIndex(0);
            newEntryTextList.GetArrayElementAtIndex(0).stringValue = "";
        }
        serializedObject3.ApplyModifiedProperties();
    }

    void UpdateDataList()
    {
        DataName.Clear();
        SerializedProperty m_TextDatasList = serializedObject.FindProperty("textDatasList");
        for (var i = 0; i < m_TextDatasList.arraySize; i++)
            DataName.Add(i + ": " + m_TextDatasList.GetArrayElementAtIndex(i).objectReferenceValue.name);
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
        //--> Window description
        //GUI.backgroundColor = _cGreen;

        serializedObject.Update();
        if (_globalTextDatas)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
            EditorGUILayout.PropertyField(HelpBox, new GUIContent(""), GUILayout.Width(30));

            if (EditorPrefs.GetBool("MoreOptions") == true)
            {
                EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
                EditorGUILayout.PropertyField(MoreOptions, new GUIContent(""), GUILayout.Width(30));
            }
          
            EditorGUILayout.EndHorizontal();

            tab.intValue = GUILayout.Toolbar(tab.intValue, arrTab);

            if (tab.intValue == 2)
            {
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                if (MoreOptions.boolValue)
                {
                    ShowTextDatasList();
                    EditorGUILayout.LabelField("");
                } 
                CreateNewTextDatas();
                EditorGUILayout.LabelField("");
                AddNewLanguage();

                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField("");

            }
            else if (tab.intValue == 0)
            {
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                ShowLanguage();

                displaySelectedDatas();
                EditorGUILayout.EndVertical();
            }
            else if (tab.intValue == 1)
            {
                EditorGUILayout.BeginVertical(listGUIStyle[2]);
                CreateEntrySection();
                EditorGUILayout.EndVertical();
            }
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
        #endregion
    }


    void ShowTextDatasList()
    {
        #region
        SerializedProperty m_TextDatasList = serializedObject.FindProperty("textDatasList");

        for (var i = 0; i < m_TextDatasList.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_TextDatasList.GetArrayElementAtIndex(i), new GUIContent(""));

            if (MoreOptions.boolValue && GUILayout.Button("-", GUILayout.Width(20)))
            {
                m_TextDatasList.DeleteArrayElementAtIndex(i);
                currentelectedDatas.intValue = 0;
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }


    void CreateNewTextDatas()
    {
        #region
        SerializedProperty m_newDatasName = serializedObject.FindProperty("newDatasName");
        string assetName = m_newDatasName.stringValue;
        string newPath = "Assets/ARC/Assets/Datas/" + assetName + ".asset";

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Create New Datas ->", GUILayout.Width(130));
        EditorGUILayout.LabelField("Choose Name:", GUILayout.Width(85));

        EditorGUILayout.PropertyField(m_newDatasName, new GUIContent(""), GUILayout.Width(120));

        if (GUILayout.Button("Create"))
        {
            TextDatas dataAlreadyExist = AssetDatabase.LoadAssetAtPath(newPath, typeof(UnityEngine.Object)) as TextDatas;
            if (!dataAlreadyExist)
            {
                AssetDatabase.CopyAsset("Assets/ARC/Assets/Datas/Ref/TextDatasEmpty.asset", newPath);

                SerializedProperty m_TextDatasList = serializedObject.FindProperty("textDatasList");

                m_TextDatasList.InsertArrayElementAtIndex(0);
                m_TextDatasList.GetArrayElementAtIndex(0).objectReferenceValue = (TextDatas)AssetDatabase.LoadAssetAtPath(newPath, typeof(UnityEngine.Object)) as TextDatas;

                SerializedObject serializedObject3 = new UnityEditor.SerializedObject((TextDatas)m_TextDatasList.GetArrayElementAtIndex(0).objectReferenceValue);
                SerializedProperty m_TextsList = serializedObject3.FindProperty("TextsList");
                serializedObject3.Update();
                //-> Update the number of language
                SerializedObject serializedObject2 = new UnityEditor.SerializedObject((TextDatas)m_TextDatasList.GetArrayElementAtIndex(1).objectReferenceValue);
                SerializedProperty m_TextsList2 = serializedObject2.FindProperty("TextsList");
                serializedObject2.Update();

                int howManyLanguage = m_TextsList2.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").arraySize;
                //Debug.Log("HowManyLanguage: " + howManyLanguage);

                m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").ClearArray();
                for (var i = 0; i < howManyLanguage; i++)
                {
                    m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").InsertArrayElementAtIndex(0);
                }

                serializedObject2.ApplyModifiedProperties();
                serializedObject3.ApplyModifiedProperties();

                m_TextDatasList.MoveArrayElement(0, m_TextDatasList.arraySize - 1);

                UpdateDataList();
                UpdateLanguageToggleList();
                serializedObject.ApplyModifiedProperties();
                currentelectedDatas.intValue = _globalTextDatas.textDatasList.Count - 1;
            }
            else if (EditorUtility.DisplayDialog("This name is already used",
                    "",
                "Continue"))
            {
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion
    }


    void displaySelectedDatas(){

        //-> Select Popup Text data
        EditorGUILayout.BeginHorizontal();
        currentelectedDatas.intValue = EditorGUILayout.Popup(currentelectedDatas.intValue, DataName.ToArray());

        if (GUILayout.Button("Update", GUILayout.Width(60)))
        {
            UpdateDataList();
            UpdateLanguageToggleList();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        SerializedObject serializedObject2 = new UnityEditor.SerializedObject(_globalTextDatas.textDatasList[currentelectedDatas.intValue]);
        SerializedProperty m_TextsList = serializedObject2.FindProperty("TextsList");
        serializedObject2.Update();

        //-> How many entry to display
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Start :", GUILayout.Width(40));
        StartPos.intValue = EditorGUILayout.IntSlider(StartPos.intValue, 0, m_TextsList.arraySize);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("How many entries displayed: ", GUILayout.Width(170));
        EditorGUILayout.PropertyField(EndPos, new GUIContent(""), GUILayout.Width(50));
        EditorGUILayout.LabelField("(Total: "+ m_TextsList.arraySize + ")", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        //--> Scrollview
        scrollPosEntrySection = EditorGUILayout.BeginScrollView(scrollPosEntrySection/*, GUILayout.Height(600)*/);

        int iStartPos = StartPos.intValue;
        int iEndPos = iStartPos + EndPos.intValue;
        iStartPos = Mathf.Clamp(iStartPos, 0, m_TextsList.arraySize);
        iEndPos = Mathf.Clamp(iEndPos, 1, m_TextsList.arraySize);

        bool bStop = false;
        for (var i = iStartPos; i< iEndPos; i++)
        {
            if (bStop)
                break;

            for (var j = 0; j < m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").arraySize; j++)
            {
                if (toggleLanguageList.GetArrayElementAtIndex(j).boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (j == 0)
                    {
                        EditorGUILayout.BeginHorizontal (GUILayout.Width(40));
                        EditorGUILayout.LabelField(i + ": ", GUILayout.Width(30));
                        EditorGUILayout.LabelField("L" + j + ": ", GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (j == 1)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(40));

                        if(MoreOptions.boolValue){
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                m_TextsList.DeleteArrayElementAtIndex(i);
                                bStop = true;
                                break;
                            }
                            EditorGUILayout.LabelField(" ", GUILayout.Width(7));
                        }
                        else
                        {
                            EditorGUILayout.LabelField(" ", GUILayout.Width(30));
                        }
                        
                        EditorGUILayout.LabelField("L" + j + ": ", GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(40));
                        EditorGUILayout.LabelField(" ", GUILayout.Width(20));
                        EditorGUILayout.LabelField("L" + j + ": ", GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();
                    }
                    SerializedProperty mTxt = m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").GetArrayElementAtIndex(j);
                    mTxt.stringValue = EditorGUILayout.TextArea(mTxt.stringValue, EditorStyles.textArea);

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space();       
        }

        serializedObject2.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.LabelField("");

    }

    void CreateEntrySection()
    {
        //-> Select Popup Text data
        EditorGUILayout.BeginHorizontal();
        currentelectedDatas.intValue = EditorGUILayout.Popup(currentelectedDatas.intValue, DataName.ToArray());

        if (GUILayout.Button("Update", GUILayout.Width(60)))
        {
            UpdateDataList();
            UpdateLanguageToggleList();
        }
        EditorGUILayout.EndHorizontal();

        if(currentelectedDatas.intValue == 0 && !MoreOptions.boolValue)
        {
            EditorGUILayout.HelpBox("To prevent issues it is not possible to add new entry into this Text datas. You must select another list in the dropdown menu.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.LabelField("");
            SerializedObject serializedObject2 = new UnityEditor.SerializedObject(_globalTextDatas.textDatasList[currentelectedDatas.intValue]);
            SerializedProperty m_TextsList = serializedObject2.FindProperty("TextsList");
            serializedObject2.Update();

            for (var i = 0; i < newEntryTextList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("L" + i + ": ", GUILayout.Width(20));
                //EditorGUILayout.PropertyField(newEntryTextList.GetArrayElementAtIndex(i), new GUIContent(""));

                SerializedProperty mTxt = newEntryTextList.GetArrayElementAtIndex(i);
                mTxt.stringValue = EditorGUILayout.TextArea(mTxt.stringValue, EditorStyles.textArea);

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Create entry number " + m_TextsList.arraySize + " into -> " + DataName[currentelectedDatas.intValue] +" (TextDatas)"))
            {
                m_TextsList.InsertArrayElementAtIndex(0);
                m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").ClearArray();
                Debug.Log("newEntryTextList: " + newEntryTextList.arraySize);
                for (var i = newEntryTextList.arraySize - 1; i >= 0; i--)
                {
                    m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").InsertArrayElementAtIndex(0);
                    m_TextsList.GetArrayElementAtIndex(0).FindPropertyRelative("multiLanguage").GetArrayElementAtIndex(0).stringValue = newEntryTextList.GetArrayElementAtIndex(i).stringValue;
                }

                m_TextsList.MoveArrayElement(0, m_TextsList.arraySize - 1);
            }
            serializedObject2.ApplyModifiedProperties();
        }
    }

    void AddNewLanguage()
    {
        if (GUILayout.Button("Add New Language"))
        {
            for (var k = 0; k < _globalTextDatas.textDatasList.Count; k++)
            {
                SerializedObject serializedObject2 = new UnityEditor.SerializedObject(_globalTextDatas.textDatasList[k]);
                SerializedProperty m_TextsList = serializedObject2.FindProperty("TextsList");
                serializedObject2.Update();

                for (var i = 0; i < m_TextsList.arraySize; i++)
                {
                    m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").InsertArrayElementAtIndex(0);

                    m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").GetArrayElementAtIndex(0).stringValue = "";

                    m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").MoveArrayElement(
                        0,
                        m_TextsList.GetArrayElementAtIndex(i).FindPropertyRelative("multiLanguage").arraySize - 1);
                }

                serializedObject2.ApplyModifiedProperties();

            }
            UpdateLanguageToggleList();
        }
    }

    void ShowLanguage()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Language: ", GUILayout.Width(100));
        for (var i = 0; i < toggleLanguageList.arraySize; i++)
        {
            EditorGUILayout.PropertyField(toggleLanguageList.GetArrayElementAtIndex(i), new GUIContent(""), GUILayout.Width(20));
            EditorGUILayout.LabelField("" + i, GUILayout.Width(20));
        }
        EditorGUILayout.EndHorizontal();

        if (MoreOptions.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Delete Language: ", GUILayout.Width(100));
            for (var i = 0; i < toggleLanguageList.arraySize; i++)
            {   
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    for (var k = 0; k < _globalTextDatas.textDatasList.Count; k++)
                    {
                        SerializedObject serializedObject2 = new UnityEditor.SerializedObject(_globalTextDatas.textDatasList[k]);
                        SerializedProperty m_TextsList = serializedObject2.FindProperty("TextsList");
                        serializedObject2.Update();

                        for (var m = 0; m < m_TextsList.arraySize; m++)
                        {
                            m_TextsList.GetArrayElementAtIndex(m).FindPropertyRelative("multiLanguage").DeleteArrayElementAtIndex(i);
                        }
                        serializedObject2.ApplyModifiedProperties();
                    }
                    UpdateDataList();
                    UpdateLanguageToggleList();
                }
                EditorGUILayout.LabelField("", GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
#endif