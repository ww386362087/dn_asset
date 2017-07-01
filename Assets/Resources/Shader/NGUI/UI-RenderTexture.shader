Shader "Custom/UI/RenderTexture" {
    Properties {
        _MainTex ("Black (RGB)", 2D) = "black" {}
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ColorMask RGBA
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off
 
        Pass {  
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
           
                #include "UnityCG.cginc"
 
                struct appdata_t {
                    fixed4 vertex : POSITION;
                    fixed2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
                };
 
                struct v2f {
                    fixed4 vertex : POSITION;
                    fixed2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
                };
 
                sampler2D _MainTex;
           
                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.texcoord = v.texcoord;
					o.color = v.color;
                    return o;
                }
           
                fixed4 frag (v2f i) : COLOR
                {
                    return tex2D(_MainTex, i.texcoord);
                }
            ENDCG
        }
    }
}