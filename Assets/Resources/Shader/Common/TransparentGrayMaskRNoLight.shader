Shader "Custom/Common/TransparentGrayMaskRNoLight" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Pass
		{
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			//define
			#define NONORMAL
			#define MASKTEX
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			//custom frag fun
			fixed4 BasicColor(in v2f i, in fixed4 mask)
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed grey = Luminance(col.rgb);
				col.rgb = fixed3(grey, grey, grey);
				col.a = mask.r + 0.01;
				return col;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}
	}	
}
