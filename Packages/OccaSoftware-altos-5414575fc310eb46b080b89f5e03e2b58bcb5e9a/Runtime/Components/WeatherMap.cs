using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Altos.Runtime
{
    [ExecuteAlways]
    [AddComponentMenu("OccaSoftware/Altos/Weather Map")]
    public class WeatherMap : MonoBehaviour
    {
        public Texture2D GlobalCloudMap
        {
            get => globalCloudMap;
        }

        public RenderTexture GlobalCloudMapRt => cloudMap.rt;

        AltosSkyDirector skyDirector => AltosSkyDirector.Instance;
        Action<AsyncGPUReadbackRequest> gpuReadbackAction;
        Texture2D globalCloudMap;
        Vector4[] weatherManager = new Vector4[8]; // xy = position.xz, z = radius, w = precipitation amount
        RTHandle cloudMap;

        Material cloudMapRenderer;
        string cloudMapId = "altos_cloud_map";
        int currentFrame = -1;

        public float Extents
        {
            get
            {
                if (skyDirector?.cloudDefinition?.cloudFadeDistance != null)
                {
                    return skyDirector.cloudDefinition.cloudFadeDistance;
                }

                return 10000;
            }
        }

        private struct UV
        {
            public float x;
            public float y;

            public UV(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private UV TransformPositionWorldSpaceToUVCoordinate(float positionX, float positionZ)
        {
            float x = (((positionX - transform.position.x) / skyDirector.cloudDefinition.cloudFadeDistance) + 1.0f) * 0.5f;
            float z = (((positionZ - transform.position.z) / skyDirector.cloudDefinition.cloudFadeDistance) + 1.0f) * 0.5f;

            return new UV(x, z);
        }

        /// <summary>
        /// Get the precipitation amount from the Cloud Map at this world position.
        /// </summary>
        /// <param name="position">World position to sample</param>

        public float GetIntensity(Vector3 position)
        {
            if (globalCloudMap == null)
            {
                return 0;
            }

            UV uv = TransformPositionWorldSpaceToUVCoordinate(position.x, position.z);

            return globalCloudMap.GetPixelBilinear(uv.x, uv.y).g;
        }

        private void OnEnable()
        {
            globalCloudMap = new Texture2D(512, 512, TextureFormat.RGBA32, false, true);
            globalCloudMap.name = "altos_GlobalCloudMap";
            globalCloudMap.wrapMode = TextureWrapMode.Repeat;
            Color[] colors = new Color[globalCloudMap.width * globalCloudMap.height];
            globalCloudMap.SetPixels(colors);
            globalCloudMap.Apply();

            cloudMapRenderer = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.cloudMap);
            RenderPipelineManager.beginContextRendering += RenderWeathermap;
            gpuReadbackAction = OnGPUReadback;
        }

        private void RenderWeathermap(ScriptableRenderContext context, List<Camera> cameras)
        {
            if (Time.frameCount == currentFrame)
            {
                return;
            }
            currentFrame = Time.frameCount;

            Profiler.BeginSample("Altos_Weathermap Rendering");

            AltosSkyDirector.Instance.SetWeatherMap(this);

            if (cloudMap == null)
            {
                cloudMap = RTHandles.Alloc(Shader.PropertyToID(cloudMapId), name: cloudMapId);
            }

            RenderTextureDescriptor cloudMapDescriptor = new RenderTextureDescriptor(512, 512, RenderTextureFormat.ARGB32, 0, 0);
            RenderingUtils.ReAllocateIfNeeded(ref cloudMap, cloudMapDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: cloudMapId);

            var cmd = new CommandBuffer();
            cmd.name = "Render Weathermap";

            cloudMapRenderer.SetFloat("_Gain", skyDirector.cloudDefinition.weathermapGain);
            cloudMapRenderer.SetFloat("_Lacunarity", skyDirector.cloudDefinition.weathermapLacunarity);
            cloudMapRenderer.SetFloat("_Octaves", skyDirector.cloudDefinition.weathermapOctaves);
            cloudMapRenderer.SetFloat("_Scale", skyDirector.cloudDefinition.weathermapScale);
            cloudMapRenderer.SetVector("_Speed", (Vector4)skyDirector.cloudDefinition.weathermapVelocity);
            cloudMapRenderer.SetVector("_Weather", Vector4.zero);
            cloudMapRenderer.SetFloat("_PrecipitationGlobal", skyDirector.GetCurrentPrecipitation());

            cmd.SetGlobalVector("altos_WeathermapPosition", transform.position);

            int i = 0;
            foreach (WeatherManager m in WeatherManager.WeatherManagers)
            {
                weatherManager[i].x = m.transform.position.x;
                weatherManager[i].y = m.transform.position.z;
                weatherManager[i].z = m.radius;
                weatherManager[i].w = m.precipitationIntensity;
                i++;
            }
            cloudMapRenderer.SetFloat("_PrecipitationDataCount", i);
            cloudMapRenderer.SetVectorArray("_PrecipitationData", weatherManager);

            cmd.SetRenderTarget(cloudMap);
            Blitter.BlitTexture(cmd, cloudMap, Vector2.one, cloudMapRenderer, 0);
            cmd.SetGlobalTexture("altos_cloud_map", cloudMap.nameID);

            AsyncGPUReadback.Request(cloudMap, 0, TextureFormat.RGBA32, gpuReadbackAction);

            context.ExecuteCommandBuffer(cmd);
            cmd.Release();

            // Tell the Scriptable Render Context to tell the graphics API to perform the scheduled commands
            context.Submit();
            Profiler.EndSample();
        }

        void OnGPUReadback(AsyncGPUReadbackRequest request)
        {
            Profiler.BeginSample("Altos_Weathermap GPU Readback");
            if (request.hasError)
                return;

            if (globalCloudMap == null)
                return;

            globalCloudMap.LoadRawTextureData(request.GetData<uint>());
            globalCloudMap.Apply(false, false);
            Profiler.EndSample();
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginContextRendering -= RenderWeathermap;

            if (cloudMap != null)
            {
                cloudMap.Release();
                cloudMap = null;
            }

            globalCloudMap = null;

            if (cloudMapRenderer != null)
            {
                CoreUtils.Destroy(cloudMapRenderer);
                cloudMapRenderer = null;
            }

            if (AltosSkyDirector.Instance?.weatherMap == this)
            {
                AltosSkyDirector.Instance.ClearWeatherMap();
            }
        }
    }
}
