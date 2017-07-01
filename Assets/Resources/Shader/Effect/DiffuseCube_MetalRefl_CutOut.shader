  Shader "Custom/Effect/DiffuseCube_MetalRef_Cutout" {
    Properties { 
		      
      _MainTex ("BaseColor", 2D) = "white" {}
	  _Mask ("R-光滑度  G-金属性", 2D) = "white" {}      
	  _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5 
	  _Cube ("Cubemap", CUBE) = "" {}
	     
	  _CubeColor ("RGB-反射颜色  ", Color) = (1,1,1,0.5)   
	  _RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
	  _LightArgs("x:MainColor Scale y:Light Scale z:反射强度 w: Rim Power",Vector)= (1.0,1.0,0.7,0.55)
	  _UIRimMask("UI Rim Mask",Vector) = (1,1,0,0)
	  //_LightArgs("x:Specular y:Gloss z:Cube Power w: Color Power",Vector) = (1.0,1.0,0.7,0.55)
	      
    }          
	        
	SubShader {     
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
	LOD 400            
	Pass {     
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }             
		Cull Back   
		Lighting On   
		 
        CGPROGRAM  
		//define
		#define MASK
		#define METALREFLCUTOUT
		#define LIGHTON 
		//#define REFLECT
		#define RIMLIGHT
		//#define UIRIM
		#define SHLIGHTON 
		#define VERTEXLIGHTON
		//#define MATCAP
		//#define METALREFL
		#define METALREFLCUBE



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
			 c.a = tex2D(_Mask, i.uv).g;
			return c;
		}
		fixed4 EmissionColor(in v2f i)
		{
			
			return 0;
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
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
	LOD 200            
	Pass {     
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }             
		Cull Back   
		Lighting On   
		   
        CGPROGRAM   
		//define
		#define MASK
		#define METALREFLCUTOUT
		#define LIGHTON  
		#define RIMLIGHT 
		//#define UIRIM  
		#define SHLIGHTON 
		#define VERTEXLIGHTON 
		 
		//head
		#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
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
			 c.a = tex2D(_Mask, i.uv).g;
			return c;
		}
		//include
			#include "../Include/CommonBasic_Include.cginc"

        ENDCG		
	}  
} 

SubShader {     
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"  }
	LOD 100            
	Pass {     
	Name "Basic"
        Tags { "LightMode"="ForwardBase" }             
		Cull Back   
		Lighting On   
		 
        CGPROGRAM  
		//define 
		#define MASK
		#define METALREFLCUTOUT
		#define LIGHTON
		//#define UV2
		//#define REFLECT
		//#define RIMLIGHT
		//#define UIRIM
		//#define VERTEXLIGHTON
		//#define MATCAP
		//#define METALREFL
		//#define METALREFLCUBE
		//#define EMISSION
		//head 
				#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
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
			 c.a = tex2D(_Mask, i.uv).g;
			return c;
		}

		//include
		#include "../Include/CommonBasic_Include.cginc"

        ENDCG		
	}  
} 
 }
