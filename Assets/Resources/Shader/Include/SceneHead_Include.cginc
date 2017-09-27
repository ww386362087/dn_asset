
#ifndef SCENEHEAD_INCLUDED
#define SCENEHEAD_INCLUDED

#include "UnityCG.cginc"

struct appdata_t
{
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
#if defined(LM)|defined(UV2)
	half2 uv2 : TEXCOORD1;
#endif
#ifdef VC
	fixed4 color : COLOR;
#endif
};

struct v2f
{
	float4 vertex : SV_POSITION;
	half2 texcoord : TEXCOORD0;
#if defined(LM)|defined(UV2)
	half2 uv2 : TEXCOORD1;
#endif
	UNITY_FOG_COORDS(2)
#ifdef WP
	float3 worldPos : TEXCOORD3;
#endif
#ifdef VC
	fixed4 color : COLOR;
#endif
};

#ifdef TERRAIN
struct appterrain
{
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
	float2 texcoord1 : TEXCOORD1;
#ifdef TERRAIN_FIRSTPASS
	float2 texcoord2 : TEXCOORD2;
	float2 texcoord3 : TEXCOORD3;
#endif
};

struct v2fTerrain
{
	half4 vertex : SV_POSITION;
	half2 tc_Control : TEXCOORD0;
#ifdef LM
	half2 uv2 : TEXCOORD1;
#endif
	half2 uv_Splat0 : TEXCOORD2;
	half2 uv_Splat1 : TEXCOORD3;
#ifdef TERRAIN_FIRSTPASS
	half2 uv_Splat2 : TEXCOORD4;
	half2 uv_Splat3 : TEXCOORD5;
#endif
	UNITY_FOG_COORDS(6)
#ifdef CUSTOM_SHADOW_ON
		half4 _WorldPosViewZ : TEXCOORD7;
#endif
};
#endif

#endif //SCENE_INCLUDED