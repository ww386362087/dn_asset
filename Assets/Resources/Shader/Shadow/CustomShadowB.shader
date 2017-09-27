Shader "Custom/Scene/CustomShadowB"
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
			Name "ShadowCasterB"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask B
			CGPROGRAM
			//include
		#include "../Include/Shadow_Include.cginc"
		#pragma vertex vertCustomCast
		#pragma fragment fragCustomCastB

			ENDCG
		}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" }
		Pass
		{
			Name "CutoutShadowCasterB"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask B
			CGPROGRAM
			//include
			#include "../Include/Shadow_Include.cginc"
			#pragma vertex vertCustomCast
			#pragma fragment fragCustomCastBCutout

			ENDCG
		}
	}
}
