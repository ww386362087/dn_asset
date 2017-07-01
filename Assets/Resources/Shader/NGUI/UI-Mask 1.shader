Shader "Custom/UI/UIMask 1" {
	Properties  
        {  
        _Color ("Main Color", Color) = (1,1,1,1)  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _MaskTex ("Mask (A)", 2D) = "white" {}  
        }  
        Category  
        {  
            Lighting Off  
            ZWrite Off  
            Cull back  
            Fog { Mode Off }  
            Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha  
            SubShader  
            {  
                Pass  
                {  
                    CGPROGRAM  
                    #pragma vertex vert  
                    #pragma fragment frag  
                    #include "UnityCG.cginc"
					#include "UI_Include.cginc"
                    sampler2D _MainTex;  
                    sampler2D _MaskTex;  
                    float4 _MainTex_ST;
                    fixed4 _Color; 
                    struct appdata  
                    {  
                        float4 vertex : POSITION;  
                        float4 texcoord : TEXCOORD0;  
                    };  
                    struct v2f  
                    {  
                        float4 pos : SV_POSITION;  
                        float2 uv : TEXCOORD0;  
                        float2 worldPos : TEXCOORD1;
                    };  
                    v2f vert (appdata v)  
                    {  
                        v2f o;  
                        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);  
                        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                        o.worldPos = Clip1(v.vertex.xy);
                        return o;  
                }  
                    half4 frag(v2f i) : COLOR  
                    { 
						fixed4 final = tex2D(_MainTex, i.uv) * _Color;
						final.a *= 1- tex2D(_MaskTex, i.uv).a;

						return UI1Clip(final, i.worldPos);
                    }  
                    ENDCG  
                }  
            }                
        }  
}
