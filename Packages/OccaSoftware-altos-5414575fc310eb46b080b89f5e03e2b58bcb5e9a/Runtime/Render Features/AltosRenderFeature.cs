using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// Renders the Altos Sky, Sky Objects, Stars, Clouds, and Cloud Shadows.
    /// </summary>
    internal sealed class AltosRenderFeature : ScriptableRendererFeature
    {
        private SkyRenderPass skyRenderPass;
        private AtmosphereBlendingPass atmospherePass;
        private VolumetricCloudsRenderPass cloudRenderPass;
        private CloudShadowsRenderPass shadowRenderPass;
        private ScreenShadowsPass screenShadowsPass;

        internal Material cloudRenderMaterial = null;

        protected override void Dispose(bool disposing)
        {
            skyRenderPass?.Dispose();
            atmospherePass?.Dispose();
            cloudRenderPass?.Dispose();
            shadowRenderPass?.Dispose();
            screenShadowsPass?.Dispose();

            skyRenderPass = null;
            atmospherePass = null;
            cloudRenderPass = null;
            shadowRenderPass = null;
            screenShadowsPass = null;
            CoreUtils.Destroy(cloudRenderMaterial);
            base.Dispose(disposing);
        }

        public override void Create()
        {
            Dispose(true);

            skyRenderPass = new SkyRenderPass();
            skyRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

            shadowRenderPass = new CloudShadowsRenderPass();
            shadowRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingShadows;

            screenShadowsPass = new ScreenShadowsPass();
            screenShadowsPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques + 1;

            atmospherePass = new AtmosphereBlendingPass();
            atmospherePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques + 2;

            cloudRenderPass = new VolumetricCloudsRenderPass();
            cloudRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }

        internal AltosSkyDirector skyDirector;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Shader.SetGlobalFloat("_AltosAtmosphericFogIsEnabled", 0f);
            Shader.SetGlobalFloat("_AltosIsEnabled", 0f);
            Shader.SetGlobalFloat("_AltosCloudsIsEnabled", 0f);

            if (renderingData.cameraData.camera.TryGetComponent(out DisableAltosRendering _))
            {
                return;
            }

            skyDirector = AltosSkyDirector.Instance;

            if (skyDirector == null)
            {
                return;
            }

            bool isAltosEnabled = false;
            bool isAtmosphericFogEnabled = false;
            if (PassValidator.IsValidSkyPass(renderingData.cameraData.camera, skyDirector))
            {
                skyRenderPass.Setup(skyDirector);
                renderer.EnqueuePass(skyRenderPass);

                isAltosEnabled |= true;

                if (PassValidator.IsValidAtmospherePass(renderingData.cameraData.camera, skyDirector))
                {
                    atmospherePass.Setup(skyDirector);
                    renderer.EnqueuePass(atmospherePass);
                    isAtmosphericFogEnabled = true;
                    isAltosEnabled |= true;
                }
            }
            if (PassValidator.IsValidCloudPass(renderingData.cameraData.camera, skyDirector))
            {
                if (cloudRenderMaterial == null)
                    cloudRenderMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.renderClouds);

                SetDefaultCloudSettings();
                cloudRenderPass.Setup(skyDirector, cloudRenderMaterial);
                renderer.EnqueuePass(cloudRenderPass);
                Shader.SetGlobalFloat("_AltosCloudsIsEnabled", 1f);
                isAltosEnabled |= true;

                if (skyDirector.cloudDefinition.castShadowsEnabled)
                {
                    shadowRenderPass.Setup(skyDirector, cloudRenderMaterial);
                    renderer.EnqueuePass(shadowRenderPass);

                    if (skyDirector.cloudDefinition.screenShadows)
                    {
                        screenShadowsPass.Setup(skyDirector);
                        renderer.EnqueuePass(screenShadowsPass);
                    }
                    else
                    {
                        if (screenShadowsPass.Active)
                        {
                            screenShadowsPass?.Dispose();
                        }
                    }
                }
                else
                {
                    if (shadowRenderPass.Active)
                    {
                        shadowRenderPass?.Dispose();
                    }
                    if (screenShadowsPass.Active)
                    {
                        screenShadowsPass?.Dispose();
                    }
                }
            }

            Shader.SetGlobalFloat("_AltosAtmosphericFogIsEnabled", isAtmosphericFogEnabled ? 1 : 0);
            Shader.SetGlobalFloat("_AltosIsEnabled", isAltosEnabled ? 1 : 0);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            skyRenderPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            cloudRenderPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            shadowRenderPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            atmospherePass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
        }

        private void SetDefaultCloudSettings()
        {
            Shader.SetGlobalInt(ShaderParams._HasSkyTexture, 0);
            Shader.SetGlobalColor(ShaderParams._HorizonColor, RenderSettings.ambientEquatorColor);
            Shader.SetGlobalColor(ShaderParams._ZenithColor, RenderSettings.ambientSkyColor);
        }

        private static class ShaderParams
        {
            public static int _HasSkyTexture = Shader.PropertyToID("_HasSkyTexture");
            public static int _HorizonColor = Shader.PropertyToID("_HorizonColor");
            public static int _ZenithColor = Shader.PropertyToID("_ZenithColor");
        }

        private static class PassValidator
        {
            public static bool IsValidCloudPass(Camera camera, AltosSkyDirector skyDirector)
            {
                if (skyDirector.cloudDefinition == null)
                    return false;

#if UNITY_EDITOR
                if (Check.IsPreviewCamera(camera))
                    return false;

                if (Check.IsSceneCamera(camera) && skyDirector.cloudDefinition.renderInSceneView)
                {
                    if (!Check.IsSkyboxEnabled)
                        return false;

                    if (!Check.IsDrawingTextured)
                        return false;

                    if (Check.IsPrefabStage())
                        return false;
                }
#endif

                if (Check.IsReflectionCamera(camera))
                    return false;

                return true;
            }

            public static bool IsValidSkyPass(Camera c, AltosSkyDirector skyDirector)
            {
#if UNITY_EDITOR
                if (Check.IsPreviewCamera(c))
                    return false;

                if (Check.IsSceneCamera(c))
                {
                    if (!Check.IsSkyboxEnabled)
                        return false;

                    if (!Check.IsDrawingTextured)
                        return false;

                    if (Check.IsPrefabStage())
                        return false;
                }
#endif

                if (skyDirector.skyDefinition == null)
                    return false;

                return true;
            }

            public static bool IsValidAtmospherePass(Camera c, AltosSkyDirector skyDirector)
            {
#if UNITY_EDITOR
                if (Check.IsPreviewCamera(c))
                    return false;

                if (Check.IsSceneCamera(c))
                {
                    if (!Check.IsFogEnabled)
                        return false;

                    if (!Check.IsDrawingTextured)
                        return false;

                    if (Check.IsPrefabStage())
                        return false;
                }
#endif

                if (Check.IsReflectionCamera(c))
                    return false;

                if (skyDirector.skyDefinition == null)
                    return false;

                if (skyDirector.atmosphereDefinition == null)
                    return false;

                return true;
            }

            private static class Check
            {
                public static bool IsReflectionCamera(Camera c)
                {
                    return c.cameraType == CameraType.Reflection;
                }

#if UNITY_EDITOR
                public static bool IsPreviewCamera(Camera c)
                {
                    return c.cameraType == CameraType.Preview;
                }

                public static bool IsSceneCamera(Camera c)
                {
                    return c.cameraType == CameraType.SceneView;
                }

                public static bool IsSkyboxEnabled
                {
                    get => UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.skyboxEnabled;
                }

                public static bool IsFogEnabled
                {
                    get => UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.fogEnabled;
                }

                public static bool IsDrawingTextured
                {
                    get => UnityEditor.SceneView.currentDrawingSceneView.cameraMode.drawMode == UnityEditor.DrawCameraMode.Textured;
                }

                public static bool IsPrefabStage()
                {
                    return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
                }
#endif
            }
        }
    }
}
