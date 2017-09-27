Shader "Custom/SFX/AlphaBlendRol" 
{
    Properties 
	{
        _Main_Color ("Main_Color", Color) = (1,1,1,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _SFX_Color ("SFX_Color", Color) = (0.5,0.5,0.5,1)
        _SFX_Tex ("SFX_Tex", 2D) = "white" {}
        _Clip_Str ("Clip_Str", Range(0, 1)) = 1
        _SFX_Str ("SFX_Str", Float ) = 0
        _SFX_U ("SFX_U", Float ) = 0
        _SFX_V ("SFX_V", Float ) = 0
		_Clip("Clip", Vector) = (1,0,0,0)
        _Clip_Tex ("Clip_Tex", 2D) = "white" {}
    }
    SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off ZWrite Off Fog{ Mode Off }
        Pass 
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            uniform float4 _Main_Color;
            uniform sampler2D _Main_Tex; 
			uniform float4 _Main_Tex_ST;
            uniform sampler2D _Clip_Tex; 
			uniform float4 _Clip_Tex_ST;
            uniform float _Clip_Str;
            uniform sampler2D _SFX_Tex;
			uniform float4 _SFX_Tex_ST;
            uniform float4 _SFX_Color;
            uniform float _SFX_Str;
			half4 _Clip;
            uniform float _SFX_V;
            uniform float _SFX_U;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Main_Tex);
				o.uv1 = lerp(half2(o.uv0.x+ _SFX_U*_Time.g, o.uv0.y), half2(o.uv0.x, o.uv0.y+ _SFX_V*_Time.g), 0.5);
				o.uv1 = TRANSFORM_TEX(o.uv1, _SFX_Tex);
				o.uv2 = TRANSFORM_TEX(o.uv0, _Clip_Tex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _Main_Tex_var = tex2D(_Main_Tex, i.uv0);
				fixed4 _SFX_Tex_var = tex2D(_SFX_Tex, i.uv1);
				fixed4 _Clip_Tex_var = tex2D(_Clip_Tex,i.uv2);
				

				float c = dot(_Clip_Tex_var.rgb, _Clip.rgb)+_Clip_Str*-1.4+0.1;
				c = step(0.01, c);

				fixed3 diffuseColor = _Main_Color.rgb*_Main_Tex_var.rgb;
				fixed3 emissive = _SFX_Color.rgb*_SFX_Tex_var.rgb*_SFX_Str;
				fixed3 finalColor = diffuseColor + emissive;
                return fixed4(finalColor,c);
            }
            ENDCG
        }        
    }
}
