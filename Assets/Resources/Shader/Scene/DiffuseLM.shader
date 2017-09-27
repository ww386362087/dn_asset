Shader "Custom/Scene/DiffuseLM" 
{
	Properties 
	{
		[Header(scene diffuse with lightmap)]
		_MainTex ("Base (RGB) ", 2D) = "white" {}
	}
	SubShader 
	{  
		Tags { "RenderType"="Opaque" }	  
		LOD 100
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			//define
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
			#define LM
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"			
			ENDCG
		}
		
		UsePass "Custom/Common/META"
		UsePass "Custom/Common/CASTSHADOW"
	} 
	FallBack Off
}