using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Altos.Runtime
{
    internal class ScreenShadowsPass : ScriptableRenderPass
    {
        private AltosSkyDirector skyDirector;
        private RTHandle screenShadowsTarget;
        private RTHandle mergeTarget;

        private const string screenShadowsId = "_CloudScreenShadows";
        private const string mergeId = "_CloudShadowsOnScreen";

        private Material screenShadows;
        private Material shadowsToScreen;

        private const string profilerTag = "Altos: Cloud Shadows";

        private bool active;
        public bool Active => active;

        public ScreenShadowsPass()
        {
            screenShadowsTarget = RTHandles.Alloc(Shader.PropertyToID(screenShadowsId), name: screenShadowsId);
            mergeTarget = RTHandles.Alloc(Shader.PropertyToID(mergeId), name: mergeId);
        }

        public void Dispose()
        {
            screenShadowsTarget?.Release();
            mergeTarget?.Release();

            screenShadowsTarget = null;
            mergeTarget = null;

            CoreUtils.Destroy(screenShadows);
            CoreUtils.Destroy(shadowsToScreen);

            screenShadows = null;
            shadowsToScreen = null;

            active = false;
        }

        public void Setup(AltosSkyDirector skyDirector)
        {
            active = true;

            this.skyDirector = skyDirector;

            if (screenShadows == null)
                screenShadows = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.screenShadows);
            if (shadowsToScreen == null)
                shadowsToScreen = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.renderShadowsToScreen);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtDescriptor = cameraTextureDescriptor;
            StaticHelpers.AssignDefaultDescriptorSettings(ref rtDescriptor);
            RenderingUtils.ReAllocateIfNeeded(
                ref screenShadowsTarget,
                rtDescriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: screenShadowsId
            );

            RenderingUtils.ReAllocateIfNeeded(ref mergeTarget, rtDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: mergeId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Setup
            Profiler.BeginSample(profilerTag);
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Draw Screen Space Shadows
            cmd.SetGlobalInt(CloudShaderParamHandler.ShaderParams._CastScreenCloudShadows, skyDirector.cloudDefinition.screenShadows ? 1 : 0);
            cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams._ScreenTexture, source);
            Blitter.BlitCameraTexture(cmd, source, screenShadowsTarget, screenShadows, 0);

            // Render to Screen
            cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams._ScreenTexture, source);
            cmd.SetGlobalTexture(CloudShaderParamHandler.ShaderParams.Shadows._CloudScreenShadows, screenShadowsTarget);
            Blitter.BlitCameraTexture(cmd, source, mergeTarget, shadowsToScreen, 0);
            Blitter.BlitCameraTexture(cmd, mergeTarget, source);

            // Cleanup
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
            Profiler.EndSample();
        }
    }
}
