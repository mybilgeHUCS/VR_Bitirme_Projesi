//Description : RuntimeInputManagerEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using TS.Generics;

[CustomEditor(typeof(InfoInputs))]
public class RuntimeInputManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty ListOfInputsForEachPlayer;
   SerializedProperty  editorCurrentSelectedPlayer;
    SerializedProperty SaveName;
    SerializedProperty editorCurrentInputType;
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

    SerializedProperty defaultInput;

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


    public String[] listInputMode = new string[2] { "Keyboard", "Gamepad" };
 
    public List<string> playerList = new List<string>();

    public String[] listInputType = new string[3] { "Input", "bool", "float"};

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
        ListOfInputsForEachPlayer = serializedObject.FindProperty("ListOfInputsForEachPlayer");
        SaveName = serializedObject.FindProperty("SaveName");
        editorCurrentSelectedPlayer = serializedObject.FindProperty("editorCurrentSelectedPlayer");
        editorCurrentInputType = serializedObject.FindProperty("editorCurrentInputType");
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");

        defaultInput = serializedObject.FindProperty("defaultInput");

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        UpdateLayerListPopUp();

        #endregion
    }

    public void UpdateLayerListPopUp()
    {
        for (var i = 0;i< ListOfInputsForEachPlayer.arraySize; i++)
        {
            playerList.Add("Player " + i);
        }
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

        if (helpBox.boolValue) HelpZone_01();

        //-> Reset Zone
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete Inputs saved datas"))
        {
            if (EditorUtility.DisplayDialog("Delete all the saved files",
                                        "Are you sure you delete " + SaveName.stringValue + ".Dat", "Yes", "No"))
            {
                string itemPath = Application.persistentDataPath;
                itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
                FileUtil.DeleteFileOrDirectory(itemPath + "/" + SaveName.stringValue + ".dat");
            }
        }
        if (GUILayout.Button("Show .Dat In Explorer"))
        {
            ShowDataInExplorer();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset PC Inputs"))
        {
            InfoInputs infoInputs = (InfoInputs)target;
            defaultInput.intValue = 0;
            ResetInputs(new List<InputInfoParams>(infoInputs.InputPCListP1), new List<InputInfoParams>(infoInputs.InputPCListP2), new List<FloatInfoParams>(infoInputs.FloatInfoList), new List<BoolInfoParams>(infoInputs.BoolInfoList));
        }
        if (GUILayout.Button("Reset Mac Inputs"))
        {
            InfoInputs infoInputs = (InfoInputs)target;
            defaultInput.intValue = 1;
            ResetInputs(new List<InputInfoParams>(infoInputs.InputMacListP1), new List<InputInfoParams>(infoInputs.InputMacListP2), new List<FloatInfoParams>(infoInputs.FloatInfoList), new List<BoolInfoParams>(infoInputs.BoolInfoList));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("");




        //-> Display current Selected player
        EditorGUILayout.BeginVertical();
        var j = editorCurrentSelectedPlayer.intValue;
        EditorGUILayout.BeginHorizontal();
        editorCurrentSelectedPlayer.intValue = EditorGUILayout.Popup(editorCurrentSelectedPlayer.intValue, playerList.ToArray());
        if (moreOptions.boolValue && ListOfInputsForEachPlayer.arraySize > 1 && GUILayout.Button("Delete", GUILayout.Width(50)))
        {
            ListOfInputsForEachPlayer.DeleteArrayElementAtIndex(j);
        }

        EditorGUILayout.EndHorizontal();


        //-> Select an Input Type
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        editorCurrentInputType.intValue = EditorGUILayout.Popup(editorCurrentInputType.intValue, listInputType, GUILayout.Width(100));
        if (GUILayout.Button("Create new " + listInputType[editorCurrentInputType.intValue]))
        {
            if(editorCurrentInputType.intValue == 0) {
                SerializedProperty m_ListOfButtons = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfButtons");
                if (m_ListOfButtons.arraySize == 0)
                    m_ListOfButtons.InsertArrayElementAtIndex(0);
                else
                    m_ListOfButtons.InsertArrayElementAtIndex(m_ListOfButtons.arraySize - 1);
            }
            if (editorCurrentInputType.intValue == 1)
            {
                SerializedProperty m_ListOfBool = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfBool");
                if (m_ListOfBool.arraySize == 0)
                    m_ListOfBool.InsertArrayElementAtIndex(0);
                else
                    m_ListOfBool.InsertArrayElementAtIndex(m_ListOfBool.arraySize - 1);
            }
            if (editorCurrentInputType.intValue == 2)
            {
                SerializedProperty m_ListOfFloat = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfFloat");
                if (m_ListOfFloat.arraySize == 0)
                    m_ListOfFloat.InsertArrayElementAtIndex(0);
                else
                    m_ListOfFloat.InsertArrayElementAtIndex(m_ListOfFloat.arraySize - 1);
            }

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        //-> Display Inputs
        if (editorCurrentInputType.intValue == 0)
            displayButtons(listGUIStyle[2], j);

        //-> Display Bool
        if (editorCurrentInputType.intValue == 1)
            displayBool(listGUIStyle[2], j);

        //-> Display float
        if (editorCurrentInputType.intValue == 2)
            displayFloat(listGUIStyle[2], j);


        EditorGUILayout.EndVertical();


        //-> Add new player zone
        if (moreOptions.boolValue && GUILayout.Button("Add a new player"))
            ListOfInputsForEachPlayer.InsertArrayElementAtIndex(ListOfInputsForEachPlayer.arraySize - 1);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }

    public void displayBool(GUIStyle style_Color02, int value)
    {
        int size = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfBool").arraySize;
        for (var i = 0; i < size; i++)
        {
            SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfBool").GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
            SerializedProperty mb_State = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfBool").GetArrayElementAtIndex(i).FindPropertyRelative("b_State");
            EditorGUILayout.BeginVertical(style_Color02);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + ":", GUILayout.Width(20));
            EditorGUILayout.PropertyField(m_Name, new GUIContent(""), GUILayout.Width(200));
            EditorGUILayout.PropertyField(mb_State, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }

    public void displayFloat(GUIStyle style_Color02, int value)
    {
        int size = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfFloat").arraySize;
        for (var i = 0; i < size; i++)
        {
            SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfFloat").GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
            SerializedProperty mb_Value = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfFloat").GetArrayElementAtIndex(i).FindPropertyRelative("_Value");
            EditorGUILayout.BeginVertical(style_Color02);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + ":", GUILayout.Width(20));
            EditorGUILayout.PropertyField(m_Name, new GUIContent(""), GUILayout.Width(200));
            EditorGUILayout.PropertyField(mb_Value, new GUIContent(""), GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }


    public void displayButtons(GUIStyle style_Color02, int value)
    {
        #region
        InfoInputs myScript = (InfoInputs)target;
        int numberOfKeycode = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfButtons").arraySize;

        for (var i = 0; i < numberOfKeycode; i++)
        {
            EditorGUILayout.BeginVertical(style_Color02);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Input " + i + ":",EditorStyles.boldLabel, GUILayout.Width(50));

            SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfButtons").GetArrayElementAtIndex(i).FindPropertyRelative("_Names");
            EditorGUILayout.PropertyField(m_Name, new GUIContent(""));

            SerializedProperty m_listButton = ListOfInputsForEachPlayer.GetArrayElementAtIndex(value).FindPropertyRelative("listOfButtons").GetArrayElementAtIndex(i);
            SerializedProperty m_Keycode    = m_listButton.FindPropertyRelative("_Keycode");
            SerializedProperty m_AxisName   = m_listButton.FindPropertyRelative("_AxisName");
            SerializedProperty m_AxisPositiveOrNegative = m_listButton.FindPropertyRelative("_AxisPositiveOrNegative");
            SerializedProperty m_bUseAxisDirection = m_listButton.FindPropertyRelative("bUseAxisDirection");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("|Keyboard ", GUILayout.Width(73));
            EditorGUILayout.PropertyField(m_Keycode, new GUIContent(""));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("|Gamepad ", GUILayout.Width(73));
            EditorGUILayout.PropertyField(m_AxisName, new GUIContent(""));

            EditorGUILayout.LabelField("|Dir ", GUILayout.Width(30));
            EditorGUILayout.PropertyField(m_bUseAxisDirection, new GUIContent(""), GUILayout.Width(20));
            if(m_bUseAxisDirection.boolValue)
                EditorGUILayout.PropertyField(m_AxisPositiveOrNegative, new GUIContent(""), GUILayout.Width(20));
            
            EditorGUILayout.EndHorizontal();

            if (moreOptions.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IsPressed: ", GUILayout.Width(70));

                SerializedProperty m_IsPressed = m_listButton.FindPropertyRelative("b_GetKeyDown");
                EditorGUILayout.PropertyField(m_IsPressed, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Axis current value: ", GUILayout.Width(70));

                SerializedProperty m__AxisCurrentValue = m_listButton.FindPropertyRelative("_AxisCurrentValue");
                EditorGUILayout.PropertyField(m__AxisCurrentValue, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

        }
        #endregion
    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }

    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "Inputs are saved on " + SaveName.stringValue + ".DAT.", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Return a Keycode: ReturnInputKeyCode(int _Player,int _InputNumber)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Return a Unity Input (Axis): ReturnInputUnityInput(int _Player, int _InputNumber)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Replace a KeyCode with a new one:" +
            "\n" +
            "UpdateKeyCode(int _Player, int _InputNumber, KeyCode _NewKeycode)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Replace a Unity Input with a new one:" +
            "\n" +
            "UpdateUnityInput(int _Player, int _InputNumber, string _NewName)", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Load All Inputs (bool method):" +
            "Bool_LoadAllInputs()", MessageType.Info);

        EditorGUILayout.HelpBox(
            "Save All Inputs (bool method):" +
            "Bool_SaveAllInputs()", MessageType.Info);
    }


    // -> Default PC Values
    public List<InputInfoParams> InputPCListP1 = new List<InputInfoParams>()
    {
        new InputInfoParams("Back"          ,KeyCode.Escape,    "Joystick_Button1", false,  0),
        new InputInfoParams("Validation"    ,KeyCode.Return,    "Joystick_Button0", false,  0),
        new InputInfoParams("Pause"         ,KeyCode.P,         "Joystick_Button7", false,  0),
        new InputInfoParams("Left"          ,KeyCode.LeftArrow, "Joystick_Axis1",   true,   -1),
        new InputInfoParams("Right"         ,KeyCode.RightArrow,"Joystick_Axis1",   true,   1),
        new InputInfoParams("Up"            ,KeyCode.UpArrow,   "Joystick_Axis2",   true,   -1),
        new InputInfoParams("Down"          ,KeyCode.DownArrow, "Joystick_Axis2",   true,   1),
        new InputInfoParams("Booster"       ,KeyCode.B,         "Joystick_Button5", false,  0),
        new InputInfoParams("PowerUp"       ,KeyCode.Space,     "Joystick_Button4", false,  0),
        new InputInfoParams("Acceleration"  ,KeyCode.V,         "Joystick_Axis10",  false,  0),
        new InputInfoParams("Camera"        ,KeyCode.C,         "Joystick_Button3",   false,  0)
    };

    public List<InputInfoParams> InputPCListP2 = new List<InputInfoParams>()
    {
        new InputInfoParams("Back"          ,KeyCode.Escape,    "Joystick_Button1", false,  0),
        new InputInfoParams("Validation"    ,KeyCode.Return,    "Joystick_Button0", false,  0),
        new InputInfoParams("Pause"         ,KeyCode.P,         "Joystick_Button7", false,  0),
        new InputInfoParams("Left"          ,KeyCode.Q,         "Joystick_Axis1",   true,   -1),
        new InputInfoParams("Right"         ,KeyCode.D,         "Joystick_Axis1",   true,   1),
        new InputInfoParams("Up"            ,KeyCode.Z,         "Joystick_Axis2",   true,   -1),
        new InputInfoParams("Down"          ,KeyCode.S,         "Joystick_Axis2",   true,   1),
        new InputInfoParams("Booster"       ,KeyCode.G,         "Joystick_Button5", false,  0),
        new InputInfoParams("PowerUp"       ,KeyCode.R,         "Joystick_Button4", false,  0),
        new InputInfoParams("Acceleration"  ,KeyCode.F,         "Joystick_Axis10",  false,  0),
        new InputInfoParams("Camera"        ,KeyCode.T,         "Joystick_Button3",   false,  0)
    };

    public List<FloatInfoParams> FloatInfoList = new List<FloatInfoParams>()
    {
        //new FloatInfoParams("B",.75f)
    };

    public List<BoolInfoParams> BoolInfoList = new List<BoolInfoParams>()
    {
        new BoolInfoParams("Inverse UpDown",false)
    };

    // -> Default Mac Values
    // -> Default PC Values
    public List<InputInfoParams> InputMacListP1 = new List<InputInfoParams>()
    {
        new InputInfoParams("Back"          ,KeyCode.Escape,    "Joystick_Button17", false,  0),
        new InputInfoParams("Validation"    ,KeyCode.Return,    "Joystick_Button16", false,  0),
        new InputInfoParams("Pause"         ,KeyCode.P,         "Joystick_Button9", false,  0),
        new InputInfoParams("Left"          ,KeyCode.LeftArrow, "Joystick_Axis1",   true,   -1),
        new InputInfoParams("Right"         ,KeyCode.RightArrow,"Joystick_Axis1",   true,   1),
        new InputInfoParams("Up"            ,KeyCode.UpArrow,   "Joystick_Axis2",   true,   -1),
        new InputInfoParams("Down"          ,KeyCode.DownArrow, "Joystick_Axis2",   true,   1),
        new InputInfoParams("Booster"       ,KeyCode.B,         "Joystick_Button14", false,  0),
        new InputInfoParams("PowerUp"       ,KeyCode.Space,     "Joystick_Button4", false,  0),
        new InputInfoParams("Acceleration"  ,KeyCode.V,         "Joystick_Button13",  false,  0),
        new InputInfoParams("Camera"        ,KeyCode.C,         "Joystick_Button19",   false,  0)
    };

    public List<InputInfoParams> InputMacListP2 = new List<InputInfoParams>()
    {
        new InputInfoParams("Back"          ,KeyCode.Escape,    "Joystick_Button17", false,  0),
        new InputInfoParams("Validation"    ,KeyCode.Return,    "Joystick_Button16", false,  0),
        new InputInfoParams("Pause"         ,KeyCode.P,         "Joystick_Button9", false,  0),
        new InputInfoParams("Left"          ,KeyCode.Q,         "Joystick_Axis1",   true,   -1),
        new InputInfoParams("Right"         ,KeyCode.D,         "Joystick_Axis1",   true,   1),
        new InputInfoParams("Up"            ,KeyCode.Z,         "Joystick_Axis2",   true,   -1),
        new InputInfoParams("Down"          ,KeyCode.S,         "Joystick_Axis2",   true,   1),
        new InputInfoParams("Booster"       ,KeyCode.G,         "Joystick_Button14", false,  0),
        new InputInfoParams("PowerUp"       ,KeyCode.R,         "Joystick_Button13", false,  0),
        new InputInfoParams("Acceleration"  ,KeyCode.F,         "Joystick_Axis6",  false,  0),
        new InputInfoParams("Camera"        ,KeyCode.T,         "Joystick_Button19",   false,  0)
    };



    void ResetInputs(List<InputInfoParams> InputListP1, List<InputInfoParams> InputListP2, List<FloatInfoParams> FloatInfoList, List<BoolInfoParams> BoolInfoList)
    {
        List<InputInfoParams> tmpList = new List<InputInfoParams>(InputListP1);

        for (var j = 0; j < ListOfInputsForEachPlayer.arraySize; j++)
        {
            if(j == 1) tmpList = new List<InputInfoParams>(InputListP2);
            //-> Init Inputs
            for (var i = 0; i < tmpList.Count; i++)
            {
                SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfButtons").GetArrayElementAtIndex(i).FindPropertyRelative("_Names");

                SerializedProperty m_listButton = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfButtons").GetArrayElementAtIndex(i);
                SerializedProperty m_Keycode = m_listButton.FindPropertyRelative("_Keycode");
                SerializedProperty m_AxisName = m_listButton.FindPropertyRelative("_AxisName");
                SerializedProperty m_AxisPositiveOrNegative = m_listButton.FindPropertyRelative("_AxisPositiveOrNegative");
                SerializedProperty m_bUseAxisDirection = m_listButton.FindPropertyRelative("bUseAxisDirection");

                m_Name.stringValue = tmpList[i].Name;

                int index = 0;
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (tmpList[i].keyCode == key) break;
                    else index++;
                }

                m_Keycode.enumValueIndex = index;
                m_AxisName.stringValue = tmpList[i].AxisName.Replace("_",(j+1).ToString());
                m_AxisPositiveOrNegative.intValue = tmpList[i].AxisPositiveOrNegative;
                m_bUseAxisDirection.boolValue = tmpList[i].bUseAxisDirection;
            }

            //-> Init Float
            for (var i = 0; i < FloatInfoList.Count; i++)
            {
                SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfFloat").GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
                SerializedProperty m_Value = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfFloat").GetArrayElementAtIndex(i).FindPropertyRelative("_Value");
               

                m_Name.stringValue = FloatInfoList[i].Name;
                m_Value.floatValue = FloatInfoList[i].value;
            }

            //
            //-> Init Bool
            for (var i = 0; i < BoolInfoList.Count; i++)
            {
                SerializedProperty m_Name = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfBool").GetArrayElementAtIndex(i).FindPropertyRelative("_Name");
                SerializedProperty m_b_State = ListOfInputsForEachPlayer.GetArrayElementAtIndex(j).FindPropertyRelative("listOfBool").GetArrayElementAtIndex(i).FindPropertyRelative("b_State");


                m_Name.stringValue = BoolInfoList[i].Name;
                m_b_State.boolValue = BoolInfoList[i].bState;
            }
        }


    }
    

    void OnSceneGUI()
    {
    }
}
#endif
