Shader "Custom/SFX/AlphaBlendNoVC"
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite Off Fog{ Mode Off }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			struct appdata_t 
			{
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			float4 _MainTex_ST;
			sampler2D _MainTex;
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return  tex2D(_MainTex, i.texcoord);
			}
			ENDCG
		}
	}
}
