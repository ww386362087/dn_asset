Shader "Custom/SFX/AlphaBlendEmis" 
{
	Properties 
	{
		_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_EmisColor ("Emission Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Mask0 ("Mask0", 2D) = "white" {}
		_parameter("R(EmisScale) G(NoiseAlpha) B(VC_Alpha) A(Alpha)",vector)=(1,1,1,1)
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off 
		Fog{ Mode Off }

		SubShader 
		{
			Pass 
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _Color;
				fixed4 _EmisColor;
				sampler2D _Mask0;
				half4 _parameter;

				struct appdata_t 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					half4 vertex : POSITION;
					fixed4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};
			
				float4 _MainTex_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{				
					fixed4 Diff = tex2D(_Mask0, i.texcoord);
					fixed4 col = tex2D(_MainTex, i.texcoord)*_Color + Diff * _parameter.r*_EmisColor * 2 * _EmisColor.a;
					col.a=i.color .a*_Color.a;
					return  col;
				}
				ENDCG 
			}
		}	
	}
}
