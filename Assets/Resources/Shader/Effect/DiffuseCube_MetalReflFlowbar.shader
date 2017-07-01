  Shader "Custom/Effect/DiffuseCube_MetalRefFlowbar" {
    Properties { 
		      
      _MainTex ("BaseColor", 2D) = "white" {}
	  _Mask ("R-光滑度  G-金属性", 2D) = "white" {}      
	  _Cube ("Cubemap", CUBE) = "" {}
	    
	  _CubeColor ("RGB-反射颜色  ", Color) = (1,1,1,0.5)   
	  _EmissionColor ("流光颜色  ", Color) = (1,1,1,1)   
_FlowDir("x:U速度 y:V速度  z:流光亮度 w:流光POW",Vector)= (1.0,1.0,1.0,10)
		
	  _RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
	  _LightArgs("x:MainColor Scale y:Light Scale z:反射强度 w: Rim Power",Vector)= (1.0,1.0,1.0,0.55)
	  _UIRimMask("UI Rim Mask",Vector) = (1,1,0,0)
	  //_LightArgs("x:Specular y:Gloss z:Cube Power w: Color Power",Vector) = (1.0,1.0,0.7,0.55)
	     
    }          
	        
	SubShader {     
	Tags { "RenderType" = "Opaque"  } 
	LOD 400            
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
		#define METALREFLCUBE
		#define EMISSION
		//head
			#include "../Include/CommonHead_Include.cginc"
		//fixed4 BasicColor(in v2f i);
		//fixed4 EmissionColor(in v2f i,fixed4 _BasicColor);
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
		//fixed4 c = tex2D(_MainTex, i.uv);
		//fixed f= saturate(1-(abs(fmod(i.uv.x+fmod(_Time.x*5,1),1)-0.5f))*5)+0.2;
		float a=abs(fmod(i.uv1.x-fmod(-_Time.x*_FlowDir.x,1),1));
		float f=1-a*(1-a)*4;
		float4 x=m.b*f;
		
		
		x=saturate(pow(x,_FlowDir.w))*_EmissionColor;
		// return a;
			return x*_BasicColor*_FlowDir.z;
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
		#define UV2
		#define LIGHTON  
		#define RIMLIGHT 
		//#define UIRIM 
		#define SHLIGHTON 
		#define VERTEXLIGHTON
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
		//fixed4 c = tex2D(_MainTex, i.uv);
		//fixed f= saturate(1-(abs(fmod(i.uv.x+fmod(_Time.x*5,1),1)-0.5f))*5)+0.2;
		float a=abs(fmod(i.uv1.x-fmod(-_Time.x*_FlowDir.x,1),1));
		float f=1-a*(1-a)*4;
		float4 x=m.b*f;
		
		
		x=saturate(pow(x,_FlowDir.w))*_EmissionColor;
		// return a;
			return x*_BasicColor*_FlowDir.z;
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

		#define LIGHTON

		//#define REFLECT
		//#define RIMLIGHT
		//#define UIRIM
		//#define VERTEXLIGHTON
		//#define MATCAP
		//#define METALREFL
		//#define METALREFLCUBE

		//head
				#include "../Include/CommonHead_Include.cginc"
		fixed4 BasicColor(in v2f i);

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

		//include
#include "../Include/CommonBasic_Include.cginc"

        ENDCG		
	}  
} 
 }
