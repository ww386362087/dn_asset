Shader "Custom/SFX/AlphaBlendKaliClaw" 
{
    Properties 
	{
        _Main_Color ("Main_Color", Color) = (1,1,1,1)
        _MainSFX_Color ("MainSFX_Color", Color) = (0,0,0,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _MainSFX_Srt ("MainSFX_Srt", Float ) = 0
        _Main_SetRGBWRGB_Srt ("Main_Set[RGB]W=RGB_Srt", Vector) = (1,0,0,1)
        _SFX_Color ("SFX_Color", Color) = (0.5,0.5,0.5,1)
        _SFX_Tex ("SFX_Tex", 2D) = "white" {}
        _SFX_U ("SFX_U", Float ) = 0
        _SFX_V ("SFX_V", Float ) = 0
        _SFX_SetRGBWRGB_Srt ("SFX_Set[RGB]W=RGB_Srt", Vector) = (0,0,0,1)
        _Clip_Tex ("Clip_Tex", 2D) = "white" {}
        _AlphaClip ("AlphaClip", Range(0, 1)) = 0.5433631
        _Clip_SetRGB ("Clip_Set[RGB]", Vector) = (1,0,0,0)
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

            uniform fixed4 _Main_Color;
            uniform sampler2D _Main_Tex; 
			uniform half4 _Main_Tex_ST;
            uniform sampler2D _SFX_Tex; 
			uniform half4 _SFX_Tex_ST;
            uniform float _AlphaClip;
            uniform fixed4 _SFX_Color;
            uniform sampler2D _Clip_Tex; 
			uniform half4 _Clip_Tex_ST;
            uniform half _SFX_V;
            uniform half _SFX_U;
            uniform fixed4 _MainSFX_Color;
            uniform half _MainSFX_Srt;
            uniform half4 _SFX_SetRGBWRGB_Srt;
            uniform half4 _Main_SetRGBWRGB_Srt;
            uniform half4 _Clip_SetRGB;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                half2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
                fixed4 vertexColor : COLOR;

            };

            VertexOutput vert (VertexInput v)
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0, _Main_Tex);
				o.uv1 = TRANSFORM_TEX(v.texcoord0, _Clip_Tex);
				o.uv2 = lerp(float2(0,v.texcoord0.x+_Time.y*_SFX_V),float2(v.texcoord0.y+_Time.y*_SFX_U,0),0.5);
				o.uv2 = TRANSFORM_TEX(o.uv2, _SFX_Tex);
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );

                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _Main_Tex_var = tex2D(_Main_Tex,i.uv0);
                fixed4 _Clip_Tex_var = tex2D(_Clip_Tex,i.uv1);
                fixed4 _SFX_Tex_var = tex2D(_SFX_Tex,i.uv2);

                fixed3 col = ((_Main_SetRGBWRGB_Srt.a*_Main_Tex_var.rgb)*_Main_Color.rgb*i.vertexColor.rgb);
                
                fixed3 emissive = ((round((dot(_Main_SetRGBWRGB_Srt.rgb,_Main_Tex_var.rgb)))*_MainSFX_Color.rgb*_MainSFX_Srt)+(_SFX_Color.rgb*(dot(_SFX_SetRGBWRGB_Srt.rgb,_SFX_Tex_var.rgb)+(_SFX_SetRGBWRGB_Srt.a*_SFX_Tex_var.rgb))));
                fixed3 finalColor = col + emissive;
				fixed _clip = ceil((dot(_Clip_SetRGB.rgb,_Clip_Tex_var.rgb))-_AlphaClip);
                return fixed4(finalColor,i.vertexColor.r*_clip);
            }
            ENDCG
        }
    }
}
