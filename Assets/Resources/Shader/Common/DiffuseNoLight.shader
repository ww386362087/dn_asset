Shader "Custom/Common/DiffuseNoLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Back
		Lighting Off
		Fog { Mode Off }

		Pass
		{
		    CGPROGRAM 
			//define
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
