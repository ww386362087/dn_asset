Shader "Custom/Common/CutoutDiffuseMaskRNoLightSplit" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1,0.3)

		_MainTex1 ("Texture1", 2D) = "white" {}
		_Mask1 ("Mask (A)", 2D) = "white" {}		
		_UVScale("UV Scale", Vector) = (-0.5, 0.0, 2, 1.0)
		_UVRange("UV Range", Vector) = (1.0, 1.0, 0.5, 0.0)
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
			#define ENABLE_SPLIT
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			fixed4 _Color;
			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = Combine2Tex(_MainTex,_MainTex1,i.uv,_UVScale,_UVRange);
				mask = Combine2Tex(_Mask,_Mask1,i.uv,_UVScale,_UVRange);
				c.a = mask.r*_Color.a;
				return c;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}  
	} 
}
