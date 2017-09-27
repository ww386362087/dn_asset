Shader "Custom/Effect/UVMoveAdd2" 
{
    Properties 
	{
		_Tex ("Main Texture", 2D) = "white" {}
        _Color ("Color0", Color) = (1,1,1,1)

		 _Mask ("Mask Tex", 2D) = "white" {}
		 _MaskColor ("Mask Color", Color) = (1,1,1,1)
        _Strength ("Strength", Float ) = 1
        _Alpha_attenuation ("Alpha Attenuation", Range(-1, 0)) = 0

        _Mask_VSpeed ("Mask_VSpeed", Float ) = 0
        _Mask_USpeed ("Mask_USpeed", Float ) = 0
        _Emm_Strength ("Mask_Strength", Float) = 10
    }
	SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Fog{ Mode Off }
        Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
            
			sampler2D _Tex; 
			fixed4 _Color;

			sampler2D _Mask; 
			fixed4 _MaskColor;
			fixed _Strength;
			fixed _Alpha_attenuation;

			fixed _Mask_USpeed;
			fixed _Mask_VSpeed;
			fixed _Emm_Strength;

			struct VertexInput 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 vertexColor : COLOR;
			};

			struct VertexOutput 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 vc : COLOR0;
			};

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
				o.uv = v.texcoord;
				o.vc = v.vertexColor*_Color;
				return o;
			}

			fixed4 frag(VertexOutput i) : COLOR 
			{
				fixed4 base = tex2D(_Tex,i.uv);
				fixed4 mask = tex2D(_Mask,i.uv);
				fixed4 final = fixed4(base.rgb,mask.r)*i.vc;

				fixed3 emissive = ((base.rgb*i.vc.rgb)*(mask.r*i.vc.a)*_Strength)+_Alpha_attenuation;

				half2 flowUV = lerp((i.uv+(_Time.y*_Mask_VSpeed)*float2(0,1)),(i.uv+(_Time.y*_Mask_USpeed)*half2(1,0)),0.5);
				fixed4 flow = tex2D(_Mask,flowUV);


				return fixed4(final.rgb+emissive*flow.g*_Emm_Strength,flow.g*final.a+final.a);
			}
            ENDCG
        }
    }
}
