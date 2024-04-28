Shader "Hidden/OccaSoftware/Altos/DitherDepth"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off
        Cull Off
        ZTest Always
        
        Pass
        {
            Name "Dither Depth"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/TextureUtils.hlsl"
            
            
            float Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return DitherDepth(input.texcoord);
            }
            ENDHLSL
        }
    }
}