Shader "Custom/SFX/AlphaBlend2MaskPanel"
{
	Properties
	{
		_Mask0 ("Mask0", 2D) = "white" {}
		_Mask1 ("Mask1", 2D) = "white" {}
		_Mask0_intensity("Texture Intensity",Range(0,5))=3
		_Mask1_intensity("Mask Intensity",Range(0,5))=1
		_Texture_color("Texture Color",color)=(1,1,1,1)
		_Mask0_speed_X("Texture_speed_X",float)=0
		_Mask0_speed_Y("Texture_speed_Y",float)=0
		_Mask1_speed_X("Mask_speed_X",float)=0
		_Mask1_speed_Y("Mask_speed_Y",float)=3


	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite Off Fog{ Mode Off }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;				
				fixed4 color : TEXCOORD2;
			};

			sampler2D _Mask0;
			float4 _Mask0_ST;
			sampler2D _Mask1;
			float4 _Mask1_ST;
			float _Mask0_speed_X;
			float _Mask0_speed_Y;
			float _Mask1_speed_X;
			float _Mask1_speed_Y;
			fixed4 _Texture_color;
			float _Mask0_intensity;
			float _Mask1_intensity;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float2 _uv0_offset=float2(_Mask0_speed_X,_Mask0_speed_Y)*_Time.x;
				float2 _uv1_offset=float2(_Mask1_speed_X,_Mask1_speed_Y)*_Time.x;
				o.uv0 = TRANSFORM_TEX(v.uv, _Mask0)+_uv0_offset;
				o.uv1 = TRANSFORM_TEX(v.uv, _Mask1)+_uv1_offset;
				o.color=v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Texture_color*i.color;
				col.w = (saturate( tex2D(_Mask0, i.uv0).r*_Mask0_intensity)*saturate( tex2D(_Mask1, i.uv1).r*_Mask1_intensity)*i.color.a)*_Texture_color.a;

				return col;
			}
			ENDCG
		}
	}
}
