  Shader "Custom/Skin/Fade8" {
    Properties {
		_Tex0("Texture0", 2D) = "black" {}		
		_Tex1("Texture1", 2D) = "black" {}
		_Tex2("Texture2", 2D) = "black" {}			
		_Tex3("Texture3", 2D) = "black" {}		
		_Tex4("Texture4", 2D) = "black" {}		
		_Tex5("Texture5", 2D) = "black" {}		
		_Tex6("Texture6", 2D) = "black" {}		
		_Tex7("Texture7", 2D) = "black" {}
		_HairColor("Hair Color", Color) = (1, 1, 1, 1)
		_Color ("Additive Color", Color) = (0.5, 0.5, 0.5, 0.0)
		_Power ("Power", Range (0.1, 0.7)) = 0.1
    }
	SubShader{			
			Pass {  
			Name "Fade"
            Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
				ZWrite On
                  
                Cull Back
				Lighting Off 
                  
                CGPROGRAM 
				//define
				#define SKINTEX
				#define SKINTEX8
				//head
				#include "../Include/CommonHead_Include.cginc"
				#include "../Include/SkinBlend_Include.cginc"
				//vertex&fragment
				#pragma vertex vert
                #pragma fragment frag 

				fixed4 _Color;
				half _Power;
				//custom frag fun
				fixed4 BasicColor(in v2f i)
				{
					fixed4 c = BlendColor(i);
					c.rgb += _Color.rgb * _Power;
					c.a = _Color.a;	
					return c;
				}
				//include
                #include "UnityCG.cginc"  
                #include "../Include/CommonBasic_Include.cginc"
                ENDCG  
            }			
		}
}	

