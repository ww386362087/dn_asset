Shader "Custom/Effect/DiffuseCube_Glass"
{
	Properties    
	{     
		_MainTex ("Texture", 2D) = "white" {}
		_Mask ("R-光滑度  G-金属性 B-透明度", 2D) = "white" {}   
		_Cube ("Cubemap", CUBE) = "" {}
      //_RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)   
      _LightArgs("x:-------y:Rim Scale z:cap scale w: Rim Power",Vector) = (1.0,0.21,1.2,3.0) 
	 // _UIRimMask("UI Rim Mask",Vector) = (1,1,0,0)    
	}     
	SubShader      
	{    
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }  
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB  
		//Cull Off  
		Lighting Off      
		ZWrite Off   
		Fog{ Mode Off } 
		LOD 200  

		Pass
		{     
			Name "Basic"          
			CGPROGRAM 
			
			//define 
			#define MASK
			#define LIGHTON  
			//#define RIMLIGHT
			//#define UIRIM
			//#define SHLIGHTON
			#define VERTEXLIGHTON
			//#define MATCAP
			#define METALREFL 
			#define GLASSCUBE
			//head 
			#include "../Include/CommonHead_Include.cginc"
			//fixed4 BasicColor(in v2f i);
			#include "UnityCG.cginc"

			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			
			//custom frag fun
			fixed4 BasicColor(in v2f i)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.a = 1;
				return c;
			}
			
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG		
		}  
	}
			SubShader   
	{    
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }  
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB  
		//Cull Off  
		Lighting Off     
		ZWrite Off   
		Fog{ Mode Off } 
		LOD 100 

		Pass
		{     
			Name "Basic"          
			CGPROGRAM 
			
			//define 
			#define MASK
			//#define LIGHTON  
			//#define RIMLIGHT
			//#define UIRIM
			//#define SHLIGHTON
			//#define VERTEXLIGHTON
			//#define MATCAP
			//#define METALREFL 
			#define GLASS
			#define BLEND
			//head 
			#include "../Include/CommonHead_Include.cginc"
			//fixed4 BasicColor(in v2f i);
			//fixed Alpha(in v2f i);
			#include "UnityCG.cginc"

			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			
			//custom frag fun
			fixed4 BasicColor(in v2f i)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.a = 1;
				return c;
			}
			fixed Alpha(in v2f i)
				{
			
				return tex2D(_Mask, i.uv).b;
				}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG		
		}  
	}
}
