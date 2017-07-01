#ifndef SHADOW_INCLUDED
#define SHADOW_INCLUDED
	
#define SHADOW_COLLECTOR_PASS
#include "UnityCG.cginc"
struct appdata {
	float4 vertex : POSITION;
	half2 texcoord : TEXCOORD0;
};
struct v2fCast { 
	V2F_SHADOW_CASTER;
	float2  uv : TEXCOORD1;
};
struct v2fColl {
	V2F_SHADOW_COLLECTOR;
};

v2fCast vertCast( appdata v )
{
	v2fCast o;
	TRANSFER_SHADOW_CASTER(o)
	o.uv = v.texcoord;
	return o;
}

#ifdef CUTOUT
uniform sampler2D _Mask;
uniform fixed _Cutoff;
#endif

fixed4 fragCast( v2fCast i ) : SV_Target
{
#ifdef CUTOUT
	fixed4 texcol = tex2D( _Mask, i.uv );
	clip( texcol.r- _Cutoff );
#endif
	SHADOW_CASTER_FRAGMENT(i)
}

v2fColl vertColl (appdata v)
{
	v2fColl o;
	TRANSFER_SHADOW_COLLECTOR(o)
	return o;
}

fixed4 fragColl (v2fColl i) : SV_Target
{
	SHADOW_COLLECTOR_FRAGMENT(i)
}
#endif //SHADOW_INCLUDED