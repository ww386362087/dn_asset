Shader "Custom/Effect/TexScaleAlpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}
Category {
	Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
	Cull off Lighting Off ZWrite Off Fog{ Mode Off }
	ColorMask RGBA
	Blend SrcAlpha OneMinusSrcAlpha
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			struct appdata_t {
				half4 vertex : POSITION;		
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				half4 vertex : SV_POSITION;				
				half2 uv : TEXCOORD0;
			};
			

			v2f vert (appdata_t v)
			{
				v2f o = (v2f)0;

				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{				
				return fixed4(0,0,0,tex2D(_MainTex, i.uv).a);
			}
			ENDCG 
		}
	}	
}
}
