// Description: w_Various 
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TS.Generics;
using Object = UnityEngine.Object;
using TMPro;

public class w_Various : EditorWindow
{
    private Vector2 scrollPosAll;
    SerializedObject serializedObject;
    SerializedProperty fontFrom;
    SerializedProperty fontTo;
    SerializedProperty fontTMProFrom;
    SerializedProperty fontTMProTo;

    SerializedProperty currentTextType;

    SerializedProperty b_UseFontStyleFrom;
    SerializedProperty fontStyleFrom;
    SerializedProperty b_UseFontSizeFrom;
    SerializedProperty fontSizeFrom;
    SerializedProperty b_UseFontColorFrom;
    SerializedProperty fontColorFrom;

    SerializedProperty b_UseFontStyleTo;
    SerializedProperty fontStyleTo;
    SerializedProperty b_UseFontSizeTo;
    SerializedProperty fontSizeTo;
    SerializedProperty b_UseFontColorTo;
    SerializedProperty fontColorTo;

    TextModifierDatas textModifierDatas;

    public Object ObjFromTmp;
    public Object ObjToTmp;

    SerializedProperty currentSection;


    [MenuItem("Tools/TS/Other/w_Various")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Various));
    }

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
        string objectPath = "Assets/ARC/Assets/Datas/Ref/TextModifierDatas.asset";
        textModifierDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as TextModifierDatas;

        if (textModifierDatas)
        {
            serializedObject = new UnityEditor.SerializedObject(textModifierDatas);
            fontFrom = serializedObject.FindProperty("fontFrom");
            fontTo = serializedObject.FindProperty("fontTo");

            fontTMProFrom = serializedObject.FindProperty("fontTMProFrom");
            fontTMProTo = serializedObject.FindProperty("fontTMProTo");

            currentTextType = serializedObject.FindProperty("currentTextType");

            b_UseFontStyleFrom = serializedObject.FindProperty("b_UseFontStyleFrom");
            fontStyleFrom = serializedObject.FindProperty("fontStyleFrom");
            b_UseFontSizeFrom = serializedObject.FindProperty("b_UseFontSizeFrom");
            fontSizeFrom = serializedObject.FindProperty("fontSizeFrom");
            b_UseFontColorFrom = serializedObject.FindProperty("b_UseFontColorFrom");
            fontColorFrom = serializedObject.FindProperty("fontColorFrom");

            b_UseFontStyleTo = serializedObject.FindProperty("b_UseFontStyleTo");
            fontStyleTo = serializedObject.FindProperty("fontStyleTo");
            b_UseFontSizeTo = serializedObject.FindProperty("b_UseFontSizeTo");
            fontSizeTo = serializedObject.FindProperty("fontSizeTo");

            b_UseFontColorTo = serializedObject.FindProperty("b_UseFontColorTo");
            fontColorTo = serializedObject.FindProperty("fontColorTo");

            currentSection = serializedObject.FindProperty("currentSection");

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

        serializedObject.Update();

        currentSection.intValue = GUILayout.SelectionGrid(currentSection.intValue, new string[] { "Object Placer", "Text Modifier" }, 2);

        EditorGUILayout.LabelField("");

        switch (currentSection.intValue)
        {
            case 0:
                MoveObjectToPosition();
                break;
            case 1:
                TextModifier();
                break;
            
        }
        EditorGUILayout.LabelField("");

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();
        #endregion
    }


    void TextModifier()
    {
        string[] arrName = new string[2] { "Text", "TMPro" };
        currentTextType.intValue = EditorGUILayout.Popup(currentTextType.intValue, arrName);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("From Font:",EditorStyles.boldLabel, GUILayout.Width(85));

        if(currentTextType.intValue == 0)
            EditorGUILayout.PropertyField(fontFrom, new GUIContent(""));
        else
            EditorGUILayout.PropertyField(fontTMProFrom, new GUIContent(""));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Style:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontStyleFrom, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontStyleFrom, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Size:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontSizeFrom, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontSizeFrom, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Color:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontColorFrom, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontColorFrom, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("To Font:", EditorStyles.boldLabel, GUILayout.Width(85));

        if (currentTextType.intValue == 0)
            EditorGUILayout.PropertyField(fontTo, new GUIContent(""));
        else
            EditorGUILayout.PropertyField(fontTMProTo, new GUIContent(""));
       
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Style:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontStyleTo, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontStyleTo, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Size:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontSizeTo, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontSizeTo, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Color:", GUILayout.Width(85));
        EditorGUILayout.PropertyField(b_UseFontColorTo, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.PropertyField(fontColorTo, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        if (GUILayout.Button("Change"))
        {
            if (currentTextType.intValue == 0)
                ChangeFontTextCase();
            else
                ChangeFontTMProCase();
        }
    }

    void ChangeFontTMProCase()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject go = rootObjects[i];

            TextMeshProUGUI[] allText = go.GetComponentsInChildren<TextMeshProUGUI>(true);
            List<TextMeshProUGUI> compareTextStyle = new List<TextMeshProUGUI>();

            // Compare Style
            foreach (TextMeshProUGUI txt in allText)
            {
                if ((TMP_FontAsset)fontTMProFrom.objectReferenceValue != null &&
                    txt.font == (TMP_FontAsset)fontTMProFrom.objectReferenceValue)
                {
                    if (b_UseFontStyleFrom.boolValue)
                    {
                        if ((FontStyles)fontStyleFrom.enumValueIndex == txt.fontStyle)
                            compareTextStyle.Add(txt);
                    }
                    else
                    {
                        compareTextStyle.Add(txt);
                    }
                }
            }

            //Debug.Log("How many text to change (Style): " + compareTextStyle.Count);

            List<TextMeshProUGUI> compareTextSize = new List<TextMeshProUGUI>();
            // Compare Size
            foreach (TextMeshProUGUI txt in compareTextStyle)
            {
                if ((TMP_FontAsset)fontTMProFrom.objectReferenceValue != null &&
                    txt.font == (TMP_FontAsset)fontTMProFrom.objectReferenceValue)
                {
                    if (b_UseFontSizeFrom.boolValue)
                    {
                        if (fontSizeFrom.intValue == txt.fontSize)
                            compareTextSize.Add(txt);
                    }
                    else
                    {
                        compareTextSize.Add(txt);
                    }
                }
            }

            
            //Debug.Log("How many text to change (Size): " + compareTextSize.Count);

            List<TextMeshProUGUI> compareTextColor = new List<TextMeshProUGUI>();
            // Compare Color
            foreach (TextMeshProUGUI txt in compareTextSize)
            {
                if ((TMP_FontAsset)fontTMProFrom.objectReferenceValue != null &&
                    txt.font == (TMP_FontAsset)fontTMProFrom.objectReferenceValue)
                {
                    if (b_UseFontColorFrom.boolValue)
                    {
                        if (fontColorFrom.colorValue == txt.color)
                            compareTextColor.Add(txt);
                    }
                    else
                    {
                        compareTextColor.Add(txt);
                    }
                }
            }
            

            foreach (TextMeshProUGUI txt in compareTextColor)
            {
                Undo.RegisterFullObjectHierarchyUndo(txt.gameObject, txt.gameObject.name);
                txt.font = (TMP_FontAsset)fontTMProTo.objectReferenceValue;

                if (b_UseFontStyleTo.boolValue)
                {
                    txt.fontStyle = (FontStyles)fontStyleTo.enumValueIndex;
                }

                if (b_UseFontSizeTo.boolValue)
                {
                    txt.fontSize = fontSizeTo.intValue;
                }

                if (b_UseFontColorTo.boolValue)
                {
                    txt.color = fontColorTo.colorValue;
                }
                EditorUtility.SetDirty(txt);
            }
        }
    }

    //
    void ChangeFontTextCase()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject go = rootObjects[i];

            Text[] allText = go.GetComponentsInChildren<Text>(true);
            List<Text> compareTextStyle = new List<Text>();

           // Compare Style
           foreach(Text txt in allText)
            {
                if((Font)fontFrom.objectReferenceValue != null &&
                    txt.font == (Font)fontFrom.objectReferenceValue)
                {
                    if (b_UseFontStyleFrom.boolValue)
                    {
                        if ((FontStyle)fontStyleFrom.enumValueIndex == txt.fontStyle)
                            compareTextStyle.Add(txt);
                    }
                    else
                    {
                        compareTextStyle.Add(txt);
                    }
                }
            }

            //Debug.Log("How many text to change (Style): " + compareTextStyle.Count);

            List<Text> compareTextSize = new List<Text>();
            // Compare Size
            foreach (Text txt in compareTextStyle)
            {
                if ((Font)fontFrom.objectReferenceValue != null &&
                    txt.font == (Font)fontFrom.objectReferenceValue)
                {
                    if (b_UseFontSizeFrom.boolValue)
                    {
                        if (fontSizeFrom.intValue == txt.fontSize)
                            compareTextSize.Add(txt);
                    }
                    else
                    {
                        compareTextSize.Add(txt);
                    }
                }
            }

            //Debug.Log("How many text to change (Size): " + compareTextSize.Count);

            List<Text> compareTextColor = new List<Text>();
            // Compare Color
            foreach (Text txt in compareTextSize)
            {
                if ((Font)fontFrom.objectReferenceValue != null &&
                    txt.font == (Font)fontFrom.objectReferenceValue)
                {
                    if (b_UseFontColorFrom.boolValue)
                    {
                        if (fontColorFrom.colorValue == txt.color)
                            compareTextColor.Add(txt);
                    }
                    else
                    {
                        compareTextColor.Add(txt);
                    }
                }
            }


            foreach (Text txt in compareTextColor)
            {
                Undo.RegisterFullObjectHierarchyUndo(txt.gameObject, txt.gameObject.name);
                txt.font = (Font)fontTo.objectReferenceValue;

                if (b_UseFontStyleTo.boolValue)
                {
                    txt.fontStyle = (FontStyle)fontStyleTo.enumValueIndex;
                }

                if (b_UseFontSizeTo.boolValue)
                {
                    txt.fontSize = fontSizeTo.intValue;
                }

                if (b_UseFontColorTo.boolValue)
                {
                    txt.color = fontColorTo.colorValue;
                }
                EditorUtility.SetDirty(txt);
            }
        }
    }


    void OnInspectorUpdate()
    {
        Repaint();
    }

    

    void MoveObjectToPosition()
    {
        if (textModifierDatas)
        {

            EditorGUILayout.HelpBox(
               "Select the object to move", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("From: ", GUILayout.Width(40));


            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                ObjFromTmp = Selection.activeGameObject;
            }
            ObjFromTmp = EditorGUILayout.ObjectField(ObjFromTmp, typeof(Object), true, GUILayout.MinWidth(100));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
               "Select the position where the object should be moved", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("To: ", GUILayout.Width(40));
            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                ObjToTmp = Selection.activeGameObject;
            }
            ObjToTmp = EditorGUILayout.ObjectField(ObjToTmp, typeof(Object), true, GUILayout.MinWidth(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
               "Move the FROM object to the position of the TO object", MessageType.Info);
            if (GUILayout.Button("Process movement", GUILayout.MinWidth(150)))
            {
                if(ObjFromTmp && ObjToTmp)
                {
                    GameObject obj01 = (GameObject)ObjFromTmp;
                    GameObject obj02 = (GameObject)ObjToTmp;

                    Undo.RegisterFullObjectHierarchyUndo(obj01.gameObject, obj01.name);
                    obj01.transform.position = obj02.transform.position;
                    obj01.transform.rotation = obj02.transform.rotation;
                }
            }
            if (GUILayout.Button("Reset", GUILayout.MinWidth(150)))
            {
                ObjFromTmp = null;
                ObjToTmp = null;
            }
        }
    }
}
#endif