Shader "Custom/Common/DiffuseNoLight" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Back Fog{ Mode Off }	
		Pass
		{
			Name "BASIC"
		    CGPROGRAM 
			//define
			#define NONORMAL
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
