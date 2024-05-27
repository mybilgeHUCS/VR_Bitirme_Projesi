//Description: SceneStepsManagerEditor: Custom Editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TS.Generics;

[CustomEditor(typeof(SceneStepsManager))]
public class SceneStepsManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty currentStepListDisplayedEditor;
    SerializedProperty moreOptions;
    SerializedProperty helpBox;

    SerializedProperty currentStepSequence;
    SerializedProperty currentStep;

    SerializedProperty SceneStepsList;

    public EditorMethods_Pc editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule_Pc methodModule;

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

    public List<string> StepsListName = new List<string>();

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

        currentStepListDisplayedEditor = serializedObject.FindProperty("currentStepListDisplayedEditor");
        moreOptions = serializedObject.FindProperty("moreOptions");
        helpBox = serializedObject.FindProperty("helpBox");
        SceneStepsList = serializedObject.FindProperty("SceneStepsList");

        currentStepSequence = serializedObject.FindProperty("currentStepSequence");
        currentStep = serializedObject.FindProperty("currentStep");
        
        editorMethods = new EditorMethods_Pc();
        methodModule = new AP_MethodModule_Pc();

        #endregion
    }

    public override void OnInspectorGUI()
    {
        #region
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.LabelField("HelpBox:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBox, new GUIContent(""), GUILayout.Width(30));

        EditorGUILayout.LabelField("More Options:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(moreOptions, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        if (helpBox.boolValue) HelpZone_01();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Sequence:", GUILayout.Width(120));
        EditorGUILayout.PropertyField(currentStepSequence, new GUIContent(""), GUILayout.Width(30));
        SerializedProperty m_SceneStepsListName = SceneStepsList.GetArrayElementAtIndex(currentStepSequence.intValue).FindPropertyRelative("sName");
        EditorGUILayout.LabelField("(" + m_SceneStepsListName.stringValue + ")");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Step:", GUILayout.Width(120));
        EditorGUILayout.PropertyField(currentStep, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        StepsListName.Clear();
        for (var i = 0; i < SceneStepsList.arraySize; i++)
            StepsListName.Add(SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("sName").stringValue);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID " + currentStepListDisplayedEditor.intValue + " :", GUILayout.Width(30));
        currentStepListDisplayedEditor.intValue = EditorGUILayout.Popup(currentStepListDisplayedEditor.intValue, StepsListName.ToArray()); ;
        EditorGUILayout.EndHorizontal();

        displayAllTheMethods(listGUIStyle[0], listGUIStyle[1]);

        EditorGUILayout.LabelField("");

        serializedObject.ApplyModifiedProperties();
        #endregion
    }

    //--> display a list of methods call when the scene starts
    private void displayAllTheMethods(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        SceneStepsManager myScript = (SceneStepsManager)target;

        SerializedObject serializedObject2 = new UnityEditor.SerializedObject(myScript);
        SerializedProperty m_SceneStepsList = serializedObject2.FindProperty("SceneStepsList");
        serializedObject2.Update();


        if (moreOptions.boolValue)
        {
            if (currentStepListDisplayedEditor.intValue >= m_SceneStepsList.arraySize) currentStepListDisplayedEditor.intValue = 0;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create a new Scene Steps"))
            {
                m_SceneStepsList.InsertArrayElementAtIndex(m_SceneStepsList.arraySize - 1);
                serializedObject2.ApplyModifiedProperties();
                m_SceneStepsList.GetArrayElementAtIndex(m_SceneStepsList.arraySize - 1).FindPropertyRelative("sName").stringValue = "New Step List";
            }
           
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(m_SceneStepsList.GetArrayElementAtIndex(currentStepListDisplayedEditor.intValue).FindPropertyRelative("sName"), new GUIContent(""));
        }

        EditorGUILayout.HelpBox("The methods are called in the same order as the list. " +
                                      "\nAll methods must be boolean methods. " +
                                      "\nOther methods will be ignored.", MessageType.Info);


        for (var i = 0;i< m_SceneStepsList.arraySize; i++)
        {
            if(currentStepListDisplayedEditor.intValue == i)
            {
                SerializedProperty m_SceneStepsListName = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("sName");
                SerializedProperty m_SceneStepsMultiList = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("SceneStepsMultiList");
                SerializedProperty m_b_Bypass = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_Bypass");
                SerializedProperty m_b_ShowAutoStart = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_ShowAutoStart");
                SerializedProperty m_B_AutoStart = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("B_AutoStart");
                SerializedProperty m_b_ShowBypass = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_ShowBypass");
                SerializedProperty m_b_StepName = m_SceneStepsList.GetArrayElementAtIndex(i).FindPropertyRelative("b_StepName");


                for (var j = 0; j < m_b_Bypass.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical(GUILayout.Width(83));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(m_b_StepName.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();


                    if (moreOptions.boolValue && j > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Show Link: ", GUILayout.Width(80));
                        EditorGUILayout.PropertyField(m_b_ShowAutoStart.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();
                    }

                    if (m_b_ShowAutoStart.GetArrayElementAtIndex(j).boolValue)
                    {
                        if (j > 0)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Link: ", GUILayout.Width(50));
                            EditorGUILayout.PropertyField(m_B_AutoStart.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(20));
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (moreOptions.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Show Bypass: ", GUILayout.Width(80));
                        EditorGUILayout.PropertyField(m_b_ShowBypass.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();
                    }

                    if (m_b_ShowBypass.GetArrayElementAtIndex(j).boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Bypass: ", GUILayout.Width(50));
                        EditorGUILayout.PropertyField(m_b_Bypass.GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(20));
                        EditorGUILayout.EndHorizontal();

                        if (moreOptions.boolValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("^", GUILayout.Width(20)))
                            {
                                m_b_Bypass.MoveArrayElement(j, j - 1);
                                m_B_AutoStart.MoveArrayElement(j, j - 1);
                                m_b_ShowBypass.MoveArrayElement(j, j - 1);
                                m_b_ShowAutoStart.MoveArrayElement(j, j - 1);
                                m_SceneStepsMultiList.MoveArrayElement(j, j - 1);
                                m_b_StepName.MoveArrayElement(j, j - 1);
                                break;
                            }
                            if (GUILayout.Button("v", GUILayout.Width(20)))
                            {
                                m_b_Bypass.MoveArrayElement(j, j + 1);
                                m_B_AutoStart.MoveArrayElement(j, j + 1);
                                m_b_ShowBypass.MoveArrayElement(j, j + 1);
                                m_b_ShowAutoStart.MoveArrayElement(j, j + 1);
                                m_SceneStepsMultiList.MoveArrayElement(j, j + 1);
                                m_b_StepName.MoveArrayElement(j, j + 1);
                                break;
                            }
                            if (GUILayout.Button("+", GUILayout.Width(20)))
                            {
                                m_b_Bypass.InsertArrayElementAtIndex(j);
                                m_b_Bypass.GetArrayElementAtIndex(j + 1).boolValue = false;
                                m_B_AutoStart.InsertArrayElementAtIndex(j);
                                m_B_AutoStart.GetArrayElementAtIndex(j + 1).boolValue = false;
                                m_b_ShowBypass.InsertArrayElementAtIndex(j);
                                m_b_ShowBypass.GetArrayElementAtIndex(j + 1).boolValue = true;
                                m_b_ShowAutoStart.InsertArrayElementAtIndex(j);
                                m_b_ShowAutoStart.GetArrayElementAtIndex(j + 1).boolValue = true;
                                m_SceneStepsMultiList.InsertArrayElementAtIndex(j);
                                //m_SceneStepsMultiList.GetArrayElementAtIndex(j + 1).FindPropertyRelative("SceneStepsMultiList").ClearArray();
                                m_b_StepName.InsertArrayElementAtIndex(j);
                                m_b_StepName.GetArrayElementAtIndex(j + 1).stringValue = "";
                                // serializedObject2.ApplyModifiedProperties();
                                break;
                            }
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                Debug.Log("i: " + j);
                                m_b_Bypass.DeleteArrayElementAtIndex(j);
                                m_B_AutoStart.DeleteArrayElementAtIndex(j);
                                m_b_ShowBypass.DeleteArrayElementAtIndex(j);
                                m_b_ShowAutoStart.DeleteArrayElementAtIndex(j);
                                m_SceneStepsMultiList.DeleteArrayElementAtIndex(j);
                                m_b_StepName.DeleteArrayElementAtIndex(j);
                                //serializedObject2.ApplyModifiedProperties();
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                       
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(73));
                        EditorGUILayout.EndHorizontal();

                        if (moreOptions.boolValue)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("^", GUILayout.Width(20)))
                            {
                                m_b_Bypass.MoveArrayElement(j, j - 1);
                                m_B_AutoStart.MoveArrayElement(j, j - 1);
                                m_b_ShowBypass.MoveArrayElement(j, j - 1);
                                m_b_ShowAutoStart.MoveArrayElement(j, j - 1);
                                m_SceneStepsMultiList.MoveArrayElement(j, j - 1);
                                m_b_StepName.MoveArrayElement(j, j - 1);
                                break;
                            }
                            if (GUILayout.Button("v", GUILayout.Width(20)))
                            {
                                m_b_Bypass.MoveArrayElement(j, j + 1);
                                m_B_AutoStart.MoveArrayElement(j, j + 1);
                                m_b_ShowBypass.MoveArrayElement(j, j + 1);
                                m_b_ShowAutoStart.MoveArrayElement(j, j + 1);
                                m_SceneStepsMultiList.MoveArrayElement(j, j + 1);
                                m_b_StepName.MoveArrayElement(j, j - 1);
                                break;
                            }
                            if (GUILayout.Button("+", GUILayout.Width(20)))
                            {
                                m_b_Bypass.InsertArrayElementAtIndex(j);
                                m_b_Bypass.GetArrayElementAtIndex(j + 1).boolValue = false;
                                m_B_AutoStart.InsertArrayElementAtIndex(j);
                                m_B_AutoStart.GetArrayElementAtIndex(j+1).boolValue = false;
                                m_b_ShowBypass.InsertArrayElementAtIndex(j);
                                m_b_ShowBypass.GetArrayElementAtIndex(j + 1).boolValue = true;
                                m_b_ShowAutoStart.InsertArrayElementAtIndex(j);
                                m_b_ShowAutoStart.GetArrayElementAtIndex(j + 1).boolValue = true;
                                
                                m_SceneStepsMultiList.InsertArrayElementAtIndex(j);
                                //m_SceneStepsMultiList.GetArrayElementAtIndex(j + 1).FindPropertyRelative("SceneStepsMultiList").ClearArray();
                                m_b_StepName.InsertArrayElementAtIndex(j);
                                m_b_StepName.GetArrayElementAtIndex(j + 1).stringValue = "";
                                // serializedObject2.ApplyModifiedProperties();
                                break;
                            }
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                Debug.Log("i: " + j);
                                m_b_Bypass.DeleteArrayElementAtIndex(j);
                                m_B_AutoStart.DeleteArrayElementAtIndex(j);
                                m_b_ShowBypass.DeleteArrayElementAtIndex(j);
                                m_b_ShowAutoStart.DeleteArrayElementAtIndex(j);
                                m_SceneStepsMultiList.DeleteArrayElementAtIndex(j);
                                m_b_StepName.DeleteArrayElementAtIndex(j);
                                //serializedObject2.ApplyModifiedProperties();
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();

                    SerializedProperty m_methodsList = m_SceneStepsMultiList.GetArrayElementAtIndex(j).FindPropertyRelative("methodsList");
                 
                    methodModule.displayMethodList("",
                                       editorMethods,
                                       m_methodsList,
                                       myScript.SceneStepsList[i].SceneStepsMultiList[j].methodsList,
                                       style_Blue,
                                       style_Yellow_01,
                                       "",false,true);

                    EditorGUILayout.EndHorizontal();
                }
            }
        }


        if (moreOptions.boolValue) {
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("Delete Selected Scene Steps"))
            {
                m_SceneStepsList.DeleteArrayElementAtIndex(currentStepListDisplayedEditor.intValue);
            }
        }

        serializedObject2.ApplyModifiedProperties();
        #endregion
    }

    private void HelpZone_01()
    {
        EditorGUILayout.HelpBox(
           "Call next step:\n" +
           "SceneStepsManager.instance.NextStep()", MessageType.Info);
        EditorGUILayout.HelpBox(
          "Call specific step:\n" +
          "SceneStepsManager.instance.NextStep(int StepSequence, int StepID)", MessageType.Info);
        EditorGUILayout.HelpBox(
         "Change current Step Sequence:\n" +
         "SceneStepsManager.instance.currentStepSequence", MessageType.Info);
        EditorGUILayout.HelpBox(
         "Change current Step:\n" +
         "SceneStepsManager.instance.currentStep", MessageType.Info);
    }


    void OnSceneGUI()
    {
    }
}
#endif
