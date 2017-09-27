Shader "Custom/Common/Diffuse" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightArgs("x:MainColor Scale y:Light Scale z:Add Power w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
		[HideInInspector]_Color("Effect Color", Color) = (1, 1, 1, 0)

		_MainTex1 ("Texture1", 2D) = "white" {}
		_UVScale("UV Scale", Vector) = (-0.5, 0.0, 2, 1.0)
		_UVRange("UV Range", Vector) = (1.0, 1.0, 0.5, 0.0)
		[Toggle(ENABLE_SPLIT)] _Split ("Split?", Float) = 0
		[HideInInspector] _H ("__H", Float) = 0.0
	}
	SubShader 
	{  
		Tags { "RenderType"="Opaque" }
		LOD 100            
		Pass 
		{
			Name "BASIC"
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM 
			//define
			#define SHLIGHTON
			#define VERTEXLIGHTON
			#define DEFAULTBASECOLOR
			#pragma multi_compile __ BLINK
			#pragma shader_feature ENABLE_SPLIT
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag
			//include
			fixed4 _Color;
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}
	}
	CustomEditor "CustomShaderGUI"
}
