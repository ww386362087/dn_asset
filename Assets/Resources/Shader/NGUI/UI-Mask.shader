Shader "Custom/UI/UIMask" {
	Properties  
        {  
        _Color ("Main Color", Color) = (1,1,1,1)  
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}  
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
                    sampler2D _MainTex;  
                    sampler2D _MaskTex;  
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
                    };  
                    v2f vert (appdata v)  
                    {  
                        v2f o;  
                        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);  
                        o.uv = v.texcoord.xy;  
                        return o;  
                }  
                    half4 frag(v2f i) : COLOR  
                    {  
                        fixed4 c = _Color * tex2D(_MainTex, i.uv);  
                        fixed ca = tex2D(_MaskTex, i.uv).a;  
                        c.a *= 1 - ca;  
                        return c;  
                    }  
                    ENDCG  
                }  
            }  
              
        }  
}
