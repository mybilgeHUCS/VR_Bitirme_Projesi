using System;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.VFX;

namespace OccaSoftware.Altos.Runtime
{
    [AddComponentMenu("OccaSoftware/Altos/Weather Effect")]
    [ExecuteAlways]
    public class WeatherEffect : MonoBehaviour
    {
        [Header("Components")]
        public VisualEffect visualEffect;

        [Header("Follow Options")]
        public CameraFollowSettings cameraFollowSettings = new CameraFollowSettings();

        [System.Serializable]
        public class CameraFollowSettings
        {
            public bool enabled;
            public Camera mainCamera;
            public Vector3 positionOffset = new Vector3(0, 5, 0);
        }

        [Header("Precipitation Options")]
        public WeathermapIntegrationSettings weathermapIntegrationSettings = new WeathermapIntegrationSettings();
        public PrecipitationCollisionSettings precipitationCollisionSettings = new PrecipitationCollisionSettings();

        [Header("Temperature Options")]
        public TemperatureIntegrationSettings temperatureIntegrationSettings = new TemperatureIntegrationSettings();

        [Header("Wind Zone Options")]
        public WindZoneIntegrationSettings windZoneIntegrationSettings = new WindZoneIntegrationSettings();

        [Header("Origin Management Options")]
        public OriginManagementSettings originManagementSettings = new OriginManagementSettings();

        [Header("Time of Day Options")]
        public DaytimeFactorSettings daytimeFactorSettings = new DaytimeFactorSettings();

        [System.Serializable]
        public class OriginManagementSettings
        {
            public bool enabled;
            public string originOffsetPropertyName = "altos_Origin";
        }

        [System.Serializable]
        public class TemperatureIntegrationSettings
        {
            public bool enabled;
            public string temperaturePropertyName = "altos_Temperature";
        }

        [System.Serializable]
        public class WindZoneIntegrationSettings
        {
            public bool enabled;
            public string windZonePropertyName = "altos_WindZone";
        }

        [System.Serializable]
        public class DaytimeFactorSettings
        {
            public bool enabled;
            public string daytimeFactorPropertyName = "altos_DaytimeFactor";
        }

        [System.Serializable]
        public class WeathermapIntegrationSettings
        {
            [Header("Per Object")]
            public bool enableIntensityProperty;
            public bool onlyPrecipitateBelowCloudLayer;
            public string intensityPropertyName = "altos_PrecipitationAmount";

            [Header("Per Particle")]
            public bool enableWeathermapProperty;
            public string textureProperty = "altos_WeatherMap";
            public string positionProperty = "altos_WeatherMapPositionWS";
            public string extentsProperty = "altos_WeatherMapExtents";
        }

        [System.Serializable]
        public class PrecipitationCollisionSettings
        {
            public bool enabled;
            public Camera camera;
            public string texture = "altos_PrecipitationDepthBufferTexture";
            public string enabledProperty = "altos_PrecipitationDepthEnabled";
            public string worldToCameraMatrixProperty = "altos_PrecipitationDepthWorldToCameraMatrix";
            public string cameraOrthographicSizeProperty = "altos_PrecipitationDepthCameraOrthographicSize";
            public string cameraClipPlanesProperty = "altos_PrecipitationDepthCameraClipPlanes";
        }

        public void Reset()
        {
            visualEffect = GetComponent<VisualEffect>();
        }

        private void LateUpdate()
        {
            UpdatePosition();
            UpdateProperties();
        }

        private void UpdatePosition()
        {
            if (!cameraFollowSettings.enabled)
                return;

            if (cameraFollowSettings.mainCamera == null)
                return;

            transform.position = cameraFollowSettings.mainCamera.transform.position + cameraFollowSettings.positionOffset;
        }

        void UpdateProperties()
        {
            if (AltosSkyDirector.Instance == null || visualEffect == null)
                return;

            Profiler.BeginSample("WeatherEffect");
            UpdatePrecipitation();
            UpdateTemperature();
            UpdateWindZone();
            UpdateOrigin();
            UpdateDaytimeFactor();
            UpdatePrecipitationDepthBuffer();
            Profiler.EndSample();
        }

        private void UpdatePrecipitation()
        {
            if (!weathermapIntegrationSettings.enableIntensityProperty && !weathermapIntegrationSettings.enableWeathermapProperty)
                return;

            if (AltosSkyDirector.Instance.GetWeatherMap(out WeatherMap weatherMap))
            {
                if (weathermapIntegrationSettings.enableIntensityProperty)
                {
                    float intensity = weatherMap.GetIntensity(transform.position);

                    if (weathermapIntegrationSettings.onlyPrecipitateBelowCloudLayer)
                    {
                        float intensityModifierByHeight =
                            1.0f
                            - StaticHelpers.RemapTo01(
                                AltosSkyDirector.Instance.TransformOffsetToWorldPosition(transform.position.y),
                                AltosSkyDirector.Instance.cloudDefinition.GetCloudFloor(),
                                AltosSkyDirector.Instance.cloudDefinition.GetCloudCenter()
                            );

                        intensity *= intensityModifierByHeight;
                    }

                    visualEffect.SetFloat(weathermapIntegrationSettings.intensityPropertyName, intensity);
                }

                if (weathermapIntegrationSettings.enableWeathermapProperty)
                {
                    visualEffect.SetTexture(weathermapIntegrationSettings.textureProperty, weatherMap.GlobalCloudMap);
                    visualEffect.SetVector3(weathermapIntegrationSettings.positionProperty, weatherMap.transform.position);
                    visualEffect.SetFloat(weathermapIntegrationSettings.extentsProperty, weatherMap.Extents);
                }
            }
        }

        private void UpdateTemperature()
        {
            if (!temperatureIntegrationSettings.enabled)
                return;

            float temperature = AltosSkyDirector.Instance.temperatureDefinition.GetTemperatureAtAltitude(transform.position.y);

            visualEffect.SetFloat(temperatureIntegrationSettings.temperaturePropertyName, temperature);
        }

        private void UpdateWindZone()
        {
            if (!windZoneIntegrationSettings.enabled)
                return;

            if (AltosSkyDirector.Instance.GetWind(out AltosWindZone altosWindZone))
            {
                visualEffect.SetVector3(windZoneIntegrationSettings.windZonePropertyName, altosWindZone.GetVFXWindData().velocity);
            }
        }

        Vector3 cachedOrigin = Vector3.zero;
        Vector3 originOffset = Vector3.zero;

        void UpdateOrigin()
        {
            if (!originManagementSettings.enabled)
                return;

            originOffset = AltosSkyDirector.Instance.GetOrigin() - cachedOrigin;
            cachedOrigin = AltosSkyDirector.Instance.GetOrigin();

            visualEffect.SetVector3(originManagementSettings.originOffsetPropertyName, originOffset);
        }

        void UpdateDaytimeFactor()
        {
            if (!daytimeFactorSettings.enabled)
                return;
            visualEffect.SetFloat(daytimeFactorSettings.daytimeFactorPropertyName, AltosSkyDirector.Instance.daytimeFactor);
        }

        /// <summary>
        /// To convert orthographic depth from linear to eye,
        /// you need to offset the depth by near clip, then
        /// do depth * (far - near),
        /// like: eyeDepth = near + depth * (far - near)
        /// (we have to do this math in the shader (vfx graph), though).
        /// Then to convert from eye to world position, we need to
        /// convert from "view" -> "world", using cameraToWorld.
        /// do worldPosition = mul(cameraToWorld, float4(eyeDepth * cameraForward, 1.0)
        /// </summary>
        void UpdatePrecipitationDepthBuffer()
        {
            if (!precipitationCollisionSettings.enabled)
                return;

            if (precipitationCollisionSettings.camera == null)
                return;

            if (precipitationCollisionSettings.camera.targetTexture == null)
                return;

            visualEffect.SetBool(precipitationCollisionSettings.enabledProperty, precipitationCollisionSettings.enabled);
            visualEffect.SetTexture(precipitationCollisionSettings.texture, precipitationCollisionSettings.camera.targetTexture);
            visualEffect.SetVector2(
                precipitationCollisionSettings.cameraClipPlanesProperty,
                new Vector2(precipitationCollisionSettings.camera.nearClipPlane, precipitationCollisionSettings.camera.farClipPlane)
            );
            visualEffect.SetMatrix4x4(
                precipitationCollisionSettings.worldToCameraMatrixProperty,
                precipitationCollisionSettings.camera.worldToCameraMatrix
            );
            visualEffect.SetFloat(
                precipitationCollisionSettings.cameraOrthographicSizeProperty,
                precipitationCollisionSettings.camera.orthographicSize
            );
        }
    }
}
