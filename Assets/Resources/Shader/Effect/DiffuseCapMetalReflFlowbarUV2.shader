Shader "Custom/Effect/DiffuseCapMetalRefFlowbarUV2"
{
	Properties  
	{ 
		_MainTex ("BaseColor", 2D) = "white" {}   
		_Mask ("R-光滑度  G-金属性", 2D) = "white" {}      
		_MatCap("反射图", 2D) = "white" {}  
		_MetalColor("RGB-反射颜色  ", Color) = (1,1,1,0.5)
		_EmissionColor ("流光颜色  ", Color) = (1,1,1,1)
		_FlowDir("x:U速度 y:V速度  z:流光亮度 w:流光POW",Vector)= (1.0,1.0,1.0,10)
		_RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0) 
		_LightArgs("x:MainColor Scale y:Light Scale z:not used w: Rim Power",Vector) = (1.0,0.21,1.2,3.0)  
		_EffectArgs("x:Metal Scale y:Glass Scale z:Cap Scale w:Not Used",Vector) = (1.0,1.0,1.2,0.0)
	}
	Category
	{
		Tags{ "RenderType" = "Opaque" }
		SubShader
		{
			LOD 400

			Pass
			{
				Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
				//define 
				#define MASKTEX
				#define UV2 
				#define RIMLIGHT
				#define VERTEXLIGHTON
				#define MATCAP 
				#define METALREFL
				#define EMISSION
				#define DEFAULTFLOW2
				#define DEFAULTBASECOLOR
				#pragma multi_compile __ UIRIM
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/FlowTex_Include.cginc"
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
			LOD 200
			Pass 
			{
				Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
				//define
				#define MASKTEX
				#define UV2
				#define RIMLIGHT 
				#define VERTEXLIGHTON
				#define EMISSION
				#define DEFAULTFLOW2
				#define DEFAULTBASECOLOR
				#pragma multi_compile __ UIRIM
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/FlowTex_Include.cginc"
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
				Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
				//define
				#define DEFAULTFLOW2
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
