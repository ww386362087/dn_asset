#ifndef FLOWTEX_INCLUDED
#define FLOWTEX_INCLUDED 

#ifdef DEFAULTFLOWTEX
float4 _FlowTex_ST;
float2 VertexUV2(in a2v v)
{
	return TRANSFORM_TEX(v.texcoord1, _FlowTex);
}
fixed4 _EmissionColor;
sampler2D _FlowTex;
float4 _FlowDir;
fixed4 EmissionColor(in v2f i, in fixed4 c, in fixed4 mask)
{
	float2 flowUV = i.uv1 + fmod(float2(_Time.x*_FlowDir.x, _Time.x*_FlowDir.y), 1);
	fixed4 flowC = tex2D(_FlowTex, flowUV);
	return flowC*mask.b*_EmissionColor*_FlowDir.z;
}
#endif

#ifdef DEFAULTFLOW
float2 VertexUV2(in a2v v)
{
	return v.texcoord1;
}
fixed4 _EmissionColor;
float4 _FlowDir;
fixed4 EmissionColor(in v2f i, in fixed4 c, in fixed4 mask)
{
	float a = abs(fmod(i.uv1.x * 4 - fmod(-_Time.x*_FlowDir.x, 1), 1));
	float f = 1 - a*(1 - a) * 4;
	return mask.b*_EmissionColor*f * 2;
}
#endif

#ifdef DEFAULTFLOW2
float2 VertexUV2(in a2v v)
{
	return v.texcoord1;
}
fixed4 _EmissionColor;
float4 _FlowDir;
fixed4 EmissionColor(in v2f i, in fixed4 c, in fixed4 mask)
{
	float a = abs(fmod(i.uv1.x - fmod(-_Time.x*_FlowDir.x, 1), 1));
	float f = 1 - a*(1 - a) * 4;
	float4 x = mask.b*f;
	x = saturate(pow(x, _FlowDir.w))*_EmissionColor;
	return x*c*_FlowDir.z;
}
#endif
#endif //FLOWTEX_INCLUDED