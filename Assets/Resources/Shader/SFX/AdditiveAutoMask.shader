Shader "Custom/SFX/AdditiveAutoMask" 
{
    Properties 
	{
        _Color ("Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _Strength ("Strength", Float ) = 1
        _Alpha_attenuation ("Alpha_attenuation", Range(-1, 0)) = 0
        _Mask_Tex ("Mask_Tex", 2D) = "white" {}
        _Mask_VSpeed ("Mask_VSpeed", Float ) = 0
        _Mask_USpeed ("Mask_USpeed", Float ) = 0
        _Mask_Strength ("Mask_Strength", Range(-1, 0)) = 0
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
			
            
            uniform sampler2D _Texture; 
			uniform float4 _Texture_ST;
            uniform float4 _Color;
            uniform float _Strength;
            uniform float _Alpha_attenuation;
            uniform float _Mask_VSpeed;
            uniform sampler2D _Mask_Tex; 
			uniform float4 _Mask_Tex_ST;
            uniform float _Mask_USpeed;
            uniform float _Mask_Strength;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD01;
                float3 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = TRANSFORM_TEX(v.texcoord0,_Texture);
				o.uv1 = lerp(fixed2(o.uv0.x,o.uv0.y+_Time.y*_Mask_VSpeed), fixed2(o.uv0.x+_Time.y*_Mask_USpeed, o.uv0.y), 0.5);
                o.vertexColor = v.vertexColor.rgb*_Color.rgb*v.vertexColor.a;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR 
			{

                fixed4 _Texture_var = tex2D(_Texture,i.uv0);
				fixed3 emissive = _Texture_var.rgb*i.vertexColor.rgb*_Texture_var.a*_Strength+_Alpha_attenuation;
                fixed4 _Mask_Tex_var = tex2D(_Mask_Tex, i.uv1);
                return fixed4(emissive,_Mask_Strength+_Mask_Tex_var.r);
            }
            ENDCG
        }
    }
}
