Shader "Hidden/OccaSoftware/Altos/CloudMap"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off
        Cull Off
        ZTest Always
        
        Pass
        {
            Name "Cloud Map"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/Math.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/TextureUtils.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Scale;
            float2 _Speed;
            float _Octaves;
            float _Lacunarity;
            float _Gain;
            float _PrecipitationGlobal;
            float2 altos_positionWeather;
            float4 altos_WorldPositionOffset;
            bool _USE_CLOUD_WEATHERMAP_TEX;
            Texture2D _CLOUD_WEATHERMAP_TEX;

            float2 GetFloatingPointOffset()
            {
                return altos_WorldPositionOffset.xz;
            }

            float2 PositionWSToPositionUV(float2 positionWS, float maxDistance)
            {
                float2 UV = positionWS + GetFloatingPointOffset();
                //float2 UV = positionWS;
                UV *= rcp(maxDistance); // needs to be 1.0 / max distance
                UV *= 0.5;
                UV += 0.5;
                return UV;
            }

            float2 PositionUVToPositionWS(float2 UV, float maxDistance)
            {
                float2 positionWS = UV;
                positionWS -= 0.5;
                positionWS *= 2.0;
                positionWS *= maxDistance; // needs to be  max distance
                return positionWS - GetFloatingPointOffset();
                //return positionWS;
            }

            int _PrecipitationDataCount;
            float4 _PrecipitationData[8]; // xy = position, z = radius, w = precipitation [0,1]
            

            float GetSDF(float2 positionWS, float2 cellPosition, float radiusWS)
            {
                return saturate((length(positionWS - cellPosition) - radiusWS) / radiusWS * -1.0);
            }

            float GetPrecipitationGlobal(float2 positionWS)
            {
                float map = GetLayeredPerlinNoise(_Octaves, positionWS * 0.00005 * _Scale - altos_positionWeather, _Gain, _Lacunarity, 314);
                map = os_Remap(1.0 - _PrecipitationGlobal, 1.0, 0.0, 1, map);
                map = saturate(map);
                map = pow(map, 0.5); // expose this value as a property, "Precipitation Strength"?.
                return map;
            }

            float3 GetPrecipitationData(float2 positionWS)
            {
                float sdf = 0.0;
                float state = GetPrecipitationGlobal(positionWS);
                float value = _PrecipitationGlobal;
                [loop]
                for(int i = 0; i < _PrecipitationDataCount; i++)
                {
                    float v = GetSDF(positionWS, _PrecipitationData[i].xy, _PrecipitationData[i].z);
                    
                    v = os_Remap(0.0, .2, 0, 1, v);
                    // todo: possibly replace with smooth union.
                    sdf = max(v, sdf); 
                    // todo: blend with noise data for less "circular" masks... (Remap?).
                    state = lerp(state, _PrecipitationData[i].w, v);
                    value = lerp(value, _PrecipitationData[i].w, v);
                }

                return float3(saturate(sdf), saturate(state), saturate(value));
            }

            float easeIn(float x)
            {
                return pow(x, 2.0);
            }

            float easeOut(float x)
            {
                return 1.0 - pow((1.0 - x), 2.0);
            }

            float easeInOut(float x)
            {
                return lerp(easeIn(x), easeOut(x), x);
            }

            float easeOutIn(float x)
            {
                return lerp(easeOut(x), easeIn(x), x);
            }

            float _CLOUD_COVERAGE;
            float _CLOUD_FADE_DIST;

            bool _USE_DISTANT_COVERAGE_OVERRIDE;
            float _CLOUD_DISTANT_CLOUD_COVERAGE;
            float _CLOUD_DISTANT_COVERAGE_START_DEPTH;
            float3 altos_WeathermapPosition;
            
            float4 Fragment(Varyings input) : SV_Target
            {
    
                // Setup
                float maxCloudDistance = _CLOUD_FADE_DIST;
                float2 positionWS = PositionUVToPositionWS(input.texcoord, maxCloudDistance) + altos_WeathermapPosition.xz;

                // Precipitation
                float w = _PrecipitationGlobal;
                float3 precipitation = GetPrecipitationData(positionWS);
                //n = lerp(n, 1.0, sdf.y);


                // Coverage
                float n = GetLayeredPerlinNoise(_Octaves, positionWS * 0.0001 * _Scale - altos_positionWeather, _Gain, _Lacunarity);
                
                if(_USE_CLOUD_WEATHERMAP_TEX)
                {
                    float2 samplePosition = positionWS * 10000.0 * _Scale - altos_positionWeather;
                    n = _CLOUD_WEATHERMAP_TEX.SampleLevel(altos_linear_repeat_sampler, PositionWSToPositionUV(samplePosition, maxCloudDistance), 0).r;
                }

                float coverage = _CLOUD_COVERAGE;
                
                if(_USE_DISTANT_COVERAGE_OVERRIDE)
                {
                    float distantCoverage = _CLOUD_DISTANT_CLOUD_COVERAGE;
                    float distantCoverageStart = _CLOUD_DISTANT_COVERAGE_START_DEPTH;
                    distantCoverageStart = min(distantCoverageStart, maxCloudDistance);

                    float distantCoverageBlend = os_Map01(distantCoverageStart, maxCloudDistance, length(positionWS - _WorldSpaceCameraPos.xz));
                
                    distantCoverageBlend = 1.0 - distantCoverageBlend;
                    distantCoverageBlend *= distantCoverageBlend;
                    distantCoverageBlend = 1.0 - distantCoverageBlend;
                
                    coverage = lerp(coverage, distantCoverage, distantCoverageBlend);
                }
                
                coverage = lerp(coverage, 1.0, precipitation.z);

                n = os_Remap(1.0 - coverage, 1.0, 0.0, 1, n);
                n = saturate(n);
                n = pow(n, 0.5);// expose this value as a property, "Coverage Strength"?.
                
                
                // Type
                float type = GetLayeredPerlinNoise(_Octaves, positionWS * 0.0001 * _Scale - altos_positionWeather, _Gain, _Lacunarity, 675);
                type = lerp(type, 1.0, precipitation.y);

                return float4(n, precipitation.y, type, 1.0);
            }
            ENDHLSL
        }

    }
}