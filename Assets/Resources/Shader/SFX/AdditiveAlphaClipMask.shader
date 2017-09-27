Shader "Custom/SFX/AdditiveAlphaClipMask" 
{
    Properties 
	{
        _Main_Color ("Mian_Color", Color) = (1,1,1,1)
        _Main_Tex ("Mian_Tex", 2D) = "white" {}
        _Tex_Channel_R ("Tex_Channel_R", Float ) = 0
        _Tex_Channel_G ("Tex_Channel_G", Float ) = 0
        _Tex_Channel_B ("Tex_Channel_B", Float ) = 0
        _Tex_Strength ("Tex_Strength", Float ) = 1
        _Alpha_attenuation ("Alpha_attenuation", Range(-1, 0)) = -1
        _Tex_Clips ("Tex_Clips", Range(0, 1)) = 0
        _Mask_Tex ("Mask_Tex", 2D) = "white" {}
        _Mask_VSpeed ("Mask_VSpeed", Float ) = 0
        _Mask_USpeed ("Mask_USpeed", Float ) = 0
        _Mask_Channel_R ("Mask_Channel_R", Float ) = 0
        _Mask_Channel_G ("Mask_Channel_G", Float ) = 0
        _Mask_Channel_B ("Mask_Channel_B", Float ) = 0
        _Mask_Strength ("Mask_Strength", Float ) = 1
        _Mask_Clips ("Mask_Clips", Range(0, 1)) = 0
        _Set_ClipBlend ("Set_ClipBlend", Range(0, 1)) = 0
    }
    SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		Cull Off ZWrite Off Fog{ Mode Off }
        Pass
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _Main_Tex; 
			uniform float4 _Main_Tex_ST;
            uniform float4 _Main_Color;
            uniform float _Tex_Strength;
            uniform float _Alpha_attenuation;
            uniform float _Mask_VSpeed;
            uniform sampler2D _Mask_Tex; 
			uniform float4 _Mask_Tex_ST;

            uniform float _Mask_USpeed;

            uniform float _Mask_Channel_R;
            uniform float _Mask_Channel_G;
            uniform float _Mask_Channel_B;

            uniform float _Tex_Channel_R;
            uniform float _Tex_Channel_G;
            uniform float _Tex_Channel_B;

            uniform float _Tex_Clips;
            uniform float _Mask_Strength;
            uniform float _Mask_Clips;
            uniform float _Set_ClipBlend;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
                float3 color : COLOR;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;

				o.uv = TRANSFORM_TEX(v.texcoord0, _Main_Tex);
                o.uv1 = lerp(float2(v.texcoord0.x, v.texcoord0.y + _Time.y*_Mask_VSpeed), float2(v.texcoord0.x + _Time.y*_Mask_USpeed, v.texcoord0.y), 0.5);
				o.uv1 = TRANSFORM_TEX(o.uv1, _Mask_Tex);
                o.color = v.color.rgb*_Main_Color.rgb*v.color.a*_Main_Color.a;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _Main_Tex_var = tex2D(_Main_Tex, i.uv);
				fixed4 _Mask_Tex_var = tex2D(_Mask_Tex,i.uv1);
                float maskUV = dot(_Mask_Tex_var.rgb,fixed3(_Mask_Channel_R,_Mask_Channel_G,_Mask_Channel_B));
				float c = lerp((maskUV + _Mask_Clips*-2.0 + 1.0), dot(_Main_Tex_var.rgb,fixed3(_Tex_Channel_R,_Tex_Channel_G,_Tex_Channel_B) + _Tex_Clips*-2.0 + 1.0), _Set_ClipBlend) - 0.5;
				c = step(0.01, c);
				fixed3 emissive = _Main_Tex_var.rgb*i.color.rgb*_Main_Tex_var.a*_Tex_Strength+_Alpha_attenuation;
                return fixed4(emissive,_Mask_Strength*maskUV*c);
            }
            ENDCG
        }
    }
}
