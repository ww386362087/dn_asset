Shader "Custom/Common/CutoutDiffuseMaskRNoLight" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1,0.3)
	}
	SubShader 
	{  
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
		LOD 100            
		Pass 
		{   
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			CGPROGRAM 
			//define
			#define CUTOUT
			#define MASKTEX
			#define NONORMAL
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			fixed4 _Color;
			//custom frag fun
			fixed4 BasicColor(in v2f i, in fixed4 mask)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.a = mask.r*_Color.a;
				return c;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}  
	} 
}
