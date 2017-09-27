Shader "Custom/Skin/CubeNoMask" 
{
    Properties {
		
      _MainTex ("Texture", 2D) = "white" {}
	  _Cube ("Cubemap", CUBE) = "" {}

	  _SpecColor  ("Specular Color", Color) = (1, 1, 1,1)      
	  _CubeColor ("Cube Color", Color) = (1, 1, 1,1)

	  _LightArgs("x:MainColor Scale y:Light Scale z:Cube Power w: Rim Power",Vector)= (1.0,1.0,0.7,0.55)
	  
    }

	SubShader
	{  
		Tags { "RenderType" = "Opaque"  }
		LOD 100            
		Pass
		{  
			Tags { "LightMode"="ForwardBase" }            
			Cull Back  

			CGPROGRAM 
			//define
			#define RIMLIGHT
			#define CUBE
			#define VERTEXLIGHTON
			#pragma multi_compile __ UIRIM
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			samplerCUBE _Cube;
			fixed4 _CubeColor;
			//custom frag fun
			fixed4 BasicColor(in v2f i, in fixed4 mask)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				half m = (c.r + c.g + c.b) / 3;
				fixed4 reflcol = texCUBE (_Cube, i.refluv)*m;
				c.rgb +=  reflcol.rgb * _LightArgs.z * _CubeColor;
				c.a = 1;
				return c;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"

			ENDCG		
		}  
	} 
}
