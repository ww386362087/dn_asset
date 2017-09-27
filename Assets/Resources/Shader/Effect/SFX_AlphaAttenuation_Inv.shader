Shader "Custom/Effect/SFX_AlphaAttenuation_Inv" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _Strength ("Strength", Float ) = 1
        _Alpha_attenuation ("Alpha_attenuation", Range(-1, 0)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Blend Zero OneMinusSrcColor
            Cull Off
            ZWrite Off
            Lighting Off
			Fog { Mode Off }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Texture; 
			half4 _Texture_ST;
            fixed4 _Color;
            fixed _Strength;
            fixed _Alpha_attenuation;

            struct VertexInput {
                float4 vertex : POSITION;
                half2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
                fixed3 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Texture);
				o.vertexColor = v.vertexColor*_Color.rgb*v.vertexColor.a*_Color.a;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 _Texture_var = tex2D(_Texture,i.uv0);
				return fixed4(_Texture_var.rgb*_Texture_var.a*i.vertexColor.rgb*_Strength + _Alpha_attenuation, 0);
            }
            ENDCG
        }
    }
}
