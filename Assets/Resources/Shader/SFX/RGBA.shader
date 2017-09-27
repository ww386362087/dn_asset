Shader "Custom/SFX/RGBA" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (0.0,0.0,0.0,1.0)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		Cull Back ZWrite Off Fog{ Mode Off }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;
			struct appdata_t 
			{
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				half4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
			};


			v2f vert(appdata_t v)
			{
				v2f o = (v2f)0;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
