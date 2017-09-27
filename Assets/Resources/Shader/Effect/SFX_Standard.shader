Shader "Custom/Effect/SFX_Standard" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_BelndColor("BelndColor", Color) = (0,0,0,1)
		_Texture("Texture", 2D) = "white" {}
		_Tex_intensity("Tex_intensity", Float) = 2
		_GetChanel("x: R y: G z: B w: A", Vector) = (0,0,0,1)

		_Edge_Attenuation("Edge_Attenuation ", Range(-2, 0)) = 0
		_ChanelClip("ChanelClip", Range(-1.5, 0)) = 0

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 1
	}
	SubShader{
		Tags{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
	Pass{
			Name "FORWARD"
			Blend[_SrcBlend][_DstBlend]
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _Texture;
			uniform float4 _Texture_ST;
			uniform float4 _Color;
			uniform float _Tex_intensity;
			fixed4 _GetChanel;

			uniform float _ChanelClip;

			uniform float _Edge_Attenuation;
			uniform float4 _BelndColor;

			struct VertexInput {
				float4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
			};
			struct VertexOutput {
				float4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
			};
			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0 = TRANSFORM_TEX(v.texcoord0,_Texture);
				o.vertexColor.rgb = v.vertexColor.rgb*_Color.rgb*_BelndColor.rgb;
				o.vertexColor.a = v.vertexColor.a*_Color.a;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			fixed4 frag(VertexOutput i) : COLOR{

				fixed4 _Texture_var = tex2D(_Texture,i.uv0);
				fixed srcAlpha = dot(_Texture_var,_GetChanel) + _ChanelClip;
				fixed cutout = saturate(sign(srcAlpha));
				fixed alpha = i.vertexColor.a*srcAlpha*cutout;
				fixed3 emissive = (1.0 - _BelndColor.rgb + _Texture_var.rgb*i.vertexColor.rgb*_Tex_intensity*alpha) + _Edge_Attenuation;
				return fixed4(emissive,alpha);
			}
			ENDCG
		}
	}
}
