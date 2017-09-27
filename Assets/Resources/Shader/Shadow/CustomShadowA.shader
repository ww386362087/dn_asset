Shader "Custom/Scene/CustomShadowA"
{
	Properties
	{
		[HideInInspector]_Mask("Mask (A)", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Pass
		{
			Name "ShadowCasterA"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask A
			CGPROGRAM
			//include
			#include "../Include/Shadow_Include.cginc"
			#pragma vertex vertCustomCast
			#pragma fragment fragCustomCastA

			ENDCG
		}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" }
		Pass
		{
			Name "CutoutShadowCasterA"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask A
			CGPROGRAM
			//include
			#include "../Include/Shadow_Include.cginc"
			#pragma vertex vertCustomCast
			#pragma fragment fragCustomCastACutout

			ENDCG
		}
	}
}
