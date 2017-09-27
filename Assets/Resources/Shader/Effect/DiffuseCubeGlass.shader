Shader "Custom/Effect/DiffuseCubeGlass"
{
	Properties    
	{     
		_MainTex ("Texture", 2D) = "white" {}
		_Mask ("R-光滑度  G-金属性 B-透明度", 2D) = "white" {}   
		_Cube ("Cubemap", CUBE) = "" {}
		_EffectArgs("x:Metal Scale y,z,w:(metal color)",Vector) = (1.0,1.0,1.2,0.0)
	}    
	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Fog{ Mode Off }
		SubShader
		{			
			LOD 200
			Pass
			{
				CGPROGRAM
				//define 
				#define MASKTEX
				#define GLASS
				#define CUBE
				#define DEFAULTBASECOLOR
				//head 
				#include "../Include/CommonHead_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 
				//include
				#include "../Include/CommonBasic_Include.cginc"
				ENDCG
			}
		}

		SubShader
		{
			LOD 100

			Pass
			{
				CGPROGRAM

				//define 
				#define MASKTEX
				#define GLASS
				#define DEFAULTBASECOLOR
				//head 
				#include "../Include/CommonHead_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 

				//include
				#include "../Include/CommonBasic_Include.cginc"
				ENDCG
			}
		}
	}
}
