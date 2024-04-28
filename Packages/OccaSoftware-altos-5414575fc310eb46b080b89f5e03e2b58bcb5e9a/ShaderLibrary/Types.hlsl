#ifndef ALTOS_TYPES_INCLUDED
#define ALTOS_TYPES_INCLUDED

struct AtmosphereData
{
    float atmosThickness;
    float atmosHeight;
    float cloudFadeDistance;
    float distantCoverageAmount;
    float distantCoverageDepth;
};

struct AtmosHitData
{
    bool didHit;
    bool doubleIntersection;
    float nearDist;
    float nearDist2;
    float farDist;
    float farDist2;
};


struct IntersectData
{
    bool hit;
    bool inside;
    float frontfaceDistance;
    float backfaceDistance;
};

struct StaticMaterialData
{
    float2 uv;
	
    float3 mainCameraOrigin;
    float3 rayOrigin;
    float3 sunPos;
    float3 sunColor;
    float sunIntensity;
	
    bool renderLocal;
	
    float cloudiness;
    float alphaAccumulation;
    float3 extinction;
    float3 adjustedExtinction;
    float3 highAltExtinction;
    float HG;
	
    float ambientExposure;
    float3 ambientColor;
    float3 fogColor;
    float fogPower;
	
    float multipleScatteringAmpGain;
    float multipleScatteringDensityGain;
    int multipleScatteringOctaves;
	
    Texture3D baseTexture;
    float4 baseTexture_TexelSize;
    float3 baseScale;
    float3 baseTimescale;
	
    Texture2D curlNoise;
    float4 curlNoiseTexelSize;
    float curlScale;
    float curlStrength;
    float curlTimescale;
	
    Texture3D detail1Texture;
    float4 detailTexture_TexelSize;
    float3 detail1Scale;
    float detail1Strength;
    float3 detail1Timescale;
    bool detail1Invert;
    float2 detail1HeightRemap;
	
    Texture2D highAltTex1;
    float4 highAltTex1_TexelSize;
	
    Texture2D highAltTex2;
    float4 highAltTex2_TexelSize;
	
    Texture2D highAltTex3;
    float4 highAltTex3_TexelSize;
	
    float highAltitudeAlphaAccumulation;
    float2 highAltOffset1;
    float2 highAltOffset2;
    float2 highAltOffset3;
    float2 highAltScale1;
    float2 highAltScale2;
    float2 highAltScale3;
    float highAltitudeCloudiness;
    float highAltInfluence1;
    float highAltInfluence2;
    float highAltInfluence3;
	
    int lightingDistance;
    int planetRadius;
	
    float heightDensityInfluence;
    float cloudinessDensityInfluence;
	
    Texture2D weathermapTex;
};

struct RayData
{
    float3 rayOrigin;
    float3 rayPosition;
    float3 rayDirection;
    float3 rayDirectionUnjittered;
    float relativeDepth;
    float rayDepth;
    float stepSize;
    float shortStepSize;
    float noiseAdjustment;
    float noiseIntensity;
};

#endif
