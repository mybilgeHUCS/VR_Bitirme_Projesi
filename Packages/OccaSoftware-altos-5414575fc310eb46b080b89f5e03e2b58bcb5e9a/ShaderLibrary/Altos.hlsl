#ifndef OCCASOFTWARE_ALTOS_EXTERN_INCLUDED
#define OCCASOFTWARE_ALTOS_EXTERN_INCLUDED


////////////////////////////////
// Common fields              //
////////////////////////////////

SamplerState altos_extern_linear_clamp_sampler;
float3 altos_WorldPositionOffset;

////////////////////////////////
// Shader Functions           //
////////////////////////////////

float3 GetWorldPositionOffset()
{
	return altos_WorldPositionOffset;
}

void GetWorldPositionOffset_float(out float3 WorldPositionOffset)
{
	WorldPositionOffset = GetWorldPositionOffset();
}

void GetWorldPositionOffset_half(out float3 WorldPositionOffset)
{
	WorldPositionOffset = half3(GetWorldPositionOffset());
}

////////////////////////////////
// Atmosphere Integration     //
////////////////////////////////

float _BlendStart;
float _Density;
Texture2D _SkyTexture;
float _AltosAtmosphericFogIsEnabled;

float4 AltosFog(float2 ScreenPosition, float Distance)
{
    if (_AltosAtmosphericFogIsEnabled == 0.0)
    {
        return float4(0, 0, 0, 1);
    }
	
	float distanceAdjusted = max(Distance - _BlendStart, 0);
    float d = _Density * distanceAdjusted;
    float ex = saturate(1.0 / exp(d));
	float3 skyColor = _SkyTexture.SampleLevel(altos_extern_linear_clamp_sampler, ScreenPosition, 0).rgb;
	return float4(skyColor.rgb, ex);
}

float3 AltosFogBlend(float2 ScreenPosition, float Distance, float3 InputColor)
{
	float4 fog = AltosFog(ScreenPosition, Distance);
	return lerp(fog.rgb, InputColor.rgb, fog.a);
}

void AltosFog_float(float2 ScreenPosition, float Distance, out float3 Color, out float Density)
{
	Color = 0;
	Density = 1;
	#ifndef SHADERGRAPH_PREVIEW
	float4 fog = AltosFog(ScreenPosition, Distance);
	
	// Note: To blend with target, multiply Density by pre-fog frag color, then add fog color.
	// Alternatively, use AltosFogBlend, which handles this blending for you.
	Color = fog.rgb;
	Density = fog.a;
	#endif
}

void AltosFogBlend_float(float2 ScreenPosition, float Distance, float3 InputColor, out float3 Color)
{
	Color = InputColor;
	#ifndef SHADERGRAPH_PREVIEW
	Color = AltosFogBlend(ScreenPosition, Distance, InputColor);
	#endif
}


///////////////////////////////
// Cloud Integration         //
///////////////////////////////

Texture2D _altos_CloudTexture;
float _AltosCloudsIsEnabled;

float4 AltosClouds(float2 ScreenPosition)
{
	if (_AltosCloudsIsEnabled == 0.0)
    {
        return float4(0, 0, 0, 1);
    }
	
	return _altos_CloudTexture.SampleLevel(altos_extern_linear_clamp_sampler, ScreenPosition, 0).rgba;
}

float3 AltosCloudsBlend(float2 ScreenPosition, float3 InputColor)
{
	float4 clouds = AltosClouds(ScreenPosition);
	return InputColor.rgb * (1.0 - clouds.a) + clouds.rgb;
}


void AltosClouds_float(float2 ScreenPosition, out float3 Color, out float Density)
{
	Color = 0;
	Density = 1;
	#ifndef SHADERGRAPH_PREVIEW
	float4 cloud = AltosClouds(ScreenPosition);
	
	// Note: To blend with target, multiply Density by pre-cloud frag color, then add cloud color.
	// Alternatively, use AltosCloudBlend, which handles this blending for you.
	Color = cloud.rgb;
	Density = cloud.a;
	#endif
}

void AltosCloudsBlend_float(float2 ScreenPosition, float3 InputColor, out float3 Color)
{
	Color = InputColor;
	#ifndef SHADERGRAPH_PREVIEW
	Color = AltosCloudsBlend(ScreenPosition, InputColor);
	#endif
}

#endif