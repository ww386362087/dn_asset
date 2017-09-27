#ifndef EFFECT_INCLUDED
#define EFFECT_INCLUDED 
	
#include "CommonHead_Include.cginc"

half4 _EffectArgs;

#ifdef MATCAP
sampler2D _MatCap;
#endif//MATCAP

#ifdef METALREFL 
fixed4 _MetalColor;
#endif//METALREFL

#ifdef CUBE
samplerCUBE _Cube;
#endif//CUBE

#ifdef MATCAP
void MatCap(in v2f i, inout fixed4 c)
{
	fixed4 cap = tex2D(_MatCap, i.cap);
	c.rgb *= cap.rgb*_EffectArgs.z;
}
#endif

#ifdef METALREFL
void MetalReflect(in v2f i, in fixed4 mask, inout fixed4 c)
{
	c *= _MetalColor;
}
#endif

#ifdef GLASS
void Glass(in v2f i, in fixed4 mask, inout fixed4 c)
{
	c.a = Luminance(c.rgb) + mask.b;
}
#endif

void Effect(in v2f i, in fixed4 mask, inout fixed4 c)
{
	fixed4 final = fixed4(1, 1, 1, 1);

#ifdef CUBE
	final = texCUBE(_Cube, i.refluv);
#endif

#ifdef METALREFL
	MetalReflect(i, mask, final);
#endif //METALREFL

#ifdef MATCAP
	MatCap(i, final);
#endif //MATCAP

	final *= _EffectArgs.x;

#ifdef MASKTEX
	final *= mask.r;
#endif //MASKTEX

#ifdef GLASS
	Glass(i, mask, final);
	c.a = final.a;
#endif //GLASS

#ifdef CUTOUT
	c.rgb = lerp(c.rgb, final.rgb, mask.r);
#else
	c.rgb = lerp(c.rgb, lerp(final.rgb, final.rgb *c.rgb * 2, mask.g), mask.r);
#endif
}
#endif //EFFECT_INCLUDED