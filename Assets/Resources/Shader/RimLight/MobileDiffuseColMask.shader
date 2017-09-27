Shader "Custom/RimLight/MobileDiffuseColMask" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
		_LightArgs("x:MainColor Scale y:Light Scale z:Unused w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
	}
	SubShader 
	{  
		Tags { "RenderType"="Opaque" }
		LOD 100            
		Pass 
		{
			Tags { "LightMode"="ForwardBase" }
			Cull Back
        
			CGPROGRAM 
			//define
			#define SHLIGHTON
			#define RIMLIGHT
			#define VERTEXLIGHTON
			#pragma multi_compile __ UIRIM
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			sampler2D _Mask;
			fixed4 _Color;

			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = tex2D (_MainTex, i.uv);
				mask = tex2D (_Mask, i.uv).r;
				c.rgb = (1 - mask)*c + mask*_Color*c;
				c.a = 1;
				return c;
			}
			//include 
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG    
		}  
	} 
}
