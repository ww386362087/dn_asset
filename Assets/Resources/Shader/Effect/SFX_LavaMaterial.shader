Shader "Custom/Effect/SFX_LavaMaterial" {
    Properties {
        _TintColor ("MainColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Channel_Str ("Channel_Str", Float ) = 1
        _iMin ("iMin", Range(0, 1)) = 0.5277882
        _iMax ("iMax", Range(0, 1)) = 1
        _oMin ("oMin", Range(0, 1)) = 0.6345084
        _oMax ("oMax", Range(0, 1)) = 0.4097342
        _SFX_Tex ("SFX_Tex", 2D) = "white" {}
        _SFX_Str ("SFX_Str", Float ) = 1
        _SFX_Vspeed ("SFX_Vspeed", Float ) = 0
        _SFX_Uspeed ("SFX_Uspeed", Float ) = 0
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _SFX_Tex;
			uniform float4 _SFX_Tex_ST;
            uniform float _SFX_Str;
            uniform float _iMin;
            uniform float _iMax;
            uniform float _oMin;
            uniform float _oMax;
            uniform float _SFX_Vspeed;
            uniform float _SFX_Uspeed;
            uniform float _Channel_Str;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
                fixed4 color : COLOR;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				o.uv = TRANSFORM_TEX(v.texcoord0, _MainTex);
				o.uv1 = lerp(float2(v.texcoord0.x+ _Time.y*_SFX_Uspeed, v.texcoord0.y), float2(v.texcoord0.x, v.texcoord0.y+ _Time.y*_SFX_Vspeed), 0.5);
				o.uv1 = TRANSFORM_TEX(o.uv1, _SFX_Tex);
                o.color.rgb = v.vertexColor*_TintColor.rgb;
				o.color.a = (_oMax - _oMin) / (_iMax - _iMin);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _MainTex_var = tex2D(_MainTex,i.uv);

                float powerR = pow((_MainTex_var.r*1.6-0.1),_Channel_Str);

				fixed4 _SFX_Tex_var = tex2D(_SFX_Tex, i.uv1);
				fixed3 emissive = lerp( (_oMin + (_MainTex_var.rgb*i.color.rgb*2.0 - _iMin) * i.color.a), _SFX_Tex_var.rgb*_SFX_Str, powerR.r );
                return fixed4(emissive,1);
            }
            ENDCG
        }
    }
}
