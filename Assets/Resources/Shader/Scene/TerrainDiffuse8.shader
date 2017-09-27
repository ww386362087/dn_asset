Shader "Custom/Scene/TerrainDiffuse8" 
{
	Properties
	{
		[HideInInspector]_Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector]_Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector]_Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector]_Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector]_Splat0("Layer 0 (R)", 2D) = "white" {}
		// used in fallback on old cards & base map
		[HideInInspector]_MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector]_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry-99" "RenderType" = "Opaque"}
		Pass
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vertT
			#pragma fragment fragT
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vertT
			#pragma fragment fragT
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define LM
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vertT
			#pragma fragment fragT
			#pragma multi_compile_fog
			#pragma multi_compile __ CUSTOM_SHADOW_ON 
			#pragma target 3.0
			#define LM
			#define TERRAIN
			#define TERRAIN_FIRSTPASS
			#include "../Include/SceneHead_Include.cginc"
			#include "../Include/Scene_Include.cginc"
			ENDCG
		}
	}
	Dependency "AddPassShader" = "Custom/Scene/TerrainAddPass"
}
