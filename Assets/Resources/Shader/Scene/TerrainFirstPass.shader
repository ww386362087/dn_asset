// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Terrain/Diffuse" {
Properties {
	[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	// used in fallback on old cards & base map
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)

	_CustomShadowMapTexture("ShadowMap (RGBA)", 2D) = "white" {}
	_tmp("uvx",Float) = 10
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
	CGPROGRAM
	#pragma surface surf Lambert vertex:vert
	struct Input {
		float2 uv_Control : TEXCOORD0;
		float2 uv_Splat0 : TEXCOORD1;
		float2 uv_Splat1 : TEXCOORD2;
		float2 uv_Splat2 : TEXCOORD3;
		float2 uv_Splat3 : TEXCOORD4;
		//
		float4 _WorldPosViewZ : TEXCOORD5;
		//float3 _ShadowCoord0 : TEXCOORD6;

	};
	void vert(inout appdata_full v, out Input o) {
		float4 wpos = mul(unity_ObjectToWorld, v.vertex);
		o._WorldPosViewZ.xyz = wpos;
		o._WorldPosViewZ.w = -mul(UNITY_MATRIX_MV, v.vertex).z;
		//o._ShadowCoord0 = mul(custom_World2Shadow, wpos).xyz;
	}
	sampler2D _Control;
	sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
	float4x4 custom_World2Shadow;
	UNITY_DECLARE_SHADOWMAP(_CustomShadowMapTexture);
	float _tmp;
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 splat_control = tex2D (_Control, IN.uv_Control);
		fixed3 col;
		col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0).rgb;
		col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1).rgb;
		col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2).rgb;
		col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3).rgb;
		o.Albedo = col;
		//o.Alpha = 0.0;
		float4 shadow = mul(custom_World2Shadow, float4(IN._WorldPosViewZ.xyz,1));
		float4 coord = float4(shadow.xyz/ shadow.w, 1);
		float depth = 1-step(SAMPLE_DEPTH_TEXTURE(_CustomShadowMapTexture, coord.xy), 0.99)*0.5;

		o.Albedo *=  float3(depth, depth, depth);
	}
	ENDCG  
	//Pass
	//{
	//	Name "ShadowCollector"

	//	//Tags { "LightMode" = "ShadowCollector" }
	//	Blend SrcAlpha One
	//	Fog{ Mode Off }
	//	ZWrite On ZTest LEqual

	//	CGPROGRAM
	//	//include
	//	#include "../Include/Shadow_Include.cginc"
	//	#pragma vertex vertColl
	//	#pragma fragment fragColl
	//	//#pragma multi_compile_shadowcollector
	//	ENDCG
	//}
}

Dependency "AddPassShader" = "Hidden/Custom/Splatmap/Lightmap-AddPass"
Dependency "BaseMapShader" = "Diffuse"
// Fallback to Diffuse
Fallback "Diffuse"
}
