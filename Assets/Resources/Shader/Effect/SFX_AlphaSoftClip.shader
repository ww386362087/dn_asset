Shader "Custom/Effect/SFX_AlphaSoftClip" {
    Properties {
        _Color ("Color", Color) = (1,1,1,0.503)
        _Texture ("Texture", 2D) = "white" {}
        _Channel_R ("Channel_R", Float ) = 1
        _Channel_G ("Channel_G", Float ) = 0
        _Channel_B ("Channel_B", Float ) = 0
        _AlphaClips ("AlphaClips", Range(-1, 1)) = 1
        _Strength ("Strength", Range(0, 5)) = 1
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

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _Texture;
			uniform float4 _Texture_ST;
            uniform fixed4 _Color;
            uniform fixed _Channel_R;
            uniform fixed _Channel_G;
            uniform fixed _Channel_B;
            uniform fixed _AlphaClips;
            uniform fixed _Strength;

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
                o.vertexColor = v.vertexColor.rgb*_Color.rgb*v.vertexColor.a;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR 
			{
////// Lighting:
////// Emissive:
                fixed4 _Texture_var = tex2D(_Texture,i.uv0);
                return fixed4(_Texture_var.rgb*i.vertexColor.rgb*_Strength,saturate(dot(_Texture_var.rgb,fixed3(_Channel_R, _Channel_G, _Channel_B))+_AlphaClips));
            }
            ENDCG
        }
    }
}
