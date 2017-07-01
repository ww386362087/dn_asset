// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Hidden/Unlit/Particles/Additive 3" {
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_Mask ("Mask (A)", 2D) = "white" {}
    }

    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            AlphaTest Off
            Fog { Mode Off }
            Offset -1, -1
            ColorMask RGB
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			#include "UI_Include.cginc"
            sampler2D _MainTex;
			sampler2D _Mask;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float2 worldPos2 : TEXCOORD2;
            };


            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = v.texcoord;
				o.worldPos.xy = Clip1(v.vertex.xy);
				o.worldPos.zw = Clip2(v.vertex.xy);
				o.worldPos2 = Clip3(v.vertex.xy);                
				return o;
            }

            half4 frag (v2f IN) : COLOR
            {
				return UI3ClipFade(UISampleFx(_MainTex, _Mask, IN.color, IN.texcoord), IN.worldPos, IN.worldPos2);
            }
            ENDCG
        }
    }    
}
