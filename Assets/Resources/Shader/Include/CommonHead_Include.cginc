#ifndef COMMONHEAD_INCLUDED
#define COMMONHEAD_INCLUDED
#include "UnityCG.cginc" 
struct a2v 
{  
	float4 vertex : POSITION; 
#ifndef NONORMAL	
	float3 normal : NORMAL;	
#endif//NONORMAL
	float2 texcoord : TEXCOORD0;
#ifdef UV2
	float2 texcoord1 : TEXCOORD1;
#endif//UV2
}; 


#if defined(RIMLIGHT)|defined(BLINNPHONG)|defined(GLASS)
	#define VIEWDIR
#else
	#undef VIEWDIR
#endif

#if defined(CUBE)
	#define REFLECTUV
#else
	#undef REFLECTUV
#endif

#if defined(VERTEXLIGHTON)|defined(LAMBERT)|defined(BLINNPHONG)
	#define LIGHTON
#else
	#undef LIGHTON
#endif

struct v2f 
{  
	float4 pos : SV_POSITION;  	
#ifndef NONORMAL
	half3 normal: NORMAL;
#endif//NONORMAL		  
	half2 uv : TEXCOORD0;
#ifdef UV2
	half2 uv1 : TEXCOORD1;
#endif//UV2
 
#ifdef LIGHTON
	fixed3 vertexLighting : TEXCOORD2;
#endif//VERTEXLIGHTON

#ifdef VIEWDIR
		half3 viewDir: TEXCOORD3;
#ifdef REFLECTUV
		half3 refluv : TEXCOORD4;
#endif //REFLECTUV

#endif//VIEWDIR

#ifdef SKINTEX
fixed4 mask0 : TEXCOORD5;
#ifdef SKINTEX8
fixed4 mask1 : TEXCOORD6;
#endif//SKINTEX8
#endif//SKINTEX

#ifndef NONORMAL
	#ifdef MATCAP
		half2 cap : TEXCOORD7;
	#endif
#endif
}; 
#if defined(LIGHTON)|defined(RIMLIGHT)|defined(BLINK)
half4 _LightArgs;
#endif

half InRange(half2 uv, half4 ranges)
{
	half2 value = step(uv, ranges.xy) * step(ranges.zw, uv);
	return value.x * value.y;
}

fixed4 Combine2Tex(sampler2D tex0,sampler2D tex1,half2 uv,half4 uvScale,half4 uvRange)
{
	half2 uv0 = uv*uvScale.zw;
	half2 uv1 = (uv + uvScale.xy)*uvScale.zw;
	half maskY = InRange(uv, uvRange);
	fixed4 col1 = tex2D(tex1, uv1)*maskY;
	half maskX = 1 - maskY;
	fixed4 col0 = tex2D(tex0, uv0)*maskX;
	return col0+col1;
}

fixed4 Combine2Mask(sampler2D tex0,sampler2D tex1,half2 uv,half4 uvScale,half4 uvRange)
{
	half2 uv0 = uv*uvScale.zw;
	half2 uv1 = (uv + uvScale.xy)*uvScale.zw;
	half maskY = InRange(uv, uvRange);
	fixed4 col1 = tex2D(tex1, uv1)*maskY;
	half maskX = 1 - maskY;
	fixed4 col0 = tex2D(tex0, uv0)*maskX;
	return col0+col1;
}

#ifdef ENABLE_SPLIT
sampler2D _MainTex1;
fixed4 _UVScale;
fixed4 _UVRange;
#endif

#ifdef MASKTEX
sampler2D _Mask;
#ifdef ENABLE_SPLIT
sampler2D _Mask1;
#endif
#endif
#endif //COMMONHEAD_INCLUDED