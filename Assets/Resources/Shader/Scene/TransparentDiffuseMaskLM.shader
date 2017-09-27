Shader "Custom/Scene/TransparentDiffuseMaskLM" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  }
		LOD 100 
	
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			//define
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
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog 
			#define LM
			#define MASKTEX
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}	        


		Pass 
		{  
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog 
			#define LM
			#define MASKTEX
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		UsePass "Custom/Common/META"
		UsePass "Custom/Common/CASTSHADOWCUTOUT"
	} 
}