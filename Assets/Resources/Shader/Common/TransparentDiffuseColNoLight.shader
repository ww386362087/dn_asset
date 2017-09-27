Shader "Custom/Common/TransparentDiffuseColNoLight" 
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
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
			//head
			#include "../Include/CommonHead_Include.cginc"
			//vertex&fragment
			#pragma vertex vert
			#pragma fragment frag 

			sampler2D _MainTex;
			fixed4 _Color;
			//custom frag fun
			fixed4 BasicColor(in v2f i, inout fixed4 mask)
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.a *= _Color.a;
				return c;
			}
			//include
			#include "../Include/CommonBasic_Include.cginc"
			ENDCG
		}
	}
}
