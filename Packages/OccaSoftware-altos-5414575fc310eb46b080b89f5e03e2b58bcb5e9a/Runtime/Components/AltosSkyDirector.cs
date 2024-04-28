using System;

using System.Linq;
using System.Collections.Generic;

using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Profiling;

namespace OccaSoftware.Altos.Runtime
{
    [AddComponentMenu("OccaSoftware/Altos/Altos Sky Director")]
    [ExecuteAlways]
    public class AltosSkyDirector : MonoBehaviour
    {
        private static AltosSkyDirector instance;
        public static AltosSkyDirector Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<AltosSkyDirector>();

                return instance;
            }
        }

        public SkyDefinition skyDefinition;

        public AtmosphereDefinition atmosphereDefinition;

        //[Reload("Definitions/Cloud Definition.asset")]
        public CloudDefinition cloudDefinition;

        public TemperatureDefinition temperatureDefinition;

        public PrecipitationDefinition precipitationDefinition;

        public Action<Vector3> onOriginShift;

        public float GetCurrentPrecipitation()
        {
            return precipitationDefinition == null ? 0f : precipitationDefinition.currentPrecipitation;
        }

        public float GetCurrentCloudiness()
        {
            return cloudDefinition ? cloudDefinition.currentCloudiness : 0f;
        }

        [Reload("Runtime/Data/AltosDataAsset.asset")]
        public AltosData data;

        [HideInInspector]
        public AltosWindZone windZone;

        public bool GetWind(out AltosWindZone windZone)
        {
            windZone = null;
            if (this.windZone != null)
            {
                windZone = this.windZone;
                return true;
            }

            return false;
        }

        public void SetWindZone(AltosWindZone windZone)
        {
            this.windZone = windZone;
        }

        public void ClearWindZone()
        {
            windZone = null;
        }

        [HideInInspector]
        public WeatherMap weatherMap;

        public bool GetWeatherMap(out WeatherMap weatherMap)
        {
            weatherMap = null;
            if (this.weatherMap != null)
            {
                weatherMap = this.weatherMap;
                return true;
            }

            return false;
        }

        public void SetWeatherMap(WeatherMap weatherMap)
        {
            this.weatherMap = weatherMap;
        }

        public void ClearWeatherMap()
        {
            windZone = null;
        }

        private const float maxDistance = 10f;
        public static float _HOURS_TO_DEGREES = 15f;
        private bool isValidSetup;
        private CloudState cloudState;

        /// <summary>
        /// Returns the current cloud state, which details the cloud positions and has some functions to manage cloud positions.
        /// </summary>
        public CloudState CloudState
        {
            get => cloudState;
        }

        private List<SkyObject> _SkyObjects = new List<SkyObject>();
        public List<SkyObject> SkyObjects
        {
            get => _SkyObjects;
        }

        private List<SkyObject> _Sun = new List<SkyObject>();
        public SkyObject Sun
        {
            get => _Sun.Count > 0 ? _Sun[0] : null;
        }

        internal void RegisterSkyObject(SkyObject skyObject)
        {
            if (!_SkyObjects.Contains(skyObject))
            {
                _SkyObjects.Add(skyObject);
            }

            if (skyObject.type == SkyObject.ObjectType.Sun && !_Sun.Contains(skyObject))
            {
                _Sun.Add(skyObject);
            }

            _SkyObjects = _SkyObjects.OrderByDescending(o => o.sortOrder).ToList();
        }

        internal void DeregisterSkyObject(SkyObject skyObject)
        {
            _SkyObjects.Remove(skyObject);
            _Sun.Remove(skyObject);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            ReloadNullProperties();
            SetIcon();
#endif
            instance = this;
            cloudState = new CloudState(this);
            Initialize();
        }

        #region Editor
#if UNITY_EDITOR
        private void ReloadNullProperties()
        {
            ResourceReloader.TryReloadAllNullIn(this, "Packages/com.occasoftware.altos");
        }

        private void SetIcon()
        {
            string directory = AltosData.packagePath + "/Textures/Editor/";
            string id = "day-night-icon.png";
            Texture2D icon = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(directory + id, typeof(Texture2D));
            UnityEditor.EditorGUIUtility.SetIconForObject(gameObject, icon);
        }
#endif
        #endregion


        public void Initialize()
        {
            isValidSetup = ValidateSetup();

            if (!isValidSetup)
                return;

            skyDefinition.Initialize();
        }

        private bool ValidateSetup()
        {
            if (skyDefinition == null)
                return false;

            return true;
        }

        float freezingLevel;

        private void Update()
        {
            Profiler.BeginSample("TimeOfDayManager: Update Execution");
            Shader.SetGlobalFloat(ShaderParams.altos_DaytimeFactor, daytimeFactor);
            Shader.SetGlobalFloat(ShaderParams.altos_FreezingLevel, freezingLevel);
            Shader.SetGlobalVector(ShaderParams.altos_WorldPositionOffset, (Vector4)origin);
            Shader.SetGlobalVector("altos_WeathermapPosition", transform.position);

            if (!isValidSetup)
                return;

            if (skyDefinition != null)
            {
                skyDefinition.Update();
                daytimeFactor = skyDefinition.GetDaytimeFactor();
            }

            if (temperatureDefinition != null)
            {
                temperatureDefinition.UpdateTemperature(skyDefinition.SystemTime);
                freezingLevel = temperatureDefinition.GetFreezingAltitude();
            }

            if (precipitationDefinition != null)
            {
                precipitationDefinition.UpdatePrecipitation(skyDefinition.SystemTime);
            }

            if (cloudDefinition != null)
            {
                cloudDefinition.UpdateCloudiness(skyDefinition.SystemTime);
            }

            if (cloudState != null)
            {
                cloudState.Update();
            }

            ResetScaleAndRotation();

            Profiler.EndSample();
        }

        /// <summary>
        /// Returns current daytime factor. 1 = Day, 0 = Night.
        /// </summary>
        [HideInInspector]
        public float daytimeFactor = 1f;

        private void ResetScaleAndRotation()
        {
            if (transform.localScale != Vector3.one || transform.rotation != Quaternion.identity)
            {
                transform.localScale = Vector3.one;
                transform.rotation = Quaternion.identity;
            }
        }

        private Vector3 origin = Vector3.zero;

        /// <summary>
        /// Override the current world origin for origin shifting.
        /// </summary>
        /// <param name="origin">The origin offset</param>
        public void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
            Shader.SetGlobalVector(ShaderParams.altos_WorldPositionOffset, (Vector4)origin);
            if (onOriginShift != null)
            {
                onOriginShift(origin);
            }
        }

        public Vector3 GetOrigin()
        {
            return origin;
        }

        /// <summary>
        /// Transform an altitude from offset space to non-offset world space.
        /// </summary>
        /// <param name="altitude"></param>
        /// <returns></returns>
        public float TransformOffsetToWorldPosition(float altitude)
        {
            return altitude -= GetOrigin().y;
        }

        /// <summary>
        /// Transform a position from offset space to non-offset world space.
        /// </summary>
        /// <param name="altitude"></param>
        /// <returns></returns>
        public Vector3 TransformOffsetToWorldPosition(Vector3 position)
        {
            return position -= GetOrigin();
        }

        /// <summary>
        /// Transform an altitude from non-offset spce to offset space.
        /// </summary>
        /// <param name="altitude"></param>
        /// <returns></returns>
        public float TransformWorldToOffsetPosition(float altitude)
        {
            return altitude += GetOrigin().y;
        }

        private static class ShaderParams
        {
            public static int altos_WorldPositionOffset = Shader.PropertyToID("altos_WorldPositionOffset");
            public static int altos_DaytimeFactor = Shader.PropertyToID("altos_DaytimeFactor");
            public static int altos_FreezingLevel = Shader.PropertyToID("altos_FreezingLevel");
        }

        private void OnGUI()
        {
            if (!enableDebugView)
                return;

            DrawTimeDebugView();
            DrawTemperatureDebugView();
            DrawPrecipitationDebugView();
            DrawCloudinessDebugView();
        }

        private List<float> temperatures = new List<float>(12);

        public bool enableDebugView = true;

        private void DrawTimeDebugView()
        {
            GUILayout.BeginArea(new Rect(20, 20, 100, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.Label("Time");
            GUILayout.Label(skyDefinition.SystemTime.ToString("0.0"));
            GUILayout.Space(20);

            for (int i = 0; i < 12; i++)
            {
                GUILayout.Label(((int)skyDefinition.SystemTime + i).ToString("0.0"));
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawTemperatureDebugView()
        {
            temperatures.Clear();

            for (int i = 0; i < 12; i++)
            {
                temperatures.Add(temperatureDefinition.GetTemperatureAtTime((int)skyDefinition.SystemTime + i));
            }

            GUILayout.BeginArea(new Rect(220, 20, 100, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.Label("Temperature");
            GUILayout.Label(temperatureDefinition.currentTemperatureAtSeaLevel.ToString("0.0"));
            GUILayout.Space(20);

            for (int i = 0; i < temperatures.Count; i++)
            {
                GUILayout.Label(temperatures[i].ToString("0.0"));
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private List<float> precipitations = new List<float>(12);

        private void DrawPrecipitationDebugView()
        {
            precipitations.Clear();
            for (int i = 0; i < 12; i++)
            {
                precipitations.Add(precipitationDefinition.GetPrecipitationAtTime((int)skyDefinition.SystemTime + i));
            }

            GUILayout.BeginArea(new Rect(320, 20, 100, Screen.height));

            GUILayout.BeginVertical();
            GUILayout.Label("Precipitation");
            GUILayout.Label(precipitationDefinition.currentPrecipitation.ToString("0%"));
            GUILayout.Space(20);

            for (int i = 0; i < precipitations.Count; i++)
            {
                GUILayout.Label(precipitations[i].ToString("0%"));
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private List<float> cloudinessLevels = new List<float>(12);

        private void DrawCloudinessDebugView()
        {
            cloudinessLevels.Clear();
            for (int i = 0; i < 12; i++)
            {
                cloudinessLevels.Add(cloudDefinition.GetCloudinessAtTime((int)skyDefinition.SystemTime + i));
            }

            GUILayout.BeginArea(new Rect(420, 20, 100, Screen.height));

            GUILayout.BeginVertical();
            GUILayout.Label("Cloudiness");
            GUILayout.Label(cloudDefinition.currentCloudiness.ToString("0%"));
            GUILayout.Space(20);

            for (int i = 0; i < cloudinessLevels.Count; i++)
            {
                GUILayout.Label(cloudinessLevels[i].ToString("0%"));
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

#if UNITY_EDITOR
        #region Editor
        private void OnDrawGizmos()
        {
            DrawVisuals();
            DrawText();
        }

        private void DrawVisuals()
        {
            float length = 1f;
            float thickness = 3f;
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawLine(transform.position - new Vector3(0, 0, length), transform.position + new Vector3(0, 0, length), thickness);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(transform.position - new Vector3(length, 0, 0), transform.position + new Vector3(length, 0, 0), thickness);

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawLine(transform.position - new Vector3(0, length, 0), transform.position + new Vector3(0, length, 0), thickness);

            // Draw Upper and Lower Hemispheres + Celestial Horizon
            UnityEditor.Handles.color = new Color(0, 0, 0, 0.3f);

            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, maxDistance);

            for (int i = 1; i <= (int)maxDistance; ++i)
            {
                UnityEditor.Handles.color = new Color(1, 1, 1, 0.05f);
                UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, i);
            }
        }

        private void DrawText()
        {
            GUIStyle s = new GUIStyle();
            s.fontSize = 12;
            s.fontStyle = FontStyle.Bold;
            s.normal.textColor = Color.white;
            s.alignment = TextAnchor.UpperRight;

            float mm = 0;
            float hh = 0;

            if (skyDefinition != null)
            {
                mm = skyDefinition.CurrentTime - Mathf.Floor(skyDefinition.CurrentTime);
                mm /= 1.67f;
                mm *= 100f;
                hh = Mathf.Floor(skyDefinition.CurrentTime);
            }

            UnityEditor.Handles.Label(transform.position + new Vector3(0, 3f, 5f), new GUIContent($"time"), s);
            s.normal.textColor = new Color(1, 1, 1, 1);
            UnityEditor.Handles.Label(transform.position + new Vector3(0, 3f, 5f), new GUIContent($"\n{hh:00}:{mm:00}"), s);

            s.normal.textColor = new Color(1, 1, 1, 1);
            UnityEditor.Handles.Label(transform.position + Vector3.right * maxDistance, new GUIContent("east"), s);
            UnityEditor.Handles.Label(transform.position + Vector3.left * maxDistance, new GUIContent("west"), s);
            UnityEditor.Handles.Label(transform.position + Vector3.forward * maxDistance, new GUIContent("north"), s);
            UnityEditor.Handles.Label(transform.position + Vector3.back * maxDistance, new GUIContent("south"), s);
        }
        #endregion
#endif
    }

    /// <summary>
    /// This class maintains the position offsets for the various cloud maps.
    /// </summary>
    public class CloudState
    {
        private Vector2 positionWeather = Vector2.zero;
        private Vector3 positionBase = Vector3.zero;
        private Vector3 positionDetail = Vector3.zero;
        private float positionCurl = 0;
        private Vector4[] positionHighAlt = { Vector4.zero, Vector4.zero, Vector4.zero };
        private AltosSkyDirector altosSkyDirector = null;
        private bool automaticUpdatesEnabled = true;

        /// <summary>
        /// The offset of the weather texture.
        /// </summary>
        public Vector2 PositionWeather
        {
            get => positionWeather;
        }

        /// <summary>
        /// The offset of the base texture.
        /// </summary>
        public Vector3 PositionBase
        {
            get => positionBase;
        }

        /// <summary>
        /// The offset of the detail texture.
        /// </summary>
        public Vector3 PositionDetail
        {
            get => positionDetail;
        }

        /// <summary>
        /// The offset of the curl texture.
        /// </summary>
        public float PositionCurl
        {
            get => positionCurl;
        }

        /// <summary>
        /// The offset of the high altitude weather texture.
        /// </summary>
        public Vector2 positionHighAltWeather
        {
            get => positionHighAlt[0];
        }

        /// <summary>
        /// The offset of the high altitude texture 1.
        /// </summary>
        public Vector2 positionHighAltTex1
        {
            get => positionHighAlt[1];
        }

        /// <summary>
        /// The offset of the high altitude texture 2.
        /// </summary>
        public Vector2 positionHighAltTex2
        {
            get => positionHighAlt[2];
        }

        /// <summary>
        /// Create a new CloudState.
        /// </summary>
        /// <param name="altosSkyDirector">Expects the current sky director. Will automatically pull the cloud definition from it.</param>
        internal CloudState(AltosSkyDirector altosSkyDirector)
        {
            this.altosSkyDirector = altosSkyDirector;
        }

        internal void Update()
        {
            if (altosSkyDirector.cloudDefinition == null)
                return;

            UpdatePositions();
            UpdateShaderVariables();
        }

        /// <summary>
        /// Manually override the calculated positions for the low altitude clouds.
        /// </summary>
        /// <param name="positionWeather"></param>
        /// <param name="positionBase"></param>
        /// <param name="positionDetail"></param>
        /// <param name="positionCurl"></param>
        public void SetPositionsLowAltitude(Vector3 positionWeather, Vector3 positionBase, Vector3 positionDetail, float positionCurl)
        {
            this.positionWeather = positionWeather;
            this.positionBase = positionBase;
            this.positionDetail = positionDetail;
            this.positionCurl = positionCurl;
        }

        /// <summary>
        /// Manually override the calculated positions for the high altitude clouds.
        /// </summary>
        /// <param name="weathermap"></param>
        /// <param name="texture1"></param>
        /// <param name="texture2"></param>
        public void SetPositionsHighAltitude(Vector2 weathermap, Vector2 texture1, Vector2 texture2)
        {
            positionHighAlt[0] = weathermap;
            positionHighAlt[1] = texture1;
            positionHighAlt[2] = texture2;
        }

        /// <summary>
        /// By default, the cloud state will update the positions every frame.
        /// You can override that behavior by setting the automatic update state to false.
        /// </summary>
        /// <param name="automaticUpdatesEnabled"></param>
        public void SetAutomaticUpdateState(bool automaticUpdatesEnabled)
        {
            this.automaticUpdatesEnabled = automaticUpdatesEnabled;
        }

        private void UpdatePositions()
        {
            if (!automaticUpdatesEnabled)
                return;

            if (altosSkyDirector.GetWind(out AltosWindZone altosWindZone))
            {
                float windSpeed = altosWindZone.GetCloudWindData().speed;
                Vector3 wind3 = altosWindZone.GetCloudWindData().velocity;
                Vector2 wind2 = new Vector2(wind3.x, wind3.z);
                float wind = windSpeed;

                positionWeather += Time.deltaTime * wind2 * 0.1f;
                positionBase += Time.deltaTime * wind3 * 0.05f;
                positionDetail += Time.deltaTime * wind3 * 0.2f;
                positionCurl += Time.deltaTime * wind * 0.01f;
                positionHighAlt[0] += Time.deltaTime * (Vector4)wind2 * 0.01f;
                positionHighAlt[1] += Time.deltaTime * (Vector4)wind2 * 0.1f;
                positionHighAlt[2] += Time.deltaTime * (Vector4)wind2 * 0.1f;
            }
            else
            {
                positionWeather += Time.deltaTime * altosSkyDirector.cloudDefinition.weathermapVelocity * 0.1f;
                positionBase += Time.deltaTime * altosSkyDirector.cloudDefinition.baseTextureTimescale * 0.05f;

                positionDetail += Time.deltaTime * altosSkyDirector.cloudDefinition.detail1TextureTimescale * 0.2f;
                positionCurl += Time.deltaTime * altosSkyDirector.cloudDefinition.curlTextureTimescale * 0.01f;

                positionHighAlt[0] += Time.deltaTime * (Vector4)altosSkyDirector.cloudDefinition.highAltTimescale1 * 0.01f;
                positionHighAlt[1] += Time.deltaTime * (Vector4)altosSkyDirector.cloudDefinition.highAltTimescale2 * 0.1f;
                positionHighAlt[2] += Time.deltaTime * (Vector4)altosSkyDirector.cloudDefinition.highAltTimescale3 * 0.1f;
            }
        }

        private void UpdateShaderVariables()
        {
            Shader.SetGlobalVector(ShaderParams.altos_positionWeather, positionWeather);
            Shader.SetGlobalVector(ShaderParams.altos_positionBase, positionBase);
            Shader.SetGlobalVector(ShaderParams.altos_positionDetail, positionDetail);
            Shader.SetGlobalFloat(ShaderParams.altos_positionCurl, positionCurl);
            Shader.SetGlobalVectorArray(ShaderParams.altos_positionHighAlt, positionHighAlt);
        }

        private static class ShaderParams
        {
            public static int altos_positionWeather = Shader.PropertyToID("altos_positionWeather");
            public static int altos_positionBase = Shader.PropertyToID("altos_positionBase");
            public static int altos_positionDetail = Shader.PropertyToID("altos_positionDetail");
            public static int altos_positionCurl = Shader.PropertyToID("altos_positionCurl");
            public static int altos_positionHighAlt = Shader.PropertyToID("altos_positionHighAlt");
        }
    }
}
