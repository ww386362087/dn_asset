Shader "Custom/Skin/RimlightBlend" 
{  
    Properties 
	{  
		_Face("Face", 2D) = "black" {}		
		_Hair("Hair", 2D) = "black" {}
		_Body("Body", 2D) = "black" {}	

		_HairColor("Hair Color", Color) = (1, 1, 1, 1)
		_Color ("Additive Color", Color) = (1, 1, 1, 1)
		_RimColor("Rim Color", Color) = (0.353, 0.353, 0.353, 0.0)
		_LightArgs("x:MainColor Scale y:Light Scale z:Unused w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
    }  
	Category
	{
		Tags{ "RenderType" = "Opaque" }
		SubShader
		{			
			LOD 200
			Pass
			{
				Tags { "LightMode" = "ForwardBase" }

				ZWrite On
				ZTest On
				Cull Back

				CGPROGRAM
				//define
				#define SKINTEX
				#define RIMLIGHT
				#define SHLIGHTON
				#define VERTEXLIGHTON
				#pragma multi_compile __ UIRIM
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 

				sampler2D _Face;
				sampler2D _Hair;
				sampler2D _Body;

				fixed4 _Color;
				//custom frag fun
				fixed4 BasicColor(in v2f i, inout fixed4 mask)
				{
					fixed4 c = fixed4(0, 0, 0, 1);
					float2 uvOffset = float2(0, 0);
					c.rgb = tex2D(_Face, i.uv - uvOffset).rgb*i.mask0.x;
					uvOffset.x += 1;
					c.rgb += tex2D(_Hair, i.uv - uvOffset).rgb*_HairColor.rgb*i.mask0.y;
					c.a += i.mask0.y;
					uvOffset.x += 1;
					c.rgb += tex2D(_Body, i.uv - uvOffset).rgb*i.mask0.z;
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


				ZWrite On
				ZTest On
				Cull Back

				CGPROGRAM
				//define
				#define SKINTEX
				#define NONORMAL
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
				#pragma fragment frag 

				sampler2D _Face;
				sampler2D _Hair;
				sampler2D _Body;

				fixed4 _Color;
				//custom frag fun
				fixed4 BasicColor(in v2f i, inout fixed4 mask)
				{
					fixed4 c = fixed4(0, 0, 0, 1);
					float2 uvOffset = float2(0, 0);
					c.rgb = tex2D(_Face, i.uv - uvOffset).rgb*i.mask0.x;
					uvOffset.x += 1;
					c.rgb += tex2D(_Hair, i.uv - uvOffset).rgb*_HairColor.rgb*i.mask0.y;
					c.a += i.mask0.y;
					uvOffset.x += 1;
					c.rgb += tex2D(_Body, i.uv - uvOffset).rgb*i.mask0.z;
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