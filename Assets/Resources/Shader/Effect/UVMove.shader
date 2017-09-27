Shader "Custom/Effect/UVMove" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVLength("UVLength",Float) = 8
		_UVTime("UVTime",Float) = 0.1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		Cull Off
		ZWrite Off
		AlphaTest Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};
	
        	sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _UVLength;
			fixed _UVTime;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				half timeInOneCycle = fmod(_Time.y,_UVLength*_UVTime);
				timeInOneCycle = floor(timeInOneCycle/_UVTime);
				o.texcoord.x += timeInOneCycle*(1/_UVLength);
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				return tex2D(_MainTex, i.texcoord);
			}
			ENDCG
		}
	}
}
