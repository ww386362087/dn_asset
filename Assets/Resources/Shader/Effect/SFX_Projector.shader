Shader "Custom/Effect/SFX_ProjectorTexAnim" {
    Properties {
		_Color1("Color1", Color) = (0.5,0.5,0.5,1)
        _Color1_intensity ("Color1_intensity", Range(0, 5)) = 1
        _Tex1 ("Tex1", 2D) = "white" {}
        _Tex1_Scale ("Tex1_Scale", Range(1, 10)) = 1
        _Color2 ("Color2", Color) = (0.5,0.5,0.5,1)
        _Color2_intensity ("Color2_intensity", Range(0, 5)) = 1
        _Tex2 ("Tex2", 2D) = "white" {}
        _Tex2_Scale ("Tex2_Scale", Range(0.1, 1)) = 1
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _Tex1; 
			uniform half4 _Tex1_ST;
            uniform fixed4 _Color1;
            uniform sampler2D _Tex2; 
			uniform half4 _Tex2_ST;
            uniform fixed4 _Color2;
            uniform fixed _Tex2_Scale;
            uniform fixed _Tex1_Scale;
            uniform fixed _Color1_intensity;
            uniform fixed _Color2_intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
                fixed4 color0 : COLOR0;
				fixed3 color1 : COLOR1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(half2(0.5, 0.5) - _Tex1_Scale*(v.uv + half2(-0.5, -0.5)), _Tex1);
				o.uv1 = TRANSFORM_TEX(half2(0.5, 0.5) - _Tex2_Scale*(v.uv + half2(-0.5, -0.5)), _Tex2);
                o.color0.rgb = v.color.rgb*_Color1.rgb;
				o.color0.a = v.color.a*_Color1.a*_Color2.a;
				o.color1.rgb = v.color.rgb*_Color2.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 _Tex1_var = tex2D(_Tex1, i.uv0);
                fixed4 _Tex2_var = tex2D(_Tex2, i.uv1);
                return fixed4(saturate(1.0+_Color1_intensity*_Tex1_var.rgb*i.color0.rgb*(1.0-_Color2_intensity*_Tex2_var.rgb*i.color1.rgb)),i.color0.a*_Tex1_var.a*_Tex2_var.a);
            }
            ENDCG
        }
    }
}
