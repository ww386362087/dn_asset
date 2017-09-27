Shader "Custom/Effect/ParticleUVanim_Add" {
    Properties {
        _U_Time ("U_Time", Float ) = 1
        _V_Time ("V_Time", Float ) = 0
        _Min_Textures ("Min_Textures", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            Lighting Off
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform half _U_Time;
            uniform half _V_Time;
            uniform sampler2D _Min_Textures; 
			uniform half4 _Min_Textures_ST;

            struct VertexInput {
                float4 vertex : POSITION;
                half2 texcoord0 : TEXCOORD0;
                fixed4 color : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
                fixed4 color : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Min_Textures);
				o.uv0 = o.uv0 + half2(_Time.y*_U_Time, _Time.y*_V_Time);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 color = tex2D(_Min_Textures,i.uv0);
                return fixed4((color.rgb*color.a) * i.color,i.color.a);
            }
            ENDCG
        }
    }
}
