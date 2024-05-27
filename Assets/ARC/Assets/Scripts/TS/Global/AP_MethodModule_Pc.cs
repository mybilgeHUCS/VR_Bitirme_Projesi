//Description: AP_MethodModule_Pc: Use to display in a custom editor the module that allow to choose a list of methods to call.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TS.Generics
{
    public class AP_MethodModule_Pc
    {
        private bool b_addMethods = false;                                  // use when you press button +

        // Display a list of custom method in the custom editor
        public void displayMethodList(string _Title,
                                      EditorMethods_Pc editorMethods,
                                      SerializedProperty _methods,
                                      List<EditorMethodsList_Pc.MethodsList> myScriptMethods,
                                      GUIStyle style_Color01,
                                      GUIStyle style_Color02,
                                      string helpBoxText)
        {
            #region
            //-> Custom methods
            EditorGUILayout.BeginVertical(style_Color01);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_Title, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (helpBoxText != "")
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);

            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                b_addMethods = true;
            }

            editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02);

            if (b_addMethods)
            {
                editorMethods.AddMethodsToList(_methods);
                b_addMethods = false;
            }

            EditorGUILayout.EndVertical();
            #endregion
        }

        // Display a list of custom method in the custom editor
        public void displayMethodList(string _Title,
                                      EditorMethods_Pc editorMethods,
                                      SerializedProperty _methods,
                                      List<EditorMethodsList_Pc.MethodsList> myScriptMethods,
                                      GUIStyle style_Color01,
                                      GUIStyle style_Color02,
                                      string helpBoxText,
                                      bool b_ShowHelpBoxText = true,
                                      bool b_ShowARgument = true)
        {
            #region
            //-> Custom methods
            EditorGUILayout.BeginVertical(style_Color01);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_Title, EditorStyles.boldLabel);
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                b_addMethods = true;
            }
            EditorGUILayout.EndHorizontal();

            if (b_ShowHelpBoxText)
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);



            editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02, false, true, b_ShowARgument);

            if (b_addMethods)
            {
                editorMethods.AddMethodsToList(_methods);
                b_addMethods = false;
            }

            EditorGUILayout.EndVertical();
            #endregion
        }

        public void displayOneMethod(string _Title,
                                     EditorMethods_Pc editorMethods,
                                     SerializedProperty _methods,
                                     List<EditorMethodsList_Pc.MethodsList> myScriptMethods,
                                     GUIStyle style_Color01,
                                     GUIStyle style_Color02,
                                     string helpBoxText)
        {
            #region
            //-> Custom methods
            EditorGUILayout.BeginVertical(style_Color01);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_Title, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (helpBoxText != "")
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);

            if (_methods.arraySize < 1)
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    b_addMethods = true;
                }

            editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02);

            if (b_addMethods)
            {
                editorMethods.AddMethodsToList(_methods);
                b_addMethods = false;
            }

            EditorGUILayout.EndVertical();
            #endregion
        }

        // Display a list of custom method in the custom editor
        public void displayOneMethod(string _Title,
                                      EditorMethods_Pc editorMethods,
                                      SerializedProperty _methods,
                                      List<EditorMethodsList_Pc.MethodsList> myScriptMethods,
                                      GUIStyle style_Color01,
                                      GUIStyle style_Color02,
                                      string helpBoxText,
                                      bool b_ShowHelpBoxText = true,
                                      bool b_ShowARgument = true)
        {
            #region
            //-> Custom methods
            EditorGUILayout.BeginVertical(style_Color01);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_Title, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            if (b_ShowHelpBoxText)
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);



            editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02, false, true, b_ShowARgument);

            if (b_addMethods)
            {
                editorMethods.AddMethodsToList(_methods);
                b_addMethods = false;
            }

            EditorGUILayout.EndVertical();
            #endregion
        }

    }
}
#endif
