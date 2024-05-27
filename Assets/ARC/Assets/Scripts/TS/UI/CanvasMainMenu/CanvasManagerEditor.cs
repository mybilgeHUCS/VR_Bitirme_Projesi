//Description: CanvasManagerEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(CanvasManager))]
public class CanvasManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;

    SerializedProperty listMenu;
    SerializedProperty currentSelectedPage;
    SerializedProperty RefNewPage;

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

    private List<Texture2D>     listTex = new List<Texture2D>();
    public  List<GUIStyle>      listGUIStyle = new List<GUIStyle>();
    private List<Color>         listColor = new List<Color>();
    #endregion


    public void OnEnable()
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
        helpBox = serializedObject.FindProperty("helpBox");
        moreOptions = serializedObject.FindProperty("moreOptions");
        listMenu = serializedObject.FindProperty("listMenu");
        currentSelectedPage = serializedObject.FindProperty("currentSelectedPage");
        RefNewPage = serializedObject.FindProperty("RefNewPage");
        #endregion
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

        EditorGUILayout.LabelField("");
        displayMenuPage();
        EditorGUILayout.LabelField("");
        CreateNewPage();
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.LabelField("");
        #endregion
    }



    public void displayMenuPage()
    {
        EditorGUILayout.BeginVertical(listGUIStyle[2]);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("|ID", GUILayout.Width(20));
        EditorGUILayout.LabelField("", GUILayout.Width(30));
        EditorGUILayout.LabelField("|Page Name", GUILayout.Width(150));
        EditorGUILayout.LabelField("|Select in Hierachy");
        EditorGUILayout.EndHorizontal();

        for (var i = 0;i< listMenu.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i +":", GUILayout.Width(20));

            string sSelected = "Off";
            if (currentSelectedPage.intValue == i)
            {
                sSelected = "On";
                GUI.backgroundColor = Color.green;
            }


            if (GUILayout.Button(sSelected, GUILayout.Width(30)))
            {
                if((GameObject)listMenu.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    for (var j = 0; j < listMenu.arraySize; j++)
                    {
                        SerializedObject serializedObject2 = new UnityEditor.SerializedObject((GameObject)listMenu.GetArrayElementAtIndex(j).objectReferenceValue);
                        SerializedProperty m_IsActive = serializedObject2.FindProperty("m_IsActive");

                        serializedObject2.Update();
                        if (j == i)
                        {
                            GameObject obj = (GameObject)listMenu.GetArrayElementAtIndex(j).objectReferenceValue;
                            SerializedObject serializedObject3 = new UnityEditor.SerializedObject(obj.GetComponent<CanvasGroup>());
                            SerializedProperty m_Alpha = serializedObject3.FindProperty("m_Alpha");
                            serializedObject3.Update();
                            m_Alpha.floatValue = 1;
                            serializedObject3.ApplyModifiedProperties();
                            m_IsActive.boolValue = true;
                        }
                        else m_IsActive.boolValue = false;

                        serializedObject2.ApplyModifiedProperties();
                    }
                    currentSelectedPage.intValue = i;
                }
                break;
            }
            GUI.backgroundColor = Color.white;

            if ((GameObject)listMenu.GetArrayElementAtIndex(i).objectReferenceValue != null)
            {
                GameObject tmpObj = (GameObject)listMenu.GetArrayElementAtIndex(i).objectReferenceValue;
                EditorGUILayout.LabelField(tmpObj.transform.parent.name, GUILayout.Width(150));
            }
            else
                EditorGUILayout.LabelField("", GUILayout.Width(150));


            EditorGUILayout.PropertyField(listMenu.GetArrayElementAtIndex(i), new GUIContent(""));
            if (moreOptions.boolValue)
            {
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    GameObject tmpObj = (GameObject)listMenu.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (tmpObj)
                    {
                        Undo.DestroyObjectImmediate(tmpObj.transform.parent.gameObject);
                        listMenu.DeleteArrayElementAtIndex(i);
                    }
                    listMenu.DeleteArrayElementAtIndex(i);
                    currentSelectedPage.intValue = 0;
                    break;
                }
            }
            
           
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }


    public void CreateNewPage()
    {
        EditorGUILayout.BeginHorizontal();
        CanvasMainMenuManager   myScript = (CanvasMainMenuManager)target;
        CanvasManager           myScript2 = (CanvasManager)target;

        SerializedObject        serializedObject2 = new UnityEditor.SerializedObject(myScript);
        SerializedProperty      newPageName = serializedObject2.FindProperty("newPageName");

        serializedObject2.Update();


        if (GUILayout.Button("Create New Page"))
        {
            bool b_NameAlreadyExist = false;
            for (var i = 0; i < myScript2.listMenu.Count; i++)
            {
                if (newPageName.stringValue == myScript2.listMenu[i].transform.parent.name)
                {
                    b_NameAlreadyExist = true;
                    break;
                }
            }


            if (b_NameAlreadyExist && EditorUtility.DisplayDialog("This name is already used",
                    "",
                "Continue"))
            {

            }
            else
            {
                //-> Disable all the page
                for (var j = 0; j < listMenu.arraySize; j++)
                {
                    SerializedObject serializedObject3 = new UnityEditor.SerializedObject((GameObject)listMenu.GetArrayElementAtIndex(j).objectReferenceValue);
                    SerializedProperty m_IsActive = serializedObject3.FindProperty("m_IsActive");

                    serializedObject3.Update();
                    m_IsActive.boolValue = false;
                    serializedObject3.ApplyModifiedProperties();
                }
                
                //-> Create and enable the new page 
                listMenu.InsertArrayElementAtIndex(listMenu.arraySize - 1);
                GameObject newPage = PrefabUtility.InstantiatePrefab((GameObject)RefNewPage.objectReferenceValue, myScript.objCanvasMainMenuManager.transform) as GameObject;

                newPage.name = newPageName.stringValue;
                Undo.RegisterCreatedObjectUndo(newPage, "NewPage");
                serializedObject.ApplyModifiedProperties();
                listMenu.GetArrayElementAtIndex(listMenu.arraySize - 1).objectReferenceValue = newPage.transform.GetChild(0).gameObject;
                currentSelectedPage.intValue = listMenu.arraySize - 1;
            }
        }

        EditorGUILayout.PropertyField(newPageName, new GUIContent(""), GUILayout.Width(200));

        serializedObject2.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();
    }


    private void HelpZone_01()
    {
        #region
        /*EditorGUILayout.HelpBox(
          "", MessageType.Info);*/
        #endregion
    }


    void OnSceneGUI()
    {
    }

}


#endif
