using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace OccaSoftware.Altos.Runtime
{
    [Serializable]
    public class AltosData : ScriptableObject
    {
        public static string packagePath = "Packages/com.occasoftware.altos";

#if UNITY_EDITOR
        internal class CreateAltosDataAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<AltosData>();
                AssetDatabase.CreateAsset(instance, pathName);
                ResourceReloader.ReloadAllNullIn(instance, packagePath);
                Selection.activeObject = instance;
            }
        }

        [MenuItem("Assets/Create/Altos/AltosData", priority = CoreUtils.Sections.section5 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void CreateAltosData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateAltosDataAsset>(), "AltosDataAsset.asset", null, null);
        }
#endif

        public ShaderResources shaders;
        public TextureResources textures;
        public MeshResources meshes;

        [Serializable]
        public sealed class ShaderResources
        {
            public Shader atmosphereShader;

            public Shader backgroundShader;

            public Shader skyObjectShader;

            public Shader starShader;

            public Shader ditherDepth;

            public Shader mergeClouds;

            public Shader renderClouds;

            public Shader edgeData;

            public Shader temporalIntegration;

            public Shader reproject;

            public Shader upscaleClouds;

            public Shader renderShadowsToScreen;

            public Shader screenShadows;

            public Shader atmosphereBlending;

            public Shader cloudMap;

            public Shader cloudShadowTaa;
        }

        [Serializable, ReloadGroup]
        public sealed class TextureResources
        {
            [Reload("Textures/Noise Textures/HaltonSequence/Halton_23_Sequence.png")]
            public Texture2D halton;

            [Reload("Textures/Noise Textures/BlueNoise/LDR_LLL1_{0}.png", 0, 64)]
            public Texture2D[] blue;
        }

        [Serializable, ReloadGroup]
        public sealed class MeshResources
        {
            [Reload("Meshes/Altos_Sphere.fbx")]
            public Mesh skyboxMesh;
        }
    }
}
