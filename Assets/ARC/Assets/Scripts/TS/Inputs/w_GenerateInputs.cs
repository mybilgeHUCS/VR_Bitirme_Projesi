
// Description : w_PuzzlesCreator_Pc.cs :  Allow to create puzzles and access some puzzles parameters
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TS.Generics
{
    public class w_GenerateInputs : EditorWindow
    {
        private Vector2 scrollPosAll;
      
        [MenuItem("Tools/TS//Other/ARC/w_GenerateInputs")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(w_GenerateInputs));
        }

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

        private Texture2D Tex_01;
        private Texture2D Tex_02;
        private Texture2D Tex_03;
        private Texture2D Tex_04;
        private Texture2D Tex_05;

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
            _MakeTexture();
            #endregion
        }

        void OnGUI()
        {
            #region
            //--> Scrollview
            scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
            //--> Window description
            //GUI.backgroundColor = _cGreen;
            CheckTex();
            GUIStyle style_Yellow_01 = new GUIStyle(GUI.skin.box); style_Yellow_01.normal.background = Tex_01;
            GUIStyle style_Blue = new GUIStyle(GUI.skin.box); style_Blue.normal.background = Tex_03;
            GUIStyle style_Purple = new GUIStyle(GUI.skin.box); style_Purple.normal.background = Tex_04;
            GUIStyle style_Orange = new GUIStyle(GUI.skin.box); style_Orange.normal.background = Tex_05;
            GUIStyle style_Yellow_Strong = new GUIStyle(GUI.skin.box); style_Yellow_Strong.normal.background = Tex_02;

            EditorGUILayout.BeginVertical(style_Yellow_01);
            ARC_CreateInputs();
            ARC_CreateJoysticInputs();
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("");

            EditorGUILayout.EndScrollView();
            #endregion
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        //--> If texture2D == null recreate the texture (use for color in the custom editor)
        private void CheckTex()
        {
            if (Tex_01 == null || Tex_02 == null || Tex_03 == null || Tex_04 == null || Tex_05 == null)
            {
                _MakeTexture();
            }
        }

        private void _MakeTexture()
        {
            #region
            if (EditorPrefs.GetBool("AP_ProSkin") == true)
            {
                float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
                Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
                Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
                Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
                Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, 1f));
                Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            }
            else
            {
                Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
                Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
                Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
                Tex_04 = MakeTex(2, 2, new Color(.4f, 1f, .9F, 1f));
                Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
            }
            #endregion
        }

        private static void CreateInput(inputParams newInput)
        {
            #region
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

            serializedObject.Update();
            SerializedProperty m_Axes = serializedObject.FindProperty("m_Axes");

            m_Axes.InsertArrayElementAtIndex(m_Axes.arraySize - 1);

            SerializedProperty m_Name = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("m_Name");
            SerializedProperty descriptiveName = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("descriptiveName");
            SerializedProperty descriptiveNegativeName = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("descriptiveNegativeName");
            SerializedProperty negativeButton = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("negativeButton");
            SerializedProperty positiveButton = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("positiveButton");
            SerializedProperty altNegativeButton = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("altNegativeButton");
            SerializedProperty altPositiveButton = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("altPositiveButton");
            SerializedProperty gravity = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("gravity");
            SerializedProperty dead = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("dead");
            SerializedProperty sensitivity = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("sensitivity");
            SerializedProperty snap = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("snap");
            SerializedProperty invert = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("invert");
            SerializedProperty type = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("type");
            SerializedProperty axis = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("axis");
            SerializedProperty joyNum = m_Axes.GetArrayElementAtIndex(m_Axes.arraySize - 1).FindPropertyRelative("joyNum");

            m_Name.stringValue = newInput.name;
            descriptiveName.stringValue = newInput.descriptiveName;
            descriptiveNegativeName.stringValue = newInput.descriptiveNegativeName;
            negativeButton.stringValue = newInput.negativeButton;
            positiveButton.stringValue = newInput.positiveButton;
            altNegativeButton.stringValue = newInput.altNegativeButton;
            altPositiveButton.stringValue = newInput.altPositiveButton;
            gravity.floatValue = newInput.gravity;
            dead.floatValue = newInput.dead;
            sensitivity.floatValue = newInput.sensitivity;
            snap.boolValue = newInput.snap;
            invert.boolValue = newInput.invert;
            type.intValue = newInput.type;
            axis.intValue = newInput.axis;
            joyNum.intValue = newInput.joyNum;

            serializedObject.ApplyModifiedProperties();
            #endregion
        }

        public class inputParams
        {
            #region
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public int type;

            public int axis;
            public int joyNum;
            #endregion
        }

        public static bool checkIfAlreadyExist(string nameToCheck)
        {
            #region
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty m_Axes = serializedObject.FindProperty("m_Axes");
            serializedObject.Update();
            Debug.Log(m_Axes.arraySize);
            for (var i = 0; i < m_Axes.arraySize; i++)
            {
                if (m_Axes.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == nameToCheck)
                    return true;
            }

            return false;
            #endregion
        }

        public void ARC_CreateInputs()
        {

            EditorGUILayout.LabelField("");

            if (GUILayout.Button("Create Event System Inputs"))
            {
                #region 
                if (checkIfAlreadyExist("TSSubmit"))
                    Debug.Log("Already Exist");
                else
                {
                    CreateInput(new inputParams()
                    {
                        name = "TSSubmit",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "",
                        positiveButton = "",
                        altNegativeButton = "",
                        gravity = 1000,
                        dead = .001f,
                        sensitivity = 1000,
                        snap = true,
                        invert = false,
                        type = 0,
                        axis = 0,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSHorizontal",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "",
                        positiveButton = "",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = false,
                        type = 2,
                        axis = 0,
                        joyNum = 1
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSVertical",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "",
                        positiveButton = "",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = true,
                        type = 2,
                        axis = 1,
                        joyNum = 1
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSHorizontalRemap",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "",
                        positiveButton = "",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = false,
                        type = 2,
                        axis = 0,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSVerticalRemap",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "",
                        positiveButton = "",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = true,
                        type = 2,
                        axis = 1,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSHorizontal",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "left",
                        positiveButton = "right",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = false,
                        type = 0,
                        axis = 0,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSVertical",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "down",
                        positiveButton = "up",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = false,
                        invert = false,
                        type = 0,
                        axis = 1,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSHorizontalRemap",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "left",
                        positiveButton = "right",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = true,
                        invert = false,
                        type = 0,
                        axis = 0,
                        joyNum = 0
                    });

                    CreateInput(new inputParams()
                    {
                        name = "TSVerticalRemap",
                        descriptiveName = "",
                        descriptiveNegativeName = "",
                        negativeButton = "down",
                        positiveButton = "up",
                        altNegativeButton = "",
                        gravity = 1,
                        dead = .001f,
                        sensitivity = 1,
                        snap = false,
                        invert = false,
                        type = 0,
                        axis = 1,
                        joyNum = 0
                    });
                }
                #endregion
            }
            EditorGUILayout.LabelField("");



        }




        public void ARC_CreateJoysticInputs()
        {


            EditorGUILayout.LabelField("");

            if (GUILayout.Button("Create Joystick Inputs for player 1 and 2"))
            {
                for (var k = 0; k < 2; k++)
                {
                    int whichPlayer = k + 1;
                    //-> Create Joystick Axis
                    for (var i = 0; i < 10; i++)
                    {
                        #region 
                        if (checkIfAlreadyExist("Joystick" + whichPlayer + "Axis" + (i + 1).ToString()))
                            Debug.Log("Already Exist");
                        else
                        {
                            CreateInput(new inputParams()
                            {
                                name = "Joystick" + whichPlayer + "Axis" + (i + 1).ToString(),
                                descriptiveName = "",
                                descriptiveNegativeName = "",
                                negativeButton = "",
                                positiveButton = "",
                                altNegativeButton = "",
                                gravity = 0,
                                dead = .19f,
                                sensitivity = 1.0f,
                                snap = true,
                                invert = false,
                                type = 2,
                                axis = i,
                                joyNum = whichPlayer
                            });
                        }
                        #endregion
                    }

                    //-> Create Joystick Axis
                    for (var i = 0; i < 20; i++)
                    {
                        #region 
                        if (checkIfAlreadyExist("Joystick" + whichPlayer + "Button" + (i).ToString()))
                            Debug.Log("Already Exist");
                        else
                        {
                            CreateInput(new inputParams()
                            {
                                name = "Joystick" + whichPlayer + "Button" + (i).ToString(),
                                descriptiveName = "",
                                descriptiveNegativeName = "",
                                negativeButton = "",
                                positiveButton = "joystick " + whichPlayer + " button " + i,
                                altNegativeButton = "",
                                gravity = 1,
                                dead = .001f,
                                sensitivity = 1.0f,
                                snap = true,
                                invert = false,
                                type = 0,
                                axis = 0,
                                joyNum = whichPlayer
                            });
                        }
                        #endregion
                    }
                }

            }
            EditorGUILayout.LabelField("");
        }

    }
}
#endif

