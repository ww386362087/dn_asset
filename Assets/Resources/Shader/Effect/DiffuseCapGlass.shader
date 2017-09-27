Shader "Custom/Effect/DiffuseCapGlass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask ("R-光滑度  G-金属性 B-透明度", 2D) = "white" {}  
		_MatCap("Cap", 2D) = "white" {} 
		_EffectArgs("x:Metal Scale y:Glass Scale z:Cap Scale w:Not Used",Vector) = (1.0,1.0,2.0,0.0)
	}
	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
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
				#define MATCAP
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
				Name "Basic"
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
