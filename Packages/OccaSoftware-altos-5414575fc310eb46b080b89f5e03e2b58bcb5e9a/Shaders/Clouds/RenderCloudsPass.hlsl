#ifndef ALTOS_RENDER_CLOUDS_INCLUDED
#define ALTOS_RENDER_CLOUDS_INCLUDED

//#define USE_STATIC_MAP

#include "Packages/com.occasoftware.altos/ShaderLibrary/TextureUtils.hlsl"
#include "Packages/com.occasoftware.altos/ShaderLibrary/Types.hlsl"

#pragma shader_feature _ ALTOS_BUTO_COMPATIBILITY_ENABLED
#ifdef ALTOS_BUTO_COMPATIBILITY_ENABLED
#include "Packages/com.occasoftware.buto/Shaders/Resources/Buto.hlsl"
#endif

// Static Constant Defines
static float EPSILON_LARGE = 0.01;
static float EPSILON = 0.0001;
static float EPSILON_SMALL = 0.000001;
static float highAltitudeFadeDistance = 350000.0;
static float _CIRRUS_CLOUD_HEIGHT = 18000.0;
float3 _CLOUD_ALBEDO;
static float _CONVERT_KM_TO_M = 1000.0;


// Variable Global Property Defines

float2 _CLOUD_WEATHERMAP_VELOCITY;
int _USE_CLOUD_WEATHERMAP_TEX;
float _CLOUD_WEATHERMAP_SCALE;
float2 _CLOUD_WEATHERMAP_VALUE_RANGE;
Texture2D _CLOUD_DENSITY_CURVE_TEX;
float3 _PLANET_CENTER;

Texture2D _CLOUD_HIGHALT_TEX_1;
Texture2D _CLOUD_HIGHALT_TEX_2;
Texture2D _CLOUD_HIGHALT_TEX_3;
float4 _CLOUD_HIGHALT_TEX_1_TexelSize;
float4 _CLOUD_HIGHALT_TEX_2_TexelSize;
float4 _CLOUD_HIGHALT_TEX_3_TexelSize;

Texture2D _CLOUD_CURL_TEX;
float4 _CLOUD_CURL_TEX_TexelSize;

Texture3D _CLOUD_BASE_TEX;
float4 _CLOUD_BASE_TEX_TexelSize;

Texture3D _CLOUD_DETAIL1_TEX;
float4 _CLOUD_DETAIL1_TEX_TexelSize;

float3 _SunDirection;
float3 _SunColor;
float _SunIntensity;
float3 _CLOUD_SUN_COLOR_MASK;

float3 _ZenithColor;
float3 _HorizonColor;

Texture2D _SkyTexture;
int _HasSkyTexture;

float3 _ShadowCasterCameraForward;
float3 _ShadowCasterCameraUp;
float3 _ShadowCasterCameraRight;

int _ShadowPass;

float3 _MainCameraOrigin;
float4 _ShadowCasterCameraPosition;
float4 _CloudShadowOrthoParams; // xy = orthographic size, z = offset distance
float4 _ShadowOrthoParamsNew;
float _CloudShadowStrength;
float _ShadowRenderStepCount;

int _CheapAmbientLighting;

// Lightning Array
float4 altos_LightningArrayPosition[8];
float4 altos_LightningArrayColor[8];
int altos_LightningArraySize;


// Scattering Values (source: https://journals.ametsoc.org/view/journals/bams/79/5/1520-0477_1998_079_0831_opoaac_2_0_co_2.xml):
// Cumulus: 50 - 120
// Stratus: 40 - 60
// Cirrus: 0.1 - 0.7
// Wavelength-specific Scattering Distribution for Cloudy medium : https://www.patarnott.com/satsens/pdf/opticalPropertiesCloudsReview.pdf
float _CLOUD_EXTINCTION_COEFFICIENT;
float _CLOUD_COVERAGE;
float _CLOUD_STEP_COUNT;
float _CLOUD_LAYER_HEIGHT;
float _CLOUD_LAYER_THICKNESS;
float _CLOUD_FADE_DIST;
float3 _CLOUD_BASE_SCALE;
float _CLOUD_BLUE_NOISE_STRENGTH;
float _CLOUD_DETAIL1_STRENGTH;
float3 _CLOUD_BASE_TIMESCALE;
float3 _CLOUD_DETAIL1_TIMESCALE;
float3 _CLOUD_DETAIL1_SCALE;
float _CLOUD_FOG_POWER;
float _CLOUD_MAX_LIGHTING_DIST;
float _CLOUD_PLANET_RADIUS;
float _CLOUD_CURL_SCALE;
float _CLOUD_CURL_STRENGTH;
float _CLOUD_CURL_TIMESCALE;
float _CLOUD_AMBIENT_EXPOSURE;
float _CLOUD_DISTANT_COVERAGE_START_DEPTH;
float _CLOUD_DISTANT_CLOUD_COVERAGE;
float2 _CLOUD_DETAIL1_HEIGHT_REMAP;
bool _CLOUD_DETAIL1_INVERT;
float _CLOUD_HEIGHT_DENSITY_INFLUENCE;
float _CLOUD_COVERAGE_DENSITY_INFLUENCE;

// High alt
float2 _CLOUD_HIGHALT_OFFSET1;
float2 _CLOUD_HIGHALT_OFFSET2;
float2 _CLOUD_HIGHALT_OFFSET3;
float2 _CLOUD_HIGHALT_SCALE1;
float2 _CLOUD_HIGHALT_SCALE2;
float2 _CLOUD_HIGHALT_SCALE3;
float _CLOUD_HIGHALT_COVERAGE;
float _CLOUD_HIGHALT_INFLUENCE1;
float _CLOUD_HIGHALT_INFLUENCE2;
float _CLOUD_HIGHALT_INFLUENCE3;
float _CLOUD_HIGHALT_EXTINCTION;

bool _CLOUD_DEPTH_CULL_ON;
float _CLOUD_SCATTERING_AMPGAIN;
float _CLOUD_SCATTERING_DENSITYGAIN;
int _CLOUD_SCATTERING_OCTAVES;
float _CLOUD_HGFORWARD;
float _CLOUD_HGBACK;
float _CLOUD_HGBLEND;
float _CLOUD_HGSTRENGTH;
Texture2D _CLOUD_WEATHERMAP_TEX;
bool _CLOUD_SUBPIXEL_JITTER_ON;

float3 altos_WeathermapPosition;
float3 GetWeathermapPosition()
{
    return altos_WeathermapPosition.xyz;
}

float4 altos_WorldPositionOffset;
float3 GetFloatingPointOffset()
{
    return altos_WorldPositionOffset.xyz;
}

float CalculateHorizonFalloff(float3 rayPosition, float3 lightDirection, float planetRadius)
{
    float h = max(rayPosition.y, 0);
    float r = planetRadius;
    float a = r + h;
    float b = r / a;
    float c = acos(b);
    float angle = lightDirection.y * 1.571;
    float d = angle - c;
	
    return smoothstep(radians(0.0), radians(5.0), d);
}

float GetHeight01(float3 rayPos, float atmosThickness, float planetRadius, float atmosHeight)
{
    float height01 = distance(rayPos, _PLANET_CENTER) - (planetRadius + atmosHeight);
    height01 /= atmosThickness;
    return saturate(height01);
}

bool IsInsideSDFSphere(float3 pointToCheck, float3 spherePosition, float sphereRadius)
{
    float dist = distance(pointToCheck, spherePosition);
	
    if (dist < sphereRadius)
        return true;
	
    return false;
}


// Resources...
// http://kylehalladay.com/blog/tutorial/math/2013/12/24/Ray-Sphere-Intersection.html
// https://stackoverflow.com/questions/6533856/ray-sphere-intersection
// https://www.cs.colostate.edu/~cs410/yr2017fa/more_progress/pdfs/cs410_F17_Lecture10_Ray_Sphere.pdf
// 
bool SolveQuadratic(float a, float b, float c, out float x0, out float x1)
{
    float discr = b * b - 4 * a * c;
    if (discr < 0)
        return false;
    else if (discr == 0)
        x0 = x1 = -0.5 * b / a;
    else
    {
        float q = (b > 0) ?
            -0.5 * (b + sqrt(discr)) :
            -0.5 * (b - sqrt(discr));
        x0 = q / a;
        x1 = c / q;
    }
	
    float lT = min(x0, x1);
    float gT = max(x0, x1);
    x0 = lT;
    x1 = gT;
 
    return true;
}

bool IntersectSphere(float3 rayOrigin, float3 rayDir, float sphereRad, float3 spherePosition, out float nearHit, out float farHit)
{
    IntersectData intersectionData;
    nearHit = 0.0;
    farHit = 0.0;
	
    float a = dot(rayDir, rayDir);
    float3 L = rayOrigin - spherePosition;
    float b = 2.0 * dot(rayDir, L);
    float c = dot(L, L) - (sphereRad * sphereRad);
    float t0, t1;
    if (!SolveQuadratic(a, b, c, t0, t1))
        return false;
	
    float lt = min(t0, t1);
    float gt = max(t0, t1);
    t0 = lt;
    t1 = gt;
	
    if (t0 < 0)
    {
        t0 = t1;
        if (t0 < 0)
            return false;
    }
    nearHit = max(t0, 0);
    farHit = max(t1, 0);
    return true;
}



AtmosHitData AtmosphereIntersection(float3 rayOrigin, float3 rayDir, float atmosHeight, float planetRadius, float atmosThickness, float maxDist)
{
    float3 sphereCenter = _PLANET_CENTER;
    float innerRad = planetRadius + atmosHeight;
    float outerRad = planetRadius + atmosHeight + atmosThickness;
	
    float innerNear, innerFar, outerNear, outerFar;
    bool hitInner = IntersectSphere(rayOrigin, rayDir, innerRad, sphereCenter, innerNear, innerFar);
    bool hitOuter = IntersectSphere(rayOrigin, rayDir, outerRad, sphereCenter, outerNear, outerFar);
	
    AtmosHitData hitData;
    hitData.didHit = false;
    hitData.doubleIntersection = false;
	
    bool insideInner = IsInsideSDFSphere(rayOrigin, sphereCenter, innerRad);
    bool insideOuter = IsInsideSDFSphere(rayOrigin, sphereCenter, outerRad);
	
    float nearIntersectDistance = 0.0;
    float farIntersectDistance = 0.0;
    float nearIntersectDistance2 = 0.0;
    float farIntersectDistance2 = 0.0;
	
	//Case 1 (Below Cloud Volume)
    if (insideInner && insideOuter)
    {
        nearIntersectDistance = innerNear;
        //farIntersectDistance = outerNear;
        farIntersectDistance = min(outerNear, maxDist);
    }
	
	// Case 2 (Inside Cloud Volume)
    if (!insideInner && insideOuter)
    {
        farIntersectDistance = min(outerNear, maxDist);
		
		// InnerData.frontFaceDistance > 0 when the ray intersects with the inner sphere.
        if (innerNear > 0.0)
        {
            farIntersectDistance = min(innerNear, maxDist);
			
            if (innerFar < maxDist)
            {
                nearIntersectDistance2 = innerFar;
                farIntersectDistance2 = min(outerFar, maxDist);
            }
        }
    }
	
    bool lookingAboveClouds = false;
	// Case 3 (Above Cloud Volume)
    if (!insideInner && !insideOuter)
    {
        if (!hitInner && !hitOuter)
            lookingAboveClouds = true;
		
        nearIntersectDistance = outerNear;
        farIntersectDistance = min(outerFar, maxDist);
		
		// InnerData.frontFaceDistance > 0 when the ray intersects with the inner sphere.
        if (innerNear > 0.0)
        {
            farIntersectDistance = min(innerNear, maxDist);
            if (innerFar < maxDist)
            {
                nearIntersectDistance2 = innerFar;
                farIntersectDistance2 = min(outerFar, maxDist);
            }
        }
    }
	
    hitData.nearDist = nearIntersectDistance;
    hitData.nearDist2 = nearIntersectDistance2;
    hitData.farDist = farIntersectDistance;
    hitData.farDist2 = farIntersectDistance2;
	
    if (hitData.nearDist < maxDist)
        hitData.didHit = true;
	
    if (hitData.nearDist2 > EPSILON)
        hitData.doubleIntersection = true;
	
    if (lookingAboveClouds)
        hitData.didHit = false;
	
    return hitData;
}



AtmosphereData New_AtmosphereData(float thickness, float height, float fadeDistance, float distantCoverageAmount, float distantCoverageDepth)
{
    AtmosphereData a;
    a.atmosThickness = thickness;
    a.atmosHeight = height;
    a.cloudFadeDistance = fadeDistance;
    a.distantCoverageAmount = distantCoverageAmount;
    a.distantCoverageDepth = distantCoverageDepth;
    return a;
}




float GetMipLevel(float2 uv, float2 texelSize, float bias)
{
    float2 unnormalizeduv = uv * texelSize;
    float2 uvDDX = ddx(unnormalizeduv);
    float2 uvDDY = ddy(unnormalizeduv);
    float d = max(dot(uvDDX, uvDDX), dot(uvDDY, uvDDY));
    float mipLevel = 0.5 * log2(d);
    return max(mipLevel + bias, 0);
}

float GetMipLevel3D(float3 uv, float3 texelSize, float bias)
{
    float3 unnormalizeduv = uv * texelSize;
    float3 uvDDX = ddx(unnormalizeduv);
    float3 uvDDY = ddy(unnormalizeduv);
    float d = max(dot(uvDDX, uvDDX), dot(uvDDY, uvDDY));
    float mipLevel = 0.5 * log2(d);
    return max(mipLevel + bias, 0);
}

float4 altos_positionHighAlt[3];

float GetCloudShape2D(StaticMaterialData m, float3 rayPosition, AtmosphereData atmosData, int mip)
{
    float2 uv = rayPosition.xz * 0.00001;

    float2 uv1 = uv * m.highAltScale1 - altos_positionHighAlt[0].xy;
	
    float coverage = m.highAltTex1.SampleLevel(altos_linear_repeat_sampler, uv1, GetMipLevel(uv1, m.highAltTex1_TexelSize.zw, mip)).r;
    coverage = os_Map01(1.0 - m.highAltitudeCloudiness, 1.0, coverage);
	
    if (coverage < EPSILON)
        return 0;
	
    float2 _uv = uv1 * 10.0;
    float2x2 rm = float2x2(float2(-sin(coverage + _uv.y), cos(coverage + _uv.x)), float2(cos(coverage + _uv.y), sin(coverage + _uv.x)));
	

    float2 uv2 = uv * m.highAltScale2 - altos_positionHighAlt[1].xy;
    uv2 += 0.2 * mul(uv2, rm);


    float2 uv3 = uv * m.highAltScale3 - altos_positionHighAlt[2].xy;
    uv3 += 0.4 * mul(uv3, rm);


    float value = m.highAltTex2.SampleLevel(altos_linear_repeat_sampler, uv2, GetMipLevel(uv2, m.highAltTex2_TexelSize.zw, mip)).r * .7;
    value += m.highAltTex3.SampleLevel(altos_linear_repeat_sampler, uv3, GetMipLevel(uv3, m.highAltTex3_TexelSize.zw, mip)).r * .3;
	
    value = lerp(value, 1.0, m.highAltitudeCloudiness);
    value = os_Map01(1.0 - coverage, 1.0, value);
	
    return value;
}


float2 altos_positionWeather;
float3 altos_positionBase;
float3 altos_positionDetail;
float altos_positionCurl;

#define USE_FLOATING_ORIGIN 0
float2 GetWeathermapUV(float3 rayPosition, float3 rayOrigin, float maxRenderDistance)
{
    #ifdef USE_STATIC_MAP
    float2 UV = rayPosition.xz;
    #else
    float2 UV = rayPosition.xz - GetWeathermapPosition().xz;
    #endif
    UV += GetFloatingPointOffset().xz;

	UV *= rcp(maxRenderDistance); // needs to be 1.0 / max distance
    UV *= 0.5;
    UV += 0.5;
    return UV;
}


float _WEATHERMAP_OCTAVES;
float _WEATHERMAP_GAIN;
float _WEATHERMAP_LACUNARITY;

Texture2D altos_cloud_map;


// #define POINT_SAMPLING

#ifdef POINT_SAMPLING
#define cloudSampler altos_point_repeat_sampler
#else
#define cloudSampler altos_linear_repeat_sampler
#endif

float3 GetWeathermap(StaticMaterialData materialData, RayData rayData, float cloudinessAtPoint, float maxRenderDistance)
{
    float2 weathermapUV;
    float3 weathermapSample = 0;

    weathermapUV = GetWeathermapUV(rayData.rayPosition, materialData.mainCameraOrigin, maxRenderDistance);
    if (any(weathermapUV < 0.0) || any(weathermapUV > 1.0))
        return 0;
    
    weathermapSample = altos_cloud_map.SampleLevel(cloudSampler, weathermapUV, 0).rgb;
    /*
    float d = distance(rayData.rayPosition.xz, GetWeathermapPosition().xz + GetFloatingPointOffset().xz);
    if(d > maxRenderDistance)
        return 0;
    */
    
    return weathermapSample;
}

float GetCloudDensityByHeight(StaticMaterialData materialData, RayData rayData, float height01, float3 weather)
{
    float x = weather.b; 
    
    float result;

    // These are our cloud shape presets.
    // We don't go all the way to 0 or 1 so that we have some room to play around
    // Low level clouds...
    float a = 1.0 - abs(os_Remap(0.1, 0.3, -1, 1, height01));
    a = pow(a, 0.5);

    // Mid-level clouds...
    float b = 1.0 - abs(os_Remap(0.15, 0.6, -1, 1, height01));
    b = pow(b, 0.5);

    // Super tall clouds!
    float c = 1.0 - abs(os_Remap(0.05, 0.95, -1, 1, height01));
    c = pow(c, 0.5);

    // To interpolate, we span [0,1] to [0,2], lerp the bottom range
    // then shift [1,2] to [0,1] and lerp the top range
    x *= 2.0;
    float ab = lerp(a, b, saturate(x));
    x -= 1.0;
    result = lerp(ab, c, saturate(x));
    return result;

    /*
    Using our cloud presets lets us have more choices than a single curve.
    return saturate(_CLOUD_DENSITY_CURVE_TEX.SampleLevel(altos_linear_clamp_sampler, float2(height01, 0), 0).r);
    */
}


float GetCloudShapeVolumetric(StaticMaterialData m, RayData rayData, AtmosphereData atmosData, float2 weathermap, float densityAtHeight, float height01, float cloudinessAtPoint, int mip, bool doSampleDetail)
{
    float coverage = weathermap.r * densityAtHeight;

    #ifdef USE_STATIC_MAP
    rayData.rayPosition.xz -= _WorldSpaceCameraPos.xz;
    #endif

    float3 uvw = rayData.rayPosition * 0.0001;
	float cloudAdjFactor = weathermap.r * 0.7;

	// Sample Curl
    float3 curlUV = uvw * m.curlScale - altos_positionCurl;
    float3 curlSampleXZ = m.curlNoise.SampleLevel(cloudSampler, curlUV.xy, mip).rgb;
    float3 curlSampleXY = m.curlNoise.SampleLevel(cloudSampler, curlUV.zy, mip).rgb;
    float3 curlSample = curlSampleXY + curlSampleXZ;
    curlSample = curlSample - 1.0;
    curlSample *= m.curlStrength * 10.0;
	
	
	// Sample Base
    float3 heightOffset = (height01 * height01 * m.baseTimescale); // Bias the top of the clouds so that they lean in the wind direction (i.e., make it look like wind is stronger at top of clouds)

    float3 baseUVW = uvw * m.baseScale - altos_positionBase;
    baseUVW += curlSample + heightOffset;
	
    float baseVal = m.baseTexture.SampleLevel(cloudSampler, baseUVW, mip).r;
    baseVal = saturate(baseVal);
    
    baseVal = lerp(baseVal, 1.0, cloudAdjFactor);
    float value = os_Map01(1.0 - baseVal, 1.0, coverage);
	
    if (value < EPSILON_LARGE)
        return 0;
	
    #define MAX_DETAIL_DISTANCE 23000
	[branch]
    if (doSampleDetail && rayData.rayDepth < MAX_DETAIL_DISTANCE && m.detail1Strength > EPSILON)
    {
        float3 detail1UVW = uvw * m.detail1Scale - altos_positionDetail;
	
        float valueDetail = m.detail1Texture.SampleLevel(cloudSampler, detail1UVW, mip).r;
        valueDetail = saturate(valueDetail);
        // Map negative to [0, 0.3], positive to [0.3, 1.0].
        float detailSwitch = saturate(height01 * 3.0);
        valueDetail = lerp(1.0 - valueDetail, valueDetail, detailSwitch);
        
        /* I prefer using the switch as above.
        valueDetail = lerp(lerp(1.0 - valueDetail, 0.0, saturate(height01 * rcp(detailSwitch))), valueDetail, saturate((height01 - detailSwitch) * rcp(1.0 - detailSwitch)));
        float falloff = saturate(RemapUnclamped(m.detail1HeightRemap.x, m.detail1HeightRemap.y, 0.0, 1.0, height01));
        valueDetail = lerp(1.0 - valueDetail, valueDetail, falloff);
        */

        valueDetail = lerp(valueDetail, 1.0, cloudAdjFactor);
        valueDetail = lerp(1.0, valueDetail, m.detail1Strength);
        
        value = os_Map01(1.0 - valueDetail, 1.0, value);
    }
	
    value *= lerp(1.0, height01, m.heightDensityInfluence);
    value *= lerp(1.0, weathermap.r, m.cloudinessDensityInfluence);
    
    
    value *= (1.0 + weathermap.g);

    return saturate(value);
}

float BeerLambert(float absorptionCoefficient, float stepSize, float density)
{
    return exp(-absorptionCoefficient * stepSize * density);
}

float HenyeyGreenstein(float cos_angle, float eccentricity)
{
    float e2 = eccentricity * eccentricity;
    float f = abs((1.0 + e2 - 2.0 * eccentricity * cos_angle));
    float n = 1.0 - e2;
    float d = pow(f, 1.5);
    return (n / d) * 0.7854; // equivalent to below
    //return ((1.0 - e2) / pow(f, 1.5)) / 4.0 * 3.1416;
}

struct OSLightingData
{
    float3 baseLighting;
    float3 outScatterLighting;
    float3 additionalLighting;
};

#define RAIN_DENSITY_MODIFIER 4.0
float GetRainDensity(float p)
{
    return 1.0 + (p * RAIN_DENSITY_MODIFIER); // 1 + [0,1] * 4 = [1,5].
}


#define RAIN_LIGHTING_STRENGTH 0.8
float GetRainLightingReduction(float p)
{
    return 1.0 - p * p * RAIN_LIGHTING_STRENGTH;
}

float _SHADING_STRENGTH_FALLOFF;

OSLightingData GetLightingDataVolumetric(StaticMaterialData materialData, RayData rayData, AtmosphereData atmosData, int mip)
{
    OSLightingData data;
	
    float3 cachedRayOrigin = rayData.rayPosition;
	/* REMOVED -- This caused a "vortex" effect when looking directly up.
	//float r = GetHaltonFromTexture(rayData.rayDepth + _FrameId).g;
	//r = lerp(0, r, rayData.noiseIntensity);
	//r = Remap(0.0, 1.0, 2.0, 3.0, r);
	*/
	
    #define LIGHTING_SAMPLE_COUNT 7

	/* REMOVED - Performance Improvement, Minor Visual Difference
	float t0, t1;
	IntersectSphere(rayData.rayPosition, materialData.sunPos, materialData.planetRadius + atmosData.atmosHeight + atmosData.atmosThickness, _PLANET_CENTER, t0, t1);
	float lightingDistanceToSample = min(t0, materialData.lightingDistance);
	*/
	
    float lightingDistanceToSample = materialData.lightingDistance;

    float totalDensity = 0.0;
    float currentStepSize = 0.0;
    float3 extinction;

    // Can we use these properties to adjust cloud precipitation lighting?
    float sAmp = 1.0;
    float3 densityAdj = 0;
    
    float samplePositions[] = { 0.02, 0.06, 0.14, 0.25, 0.4, 0.7, 1.0 };
    //float cloudinessAtPoint = GetDistantCloudiness(materialData.cloudiness, distance(rayData.rayPosition, rayData.rayOrigin), atmosData.distantCoverageDepth, atmosData.cloudFadeDistance, atmosData.distantCoverageAmount);
    float cloudinessAtPoint = 1.0;
    float3 weather = GetWeathermap(materialData, rayData, cloudinessAtPoint, atmosData.cloudFadeDistance);
    
    

    [unroll]
    float prevDistance = 0;
    for (int i = 0; i < LIGHTING_SAMPLE_COUNT; i++)
    {
        float totalDistance = samplePositions[i] * lightingDistanceToSample;
		
        rayData.rayPosition = cachedRayOrigin + (materialData.sunPos * totalDistance);
		
        //float cloudinessAtPoint = GetDistantCloudiness(materialData.cloudiness, distance(rayData.rayPosition, rayData.rayOrigin), atmosData.distantCoverageDepth, atmosData.cloudFadeDistance, atmosData.distantCoverageAmount);
        float cloudinessAtPoint = 1.0;
        float3 weather = GetWeathermap(materialData, rayData, cloudinessAtPoint, atmosData.cloudFadeDistance);
		
        [branch]
        if (weather.r > EPSILON_LARGE)
        {
            float height01 = GetHeight01(rayData.rayPosition, atmosData.atmosThickness, materialData.planetRadius, atmosData.atmosHeight);
            float densityAtHeight = GetCloudDensityByHeight(materialData, rayData, height01, weather);
            float cloudDensity = GetCloudShapeVolumetric(materialData, rayData, atmosData, weather, densityAtHeight, height01, weather.r, mip, true);
            totalDensity += cloudDensity * (totalDistance - prevDistance) * GetRainDensity(weather.g);
			
            extinction = materialData.extinction * sAmp;
            sAmp *= _SHADING_STRENGTH_FALLOFF;
            densityAdj += extinction * totalDensity;
        }
        prevDistance = totalDistance;
    }
	

    // Calculate Base Lighting
    data.baseLighting = exp(-densityAdj);


    /////////////////////////////////////////////
    // Outscattering                           //
    /////////////////////////////////////////////
    data.outScatterLighting = 0.0;
	
    float msAmp = 0.3;
    for (int octaveCounter = 1; octaveCounter < materialData.multipleScatteringOctaves; octaveCounter++)
    {
        msAmp *= materialData.multipleScatteringAmpGain;
        totalDensity *= materialData.multipleScatteringDensityGain;
        data.outScatterLighting += exp(-materialData.extinction * totalDensity) * msAmp;
    }
	
    /////////////////////////////////////////////
    // Lightning                               //
    /////////////////////////////////////////////
    for(int lightningCount = 0; lightningCount < altos_LightningArraySize; lightningCount++)
    {
        float d = distance(cachedRayOrigin, altos_LightningArrayPosition[lightningCount].xyz);
        float3 lightningIntensity = (altos_LightningArrayColor[lightningCount].rgb * altos_LightningArrayColor[lightningCount].a) / (d*d);
        data.additionalLighting += lightningIntensity;
    }

    return data;
}

#define AMBIENT_STEP_LENGTH 400

float GetAmbientDensity(StaticMaterialData materialData, RayData rayData, AtmosphereData atmosData, float3 weather, float cloudinessAtPoint, int mip, int cheapAmbientLighting)
{
    float height01 = GetHeight01(rayData.rayPosition, atmosData.atmosThickness, materialData.planetRadius, atmosData.atmosHeight);
	
    if (cheapAmbientLighting)
    {
        return height01 * height01;
    }
	
    float step = AMBIENT_STEP_LENGTH;
	
    rayData.rayPosition += float3(0, 1, 0) * step;
	
    float density = GetCloudDensityByHeight(materialData, rayData, height01, weather);
    float d = GetCloudShapeVolumetric(materialData, rayData, atmosData, weather, density, height01, cloudinessAtPoint, mip, false);
	
    float ambientDensity = exp(-d * materialData.extinction.r * step * 0.3 * GetRainDensity(weather.g));
    return ambientDensity;
}


void ApplyLighting(StaticMaterialData materialData, float atmosphereDensity, float accumulatedDepth, float3 lightEnergy, float ambientEnergy, float baseEnergy, inout float3 cloudColor)
{
    float atmosphericAttenuation = 1.0 - rcp(exp(accumulatedDepth * atmosphereDensity));
    cloudColor += materialData.fogColor * atmosphericAttenuation * baseEnergy;
    cloudColor += materialData.sunColor * materialData.sunIntensity * (1.0 - atmosphericAttenuation) * lightEnergy;
    cloudColor += materialData.ambientColor * materialData.ambientExposure * (1.0 - atmosphericAttenuation) * ambientEnergy;
}

void SampleHighAltitudeClouds(RayData rayData, StaticMaterialData materialData, AtmosphereData atmosData, float highAltitudeHitDistance, inout float alpha, inout float3 cloudColor)
{
    const float simulatedStepLength = 100.0;
	
    float fadeOut = os_Remap(highAltitudeFadeDistance * 0.8, highAltitudeFadeDistance, 1.0, 0.0, highAltitudeHitDistance);
    fadeOut = saturate(fadeOut);
	
    float3 lightEnergy = 0;
    float baseEnergy = 0;
    float ambientEnergy = 0;
	
    #define MAX_STEPS 4
    #define INV_MAX_STEPS 1.0 / 4.0
    
    [unroll(MAX_STEPS)]
    for (int a = 0; a <= MAX_STEPS; a++)
    {
        rayData.rayPosition = rayData.rayOrigin + rayData.rayDirectionUnjittered * highAltitudeHitDistance + simulatedStepLength * a;
        float valueAtPoint = GetCloudShape2D(materialData, rayData.rayPosition, atmosData, MAX_STEPS - a) * fadeOut;
        valueAtPoint *= lerp(0.6, 1.0, float(a) * INV_MAX_STEPS);
		
        if (valueAtPoint > EPSILON_LARGE)
        {
            float priorAlpha = alpha;
					
            float3 sampleExtinction = materialData.highAltExtinction * valueAtPoint;
		
            float transmittance = exp(-sampleExtinction.r * simulatedStepLength);
            alpha *= transmittance;
            float3 sampleExtinctionInverse = rcp(sampleExtinction);
			
            float totalDensity = 0;
            float stepLength = 0;
            
			// Sample Lighting
            float3 extinction;
            float sAmp = 1.0;
            float sGain = 0.3;
            float3 densityAdj = 0;
            [unroll(4)]
            for (int i = 1; i < 4; i++)
            {
                stepLength += 50.0 * i;
                float3 lightRayPos = rayData.rayPosition + materialData.sunPos * stepLength;
                float lightSample = GetCloudShape2D(materialData, lightRayPos, atmosData, i);
                totalDensity += lightSample * stepLength;
                extinction = materialData.highAltExtinction * sAmp;
                sAmp *= sGain;
                densityAdj += extinction * totalDensity;
            }
					
            
				
            float amp = 0.5;
            float outScattering = 0.0;
	
            for (int octaveCounter = 1; octaveCounter < materialData.multipleScatteringOctaves; octaveCounter++)
            {
                amp *= materialData.multipleScatteringAmpGain;
                totalDensity *= materialData.multipleScatteringDensityGain;
                outScattering += exp(-totalDensity * materialData.highAltExtinction.r) * amp;
            }
		
            float3 col = 0;

            float3 lighting = exp(-densityAdj);
            float3 lightData = materialData.sunColor * materialData.sunIntensity * ((lighting * materialData.HG) + outScattering) * sampleExtinction;
            float3 intScatter = (lightData - (lightData * transmittance)) * sampleExtinctionInverse;
            col += intScatter * priorAlpha;
			
            
            // Ambient Lighting
            float3 intAmbient = 1.0 * sampleExtinction.r * materialData.ambientColor * materialData.ambientExposure;
            intAmbient = (intAmbient - (intAmbient * transmittance)) * sampleExtinctionInverse;
            col += intAmbient * priorAlpha;
			
            // Atmospheric Fog
            float3 fogData = materialData.fogColor * (sampleExtinction - (sampleExtinction * transmittance)) * sampleExtinctionInverse;
            float3 fog = fogData * priorAlpha;
            float atmosphericAttenuation = rcp(exp(highAltitudeHitDistance *  materialData.fogPower * 0.3));
            col = lerp(fog, col, atmosphericAttenuation);


            // Buto Fog
            #ifdef ALTOS_BUTO_COMPATIBILITY_ENABLED
                float4 butoFog = ButoFog(materialData.uv, highAltitudeHitDistance);
                float3 butoFogData = butoFog.rgb * (sampleExtinction - (sampleExtinction * transmittance)) * sampleExtinctionInverse;
                float3 integratedButoFog = butoFogData * priorAlpha;
                col = (col * butoFog.a) + integratedButoFog;
            #endif

            cloudColor += col;
        }
    }
	
    //ApplyLighting(materialData, materialData.fogPower * 0.3, highAltitudeHitDistance, lightEnergy, ambientEnergy, baseEnergy, cloudColor);
}


float3 CalculateScattering(float3 scattering, float3 extinction, float3 inverseExtinction, float transmittance, float priorAlpha)
{
    return (scattering - (scattering * transmittance)) * inverseExtinction * priorAlpha;
}

void EvaluateExtinction(float3 materialExtinction, float valueAtPoint, float stepSize, out float3 sampleExtinction, out float3 inverseSampleExtinction, out float transmittance, inout float alpha)
{
    sampleExtinction = materialExtinction * valueAtPoint;
    transmittance = exp(-sampleExtinction.r * stepSize);
    alpha *= transmittance;
    inverseSampleExtinction = rcp(sampleExtinction);
}

struct FragmentOutput
{
    float4 color;
};

StaticMaterialData Setup(in float2 UV)
{
    StaticMaterialData materialData;
    materialData.sunPos = normalize(_SunDirection);
    materialData.sunColor = _SunColor * _CLOUD_SUN_COLOR_MASK + 1e-4;
    materialData.sunIntensity = _SunIntensity + 1e-4;
    materialData.ambientColor = lerp(_ZenithColor, _HorizonColor, 0.5);
    materialData.ambientExposure = _CLOUD_AMBIENT_EXPOSURE + 1e-4;
    materialData.cloudiness = _CLOUD_COVERAGE;
    materialData.alphaAccumulation = _CLOUD_EXTINCTION_COEFFICIENT * 0.01;
    materialData.multipleScatteringAmpGain = _CLOUD_SCATTERING_AMPGAIN;
    materialData.multipleScatteringDensityGain = _CLOUD_SCATTERING_DENSITYGAIN;
    materialData.multipleScatteringOctaves = _CLOUD_SCATTERING_OCTAVES;
    materialData.baseTexture = _CLOUD_BASE_TEX;
    materialData.baseScale = _CLOUD_BASE_SCALE;
    materialData.baseTimescale = _CLOUD_BASE_TIMESCALE * 0.0001;
    materialData.curlNoise = _CLOUD_CURL_TEX;
    materialData.curlNoiseTexelSize = _CLOUD_CURL_TEX_TexelSize;
    materialData.detail1Texture = _CLOUD_DETAIL1_TEX;
    materialData.detail1Scale = _CLOUD_DETAIL1_SCALE;
    materialData.detail1Strength = _CLOUD_DETAIL1_STRENGTH;
    materialData.detail1Timescale = _CLOUD_DETAIL1_TIMESCALE;
    materialData.detail1Invert = _CLOUD_DETAIL1_INVERT;
    materialData.detail1HeightRemap = _CLOUD_DETAIL1_HEIGHT_REMAP;
    materialData.lightingDistance = _CLOUD_MAX_LIGHTING_DIST;
    materialData.planetRadius = _CLOUD_PLANET_RADIUS * _CONVERT_KM_TO_M;
    materialData.curlScale = _CLOUD_CURL_SCALE;
    materialData.curlStrength = _CLOUD_CURL_STRENGTH * 0.01;
    materialData.curlTimescale = _CLOUD_CURL_TIMESCALE * 0.005;
    materialData.heightDensityInfluence = _CLOUD_HEIGHT_DENSITY_INFLUENCE;
    materialData.cloudinessDensityInfluence = _CLOUD_COVERAGE_DENSITY_INFLUENCE;
    materialData.highAltTex1 = _CLOUD_HIGHALT_TEX_1;
    materialData.highAltTex1_TexelSize = _CLOUD_HIGHALT_TEX_1_TexelSize;
    materialData.highAltTex2 = _CLOUD_HIGHALT_TEX_2;
    materialData.highAltTex2_TexelSize = _CLOUD_HIGHALT_TEX_2_TexelSize;
    materialData.highAltTex3 = _CLOUD_HIGHALT_TEX_3;
    materialData.highAltTex3_TexelSize = _CLOUD_HIGHALT_TEX_3_TexelSize;
    materialData.highAltitudeAlphaAccumulation = _CLOUD_HIGHALT_EXTINCTION * 0.001;
    materialData.highAltitudeCloudiness = _CLOUD_HIGHALT_COVERAGE;
    materialData.highAltOffset1 = _CLOUD_HIGHALT_OFFSET1;
    materialData.highAltOffset2 = _CLOUD_HIGHALT_OFFSET2;
    materialData.highAltOffset3 = _CLOUD_HIGHALT_OFFSET3;
    materialData.highAltScale1 = _CLOUD_HIGHALT_SCALE1;
    materialData.highAltScale2 = _CLOUD_HIGHALT_SCALE2;
    materialData.highAltScale3 = _CLOUD_HIGHALT_SCALE3;
    materialData.highAltInfluence1 = _CLOUD_HIGHALT_INFLUENCE1;
    materialData.highAltInfluence2 = _CLOUD_HIGHALT_INFLUENCE2;
    materialData.highAltInfluence3 = _CLOUD_HIGHALT_INFLUENCE3;
    materialData.weathermapTex = _CLOUD_WEATHERMAP_TEX;
    materialData.renderLocal = true;
    materialData.fogPower = _CLOUD_FOG_POWER;
    materialData.uv = UV;
    materialData.fogColor = _HorizonColor;
    materialData.rayOrigin = _MainCameraOrigin - GetFloatingPointOffset();
    materialData.mainCameraOrigin = _MainCameraOrigin - GetFloatingPointOffset();
    //materialData.rayOrigin = _MainCameraOrigin;
    //materialData.mainCameraOrigin = _MainCameraOrigin;
    return materialData;
}

float _COARSE_STEPS = 32.0;
float _DETAIL_STEPS = 196.0;
int _MAXIMUM_EMPTY_DETAIL_STEPS = 5;
float2 _MIPMAP_STAGES = float2(12000, 25000);
float _LIGHTING_INSCATTERING;
float _IN_SCATTERING_MULTIPLIER;
float _IN_SCATTERING_STRENGTH;

FragmentOutput SampleClouds(float2 UV, int useDownscaledDepth, int useReprojection, int useDepth)
{
    
    float alpha = 1.0;
    float3 cloudColor = float3(0, 0, 0);
    float4 cloudData = float4(cloudColor, alpha);
	FragmentOutput o;
    o.color = cloudData;

    
	// Material Data Setup
    if (useReprojection == 1 && _ShadowPass == 0)
    {
        int2 uvIndex = UV * _RenderTextureDimensions.zw * 2.0;
        int2 uvIndexOffset[4] = { int2(0,0), int2(1,0), int2(0,1), int2(1,1) }; 
        uvIndex += uvIndexOffset[_FrameId % 4];
        UV = float2(uvIndex) * _RenderTextureDimensions.xy * 0.5;
    }
    
    
    StaticMaterialData materialData = Setup(UV);
    
    if (_HasSkyTexture && _ShadowPass == 0)
    {
        materialData.fogColor = _SkyTexture.SampleLevel(altos_linear_clamp_sampler, materialData.uv, 0).rgb;
    }
    
    
    if (_ShadowPass == 1 && dot(materialData.sunPos, float3(0, 1, 0)) < 0)
    {
        o.color = float4(_INF, 0, 0, 0);
        return o;
    }
	
	// Other properties...
    float accDepthSamples = 0;
    float ambientEnergy = 0.0;
    float baseEnergy = 0.0;
    float3 lightEnergy = float3(0, 0, 0);
    float3 fogEnergy = 0;
    float valueAtPoint = 0;
    int mip = 0;
	
	
	
	// Depth, Ray, and UV Setup
    RayData rayData;
    rayData.rayOrigin = materialData.rayOrigin;
    float2 jitteredUV = UV;
    float viewLength, viewLengthUnjittered;
    GetWSRayDirectionFromUV(UV, rayData.rayDirectionUnjittered, viewLengthUnjittered);
    viewLength = viewLengthUnjittered;
    rayData.rayDirection = rayData.rayDirectionUnjittered;

    if (_CLOUD_SUBPIXEL_JITTER_ON)
    {
        float2 halton = GetHaltonFromTexture(GetPixelIndex(UV, _RenderTextureDimensions.zw) + _FrameId) - 0.5;
        float2 texCoord = _RenderTextureDimensions.xy;
        float2 jitter = texCoord * halton;
        jitteredUV += jitter;
        GetWSRayDirectionFromUV(jitteredUV, rayData.rayDirection, viewLength);
    }
    
    
	
	// Set up depth properties
    float depthRaw, depth01, depthEye, realDepthEye;
    depthRaw = 0;
    depth01 = _INF;
    depthEye = _INF;
    realDepthEye = _INF;
    [branch]
    if (useDepth == 1 && _ShadowPass == 0)
    {
        if (useDownscaledDepth == 0)
        {
            depthRaw = SampleSceneDepth(UV);
        }
        else
        {
            depthRaw = _DitheredDepthTexture.SampleLevel(altos_point_clamp_sampler, UV, 0).r;
        }
        
        depth01 = Linear01Depth(depthRaw, _ZBufferParams);
        if (depth01 < 1.0)
        {
            depthEye = LinearEyeDepth(depthRaw, _ZBufferParams);
            realDepthEye = depthEye * viewLength;
        }
    }
	
	[branch]
    if (_ShadowPass == 1)
    {
        _CLOUD_STEP_COUNT = _ShadowRenderStepCount;
        int cascadeIndex = 0;
        
        float2 orthoSize = _CloudShadowOrthoParams.xy;
        orthoSize = (UV - 0.5) * orthoSize; // Remaps xy to range of [-0.5 * orthoWidth, 0.5 * orthoWidth] on x and [-0.5 * orthoHeight, 0.5 * orthoHeight] on y.
		
        rayData.rayDirection = _ShadowCasterCameraForward;
        rayData.rayDirectionUnjittered = _ShadowCasterCameraForward;
        materialData.rayOrigin = _ShadowCasterCameraPosition.xyz - GetFloatingPointOffset() + _ShadowCasterCameraRight * orthoSize.x + _ShadowCasterCameraUp * orthoSize.y;
        rayData.rayOrigin = materialData.rayOrigin;
		
        materialData.renderLocal = false;
    }
	
	
	
	// Set up extinction properties
	// Albedo of cloudy material is near to 1. Given that extinction coefficient is calculated as absorption + scattering, when absorption = 0 then extinction = scattering.
    materialData.extinction = materialData.alphaAccumulation;
    materialData.highAltExtinction = materialData.highAltitudeAlphaAccumulation;
	
	
	// Cloud Parameter Setup
    AtmosphereData atmosData = New_AtmosphereData(_CLOUD_LAYER_THICKNESS, _CLOUD_LAYER_HEIGHT, _CLOUD_FADE_DIST, _CLOUD_DISTANT_CLOUD_COVERAGE, _CLOUD_DISTANT_COVERAGE_START_DEPTH);
	
    float maxRenderDistance = atmosData.cloudFadeDistance;
	
    _PLANET_CENTER = float3(materialData.mainCameraOrigin.x + GetFloatingPointOffset().x, 0.0 - materialData.planetRadius, materialData.mainCameraOrigin.z + GetFloatingPointOffset().z);
    if (_ShadowPass == 1)
    {
        maxRenderDistance = _CloudShadowOrthoParams.z * 2.0;
    }
	
	// Lower Atmosphere Decisioning
    bool sampleLowAltitudeClouds = false;
    AtmosHitData hitData = (AtmosHitData)0;
    hitData.nearDist = 0;
    hitData.didHit = false;
    
    [branch]
    if (materialData.extinction.r > EPSILON)
    {
        hitData = AtmosphereIntersection(rayData.rayOrigin, rayData.rayDirection, atmosData.atmosHeight, materialData.planetRadius, atmosData.atmosThickness, maxRenderDistance);
		
        if (hitData.didHit)
            sampleLowAltitudeClouds = true;
	
        if (materialData.renderLocal && hitData.nearDist > realDepthEye && depth01 < 1.0)
            sampleLowAltitudeClouds = false;
    }
	
	
	
	// Upper Atmosphere Decisioning
    bool doSampleHighAlt = false;
    float nearHitUpperAtmosphere = 0;
    float farHitUpperAtmosphere = 0;
    
    [branch]
    if (materialData.highAltitudeCloudiness > EPSILON && materialData.highAltExtinction.r > EPSILON)
    {
        float radius = materialData.planetRadius + atmosData.atmosHeight + atmosData.atmosThickness + _CIRRUS_CLOUD_HEIGHT;
		
        bool didHitUpperAtmosphere = IntersectSphere(rayData.rayOrigin, rayData.rayDirectionUnjittered, radius, _PLANET_CENTER, nearHitUpperAtmosphere, farHitUpperAtmosphere);
        rayData.rayDepth = nearHitUpperAtmosphere;
		
        if (didHitUpperAtmosphere)
            doSampleHighAlt = true;
		
            
        if (materialData.renderLocal && rayData.rayDepth > realDepthEye && depth01 < 1.0)
            doSampleHighAlt = false;
		
        if (rayData.rayDepth > highAltitudeFadeDistance)
            doSampleHighAlt = false;
            
    }
	
	
    if (_ShadowPass == 1)
    {
        doSampleHighAlt = false;
    }
	
	
	// Instead, check to see if the ray crosses the low altitude cloud layer on the way to the high altitude cloud layer.
	// If it does, then render the high altitude clouds second.
    int highAltPassId = 0;
    if (hitData.nearDist < nearHitUpperAtmosphere)
    {
        highAltPassId = 1;
    }
	
    [branch]
    if (doSampleHighAlt || sampleLowAltitudeClouds)
    {
        #define HG_INTERPOLATION_FACTOR 0.5
		// Calculate Henyey-Greenstein
		// Physically realistic HGForward Value is ~0.6. HGBack is an artistic factor.
        float cos_angle = dot(materialData.sunPos, rayData.rayDirection);
        float HGForward = HenyeyGreenstein(cos_angle, _CLOUD_HGFORWARD);
        float HGBack = HenyeyGreenstein(cos_angle, _CLOUD_HGBACK);
        float HG = lerp(HGForward, HGBack, HG_INTERPOLATION_FACTOR);
        HG = lerp(1.0, HG, saturate(_CLOUD_HGSTRENGTH));
        materialData.HG = HG;
    }
	
	[branch]
    if (doSampleHighAlt && highAltPassId == 0)
    {
        SampleHighAltitudeClouds(rayData, materialData, atmosData, nearHitUpperAtmosphere, alpha, cloudColor);
    }
	
    float sampleDepth = 0;
    float maxDepth = 0;
    float volumeThickness = 0;
	
    int r = 0;
    int g = 0;
	
    float frontDepth = -1;
    float sumExtinction = 0;
    int extinctionCounter = 0;
	
	[branch]
    if (sampleLowAltitudeClouds)
    {
        hitData.nearDist = min(hitData.nearDist, realDepthEye);
        hitData.nearDist2 = min(hitData.nearDist2, realDepthEye);
        hitData.farDist = min(hitData.farDist, realDepthEye);
        hitData.farDist2 = min(hitData.farDist2, realDepthEye);
		
        sampleDepth = hitData.nearDist;
        maxDepth = max(hitData.farDist, hitData.farDist2);
        maxDepth = min(maxDepth, maxRenderDistance);
        volumeThickness = (hitData.farDist2 - hitData.nearDist2) + (hitData.farDist - hitData.nearDist);
		
        bool accountedForDoubleIntersect = true;
		
        if (hitData.doubleIntersection)
        {
            accountedForDoubleIntersect = false;
        }
		
        
        float coarseStepSize = volumeThickness * rcp(_COARSE_STEPS);
        float detailStepSize = volumeThickness * rcp(_DETAIL_STEPS);
		
        float2 repeat = ceil(_RenderTextureDimensions.zw * _BLUE_NOISE_TexelSize.xy);
        float blueNoise = _BLUE_NOISE.SampleLevel(altos_point_repeat_sampler, UV * repeat, 0).r;
        rayData.noiseIntensity = _CLOUD_BLUE_NOISE_STRENGTH;
        rayData.noiseAdjustment = coarseStepSize * blueNoise * rayData.noiseIntensity;
		
        sampleDepth += rayData.noiseAdjustment;
		
        rayData.rayDepth = sampleDepth;
		
        bool isFirstSample = true;
        int shortStepCounter = 0;
        
        // Directives
        bool sampleDirectLighting = true;
        bool sampleAmbientLighting = true;
        
        if (_ShadowPass == 1)
        {
            sampleAmbientLighting = false;
            sampleDirectLighting = false;
        }
		
        
        for (int i = 1; i <= int(_DETAIL_STEPS); i++)
        {
            if (rayData.rayDepth > maxDepth)
                break;
			
            r++;
			
            rayData.rayPosition = rayData.rayOrigin + (rayData.rayDirection * rayData.rayDepth);
            float d = distance(materialData.mainCameraOrigin, rayData.rayPosition);
            
            mip = (d > _MIPMAP_STAGES.y) ? 2 : (d > _MIPMAP_STAGES.x) ? 1 : 0;
            
            valueAtPoint = 0;
			
            float height01 = 0;
            float density = 0;
			
            //float cloudinessAtPoint = GetDistantCloudiness(materialData.cloudiness, d, atmosData.distantCoverageDepth, atmosData.cloudFadeDistance, atmosData.distantCoverageAmount);
            float cloudinessAtPoint = 1.0;
            float3 weather = GetWeathermap(materialData, rayData, cloudinessAtPoint, atmosData.cloudFadeDistance);
			
            [branch]
            if (weather.r > EPSILON_LARGE)
            {
                height01 = GetHeight01(rayData.rayPosition, atmosData.atmosThickness, materialData.planetRadius, atmosData.atmosHeight);
                density = GetCloudDensityByHeight(materialData, rayData, height01, weather);
				
                [branch]
                if(isFirstSample)
                {
                    valueAtPoint = GetCloudShapeVolumetric(materialData, rayData, atmosData, weather, density, height01, weather.r, mip, false);
                }
                else
                {
                    valueAtPoint = GetCloudShapeVolumetric(materialData, rayData, atmosData, weather, density, height01, weather.r, mip, true);
                }
                
            }
			
            if (isFirstSample && valueAtPoint > EPSILON_LARGE && _ShadowPass == 0)
            {
                i -= 1;
                rayData.rayDepth -= coarseStepSize;
                isFirstSample = false;
                shortStepCounter = _MAXIMUM_EMPTY_DETAIL_STEPS;
                continue;
            }
			
			
            float stepSize = coarseStepSize;
            if (shortStepCounter > 0)
            {
                stepSize = detailStepSize;
            }
			
			// If the cloud exists at this point, sample the extinction and lighting
            [branch]
            if (valueAtPoint > EPSILON_LARGE)
            {
                
                g++;
                
                if (frontDepth < 0)
                {
                    frontDepth = rayData.rayDepth;
                }
                
                float priorAlpha = alpha;
                float3 sampleExtinction;
                float3 sampleExtinctionInverse;
                float transmittance;

                EvaluateExtinction(materialData.extinction, valueAtPoint, stepSize, sampleExtinction, sampleExtinctionInverse, transmittance, alpha);

                // For Shadows
                sumExtinction += sampleExtinction.r * stepSize;
                extinctionCounter++;
                
                
                // Evaluate Depth
                if (accDepthSamples < EPSILON)
                {
                    accDepthSamples = rayData.rayDepth;
                }
                else
                {
                    float energData = (sampleExtinction.r - (sampleExtinction.r * transmittance)) * sampleExtinctionInverse.r;
                    baseEnergy += energData * priorAlpha;
                    accDepthSamples += (rayData.rayDepth - accDepthSamples) * energData * priorAlpha;
                }

                
				//EvaluateDepth();
                //EvaluateLighting();

                float3 col = 0;
                
                // Direct Lighting
                
                [branch]
                if (sampleDirectLighting)
                {
                    #define _IN_SCATTERING_MIP_OFFSET 2
                    
				    // In-Scattering
                    float inScattering = GetCloudShapeVolumetric(materialData, rayData, atmosData, weather, density, height01, weather.r, mip + _IN_SCATTERING_MIP_OFFSET, false);
                    inScattering *= inScattering;
                    inScattering *= _IN_SCATTERING_MULTIPLIER;
                    inScattering = saturate(inScattering);
                    inScattering = lerp(1.0, inScattering, _IN_SCATTERING_STRENGTH);
					
                    // To do: Consider tinting albedo with in/out/scattering to get fluffy sugar look hehe
                    OSLightingData lightingData = GetLightingDataVolumetric(materialData, rayData, atmosData, mip);
                    float hg = materialData.HG * (1.0 - weather.g);
                    float3 finalLighting = materialData.sunColor * materialData.sunIntensity * ((lightingData.baseLighting * hg) + lightingData.outScatterLighting);
                    finalLighting *= GetRainLightingReduction(weather.g);
                    float3 lightData = (finalLighting + lightingData.additionalLighting) * sampleExtinction * _CLOUD_ALBEDO * inScattering;
                    float3 intScatter = (lightData - (lightData * transmittance)) * sampleExtinctionInverse;
                    col += intScatter * priorAlpha;
                }
				
				//Ambient Lighting
                [branch]
                if (sampleAmbientLighting)
                {
                    #define AMBIENT_HEIGHT_FLOOR 0.1
                    #define AMBIENT_HEIGHT_CEILING 0.7
                    #define AMBIENT_HEIGHT_MIN 0.6
                    #define AMBIENT_HEIGHT_MAX 1.0
                    
                    float ambHeight = os_Remap(AMBIENT_HEIGHT_FLOOR, AMBIENT_HEIGHT_CEILING, AMBIENT_HEIGHT_MIN, AMBIENT_HEIGHT_MAX, height01);
                    float ambDensity = GetAmbientDensity(materialData, rayData, atmosData, weather, cloudinessAtPoint, mip, _CheapAmbientLighting);
					
                    float3 ambientLighting = materialData.ambientColor * materialData.ambientExposure * (ambDensity + ambHeight) * 0.5;
                    ambientLighting *= GetRainLightingReduction(weather.g);
                    float3 ambientData = sampleExtinction * ambientLighting * _CLOUD_ALBEDO;
                    float3 intAmb = (ambientData - (ambientData * transmittance)) * sampleExtinctionInverse;
                    col += intAmb * priorAlpha;
                }

                /*
                One-step fog application:
                Color:
                Multiply .rgb * .a for each fog
                Sum each premultiplied
                Divide by the sum of .a for each fog
                
                Alpha:
                Sum of .a for each fog


                In effect, we are scaling the color of each fog by its strength...

                e.g.,:
                float4 fog1 = float4(0,1,0,1.0);
                float4 fog2 = float4(1,0,0,0.1);

                float4 finalFog = float4(((fog1.rgb * fog1.a) + (fog2.rgb * fog2.a)) / (fog1.a + fog2.a), saturate(fog1.a + fog2.a));

                Expressed another way, you can say: 
                fog1.rgb * fog1.a * rcp(fog1.a + [...fog[n].a...]) + fog2.rgb * fog2.a * rcp(....).
                ...
                summation = fog1.rgb * fog1.a 
                summation += fog2.......
                summation *= rcp(....).

                float4 materialColor = float4(1,1,1,1);
                float4 materialWithFog = lerp(materialColor, finalFog.rgb, finalFog.a);

                This approach breaks when .a for both factors is close to 0 :( (e.g., in cloud flythrough).
                Ah, just need to max(summation, 0);...
                */
                float4 buto = 0;

                // Buto Fog
                #ifdef ALTOS_BUTO_COMPATIBILITY_ENABLED
                    float4 butoFog = ButoFog(materialData.uv, rayData.rayDepth);
                    float3 butoFogData = butoFog.rgb * (sampleExtinction - (sampleExtinction * transmittance)) * sampleExtinctionInverse;
                    float3 integratedButoFog = butoFogData * priorAlpha;
                    buto.rgb = integratedButoFog;
                    buto.a = 1.0 - butoFog.a;
                    col = lerp(col, buto.rgb, buto.a);
                #endif

                // Atmospheric Fog
                float3 fogData = materialData.fogColor * (sampleExtinction - (sampleExtinction * transmittance)) * sampleExtinctionInverse;
                float3 fog = fogData * priorAlpha;
                float atmosphericAttenuation = 1.0 - rcp(exp(rayData.rayDepth * materialData.fogPower));
                col = lerp(col, fog, atmosphericAttenuation);

                cloudColor += col;
            }
			
			#define MINIMUM_IMAGE_ALPHA_FOR_BREAK 1e-5
            if (alpha < MINIMUM_IMAGE_ALPHA_FOR_BREAK)
            {
                alpha = 0.0;
                break;
            }
			
			
            if (shortStepCounter > 0)
            {
                rayData.rayDepth += detailStepSize;
                shortStepCounter -= 1;
            }
            else
            {
                rayData.rayDepth += coarseStepSize;
            }
			
			
			// Handle Double Intersect if needed.
            if (hitData.doubleIntersection && !accountedForDoubleIntersect && rayData.rayDepth > hitData.farDist)
            {
                rayData.rayDepth = hitData.nearDist2;
                accountedForDoubleIntersect = true;
            }
        }
    }
	
    if (doSampleHighAlt && highAltPassId == 1)
    {
        SampleHighAltitudeClouds(rayData, materialData, atmosData, nearHitUpperAtmosphere, alpha, cloudColor);
    }
	
	
#define DEBUG_CLOUDS 0
#if DEBUG_CLOUDS
	cloudColor = float3(0, 0, 0);
	cloudColor.r = float(r) / _CLOUD_STEP_COUNT;
	cloudColor.g = float(g) / _CLOUD_STEP_COUNT;
	cloudColor.b = baseEnergy;
	alpha = 0;
#endif
    
    if (frontDepth < 0)
    {
        frontDepth = _INF;
    }
    
    if (_ShadowPass == 1)
    {
        float g = 0;
        if (extinctionCounter > 0)
        {
            g = sumExtinction / float(extinctionCounter);
			g *= 0.0002;
            g *= _CloudShadowStrength;
        }
        cloudColor = float3(frontDepth, g, accDepthSamples);
        alpha = 0;
    }


    cloudData = float4(cloudColor, 1.0 - alpha);
    
    FragmentOutput output;
    output.color = cloudData;
    return output;
}

#endif
