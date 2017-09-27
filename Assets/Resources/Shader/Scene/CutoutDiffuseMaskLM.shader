//Scene cutout lightmap
Shader "Custom/Scene/CutoutDiffuseMaskLM" 
{
	Properties 
	{
		//[HideInInspector]_Color("Main Color",Color)=(1,1,1,1)
		_MainTex ("Base (RGB) ", 2D) = "white" {}
		_Mask ("Mask (R)", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }	  
		LOD 100
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			//define
			#define CUTOUT
			#define MASKTEX
			#define SHLIGHTON
			#define LAMBERT
			#define DEFAULTBASECOLOR
			#define ORIGINAL_LIGHT
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG 
		}

		Pass 
		{
			//Pc
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#define CUTOUT
			#define MASKTEX
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}

		Pass 
		{  
			//Moblie
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog 
			#define CUTOUT
			#define MASKTEX
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
		
		UsePass "Custom/Common/META"
		UsePass "Custom/Common/CASTSHADOWCUTOUT"
	} 
	FallBack Off
}