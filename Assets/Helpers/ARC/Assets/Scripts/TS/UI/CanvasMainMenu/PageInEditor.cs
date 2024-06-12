//Description : PageInEditor.cs. Use in association with PageIn.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using TS.Generics;

[CustomEditor(typeof(PageIn))]
public class PageInEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;
    SerializedProperty listdisplaynewCanvasSequence;
    SerializedProperty currentListDisplayed;
    SerializedProperty b_ChangeName;
    SerializedProperty newListName;
    SerializedProperty b_SpecialPage;

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


    public string[] listEditor = new string[11] {
            "Enable this page",
            "Disable current page",
            "Choose Transition",
            "Custom Method",
            "Init Page",
            "Set First Selected Button",
            "Play a sound",
            "--Special--",
            "Disable Page with Its Number",
            "Canvas Interactable",
            "Crossfade "};


    public string[] listTransition = new string[2] {
            "Use Animation",
            "No Animation"};


    public List<string> seqNameList = new List<string>();

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
        SeeInspector                    = serializedObject.FindProperty("SeeInspector");
        helpBox                         = serializedObject.FindProperty("helpBox");
        listdisplaynewCanvasSequence    = serializedObject.FindProperty("listdisplaynewCanvasSequence");
        currentListDisplayed            = serializedObject.FindProperty("currentListDisplayed");
        b_ChangeName                    = serializedObject.FindProperty("b_ChangeName");
        newListName                     = serializedObject.FindProperty("newListName");
        moreOptions                     = serializedObject.FindProperty("moreOptions");
        b_SpecialPage              = serializedObject.FindProperty("b_SpecialPage");
        updateSeqNameList();

        #endregion
    }

    public override void OnInspectorGUI()
    {
        #region
        //-> Custom editor visualisation parameters
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

        //-> Create the first element of the sequence if needed
        InitSection();

        //-> Special Option
        displaySpecialOptions();

        //-> Display the sequence
        displayNewScreenList(listGUIStyle[0]);

       

        serializedObject.ApplyModifiedProperties();        
        EditorGUILayout.LabelField("");
        #endregion
    }

    public void displaySpecialOptions()
    {
        if(b_SpecialPage.boolValue)
            EditorGUILayout.HelpBox(
            "This page can be opened from any menu: " + "\n" +
            "b_SpecialPage is set to true."
            , MessageType.Info);
    }

    //-> Create the first element of the sequence if needed
    public void InitSection()
    {
        if (listdisplaynewCanvasSequence.GetArrayElementAtIndex(0).FindPropertyRelative("_NewEntry").arraySize == 0)
        {
            if (GUILayout.Button("Init"))
            {
                //listdisplaynewCanvasSequence.InsertArrayElementAtIndex(0);
                listdisplaynewCanvasSequence.GetArrayElementAtIndex(0).FindPropertyRelative("_NewEntry").InsertArrayElementAtIndex(0);
            }
        }
    }

    //-> Display the sequence
    public void displayNewScreenList(GUIStyle color_01)
    {
        #region
       
        EditorGUILayout.BeginVertical(color_01);
        if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select:", GUILayout.Width(85));
            currentListDisplayed.intValue = EditorGUILayout.Popup(currentListDisplayed.intValue, seqNameList.ToArray());
            EditorGUILayout.EndHorizontal();
        }

        if (helpBox.boolValue) HelpZone_01();

        for (var i = 0;i< listdisplaynewCanvasSequence.arraySize; i++)
        {
            // Display selected sequence
            if(i == currentListDisplayed.intValue)
            {
                SerializedProperty _NewEntry = listdisplaynewCanvasSequence.GetArrayElementAtIndex(i).FindPropertyRelative("_NewEntry");
                SerializedProperty _SeqName = listdisplaynewCanvasSequence.GetArrayElementAtIndex(i).FindPropertyRelative("_SeqName");

                if (moreOptions.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Show Name:", GUILayout.Width(85));
                    EditorGUILayout.PropertyField(b_ChangeName, new GUIContent(""), GUILayout.Width(20));
                    EditorGUI.BeginChangeCheck();

                    if (b_ChangeName.boolValue)
                        EditorGUILayout.PropertyField(_SeqName, new GUIContent(""));

                    if (EditorGUI.EndChangeCheck())
                    updateSeqNameList();

                    EditorGUILayout.EndHorizontal();
                }

                for (var j = 0; j < _NewEntry.arraySize; j++)
                {
                    SerializedProperty _whichOperation          = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_whichOperation");
                    SerializedProperty _Value                   = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_Value");
                    SerializedProperty _Value2                  = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_Value2");
                    SerializedProperty isTransitionAvailable    = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("isTransitionAvailable");
                    SerializedProperty _MyEvent                 = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_MyEvent");
                    SerializedProperty b_EnableFade             = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("b_EnableFade");
                    SerializedProperty _FadeValue               = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_FadeValue");
                    SerializedProperty _Obj                     = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_Obj");
                    SerializedProperty _Volume                  = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("_Volume");
                    SerializedProperty b_InteractableState = _NewEntry.GetArrayElementAtIndex(j).FindPropertyRelative("b_InteractableState");
                    
                    EditorGUILayout.BeginHorizontal();
                    _whichOperation.intValue = EditorGUILayout.Popup(_whichOperation.intValue, listEditor.ToArray(), GUILayout.Width(170));
                   
                    //-> Transition
                    if (_whichOperation.intValue == 2)
                    {
                        for (var k = 0; k < _NewEntry.arraySize; k++)
                        {
                            SerializedProperty _whichOperationK = _NewEntry.GetArrayElementAtIndex(k).FindPropertyRelative("_whichOperation");
                            if (k < j && _whichOperationK.intValue == 0)
                            {
                                EditorGUILayout.HelpBox("Transition MUST be used BEFORE Enable Page", MessageType.Error);
                                break;
                            }
                        }

                        EditorGUILayout.PropertyField(isTransitionAvailable, new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.PropertyField(_Value, new GUIContent(""), GUILayout.MinWidth(20));
                    }
                    //-> Custom Method
                    if (_whichOperation.intValue == 3)
                    {
                        EditorGUILayout.PropertyField(isTransitionAvailable, new GUIContent(""), GUILayout.MinWidth(20));
                        //EditorGUILayout.PropertyField(_MyEvent, new GUIContent(""), GUILayout.MinWidth(20));
                    }
                    //-> Init Menu
                    else if (_whichOperation.intValue == 4)
                    {
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(40));
                    }
                    //-> Disable page
                    else if (_whichOperation.intValue == 1)
                    {
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(40));
                    }
                    //-> Enable/Disable page
                    else if (_whichOperation.intValue == 0)
                    {
                        if (b_EnableFade.boolValue)
                        {
                            EditorGUILayout.LabelField("Fade:", GUILayout.Width(30));
                            EditorGUILayout.PropertyField(b_EnableFade, new GUIContent(""), GUILayout.Width(20));
                            EditorGUILayout.PropertyField(_FadeValue, new GUIContent(""), GUILayout.MinWidth(20));
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Fade:", GUILayout.Width(30));
                            EditorGUILayout.PropertyField(b_EnableFade, new GUIContent(""), GUILayout.Width(20));
                            EditorGUILayout.LabelField("", GUILayout.MinWidth(20));
                        }
                    }
                    //-> Set First Selected Button
                    else if (_whichOperation.intValue == 5)
                    {
                        EditorGUILayout.PropertyField(_Obj, new GUIContent(""));
                    }
                    //-> Play a sound
                    else if (_whichOperation.intValue == 6)
                    {
                        EditorGUILayout.LabelField("ID:", GUILayout.Width(30));
                        EditorGUILayout.PropertyField(_Value, new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.LabelField("Vol:", GUILayout.Width(20));
                        EditorGUILayout.PropertyField(_Volume, new GUIContent(""), GUILayout.Width(30));
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(20));
                    }
                    //-> Disable page with its number
                    else if (_whichOperation.intValue == 8)
                    {
                        EditorGUILayout.PropertyField(_Value, new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(20));
                    }
                    //-> Change Canvas Interactable state
                    else if (_whichOperation.intValue == 9)
                    {
                        EditorGUILayout.LabelField("ID:", GUILayout.Width(30));
                        EditorGUILayout.PropertyField(_Value, new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.LabelField("State:", GUILayout.Width(20));
                        EditorGUILayout.PropertyField(b_InteractableState, new GUIContent(""), GUILayout.Width(30));
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(20));
                    }
                    //-> Canvas Crossfade
                    else if (_whichOperation.intValue == 10)
                    {
                        EditorGUILayout.LabelField("From Page:", GUILayout.Width(65));
                        EditorGUILayout.PropertyField(_Value, new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.LabelField("To: ", GUILayout.Width(20));
                        EditorGUILayout.PropertyField(_Value2, new GUIContent(""), GUILayout.Width(30));
                        EditorGUILayout.LabelField("Fade:", GUILayout.Width(35));
                        EditorGUILayout.PropertyField(_FadeValue, new GUIContent(""), GUILayout.Width(40));
                        EditorGUILayout.LabelField("", GUILayout.MinWidth(20));
                    }

                    if (GUILayout.Button("^", GUILayout.Width(20)))
                    {
                        _NewEntry.MoveArrayElement(j, j - 1);
                        break;
                    }
                    if (GUILayout.Button("v", GUILayout.Width(20)))
                    {
                        _NewEntry.MoveArrayElement(j, j + 1);
                        break;
                    }
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        _NewEntry.InsertArrayElementAtIndex(j);
                        break;
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        //if(_NewEntry.arraySize > 1)
                        _NewEntry.DeleteArrayElementAtIndex(j);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    //-> Custom Method
                    if (_whichOperation.intValue == 3)
                    {
                        EditorGUILayout.PropertyField(_MyEvent, new GUIContent(""), GUILayout.MinWidth(20));
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();


        //-> Button to create a new sequence
        if (moreOptions.boolValue)
        {
            EditorGUILayout.LabelField("");

            if (GUILayout.Button("Create a new initialization from Page A to Page B"))
            {
                listdisplaynewCanvasSequence.InsertArrayElementAtIndex(listdisplaynewCanvasSequence.arraySize - 1);
                serializedObject.ApplyModifiedProperties();
                SerializedProperty _SeqName = listdisplaynewCanvasSequence.GetArrayElementAtIndex(listdisplaynewCanvasSequence.arraySize - 1).FindPropertyRelative("_SeqName");
                _SeqName.stringValue = newListName.stringValue;
                SerializedProperty _NewEntry = listdisplaynewCanvasSequence.GetArrayElementAtIndex(listdisplaynewCanvasSequence.arraySize - 1).FindPropertyRelative("_NewEntry");

                _NewEntry.ClearArray();
                _NewEntry.InsertArrayElementAtIndex(0);

                updateSeqNameList();
                currentListDisplayed.intValue = listdisplaynewCanvasSequence.arraySize - 1;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("List Name:", GUILayout.Width(85));
            EditorGUILayout.PropertyField(newListName, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }


    public void updateSeqNameList()
    {
        #region

        seqNameList.Clear();
        for (var i = 0; i < listdisplaynewCanvasSequence.arraySize; i++)
        {
            SerializedProperty _SeqName = listdisplaynewCanvasSequence.GetArrayElementAtIndex(i).FindPropertyRelative("_SeqName");
            seqNameList.Add(_SeqName.stringValue);
        }
        #endregion
    }

    private void HelpZone_01()
    {
        #region
        PageIn myScript = (PageIn)target;
        int posInList = 0;

        CanvasMainMenuManager CanvasMM = FindObjectOfType<CanvasMainMenuManager>();

        if (CanvasMM)
        {
            for (var i = 0; i < CanvasMM.listMenu.Count; i++)
            {
                if (CanvasMM.listMenu[i].transform.parent.gameObject == myScript.gameObject)
                {
                    posInList = i;
                    break;
                }
            }
        }
      
        EditorGUILayout.HelpBox(
          "To call this entry: " + "\n" +
         "CanvasMainMenuManager.instance." +
          "DisplayNewPage(" + posInList + ");", MessageType.Info);


        EditorGUILayout.HelpBox(
                  "At the end of a custom method always call: " + "\n" +
                 "InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;", MessageType.Info);

        
        #endregion
    }


    void OnSceneGUI()
    {
    }
}
#endif
