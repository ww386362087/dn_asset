Shader "Custom/Effect/CombineTex" {
	Properties{
		_Tex0("Tex0 (RGB)", 2D) = "black" {}
		_Tex1("Tex0 (RGB)", 2D) = "black" {}
		_Tex2("Tex0 (RGB)", 2D) = "black" {}
		_Tex3("Tex0 (RGB)", 2D) = "black" {}
		_ChanelMask0("ChannelMase0", Vector) = (0.0,0.0,0.0,0.0)
		_ChanelMask1("ChannelMase1", Vector) = (0.0,0.0,0.0,0.0)
		_ChanelMask2("ChannelMase2", Vector) = (0.0,0.0,0.0,0.0)
		_ChanelMask3("ChannelMase3", Vector) = (0.0,0.0,0.0,0.0)
	}
		Category{
		Tags{ "RenderType" = "Opaque" }
		Cull Off Lighting Off ZWrite Off Fog{ Mode Off }

		SubShader{
		Pass{

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		sampler2D _Tex0;
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;
		fixed4 _ChanelMask0;
		fixed4 _ChanelMask1;
		fixed4 _ChanelMask2;
		fixed4 _ChanelMask3;
		struct appdata_t {
			half4 vertex : POSITION;
			half2 texcoord : TEXCOORD0;
		};

		struct v2f {
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
			fixed r = dot(tex2D(_Tex0,i.uv),_ChanelMask0);
			fixed g = dot(tex2D(_Tex1, i.uv), _ChanelMask1);
			fixed b = dot(tex2D(_Tex2, i.uv), _ChanelMask2);
			fixed a = dot(tex2D(_Tex3, i.uv), _ChanelMask3);
			return  fixed4(r,g,b,a);
		}
			ENDCG
		}
	}
	}
}
