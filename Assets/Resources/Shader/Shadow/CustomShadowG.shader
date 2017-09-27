Shader "Custom/Scene/CustomShadowG"
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
			Name "ShadowCasterR"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask G
			CGPROGRAM
			//include
			#include "../Include/Shadow_Include.cginc"
			#pragma vertex vertCustomCast
			#pragma fragment fragCustomCastG

			ENDCG
		}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" }
		Pass
		{
			Name "CutoutShadowCasterR"

			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			ColorMask G
			CGPROGRAM
			//include
			#include "../Include/Shadow_Include.cginc"
			#pragma vertex vertCustomCast
			#pragma fragment fragCustomCastGCutout

			ENDCG
		}
	}
}
