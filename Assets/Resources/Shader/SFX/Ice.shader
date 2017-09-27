Shader "Custom/SFX/Ice" 
{
    Properties 
	{
        _Main_Color ("Main_Color", Color) = (1,1,1,1)
        _Main_Tex ("Main_Tex", 2D) = "white" {}
        _Main_Str ("Main_Str", Float ) = 1
        _Spec_Color ("Spec_Color", Color) = (0.5,0.5,0.5,1)
        _Spec_Tex ("Spec_Tex", 2D) = "white" {}
        _Spec_Str ("Spec_Str", Float ) = 1
        _Spec_Acutance ("Spec_Acutance", Float ) = 1
        _SFX_Tex ("SFX_Tex", 2D) = "white" {}
        _SFX_Str ("SFX_Str", Range(0, 2)) = 0.2523913
        _SFX_U ("SFX_U", Float ) = 0
        _SFX_V ("SFX_V", Float ) = 0
        _Chip_R ("Chip_R", Float ) = 1
        _Chip_G ("Chip_G", Float ) = 0
        _Chip_B ("Chip_B", Float ) = 0
        _Break_Str ("Break_Str", Range(0, 1)) = 1
    }

    SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcColor OneMinusSrcColor
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
            uniform float _Main_Str;
            uniform sampler2D _Spec_Tex; 
			uniform float4 _Spec_Tex_ST;
            uniform float _Spec_Str;
            uniform sampler2D _SFX_Tex; 
			uniform float4 _SFX_Tex_ST;
            uniform float _SFX_U;
            uniform float _SFX_V;
            uniform float _Chip_G;
            uniform float _Chip_B;
            uniform float _Chip_R;
            uniform float _Break_Str;
            uniform float _Spec_Acutance;
            uniform float _SFX_Str;
            uniform float4 _Spec_Color;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
                float3 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
                o.uv = TRANSFORM_TEX(v.texcoord0, _Main_Tex);
				o.uv1 = TRANSFORM_TEX(v.texcoord0, _Spec_Tex);
				o.uv2 = lerp(float2(0.5*_Time.y*_SFX_U +v.texcoord0.x, v.texcoord0.y), float2(v.texcoord0.x, 0.5*_Time.y*_SFX_V + v.texcoord0.y), 0.5);
				o.uv2 = TRANSFORM_TEX(o.uv2, _Spec_Tex);
				o.vertexColor = v.vertexColor*_Main_Color.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _Main_Tex_var = tex2D(_Main_Tex,i.uv);

                fixed c = step(0.01,dot(_Main_Tex_var.rgb,fixed3(_Chip_R,_Chip_G,_Chip_B))+ _Break_Str*2.0 - 1.5);
				fixed4 _Spec_Tex_var = tex2D(_Spec_Tex, i.uv1); 
				fixed4 _SFX_Tex_var = tex2D(_SFX_Tex, i.uv2);

				fixed3 emissive = _Main_Tex_var.rgb*i.vertexColor.rgb*_Main_Str+_Spec_Tex_var.rgb*_Spec_Color.rgb*_Spec_Str+_Spec_Tex_var.r*_Spec_Acutance+_SFX_Tex_var.rgb*_SFX_Str;
                return fixed4(emissive*c,1);
            }
            ENDCG
        }        
    }
}
