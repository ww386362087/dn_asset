#ifndef SHADOW_INCLUDED
#define SHADOW_INCLUDED	

#include "UnityCG.cginc"
struct appdata 
{
	float4 vertex : POSITION;
	half2 texcoord : TEXCOORD0;
};

struct v2fCast 
{ 
	float4 pos : SV_POSITION;
	float2  uv : TEXCOORD1;
	float2  screenuv : TEXCOORD2;
};

sampler2D _ShadowMask;

v2fCast vertCustomCast(appdata v)
{
	v2fCast o;
	o.pos = UnityObjectToClipPos(v.vertex.xyz);
	o.pos = UnityApplyLinearShadowBias(o.pos);
	o.uv = v.texcoord;
	o.pos.xy = o.pos.xy + float2(0.3, 0.0);
	o.screenuv = (o.pos.xy + float2(1, 1))*float2(0.5, 0.5);
	return o;
}

sampler2D _Mask;
fixed4 fragCustomCastR(v2fCast i) : SV_Target
{
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(mask.r, 0, 0, 1);
}
fixed4 fragCustomCastG(v2fCast i) : SV_Target
{
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(0, mask.r, 0, 1);
}

fixed4 fragCustomCastB(v2fCast i) : SV_Target
{
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(0, 0, mask.r, 1);
}

fixed4 fragCustomCastA(v2fCast i) : SV_Target
{
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(0, 0, 0, mask.r);
}

fixed4 fragCustomCastRCutout(v2fCast i) : SV_Target
{
	fixed a = tex2D(_Mask, i.uv).r;
	clip(a - 0.5);
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(mask.r, 0, 0, 1);
}

fixed4 fragCustomCastGCutout(v2fCast i) : SV_Target
{
	fixed a = tex2D(_Mask, i.uv).r;
	clip(a - 0.5);
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(0, mask.r, 0, 1);
}

fixed4 fragCustomCastBCutout(v2fCast i) : SV_Target
{
	fixed a = tex2D(_Mask, i.uv).r;
	clip(a - 0.5);
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
return fixed4(0, 0, mask.r, 1);
}

fixed4 fragCustomCastACutout(v2fCast i) : SV_Target
{
	fixed a = tex2D(_Mask, i.uv).r;
	clip(a - 0.5);
	fixed4 mask = tex2D(_ShadowMask, i.screenuv);
	return fixed4(0, 0, 0, mask.r);
}
#endif //SHADOW_INCLUDED