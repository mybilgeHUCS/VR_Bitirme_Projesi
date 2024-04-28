// This is an extremely lightweight demonstration of how you might integrate the AltosFog or AltosFogBlend method in a custom shader.
// This is NOT directly compatible with Altos Clouds, as it doesn't include a Depth pass.
// The goal of this sample code is to demonstrate usage of the AltosFog methods, 
// This shader does NOT set out to demonstrate how to write a full-featured shader that is fully compatible with Altos.

Shader "OccaSoftware/Altos/TransparencyExample"
{
    Properties
    { }

    SubShader
    {
        // We tag this shader as Transparent so that it renders after the atmosphere and cloud passes.
        Tags { "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/Altos.hlsl"

            #pragma multi_compile _ ALTOS_BUTO_COMPATIBILITY_ENABLED
            #ifdef ALTOS_BUTO_COMPATIBILITY_ENABLED
            #include "Packages/com.occasoftware.buto/Shaders/Resources/Buto.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                
                // Include any required v2f variables in your Varyings struct
                float3 positionWS : TEXCOORD0;
                float3 positionVS : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Use Unity's SpaceTransforms to convert to view space
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionVS = TransformWorldToView(OUT.positionWS.xyz);
                return OUT;
            }

            half3 frag(Varyings IN) : SV_Target
            {
                half3 customColor = half3(0.5, 0, 0);

                // Scale the CS position by the Screen Resolution to get the UV [0,1].
                float2 UV = IN.positionHCS.xy / _ScaledScreenParams.xy;
                
                // Call AltosFogBlend, passing in the UV, -positionVS.z, and custom color as inputs
                // AltosFogBlend automatically applies the fog data to the input color.
                // Typically, you call this method where you would call MixFog() -- right before returning.
                customColor = AltosFogBlend(UV, -IN.positionVS.z, customColor);
                customColor = AltosCloudsBlend(UV, customColor);

                #ifdef ALTOS_BUTO_COMPATIBILITY_ENABLED
                customColor = ButoFogBlend(UV, -IN.positionVS.z, customColor);
                #endif

                return customColor;
            }
            ENDHLSL
        }
    }
}