//Description : btnRempInfoEditor.cs. Use in association with btnRempInfo.cs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(btnRempInfo))]
public class btnRempInfoEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;

    SerializedProperty whichDevice;
    SerializedProperty whichType;
    SerializedProperty whichInput;
    SerializedProperty whichBool;
    SerializedProperty whichFloat;
    SerializedProperty b_MustBeButton;
    SerializedProperty b_AllowRemap;
    SerializedProperty b_AllowsToHaveTheSameInputHasInput;
    SerializedProperty AllowsSameInputHasInput;

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

    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

    public List<string> listCreatedInput = new List<string>();
    public List<string> listCreatedBool = new List<string>();
    public List<string> listCreatedFloat = new List<string>();

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
        helpBox = serializedObject.FindProperty("helpBox");
        moreOptions = serializedObject.FindProperty("moreOptions");

        whichDevice = serializedObject.FindProperty("whichDevice");
        whichType = serializedObject.FindProperty("whichType");
        whichInput = serializedObject.FindProperty("whichInput");
        whichBool = serializedObject.FindProperty("whichBool");
        whichFloat = serializedObject.FindProperty("whichFloat");
        b_MustBeButton = serializedObject.FindProperty("b_MustBeButton");
        b_AllowRemap = serializedObject.FindProperty("b_AllowRemap");

        b_AllowsToHaveTheSameInputHasInput = serializedObject.FindProperty("b_AllowsToHaveTheSameInputHasInput");
        AllowsSameInputHasInput = serializedObject.FindProperty("AllowsSameInputHasInput");
        #endregion

        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        InfoInputs infoInput = FindObjectOfType<InfoInputs>();

        if (infoInput)
        {
            for (var k = 0; k < infoInput.ListOfInputsForEachPlayer[0].listOfButtons.Count; k++)
            {
                string newAxis = infoInput.ListOfInputsForEachPlayer[0].listOfButtons[k]._Names;
                listCreatedInput.Add(newAxis);
            }

            for (var k = 0; k < infoInput.ListOfInputsForEachPlayer[0].listOfBool.Count; k++)
            {
                string newAxis = infoInput.ListOfInputsForEachPlayer[0].listOfBool[k]._Name;
                listCreatedBool.Add(newAxis);
            }

            for (var k = 0; k < infoInput.ListOfInputsForEachPlayer[0].listOfFloat.Count; k++)
            {
                string newAxis = infoInput.ListOfInputsForEachPlayer[0].listOfFloat[k]._Name;
                listCreatedFloat.Add(newAxis);
            }
        }
        
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
   
        //-> Button Options
        SectionButtonOptions();

        EditorGUILayout.LabelField("");

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");
        #endregion
    }

    //0: Keyboard, 1: Gamepad, 2: Mobile, 3: Other")]
    public string[] arrWhichDevice = new string[4] {"Keyboard","Gamepad","Mobile","Other" };
    //"0: Keycode, 1: String, 2: bool, 3: float"
    public string[] arrWhichType = new string[4] { "Keycode (Only Keyboard)", "String (Only Gamepad)", "bool", "float" };


    //-> Display Button Options
    public void SectionButtonOptions()
    {
        returnWrongCases();

        EditorGUILayout.HelpBox(
              "Choose the button Type.", MessageType.Info);


        string sAllow = "The player is allowed to remap this input.";
        if (!b_AllowRemap.boolValue) sAllow = "The player is not allowed to remap this input.";
        if (GUILayout.Button(sAllow))
        {
            b_AllowRemap.boolValue = !b_AllowRemap.boolValue;
        }


        whichDevice.intValue = EditorGUILayout.Popup(whichDevice.intValue, arrWhichDevice);


        whichType.intValue = EditorGUILayout.Popup(whichType.intValue, arrWhichType);



        //-> Which Input to use
        if (whichType.intValue == 0 || whichType.intValue == 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Which Input:", GUILayout.Width(85));
            
            if(listCreatedInput.Count > 0)
            {
                whichInput.intValue = EditorGUILayout.Popup(whichInput.intValue, listCreatedInput.ToArray());
            }
            else
            {
                EditorGUILayout.PropertyField(whichInput, new GUIContent(""), GUILayout.MinWidth(30));
            }

            EditorGUILayout.EndHorizontal();
        }
        //-> Which boolean to use
        if (whichType.intValue == 2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Which Bool:", GUILayout.Width(85));
            if (listCreatedBool.Count > 0)
            {
                whichBool.intValue = EditorGUILayout.Popup(whichBool.intValue, listCreatedBool.ToArray());
            }
            else
            {
                EditorGUILayout.PropertyField(whichBool, new GUIContent(""), GUILayout.MinWidth(30));
            }

            EditorGUILayout.EndHorizontal();
        }
        //-> Which Float to use
        if (whichType.intValue == 3)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Which Float:", GUILayout.Width(85));
            if (listCreatedFloat.Count > 0)
            {
                whichFloat.intValue = EditorGUILayout.Popup(whichFloat.intValue, listCreatedFloat.ToArray());
            }
            else
            {
                EditorGUILayout.PropertyField(whichFloat, new GUIContent(""), GUILayout.MinWidth(30));
            }
            EditorGUILayout.EndHorizontal();
        }


        //-> Input must be a button Gamepad input case
        if (whichDevice.intValue == 1 && whichType.intValue == 1)
        {
            EditorGUILayout.LabelField("");
            EditorGUILayout.HelpBox(
              "If the input must be a button (not an axis) check the box below.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Must be a button:", GUILayout.Width(100));
            EditorGUILayout.PropertyField(b_MustBeButton, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }

        //-> Gamepad and desktop allows the same input for two different input
        if (whichDevice.intValue == 1 && whichType.intValue == 1)
        {
            //EditorGUILayout.LabelField("");
            EditorGUILayout.HelpBox(
               "If this input is allowed to use the same input as an other input check the box below.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Paired this input with an other input:", GUILayout.Width(200));
            EditorGUILayout.PropertyField(b_AllowsToHaveTheSameInputHasInput, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            if (b_AllowsToHaveTheSameInputHasInput.boolValue)
            {
               
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Select the paired Input:", GUILayout.Width(150));
                EditorGUILayout.PropertyField(AllowsSameInputHasInput, new GUIContent(""));
                EditorGUILayout.EndHorizontal();



            }

        }
    }

    public void returnWrongCases()
    {
        if (whichDevice.intValue == 0 && whichType.intValue == 1 ||
            whichDevice.intValue == 1 && whichType.intValue == 0 ||
            whichDevice.intValue == 2 && whichType.intValue == 0 ||
            whichDevice.intValue == 2 && whichType.intValue == 1 ||
            whichDevice.intValue == 3 && whichType.intValue == 0 ||
            whichDevice.intValue == 3 && whichType.intValue == 1
            )
        {
            EditorGUILayout.HelpBox(
             "The setup is not correct. All the parameters can't be use together.", MessageType.Warning);
        }
           
    }

    private void HelpZone_01()
    {
        #region


        EditorGUILayout.HelpBox(
          "OnClick() events are allowed only if all the conditions return true.", MessageType.Info);
        #endregion
    }


   

    void OnSceneGUI()
    {
    }
}
#endif
