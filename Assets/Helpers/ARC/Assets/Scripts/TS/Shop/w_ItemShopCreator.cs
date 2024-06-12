// Description : w_ItemShopCreator.cs :  Allow to create shop item
// Not implemented
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

public class w_ItemShopCreator : EditorWindow
{
    private Vector2 scrollPosAll;
    SerializedObject serializedObject;

    SerializedProperty HelpBox;
    SerializedProperty MoreOptions;
    SerializedProperty currentelectedDatas;
    SerializedProperty listShopItems;

    dataShop _dataShop;

    //[MenuItem("Tools/TS/w_ItemShopCreator")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_ItemShopCreator));
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

        string objectPath = "Assets/ARC/Assets/Datas/Ref/dataShop.asset";
        _dataShop = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as dataShop;

        if (_dataShop) {
            serializedObject = new UnityEditor.SerializedObject(_dataShop);
            HelpBox = serializedObject.FindProperty("HelpBox");
            MoreOptions = serializedObject.FindProperty("MoreOptions");
            currentelectedDatas = serializedObject.FindProperty("currentelectedDatas");
            listShopItems = serializedObject.FindProperty("listShopItems");

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }

  
    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
        //--> Window description
        //GUI.backgroundColor = _cGreen;

        serializedObject.Update();
        if (_dataShop)
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
        }

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Not Implemented");

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndScrollView();
        #endregion
    }

    //-> Create the first element of the sequence if needed
    public void AddItem(int value)
    {
        if (listShopItems.arraySize == 0 && GUILayout.Button("Create First Shop Item"))
        {
            listShopItems.InsertArrayElementAtIndex(0);
        }
        else if (listShopItems.arraySize != 0 && GUILayout.Button("Create New Shop Item"))
        {
            listShopItems.InsertArrayElementAtIndex(0);
            SerializedProperty m_NameEditor = listShopItems.GetArrayElementAtIndex(0).FindPropertyRelative("NameEditor");
            SerializedProperty m_NameIDMultiLanguage = listShopItems.GetArrayElementAtIndex(0).FindPropertyRelative("NameIDMultiLanguage");
            SerializedProperty m_isItemUnlocked = listShopItems.GetArrayElementAtIndex(0).FindPropertyRelative("isItemUnlocked");

            m_NameEditor.stringValue = "";
            m_NameIDMultiLanguage.intValue = 0;
            m_isItemUnlocked.boolValue = false;

            listShopItems.MoveArrayElement(0, listShopItems.arraySize - 1);
        }
    }

    void ShowItemsList()
    {
        #region

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID", GUILayout.Width(30));
        EditorGUILayout.LabelField("|Ref Name", GUILayout.MinWidth(100));
        EditorGUILayout.LabelField("|Text ID", GUILayout.Width(50));
        EditorGUILayout.LabelField("|Price", GUILayout.Width(50));
        EditorGUILayout.LabelField("|State", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();


        for (var i = 0; i < listShopItems.arraySize; i++)
        {
            SerializedProperty m_NameEditor = listShopItems.GetArrayElementAtIndex(i).FindPropertyRelative("NameEditor");
            SerializedProperty m_NameIDMultiLanguage = listShopItems.GetArrayElementAtIndex(i).FindPropertyRelative("NameIDMultiLanguage");
            SerializedProperty m_isItemUnlocked = listShopItems.GetArrayElementAtIndex(i).FindPropertyRelative("isItemUnlocked");
            SerializedProperty m_Price = listShopItems.GetArrayElementAtIndex(i).FindPropertyRelative("Price");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(i +": ", GUILayout.Width(30));
            EditorGUILayout.PropertyField(m_NameEditor, new GUIContent(""), GUILayout.MinWidth(100));
            EditorGUILayout.PropertyField(m_NameIDMultiLanguage, new GUIContent(""), GUILayout.Width(50));
            EditorGUILayout.PropertyField(m_Price, new GUIContent(""), GUILayout.Width(50));
            EditorGUILayout.PropertyField(m_isItemUnlocked, new GUIContent(""), GUILayout.Width(50));

            if (EditorPrefs.GetBool("MoreOptions") == true && MoreOptions.boolValue && GUILayout.Button("-", GUILayout.Width(20)))
            {
                listShopItems.DeleteArrayElementAtIndex(i);
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

        //serializedObject.Update();
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
                serializedObject.ApplyModifiedProperties();
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


    void OnInspectorUpdate()
    {
        Repaint();
    }

   

   

   

  
}
#endif