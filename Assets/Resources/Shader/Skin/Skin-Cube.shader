Shader "Custom/Skin/Cube" 
{
    Properties 
	{
		
      _MainTex ("Texture", 2D) = "white" {}
	  _Cube ("Cubemap", CUBE) = "" {}
	  _Mask("Mask", 2D) = "white" {}

	  _SpecColor  ("Specular Color", Color) = (1, 1, 1,1)      
	  _CubeColor ("Cube Color", Color) = (1, 1, 1,1)
	  _RimColor ("Rim Color", Color) = (0.353, 0.353, 0.353,0.0)
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
			sampler2D _Mask;
			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 cubemask = tex2D (_Mask, i.uv).rgb;
				half m = (cubemask.r + cubemask.g + cubemask.b) / 3;
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
