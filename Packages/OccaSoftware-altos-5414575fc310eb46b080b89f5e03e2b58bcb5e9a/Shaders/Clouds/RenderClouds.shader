Shader "Hidden/OccaSoftware/Altos/RenderClouds"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off
        Cull Off
        ZTest Always
        
        Pass
        {
            Name "Render Clouds"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/GetCameraMotionVectors.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/TextureUtils.hlsl"

            #include "RenderCloudsPass.hlsl"

            int _UseDownscaledDepth;
            int _UseReprojection;
            int _UseDepth;
            
            float4 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return SampleClouds(input.texcoord, _UseDownscaledDepth, _UseReprojection, _UseDepth).color;
            }
            ENDHLSL
        }
    }
}