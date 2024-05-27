//Description: AmbianceListEditor. Work in association with AmbianceList.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using TS.Generics;

[CustomEditor(typeof(AmbianceList))]
public class AmbianceListEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
    SerializedProperty helpBox;
    SerializedProperty moreOptions;
    SerializedProperty listAudioClip;

    void OnEnable()
    {
        #region
        // Setup the SerializedProperties.
        SeeInspector    = serializedObject.FindProperty("SeeInspector");
        helpBox         = serializedObject.FindProperty("helpBox");
        moreOptions     = serializedObject.FindProperty("moreOptions");
        listAudioClip   = serializedObject.FindProperty("ListAudioClip");
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
        
        if(helpBox.boolValue)HelpZone_01();

        //EditorGUILayout.LabelField("");
        displayAmbianceList();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("");
        #endregion
    }


    void displayAmbianceList()
    {
        EditorGUILayout.LabelField("ID:");
        for (var i = 0; i < listAudioClip.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + ":", GUILayout.Width(30));
            EditorGUILayout.PropertyField(listAudioClip.GetArrayElementAtIndex(i), new GUIContent(""));

            if (EditorPrefs.GetBool("MoreOptions") == true && moreOptions.boolValue)
            {
                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    listAudioClip.MoveArrayElement(i, i - 1);
                    break;
                }
                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    listAudioClip.MoveArrayElement(i, i + 1);
                    break;
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    listAudioClip.InsertArrayElementAtIndex(i);
                    listAudioClip.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    listAudioClip.MoveArrayElement(i, i + 1);
                    break;
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (listAudioClip.arraySize > 1)
                    {
                        listAudioClip.GetArrayElementAtIndex(i).objectReferenceValue = null;
                        listAudioClip.DeleteArrayElementAtIndex(i);
                    }
                       
                    break;
                }
            }

                

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add a new AudioClip to the list"))
        {
            listAudioClip.InsertArrayElementAtIndex(0);
            listAudioClip.GetArrayElementAtIndex(0).objectReferenceValue = null;
            listAudioClip.MoveArrayElement(0, listAudioClip.arraySize - 1);
        }

    }

    private void HelpZone_01()
    {

        #region
        EditorGUILayout.HelpBox(
          "Access an ambiance: \n" +
          "AmbianceList.instance.listAudioClip[int value]", MessageType.Info);

        EditorGUILayout.HelpBox(
           "Play an ambiance (Crossfade):\n" +
           "AmbianceManager.instance.MCrossFade(float crossFadeSpeed, AudioClip newClip = null, int whichAnimCurve = 0)", MessageType.Info);

        EditorGUILayout.HelpBox(
           "Stop current ambiance (fade Out): \n" +
           "AmbianceManager.instance.MFadeOut(float FadeOutSpeed, int whichAnimCurve = 0)", MessageType.Info);

        #endregion
    }




    void OnSceneGUI()
    {
    }
}
#endif
