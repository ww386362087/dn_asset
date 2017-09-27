Shader "Custom/RimLight/Diffuse" 
{
    Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
		_LightArgs("x:MainColor Scale y:Light Scale z:Add Power w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
		[HideInInspector]_Color("Effect Color", Color) = (1, 1, 1, 0)

		_MainTex1 ("Texture1", 2D) = "white" {}
		_UVScale("UV Scale", Vector) = (-0.5, 0.0, 2, 1.0)
		_UVRange("UV Range", Vector) = (1.0, 1.0, 0.5, 0.0)
		[Toggle(ENABLE_SPLIT)] _Split ("Split?", Float) = 0
		[HideInInspector] _H ("__H", Float) = 0.0
    }
	Category
	{
		Tags{ "RenderType" = "Opaque" }
		SubShader
		{			
			LOD 200
			Pass
			{
				Name "BASIC"
				Tags { "LightMode" = "ForwardBase" }

				CGPROGRAM
				//define
				#define RIMLIGHT
				#define SHLIGHTON
				#define VERTEXLIGHTON
				#define DEFAULTBASECOLOR
				//multicompile
				#pragma multi_compile __ BLINK
				#pragma multi_compile __ UIRIM
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
		SubShader
		{
			LOD 100
			UsePass "Custom/Common/Diffuse/BASIC"
		}
	}
	CustomEditor "CustomShaderGUI"
}
