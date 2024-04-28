using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Altos.Runtime
{
    internal class CloudShadowsRenderPass : ScriptableRenderPass
    {
        private AltosSkyDirector skyDirector;
        private RTHandle shadowmapTarget;
        private RTHandle cloudShadowTemporalAA;

        private const string shadowmapId = "_CloudShadowmap";

        private RTHandle cloudShadowTaaTexture;

        private Material screenShadows;
        private Material shadowsToScreen;
        private Material cloudShadowTaaMaterial;

        private const string profilerTag = "Altos: Cloud Shadows";

        private bool active;

        public bool Active => active;

        public CloudShadowsRenderPass()
        {
            shadowmapTarget = RTHandles.Alloc(Shader.PropertyToID(shadowmapId), name: shadowmapId);
            cloudShadowTemporalAA = RTHandles.Alloc("_CloudShadowTAAHistory", name: "_CloudShadowTAAHistory");
            cloudShadowTaaTexture = RTHandles.Alloc("_cloudShadowTaaTexture", name: "_cloudShadowTaaTexture");
        }

        public void Dispose()
        {
            shadowmapTarget?.Release();
            cloudShadowTaaTexture?.Release();
            cloudShadowTemporalAA?.Release();

            shadowmapTarget = null;
            cloudShadowTaaTexture = null;
            cloudShadowTemporalAA = null;
            active = false;
        }

        public void Setup(AltosSkyDirector skyDirector, Material cloudRenderMaterial)
        {
            active = true;
            this.skyDirector = skyDirector;
            this.cloudRenderMaterial = cloudRenderMaterial;

            if (screenShadows == null)
                screenShadows = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.screenShadows);
            if (shadowsToScreen == null)
                shadowsToScreen = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.renderShadowsToScreen);
            if (cloudShadowTaaMaterial == null)
                cloudShadowTaaMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.cloudShadowTaa);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
            StaticHelpers.AssignDefaultDescriptorSettings(ref rtDescriptor);

            ConfigureShadows(rtDescriptor);

            void ConfigureShadows(RenderTextureDescriptor descriptor)
            {
                if (skyDirector.cloudDefinition.castShadowsEnabled)
                {
                    descriptor.width = (int)skyDirector.cloudDefinition.shadowResolution;
                    descriptor.height = (int)skyDirector.cloudDefinition.shadowResolution;
                    descriptor.colorFormat = RenderTextureFormat.DefaultHDR;

                    RenderingUtils.ReAllocateIfNeeded(ref shadowmapTarget, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: shadowmapId);

                    RenderingUtils.ReAllocateIfNeeded(
                        ref cloudShadowTaaTexture,
                        descriptor,
                        FilterMode.Bilinear,
                        TextureWrapMode.Clamp,
                        name: "_cloudShadowTaaTexture"
                    );
                    RenderingUtils.ReAllocateIfNeeded(ref cloudShadowTemporalAA, descriptor, name: "_CloudShadowTAAHistory");
                }
            }
        }

        private struct SunData
        {
            public Vector3 forward;
            public Vector3 right;
            public Vector3 up;
        }

        bool GetSunData(out SunData sunData)
        {
            sunData = new SunData();

            if (skyDirector?.Sun != null)
            {
                Transform child = skyDirector.Sun.GetChild();
                sunData.forward = child.forward;
                sunData.up = child.up;
                sunData.right = child.right;
                return true;
            }
            else
            {
                SetMainLightShaderProperties setMainLightShaderProperties = SetMainLightShaderProperties.Instance;
                if (setMainLightShaderProperties != null)
                {
                    sunData.forward = setMainLightShaderProperties.transform.forward;
                    sunData.up = setMainLightShaderProperties.transform.up;
                    sunData.right = setMainLightShaderProperties.transform.right;
                    return true;
                }
            }
            return false;
        }

        Vector3 Div(Vector3 a, float b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        Vector3 Floor(Vector3 i)
        {
            return new Vector3(Mathf.Floor(i.x), Mathf.Floor(i.y), Mathf.Floor(i.z));
        }

        Matrix4x4 projectionMatrix = new Matrix4x4();
        Matrix4x4 viewMatrix = new Matrix4x4();
        Matrix4x4 worldToShadowMatrix = new Matrix4x4();
        Vector4 shadowCameraPosition = new Vector4();
        private Material cloudRenderMaterial;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Profiler.BeginSample(profilerTag);
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            if (GetSunData(out SunData sunData))
            {
                RenderShadows(cmd, renderingData, sunData);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
            Profiler.EndSample();

            void RenderShadows(CommandBuffer cmd, RenderingData renderingData, SunData sunData)
            {
                float zFar = 60000f;

                float halfWidth = skyDirector.cloudDefinition.shadowDistance;

                Vector3 sourcePosition = Vector3.zero;
                Vector3 shadowCasterCameraPosition = sourcePosition - sunData.forward * zFar * 0.5f;

                // Prevent shimmering when the camera moves
                // Might be unnecessary now that we switched to TAA?
                Vector3 min = shadowCasterCameraPosition - halfWidth * sunData.right - halfWidth * sunData.up;

                Vector3 max = shadowCasterCameraPosition + sunData.forward * zFar + halfWidth * sunData.right + halfWidth * sunData.up;

                float radius = (new Vector2(max.x, max.z) - new Vector2(min.x, min.z)).magnitude / 2f;
                float texelSize = radius / (0.25f * (int)skyDirector.cloudDefinition.shadowResolution);

                sourcePosition = Floor(Div(sourcePosition, texelSize));
                sourcePosition *= texelSize;

                shadowCasterCameraPosition = sourcePosition - sunData.forward * zFar * 0.5f;

                // Setup matrices
                viewMatrix = MatrixHandler.SetupViewMatrix(shadowCasterCameraPosition, sunData.forward, zFar, sunData.up);
                shadowCameraPosition = shadowCasterCameraPosition;
                projectionMatrix = MatrixHandler.SetupProjectionMatrix(halfWidth, zFar);

                cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams.Shadows._ShadowRadius, halfWidth);

                cmd.SetGlobalVector(
                    CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowOrthoParams,
                    new Vector4(halfWidth * 2, halfWidth * 2, zFar, 0)
                );

                cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraPosition, shadowCameraPosition);
                worldToShadowMatrix = MatrixHandler.ConvertToWorldToShadowMatrix(projectionMatrix, viewMatrix);

                cmd.SetGlobalMatrix(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadow_WorldToShadowMatrix, worldToShadowMatrix);
                cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams.Shadows._ShadowRenderStepCount, skyDirector.cloudDefinition.shadowStepCount);
                cmd.SetGlobalFloat(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowDistance, skyDirector.cloudDefinition.shadowDistance);
                cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraForward, sunData.forward);
                cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraUp, sunData.up);
                cmd.SetGlobalVector(CloudShaderParamHandler.ShaderParams.Shadows._ShadowCasterCameraRight, sunData.right);
                cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._ShadowPass, 1);
                cmd.SetGlobalVector(
                    CloudShaderParamHandler.ShaderParams._RenderTextureDimensions,
                    new Vector4(
                        1f / (int)skyDirector.cloudDefinition.shadowResolution,
                        1f / (int)skyDirector.cloudDefinition.shadowResolution,
                        (int)skyDirector.cloudDefinition.shadowResolution,
                        (int)skyDirector.cloudDefinition.shadowResolution
                    )
                );

                cmd.SetGlobalVector(
                    CloudShaderParamHandler.ShaderParams.Shadows._ShadowmapResolution,
                    new Vector4(
                        (int)skyDirector.cloudDefinition.shadowResolution,
                        (int)skyDirector.cloudDefinition.shadowResolution,
                        1f / (int)skyDirector.cloudDefinition.shadowResolution,
                        1f / (int)skyDirector.cloudDefinition.shadowResolution
                    )
                );

                Blitter.BlitCameraTexture(cmd, source, shadowmapTarget, cloudRenderMaterial, 0);
                cmd.SetGlobalTexture("_PREVIOUS_TAA_CLOUD_RESULTS", cloudShadowTaaTexture);
                cmd.SetGlobalTexture("_CURRENT_TAA_FRAME", shadowmapTarget);
                Blitter.BlitCameraTexture(cmd, source, cloudShadowTemporalAA, cloudShadowTaaMaterial, 0);
                cmd.CopyTexture(cloudShadowTemporalAA, cloudShadowTaaTexture);
                cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams.Shadows._CloudShadowmap, cloudShadowTemporalAA);
            }
        }
    }
}
