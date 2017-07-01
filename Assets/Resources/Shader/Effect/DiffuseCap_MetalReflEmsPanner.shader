Shader "Custom/Effect/DiffuseCap_MetalReflEmsPanner"
{
	Properties  
	{ 
	  _MainTex ("BaseColor", 2D) = "white" {}   
	  _Mask ("R-光滑度  G-金属性", 2D) = "white" {}      
	  _MatCap("反射图", 2D) = "white" {}  
	  _CubeColor ("RGB-反射颜色  ", Color) = (1,1,1,0.5)   
	  _FlowTex ("FlowTex", 2D) = "black" {}      
	  _FlowDir("x:U速度 y:V速度  z:流光亮度 ",Vector)= (1.0,1.0,1.0,0)
	  _EmissionColor ("流光颜色  ", Color) = (1,1,1,1)   
      _RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0) 
      _LightArgs("x:MainColor Scale y:Light Scale z:cap scale w: Rim Power",Vector) = (1.0,0.21,1.2,3.0)  
	  _UIRimMask("UI Rim Mask",Vector) = (1,1,0,0) 
	}  
	SubShader  
	{    
		Tags { "RenderType"="Opaque" }    
		LOD 400 
		  
		Pass 
		{
			Name "Basic" 
     Tags { "LightMode"="ForwardBase" }                 
		Cull Back   
		Lighting On

        CGPROGRAM 
		//define 
		#define MASK
		#define LIGHTON  
		#define UV2
		#define RIMLIGHT 
		//#define UIRIM 
		#define SHLIGHTON 
		#define VERTEXLIGHTON
		#define MATCAP 
		#define METALREFL
		#define EMISSION
		//head
		#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
		//fixed4 EmissionColor(in v2f i);
			#include "UnityCG.cginc"
			
		//vertex&fragment
		#pragma vertex vert
        #pragma fragment frag 

		sampler2D _MainTex;
		sampler2D _FlowTex;
		float4 _FlowTex_ST;
		fixed4 _EmissionColor;
		float4 _FlowDir;
		//custom frag fun
		fixed4 BasicColor(in v2f i)
		{
			fixed4 c = tex2D(_MainTex, i.uv);
			c.a = 1;
			return c;
		}
		fixed4 EmissionColor(in v2f i ,fixed4 _BasicColor)
		{
		float2 FlowUV=i.uv1*_FlowTex_ST.xy+_FlowTex_ST.zw+fmod( float2(_Time.x*_FlowDir.x,_Time.x*_FlowDir.y),1);
			fixed4 c = tex2D(_FlowTex, FlowUV);
			fixed4 m = tex2D(_Mask, i.uv);
			return c*m.b*_EmissionColor*_FlowDir.z;
		}
		//include
		#include "../Include/CommonBasic_Include.cginc"
        ENDCG		
	}  


		Pass 
	{
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off
		Offset 1, 1

		CGPROGRAM
		//include
		#include "../Include/Shadow_Include.cginc"
		#pragma vertex vertCast
		#pragma fragment fragCast
		#pragma multi_compile_shadowcaster
		ENDCG 
	}
	}

		SubShader {      
	Tags { "RenderType" = "Opaque"  }  
	LOD 200            
	Pass {     
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }             
		Cull Back   
		Lighting On   
		   
        CGPROGRAM    
		//define
		#define MASK
		#define LIGHTON
		#define UV2 
		//#define REFLECT
		#define RIMLIGHT 
		//#define UIRIM
		#define SHLIGHTON 
		#define VERTEXLIGHTON
		//#define MATCAP
		//#define METALREFL
		//#define METALREFLCUBE
		#define EMISSION
		//head
		#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
		//fixed4 EmissionColor(in v2f i);
			#include "UnityCG.cginc"
			
		
		//vertex&fragment
		#pragma vertex vert
        #pragma fragment frag 

		sampler2D _MainTex;
		sampler2D _FlowTex;
		float4 _FlowTex_ST;
		fixed4 _EmissionColor;
		float4 _FlowDir;
		//custom frag fun
		fixed4 BasicColor(in v2f i) 
		{
			fixed4 c = tex2D(_MainTex, i.uv);
			return c; 
		}
		fixed4 EmissionColor(in v2f i ,fixed4 _BasicColor)
		{
		float2 FlowUV=i.uv1*_FlowTex_ST.xy+_FlowTex_ST.zw+fmod( float2(_Time.x*_FlowDir.x,_Time.x*_FlowDir.y),1);
			fixed4 c = tex2D(_FlowTex, FlowUV);
			fixed4 m = tex2D(_Mask, i.uv);
			return c*m.b*_EmissionColor*_FlowDir.z;
		}
		//include
#include "../Include/CommonBasic_Include.cginc"
        ENDCG		
	}  
} 

	SubShader {     
	Tags { "RenderType" = "Opaque"  }  
	LOD 100            
	Pass {     
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }             
		Cull Back   
		Lighting On   
		   
        CGPROGRAM   
		//define
		#define MASK
		#define LIGHTON
		#define UV2 
		//#define REFLECT 
		//#define RIMLIGHT
		//#define UIRIM
		//#define VERTEXLIGHTON
		//#define MATCAP
		//#define METALREFL
		//#define METALREFLCUBE
		#define EMISSION
		//head
		#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
		//fixed4 EmissionColor(in v2f i);
			#include "UnityCG.cginc"
			
		
		//vertex&fragment
		#pragma vertex vert
        #pragma fragment frag 

		sampler2D _MainTex; 

		fixed4 _EmissionColor;
		float4 _FlowDir;
		//custom frag fun
		fixed4 BasicColor(in v2f i) 
		{
			fixed4 c = tex2D(_MainTex, i.uv);
			return c; 
		}
		fixed4 EmissionColor(in v2f i ,fixed4 _BasicColor)
		{
		fixed4 m = tex2D(_Mask, i.uv);
		//fixed f= saturate(1-(abs(fmod(i.uv.x+fmod(_Time.x*5,1),1)-0.5f))*5)+0.2;
		float a=abs(fmod(i.uv1.x*4-fmod(-_Time.x*_FlowDir.x,1),1));
		float f=1-a*(1-a)*4;
		 
			return m.b*_EmissionColor*f*2;
		}
		//include
#include "../Include/CommonBasic_Include.cginc"
        ENDCG		
	}  
} 
}
