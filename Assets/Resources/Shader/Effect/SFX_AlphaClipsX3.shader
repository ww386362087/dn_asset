Shader "Completed works/SFX_AlphaClipsX3" {
    Properties {
        _Color ("Color", Color) = (1,1,1,0.503)
        _Texture ("T1", 2D) = "black" {}
		_Color2 ("Color", Color) = (1,1,1,0.503)
		_Texture2 ("T2", 2D) = "black" {}
		_Color3 ("Color", Color) = (1,1,1,0.503)
		_Texture3 ("T3", 2D) = "black" {}
        _Tex1Channel ("T1 Channel", vector ) = (1,0,0,1)
        _Tex2Channel ("T2 Channel", vector ) = (1,0,0,1)
        _Tex3Channel ("T3 Channel", vector ) = (1,0,0,1)
        _AlphaClips ("AlphaClips (X:T1  Y:T2  Z:T3)", vector) =  (0,0,0,0)
        _Strength ("Strength (X:T1  Y:T2  Z:T3)",vector) =  (1,1,1,1)


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
			Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            uniform sampler2D _Texture; 
			uniform half4 _Texture_ST;
			uniform sampler2D _Texture2; 
			uniform half4 _Texture2_ST;
			uniform sampler2D _Texture3; 
			uniform half4 _Texture3_ST;
            uniform fixed4 _Color;
			uniform fixed4 _Color2;
			uniform fixed4 _Color3;
            uniform half4 _Tex1Channel;
            uniform half4 _Tex2Channel;
            uniform half4 _Tex3Channel;
            uniform half4 _AlphaClips;
            uniform half4 _Strength;


            struct VertexInput {
                float4 vertex : POSITION;
                half2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				half2 uv3 : TEXCOORD2;
                fixed3 vertexColor : COLOR;
				fixed3 vertexColor2 : TEXCOORD3;
				fixed3 vertexColor3 : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Texture);
				o.uv2 = TRANSFORM_TEX(v.texcoord0, _Texture2);
				o.uv3 = TRANSFORM_TEX(v.texcoord0, _Texture3);
                o.vertexColor = v.vertexColor.rgb*_Color.rgb*v.vertexColor.a;
				o.vertexColor2 = v.vertexColor.rgb*_Color2.rgb*v.vertexColor.a;
				o.vertexColor3 = v.vertexColor.rgb*_Color3.rgb*v.vertexColor.a;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {

                fixed4 _Texture_var = tex2D(_Texture,i.uv0);
				fixed4 _Texture2_var = tex2D(_Texture2,i.uv2);
				fixed4 _Texture3_var = tex2D(_Texture3,i.uv3);
			
                fixed cutoff = saturate(dot(_Texture_var.rgb, fixed3(_Tex1Channel.x, _Tex1Channel.y, _Tex1Channel.z))-_AlphaClips.x*2.0+0.5);
				fixed cutoff2 = saturate(dot(_Texture2_var.rgb, fixed3(_Tex2Channel.x, _Tex2Channel.y, _Tex2Channel.z))-_AlphaClips.y*2.0+0.5);
				fixed cutoff3 = saturate(dot(_Texture3_var.rgb, fixed3(_Tex3Channel.x, _Tex3Channel.y, _Tex3Channel.z))-_AlphaClips.z*2.0+0.5);

				fixed4 col= fixed4(_Texture_var.rgb*i.vertexColor.rgb*_Strength.x,cutoff*1000)*_Tex1Channel.w;
				fixed4 col2= fixed4(_Texture2_var.rgb*i.vertexColor2.rgb*_Strength.y,cutoff*1000)*_Tex2Channel.w;
				fixed4 col3= fixed4(_Texture3_var.rgb*i.vertexColor3.rgb*_Strength.z,cutoff*1000)*_Tex3Channel.w;
                return col+ col2+col3;
            }
            ENDCG
        }
    }
}
