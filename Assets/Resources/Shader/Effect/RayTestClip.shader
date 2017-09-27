Shader "Completed works/RayTestClip" {
    Properties {
        _E01 ("E01", 2D) = "white" {}
        _E01_Color ("E01_Color", Color) = (1,1,1,1)
        _DisE01 ("DisE01", 2D) = "white" {}
        _DisE01_value ("DisE01_value", Float ) = 0
        _DisE01_Vpan_Speed ("DisE01_Vpan_Speed", Float ) = 1
        _E02 ("E02", 2D) = "black" {}
        _E02_Bright ("E02_Bright", Float ) = 1
        _E02_Upan_Speed ("E02_Upan_Speed", Float ) = 1
        _Alpha01 ("Alpha01", 2D) = "white" {}
        _Alpha01_Bright ("Alpha01_Bright", Float ) = 1
        _Alpha01_Vpan_Speed ("Alpha01_Vpan_Speed", Float ) = 0
    }
    SubShader {
        Tags {
            "Queue"="Transparent+1"
            "RenderType"="Transparent"
        }
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _DisE01; 
			uniform half4 _DisE01_ST;
            uniform sampler2D _E01; 
			uniform half4 _E01_ST;
            uniform sampler2D _E02; 
			uniform half4 _E02_ST;
			uniform sampler2D _Alpha01; 
			uniform half4 _Alpha01_ST;

			uniform fixed4 _E01_Color;


            uniform half _DisE01_Vpan_Speed;
            uniform half _DisE01_value;
            
            uniform half _E02_Upan_Speed;
            uniform half _E02_Bright;
            uniform half _Alpha01_Vpan_Speed;
            uniform half _Alpha01_Bright;

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

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:

                half2 uv0 = (i.uv0+(_Time.y*_DisE01_Vpan_Speed)*half2(0,1));
                fixed4 _DisE01_var = tex2D(_DisE01,TRANSFORM_TEX(uv0, _DisE01));

                half2 uv1 = ((_DisE01_value*_DisE01_var.rg)+i.uv0);
                fixed4 _E01_var = tex2D(_E01,TRANSFORM_TEX(uv1, _E01));

                half2 uv2 = (i.uv0+(i.vertexColor.a*_E02_Upan_Speed)*half2(1,0));
                fixed4 _E02_var = tex2D(_E02,TRANSFORM_TEX(uv2, _E02));

                fixed3 emissive = (((_E01_Color.rgb*_E01_var.rgb)*(_E02_Bright*_E02_var.rgb))*i.vertexColor.rgb);

                half2 uv3 = (i.uv0+(_Time.y*_Alpha01_Vpan_Speed)*half2(0,1));
                fixed3 _Alpha01_var = tex2D(_Alpha01,TRANSFORM_TEX(uv3, _Alpha01));

                return fixed4(emissive,(i.vertexColor.a*((_E01_var.b*(_E02_var.r*_Alpha01_var.r))*_Alpha01_Bright)));
            }
            ENDCG
        }
    }
    FallBack "Custom/Particles/Additive"
}
