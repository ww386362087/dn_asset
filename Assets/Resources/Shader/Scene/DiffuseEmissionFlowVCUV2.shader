Shader "Custom/Scene/DiffuseEmissionFlowVCUV2" 
{
	Properties 
	{
		_Color ("Main Color(RGB)   EmiAreaLumen(A)", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Illum ("Illumin (R)", 2D) = "white" {}
		_EmissionColor ("(RGB)FlowLightColor  (A)ColorIntensity", Color) = (1,1,1,0.5)   
		_FlowDir("x:U Speed y:V Speed  z:FlowLightInte w:FlowLightPow",Vector)= (1.0,1.0,1.0,10)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UV2
			#define VC
			#define EMISSION
			#pragma multi_compile_fog
			#include "../Include/SceneHead_Include.cginc"

			sampler2D _Illum;
			fixed4 _Color;
			fixed4 _EmissionColor;
			float4 _FlowDir;

			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				fixed3 tex = col.rgb*_Color.rgb*_Color.rgb*_Color.rgb;				
				fixed4 illum = tex2D(_Illum, i.texcoord);
				float a = abs(fmod(i.uv2.x-fmod(-_Time.x*_FlowDir.x,1),1));
				float f=1-a*(1-a)*4;
				tex.rgb = tex.rgb*min(1,1-illum.x+_Color.a);
				tex.rgb += saturate(pow(f,_FlowDir.w))*_EmissionColor*illum.x*i.color.rgb*_EmissionColor.a*2;
				return tex;
			}
			#include "../Include/Scene_Include.cginc"
			ENDCG 
		}		
		UsePass "Custom/Common/META"
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100		
		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define VC
			#define EMISSION
			#include "../Include/SceneHead_Include.cginc"

			fixed3 EmissionColor(v2f i,fixed4 col,fixed4 mask)
			{
				return col.rgb*i.color.rgb*2;
			}
			#include "../Include/Scene_Include.cginc"	
			ENDCG
		}
		
		UsePass "Custom/Common/META"
	}	
}
