Shader "Custom/Skin/RimlightBlendCutout4" 
{  
    Properties 
	{  
		_Face("Face", 2D) = "black" {}		
		_Hair("Hair", 2D) = "black" {}
		_Helmet("Helmet", 2D) = "black" {}
		_Body("Body", 2D) = "black" {}
		_Alpha("Alpha", 2D) = "black" {}
		_HairColor("Hair Color", Color) = (1, 1, 1, 1)
		_Color ("Additive Color", Color) = (1, 1, 1, 1)
		_RimColor("Rim Color", Color) = (0.353, 0.353, 0.353, 0.0)
		_LightArgs("x:MainColor Scale y:Light Scale z:Unused w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
		_Cutoff("Alpha cutoff", Float) = 0.05
    }  
	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		SubShader
		{
			LOD 200
			Pass
			{
				Tags { "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha

				ZWrite On
				ZTest On
				Cull Back

				CGPROGRAM
				//define
				#define CUTOUT
				#define SKINTEX
				#define RIMLIGHT
				#define SHLIGHTON
				#define VERTEXLIGHTON
				#define BLENDCUTOUT
				#pragma multi_compile __ UIRIM
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 

				sampler2D _Face;
				sampler2D _Hair;
				sampler2D _Helmet;
				sampler2D _Body;
				sampler2D _Alpha;

				fixed4 _Color;
				//custom frag fun
				fixed4 BasicColor(in v2f i, inout fixed4 mask)
				{
					fixed4 c = fixed4(0, 0, 0, 1);
					float2 uvOffset = float2(0, 0);
					c.rgb = tex2D(_Face, i.uv - uvOffset).rgb*i.mask0.x;
					//c.a = i.mask0.x;
					uvOffset.x = 1;
					c.rgb += tex2D(_Hair, i.uv - uvOffset).rgb*_HairColor.rgb *i.mask0.y;
					uvOffset.x = 7;
					c.rgb += tex2D(_Helmet, i.uv - uvOffset).rgb *i.mask0.w;
					//c.a += i.mask0.y;
					uvOffset.x = 2;
					c.rgb += tex2D(_Body, i.uv - uvOffset).rgb*i.mask0.z;
					c.a = i.mask0.x + i.mask0.y + i.mask0.w + tex2D(_Alpha, i.uv - uvOffset).r*i.mask0.z;
					c *= _Color;
					return c;
				}
				//include
				#include "../Include/CommonBasic_Include.cginc"
				ENDCG
			}
		}
		SubShader
		{
			LOD 100
			Pass
			{
				Tags { "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha

				ZWrite On
				ZTest On
				Cull Back

				CGPROGRAM
				//define
				#define CUTOUT
				#define SKINTEX
				#define BLENDCUTOUT
				#define NONORMAL
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 

				sampler2D _Face;
				sampler2D _Hair;
				sampler2D _Helmet;
				sampler2D _Body;
				sampler2D _Alpha;

				fixed4 _Color;
				//custom frag fun
				fixed4 BasicColor(in v2f i, inout fixed4 mask)
				{
					fixed4 c = fixed4(0, 0, 0, 1);
					float2 uvOffset = float2(0, 0);
					c.rgb = tex2D(_Face, i.uv - uvOffset).rgb*i.mask0.x;
					uvOffset.x = 1;
					c.rgb += tex2D(_Hair, i.uv - uvOffset).rgb*_HairColor.rgb *i.mask0.y;
					uvOffset.x = 7;
					c.rgb += tex2D(_Helmet, i.uv - uvOffset).rgb *i.mask0.w;
					uvOffset.x = 2;
					c.rgb += tex2D(_Body, i.uv - uvOffset).rgb*i.mask0.z;
					c.a = i.mask0.x + i.mask0.y + i.mask0.w + tex2D(_Alpha, i.uv - uvOffset).r*i.mask0.z;
					c *= _Color*1.2;
					return c;
				}
				//include
				#include "../Include/CommonBasic_Include.cginc"
				ENDCG
			}
		}
	}
}  