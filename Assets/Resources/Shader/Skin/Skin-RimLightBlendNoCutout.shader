    Shader "Custom/Skin/RimlightBlendNoCutout" {  
        Properties {  
			_Face("Face", 2D) = "black" {}		
			_Hair("Hair", 2D) = "black" {}
			_Body("Body", 2D) = "black" {}
			_Alpha("Aplha", 2D) = "black" {}					
			//_Tex3("Texture3", 2D) = "black" {}		
			_HairColor("Hair Color", Color) = (1, 1, 1, 1)
			_Color ("Additive Color", Color) = (1, 1, 1, 1)
			_RimColor("Rim Color", Color) = (0.353, 0.353, 0.353, 0.0)
			_LightArgs("x:MainColor Scale y:Light Scale z:Unused w: Rim Power",Vector) = (1.0,0.21,0.0,3.0)
			_UIRimMask("UI Rim Mask",Vector) = (1,1,0,0)
        }  
        SubShader {  
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
            LOD 200            
            Pass 
			{  
				Name "Basic"
				Tags { "LightMode"="ForwardBase" }  
				Blend SrcAlpha OneMinusSrcAlpha

				ZWrite On
				ZTest On
                Cull Back  
				Lighting On

                CGPROGRAM 
				//define
				#define SKINTEX
				//#define SKINTEX4
				#define LIGHTON
				#define RIMLIGHT
				#define UIRIM
				#define SHLIGHTON
				#define VERTEXLIGHTON
				#define MASK2
				//#define TESTLIGHTING
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
                #pragma fragment frag 

				sampler2D _Face;
				sampler2D _Hair;
				sampler2D _Body;
				sampler2D _Alpha;
				
				fixed4 _Color;
				//custom frag fun
				fixed4 BasicColor(in v2f i)
				{
					fixed4 c = fixed4(0, 0, 0, 1);
					float2 uvOffset = float2(0, 0);
					c.rgb = tex2D(_Face, i.uv - uvOffset).rgb*i.mask0.x;
					c.a = i.mask0.x;
					uvOffset.x += 1;
					c.rgb += tex2D(_Hair, i.uv - uvOffset).rgb*_HairColor.rgb*i.mask0.y;
					c.a += i.mask0.y;
					uvOffset.x += 1;
					c.rgb += tex2D(_Body, i.uv - uvOffset).rgb*i.mask0.z;
					c.a += tex2D(_Alpha, i.uv - uvOffset).r*i.mask0.z;
					c *= _Color;
					return c;
				}
				//include
                #include "UnityCG.cginc"
				//#include "Lighting.cginc"
                #include "../Include/CommonBasic_Include.cginc"
                ENDCG
			}

			Pass 
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
		
				Fog {Mode Off}
				ZWrite On ZTest LEqual Cull Off
				Offset 1, 1

				CGPROGRAM
				//include
				#include "../Include/Shadow_Include.cginc"
				#pragma vertex vertCast
				#pragma fragment fragCast
				#pragma multi_compile_shadowcaster
				ENDCG 
			}
		}
    }  