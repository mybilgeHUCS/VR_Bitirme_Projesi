#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System;
using TS.Generics;

public class TextVariousMethods
{
    public void DisplaySpecificEntry(SerializedProperty _WhichTextData, SerializedProperty _DisplayedEntry, GameObject _TextManager)
    {
        #region


        String[] arrayDataName = new string[_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count];


        for (var i = 0; i < _TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count; i++)
        {
            arrayDataName[i] = _TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList[i].name;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Folder", GUILayout.Width(40));
        EditorGUILayout.LabelField(_WhichTextData.intValue + ": ", GUILayout.Width(20));
        _WhichTextData.intValue = EditorGUILayout.Popup(_WhichTextData.intValue, arrayDataName, GUILayout.MinWidth(50));
        EditorGUILayout.LabelField("ID:", GUILayout.Width(20));
        if (GUILayout.Button("<", GUILayout.Width(20)))
        {
            if (_DisplayedEntry.intValue > 0)
                _DisplayedEntry.intValue--;
        }
        EditorGUILayout.PropertyField(_DisplayedEntry, new GUIContent(""), GUILayout.Width(30));
        if (GUILayout.Button(">", GUILayout.Width(20)))
        {
            _DisplayedEntry.intValue++;
        }

        if (GUILayout.Button("Open TextEditor"))
        {
            EditorWindow.GetWindow(typeof(w_TextCreator));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.EndHorizontal();



        if (_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count > _WhichTextData.intValue)
        {
            SerializedObject serializedObject_1 = new SerializedObject((TextDatas)_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList[_WhichTextData.intValue]);
            SerializedProperty m_listOfTexts = serializedObject_1.FindProperty("TextsList");


            if (m_listOfTexts.arraySize > _DisplayedEntry.intValue)
            {
                EditorGUILayout.LabelField(m_listOfTexts.GetArrayElementAtIndex(_DisplayedEntry.intValue).FindPropertyRelative("multiLanguage").GetArrayElementAtIndex(0).stringValue, EditorStyles.textArea);
               
            }
              
            else
                EditorGUILayout.LabelField("Doesn't Exist");
        }
        else
        {
            EditorGUILayout.LabelField("Doesn't Exist");
        }
        #endregion
    }

    public string ReturnSpecificEntry(SerializedProperty _WhichTextData, SerializedProperty _DisplayedEntry, GameObject _TextManager)
    {
        #region


        String[] arrayDataName = new string[_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count];


        for (var i = 0; i < _TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count; i++)
        {
            arrayDataName[i] = _TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList[i].name;
        }



        if (_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList.Count > _WhichTextData.intValue)
        {
            SerializedObject serializedObject_1 = new SerializedObject((TextDatas)_TextManager.GetComponent<LanguageManager>()._GlobalTextDatas.textDatasList[_WhichTextData.intValue]);
            SerializedProperty m_listOfTexts = serializedObject_1.FindProperty("TextsList");


            if (m_listOfTexts.arraySize > _DisplayedEntry.intValue)
                return m_listOfTexts.GetArrayElementAtIndex(_DisplayedEntry.intValue).FindPropertyRelative("multiLanguage").GetArrayElementAtIndex(0).stringValue;
            else
                return "Doesn't Exist";
        }
        else
        {
            return "Doesn't Exist";
        }
        #endregion
    }
}
#endif