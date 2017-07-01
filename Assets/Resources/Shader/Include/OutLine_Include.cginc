#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED
	
struct a2v {  
	half4 vertex : POSITION; 	 
	half2 texcoord : TEXCOORD0;
#ifdef RIMON
	half3 normal : NORMAL; 
#endif
};  

struct v2f {
	half4 vertex : SV_POSITION;	
	half2 uv : TEXCOORD0;
	fixed4 color : COLOR;
#ifdef RIMON
	half3 viewDir: TEXCOORD1;
	half3 normal : TEXCOORD2;
#endif
	
};
fixed4 _OutLineColor;
v2f vert(a2v v) {  
	v2f o = (v2f)0;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);	
	o.uv = v.texcoord;
	half rim = 1.0f;
#ifdef RIMON
	half3 normal = mul((float3x3)_Object2World, SCALED_NORMAL);
	half3 viewDir = WorldSpaceViewDir( v.vertex );	
	rim = 1.0 - saturate(dot (normalize(viewDir), normal));
#endif	
	o.color = _OutLineColor*rim;
	return o;
}
fixed4 frag (v2f i) : SV_Target
{
	return i.color;
}
#endif //OUTLINE_INCLUDED