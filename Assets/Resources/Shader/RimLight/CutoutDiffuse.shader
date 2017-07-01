Shader "Custom/RimLight/CutoutDiffuse" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
	_LightArgs("x:MainColor Scale y:Light Scale z:Unused w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
	_UIRimMask("UI Rim Mask",Vector) = (1,1,0,0)
}
SubShader {  
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
	LOD 300            
	Pass {  
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }            
		Cull Back  
		Lighting On  
		
		CGPROGRAM 
		//define
		#define CUTOUT
		#define LIGHTON
		#define RIMLIGHT
		#define UIRIM
		#define VERTEXLIGHTON
		#define SHLIGHTON

		//head
		#include "../Include/CommonHead_Include.cginc"
		//vertex&fragment
		#pragma vertex vert
        #pragma fragment frag 

		sampler2D _MainTex;

		//custom frag fun
		fixed4 BasicColor(in v2f i)
		{
			fixed4 c = tex2D (_MainTex, i.uv);
			return c;
		}
		//include
        #include "UnityCG.cginc"  
        #include "../Include/CommonBasic_Include.cginc"
        ENDCG 
	}  
} 
}
