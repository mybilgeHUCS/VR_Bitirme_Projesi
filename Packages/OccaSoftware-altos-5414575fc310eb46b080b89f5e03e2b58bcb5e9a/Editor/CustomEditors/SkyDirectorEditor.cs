using UnityEngine;
using UnityEditor;
using OccaSoftware.Altos.Runtime;
using UnityEditor.UIElements;

namespace OccaSoftware.Altos.Editor
{
    [CustomEditor(typeof(AltosSkyDirector))]
    [CanEditMultipleObjects]
    public class SkyDirectorEditor : UnityEditor.Editor
    {
        SerializedProperty skyDefinition;
        SerializedProperty atmosphereDefinition;
        SerializedProperty cloudDefinition;
        SerializedProperty temperatureDefinition;
        SerializedProperty precipitationDefinition;
        SerializedProperty enableDebugView;

        SerializedProperty data;

        SerializedObject sky;
        SerializedProperty editorTime;
        SerializedProperty timeSystem;

        private void OnEnable()
        {
            skyDefinition = serializedObject.FindProperty("skyDefinition");
            atmosphereDefinition = serializedObject.FindProperty("atmosphereDefinition");

            cloudDefinition = serializedObject.FindProperty("cloudDefinition");
            temperatureDefinition = serializedObject.FindProperty("temperatureDefinition");
            precipitationDefinition = serializedObject.FindProperty("precipitationDefinition");
            data = serializedObject.FindProperty("data");
            enableDebugView = serializedObject.FindProperty("enableDebugView");

            sky = new SerializedObject(skyDefinition.objectReferenceValue);
            editorTime = sky.FindProperty("editorTime");
            timeSystem = sky.FindProperty("timeSystem");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AltosSkyDirector t = (AltosSkyDirector)target;

            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(skyDefinition);
            if (skyDefinition.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                sky.Update();
                SerializedProperty currentTime = editorTime;
                if (Application.isPlaying)
                {
                    currentTime = timeSystem;
                }
                EditorGUILayout.PropertyField(
                    currentTime,
                    new GUIContent("Current Time", "This option only changes the current time in-editor, not the initial time on game start.")
                );

                EditorGUI.indentLevel--;
                sky.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(atmosphereDefinition);

            EditorGUILayout.PropertyField(cloudDefinition);
            EditorGUILayout.PropertyField(temperatureDefinition);
            EditorGUILayout.PropertyField(precipitationDefinition);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Overrides", EditorStyles.boldLabel);
            GUILayoutOption[] option = new GUILayoutOption[] { GUILayout.Width(75) };

            EditorGUILayout.LabelField("WindZone");
            if (t.GetWind(out AltosWindZone windZone))
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Active", option);
                if (GUILayout.Button("Open", option))
                {
                    Selection.activeObject = windZone.gameObject;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("A WindZone is actively overriding the wind speed and direction in your scene.", EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Inactive", option);
                if (GUILayout.Button("Add", option))
                {
                    GameObject go = new GameObject("Altos Wind Zone");
                    go.AddComponent<AltosWindZone>();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("WeatherMap");
            if (t.GetWeatherMap(out WeatherMap weatherMap))
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Active", option);
                if (GUILayout.Button("Open", option))
                {
                    Selection.activeObject = weatherMap.gameObject;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("A WeatherMap is active in your scene.", EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Warning", option);
                if (GUILayout.Button("Add", option))
                {
                    GameObject go = new GameObject("WeatherMap");
                    go.AddComponent<WeatherMap>();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(
                    "You have no WeatherMap in your scene. You must add one to render volumetric clouds.",
                    EditorStyles.helpBox
                );
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("WeatherManager");
            if (GUILayout.Button("Add", option))
            {
                GameObject go = new GameObject("WeatherManager");
                go.AddComponent<WeatherManager>();
            }
            foreach (WeatherManager mgr in WeatherManager.WeatherManagers)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(mgr.gameObject.name))
                {
                    Selection.activeObject = mgr.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField(
                $"{WeatherManager.WeatherManagers.Count} WeatherManagers {(WeatherManager.WeatherManagers.Count == 1 ? "is" : "are")} active in your scene. Use a WeatherManager to override Precipitation intensity in an area.",
                EditorStyles.helpBox
            );
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableDebugView);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Resources", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(data);

            if (data.serializedObject == null)
            {
                EditorGUILayout.HelpBox("Altos requires an AltosDataAsset, which contains shader and texture data.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
