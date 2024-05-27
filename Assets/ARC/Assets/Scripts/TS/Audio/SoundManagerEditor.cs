//Description: SoundManagerEditor. Work in association with SoundManager.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using TS.Generics;


[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty moreOptions;
    SerializedProperty helpBox;
    SerializedProperty SaveName;
    SerializedProperty listAudioGroupParams;
    void OnEnable()
    {
        #region
        // Setup the SerializedProperties.
        SeeInspector            = serializedObject.FindProperty("SeeInspector");
        helpBox                 = serializedObject.FindProperty("helpBox");
        moreOptions             = serializedObject.FindProperty("moreOptions");
        SaveName                = serializedObject.FindProperty("SaveName");
        listAudioGroupParams    = serializedObject.FindProperty("listAudioGroupParams");
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

        if (helpBox.boolValue)
            HelpZone_01();

        EditorGUILayout.LabelField("");

        //-> Reset Zone
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete Audio Mixer saved data"))
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

        EditorGUILayout.LabelField("");

        displayAudioGroups();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }


    void displayAudioGroups()
    {

        EditorGUILayout.BeginHorizontal();
        if (moreOptions.boolValue)
            EditorGUILayout.LabelField("", GUILayout.Width(20));
        EditorGUILayout.LabelField("ID", GUILayout.Width(20));
        EditorGUILayout.LabelField("| Exposed Parmeters:", GUILayout.Width(130));
        EditorGUILayout.LabelField("| Default Volume");
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i< listAudioGroupParams.arraySize; i++)
        {
            SerializedProperty m_exposedParameterName = listAudioGroupParams.GetArrayElementAtIndex(i).FindPropertyRelative("exposedParameterName");
            SerializedProperty m_volume = listAudioGroupParams.GetArrayElementAtIndex(i).FindPropertyRelative("volume");
            
            EditorGUILayout.BeginHorizontal();
            if (moreOptions.boolValue && GUILayout.Button("-", GUILayout.Width(20)))
            {
                listAudioGroupParams.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.LabelField(i + ": ", GUILayout.Width(20));
            EditorGUILayout.PropertyField(m_exposedParameterName, new GUIContent(""), GUILayout.Width(130));
            EditorGUILayout.PropertyField(m_volume, new GUIContent(""), GUILayout.Width(30));

            EditorGUILayout.EndHorizontal();
        }
        if (moreOptions.boolValue && GUILayout.Button("Create a new group"))
        {
            listAudioGroupParams.InsertArrayElementAtIndex(0);
            listAudioGroupParams.GetArrayElementAtIndex(0).FindPropertyRelative("exposedParameterName").stringValue = "";
            listAudioGroupParams.GetArrayElementAtIndex(0).FindPropertyRelative("volume").floatValue = 0;
            listAudioGroupParams.MoveArrayElement(0, listAudioGroupParams.arraySize - 1);
        }
    }

    private void HelpZone_01()
    {
        #region
        EditorGUILayout.HelpBox(
            "Load AudioMixer Volumes (bool):\n" +
            " SoundManager.instance.Bool_LoadMixerValues()", MessageType.Info);

        EditorGUILayout.HelpBox(
           "Save AudioMixer Volumes (bool): \n" +
           "SoundManager.instance.Bool_SaveMixerValues()", MessageType.Info);

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

    void OnSceneGUI()
    {
    }
}
#endif
