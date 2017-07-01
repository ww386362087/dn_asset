#ifndef SKINBLEND_INCLUDED
#define SKINBLEND_INCLUDED
	
inline fixed MaskUV(half2 uv,inout fixed4 uvMask)
{
	uvMask.x = uvMask.x + 1.0;
	uvMask.z = uvMask.z + 1.0;	
	half2 inside = step(uvMask.xy, uv.xy) * step(uv.xy, uvMask.zw);
	return inside.x * inside.y;
}
inline void SkinUVMask(inout v2f o)
{
	fixed4 uvMask = fixed4(0.0,0.0,0.0,0.0);		
	uvMask.xy = fixed2(-1.0,0.0);
	uvMask.zw = fixed2(0.0,1.0);
	o.mask0.x = MaskUV(o.uv,uvMask);
	o.mask0.y = MaskUV(o.uv,uvMask);
#ifndef MASK2
	o.mask0.z = MaskUV(o.uv,uvMask);
	o.mask0.w = MaskUV(o.uv,uvMask);
#endif //MASK2
#ifdef SKINTEX8
	o.mask1.x = MaskUV(o.uv,uvMask);
	o.mask1.y = MaskUV(o.uv,uvMask);
	o.mask1.z = MaskUV(o.uv,uvMask);
	o.mask1.w = MaskUV(o.uv,uvMask);
#endif//SKINTEX8
}

#ifdef SKINTEX
sampler2D _Tex0;
sampler2D _Tex1;
sampler2D _Tex2;
sampler2D _Tex3;
fixed4 _HairColor;
#ifdef SKINTEX8
sampler2D _Tex4;
sampler2D _Tex5;
sampler2D _Tex6;
sampler2D _Tex7;
#endif	//SKINTEX8
#endif	//SKINTEX

inline fixed4 BlendColor(in v2f i)
{
	fixed4 c = fixed4(0,0,0,0);
#ifdef SKINTEX
	half2 uvOffset = float2(0,0);
	c = tex2D(_Tex0, i.uv-uvOffset)*i.mask0.x;
	uvOffset.x += 1;
	c+= tex2D(_Tex1, i.uv-uvOffset)*_HairColor*i.mask0.y;
	uvOffset.x += 1;
	c+= tex2D(_Tex2, i.uv-uvOffset)*i.mask0.z;
	uvOffset.x += 1;
	c+= tex2D(_Tex3, i.uv-uvOffset)*i.mask0.w;
#ifdef SKINTEX8	
	uvOffset.x += 1;
	c+= tex2D(_Tex4, i.uv-uvOffset)*i.mask1.x;
	uvOffset.x += 1;
	c+= tex2D(_Tex5, i.uv-uvOffset)*i.mask1.y;
	uvOffset.x += 1;
	c+= tex2D(_Tex6, i.uv-uvOffset)*i.mask1.z;
	uvOffset.x += 1;
	c+= tex2D(_Tex7, i.uv-uvOffset)*i.mask1.w;					
#endif
#endif	//SKINTEX
	return c;
}
#endif //SKINBLEND_INCLUDED